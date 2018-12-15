using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntityDeathEffectDamageExplosion : MonsterEntityDeathEffect, ICopiable, IStateOnStateChange, IRemoveOnStateChange
{
    public static int s_checksThisFrame = 0;
    public bool m_triggered;
    [Header("-------- Settings -------")]
    public float m_explosionRadius;
    public float m_damagePerHit;
    public float m_damageMax;
    public float m_hitsMax;
    public bool m_requireLos;
    public LayerMask m_layerMask;

    [Header("------- Debug -------")]
    bool b;

    public void LateUpdate()
    {
        s_checksThisFrame = 0;
    }

    public override void activateDeathEffect(MonsterEntityBase baseScript)
    {
        if(s_checksThisFrame > 30)
        {
            Debug.Log("Warning: DeathEffectExplosion m_checksThisFrame > 30");
            return;
        }
        s_checksThisFrame++;

        if (m_triggered)
            return;
        m_triggered = true;


        Collider[] colliders = Physics.OverlapSphere(transform.position, m_explosionRadius);
        foreach(Collider collider in colliders)
        {
            if (m_requireLos)
            {
                Collider[] colliders2 = Physics.OverlapBox(collider.transform.position, new Vector3(0.1f, 0.1f, 0.1f));
                foreach(Collider col in colliders2)
                {
                    if(col.isTrigger && Utility.IsInLayerMask(collider.gameObject.layer, m_layerMask))
                    {
                        //Debug.DrawLine(transform.position + Vector3.down, collider.transform.position + Vector3.down, Color.red);
                        continue;
                    }
                }

                if (Physics.Raycast(collider.transform.position, (transform.position - collider.transform.position), (transform.position - collider.transform.position).magnitude, m_layerMask))
                {
                    //Debug.DrawLine(transform.position + Vector3.down, collider.transform.position + Vector3.down, Color.red);
                    ;// continue;
                }


                //Debug.DrawLine(transform.position, collider.transform.position, Color.green);
            }
            if (collider.GetComponent<CubeEntityCharge>() != null && !(gameObject == collider.gameObject) && GetComponent<CubeEntityState>().isFoe(collider.GetComponent<CubeEntityState>()))
            {
                collider.GetComponent<CubeEntityCharge>().evaluateDischarge(-m_damagePerHit, GetComponent<CubeEntityState>().m_affiliation, CubeEntityState.s_STATE_ACTIVE, true);
            }
                
        }

        Destroy(this);
    }

    // copy
    public void setValues(MonsterEntityDeathEffectDamageExplosion copyScript)
    {
        m_explosionRadius = copyScript.m_explosionRadius;
        m_damagePerHit = copyScript.m_damagePerHit;
        m_damageMax = copyScript.m_damageMax;
        m_hitsMax = copyScript.m_hitsMax;
        m_requireLos = copyScript.m_requireLos;
        m_layerMask = copyScript.m_layerMask;
    }

    // interfaces
    public void onStateEnter()
    {

    }
    public void onStateExit()
    {
        activateDeathEffect(null);
    }
    public void onCopy(ICopiable copiable)
    {
        setValues((MonsterEntityDeathEffectDamageExplosion)copiable);
    }
    public void onRemove()
    {
        Destroy(this);
    }

    /*
    // abstract
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        setValues((MonsterEntityDeathEffectDamageExplosion)baseScript);
    }
    public override void prepareDestroyScript()
    {
        Destroy(this);
    }
    */
}
