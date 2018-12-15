using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedePushAway : MonsterEntityDeathEffect, ICopiable, IStateOnStateChange, IRemoveOnStateChange, IRemoveOnSwitchAffiliation
{
    [Header("-------- Settings -------")]
    public float m_forcePower;
    public float m_explosionRadius;
    public AnimationCurve rangeCurve;
    public bool m_los;
    public LayerMask m_layerMask;

    [Space]
    public List<CubeEntityMovementAbstract> m_movementAbstracts;
    [Header("--- (Criteria) ---")]
    public bool m_foesOnly;

    [Header("------- Debug -------")]
    bool b;
    public override void activateDeathEffect(MonsterEntityBase baseScript)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<Rigidbody>() != null && !(gameObject == collider.gameObject) && m_foesOnly ? collider.GetComponent<CubeEntityState>().isFoe(gameObject) : true)
            {
                if(collider.GetComponent<CubeEntityState>() == null)
                {
                    continue;
                }

                float distance = (collider.transform.position - transform.position).magnitude;
                //float factor = (m_explosionRadius != 0) ? ((distance / m_explosionRadius)) : (0);
                float factor = (m_explosionRadius != 0) ? (rangeCurve.Evaluate(distance / m_explosionRadius)) : (0);
                //float power = m_forcePower * rangeCurve.Evaluate(factor);
                Vector3 direction = (collider.transform.position - transform.position).normalized;
                Vector3 targetPosition = collider.transform.position + direction.normalized * 10000f;

                foreach (CubeEntityMovementAbstract script in m_movementAbstracts)
                {
                    if(script == null)
                    {
                        Debug.Log("Aborted: script was null!");
                        continue;
                    }

                    bool b = true;

                    if (m_los && Physics.Raycast(transform.position, collider.transform.position, distance, m_layerMask))
                    {
                        b = false;
                    }

                    if(b)
                    {
                        CubeEntityMovementAbstract s = collider.GetComponent<CubeEntityMovement>().addMovementComponent(script, null, targetPosition);
                        s.m_forceFactor = factor;
                    }
                }
            }
        }

        Destroy(this);
    }

    // copy
    public void setValues(MedePushAway copyScript)
    {
        m_movementAbstracts = new List<CubeEntityMovementAbstract>();
        foreach(CubeEntityMovementAbstract movementScript in copyScript.m_movementAbstracts)
        {
            CubeEntityMovementAbstract script = null;

            if (movementScript is ICopiable)
                script = (CubeEntityMovementAbstract)(GetComponent<EntitySystemBase>().copyPasteComponent((ICopiable)movementScript, true));
            else
                Debug.Log("Should not happen!");
            

            if (script == null)
                Debug.Log("Should not happen!");
            else
                script.pasteScriptButDontActivate(movementScript);
                

            if (script != null)
                m_movementAbstracts.Add(script);
        }

        m_forcePower = copyScript.m_forcePower;
        m_explosionRadius = copyScript.m_explosionRadius;
        rangeCurve = copyScript.rangeCurve;
        m_layerMask = copyScript.m_layerMask;
        m_los = copyScript.m_los;

        m_foesOnly = copyScript.m_foesOnly;
    }

    // interfaces
    public void onSwitchAffiliationRemove()
    {
        onRemove();
    }
    public void onStateEnter()
    {

    }
    public void onStateExit()
    {
        activateDeathEffect(null);
    }
    public void onCopy(ICopiable copiable)
    {
        setValues((MedePushAway)copiable);
    }
    public void onRemove()
    {
        Destroy(this);
    }
}
