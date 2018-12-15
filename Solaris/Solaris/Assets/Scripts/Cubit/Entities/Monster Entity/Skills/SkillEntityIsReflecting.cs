using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEntityIsReflecting : MonoBehaviour
{
    public int m_maxSwitchesPerFrame;
    public float m_afterImmuneTime;
    public GameObject m_spawnPrefab;
    public GameObject m_originCore;
    public GameObject m_originSkillGameObject;
    public List<CubeEntityMovementAbstract> m_movementScripts;
    public List<GameObject> m_onEnterExplosions;
    public int m_switchedThisFrame;
    public int m_maxReflectObjects;
    public int m_reflectedObjects;
    public float m_ressourceFactor;

    void switchAffiliation(GameObject other)
    {
        // check for cancel
        if(m_maxReflectObjects > 0 && m_reflectedObjects >= m_maxReflectObjects)
        {
            m_originSkillGameObject.GetComponent<MonsterEntitySkillReflect>().deactivateSkill(true);
            return;
        }
        m_reflectedObjects++;

        foreach(GameObject o in m_onEnterExplosions)
        {
            CubeEntityParticleSystem.createParticleEffet(o, other.transform.position);
        }


        IRemoveOnSwitchAffiliation[] scripts = other.GetComponents<IRemoveOnSwitchAffiliation>();
        foreach (IRemoveOnSwitchAffiliation script in scripts)
        {
            script.onSwitchAffiliationRemove();
        }

        List<CubeEntityMovementAbstract> followUp = new List<CubeEntityMovementAbstract>();
        foreach (CubeEntityMovementAbstract script in other.GetComponents<CubeEntityMovementAbstract>())
        {
            if (script == null)
                continue;

            followUp.Add(script);
            script.enabled = false;
        }

        // dont activate deathParticleEffects
        other.GetComponent<CubeEntityParticleSystem>().m_OnExitParticleEffects.Clear();// = new List<GameObject>();
        

        // change state
        other.GetComponent<CubeEntityPrefapSystem>().setToPrefab(m_spawnPrefab);

        // switch target
        CubeEntityTargetManager otherTargetManager = other.GetComponent<CubeEntityTargetManager>();
        otherTargetManager.setTarget(otherTargetManager.m_origin, m_originCore);

        // add can not be reflected script
        if (m_afterImmuneTime > 0)
        {
            CanNotBeReflected canNotBeReflected = other.AddComponent<CanNotBeReflected>();
            canNotBeReflected.m_durationEndTime = m_afterImmuneTime + Time.time;
        }


        foreach (CubeEntityMovementAbstract script in m_movementScripts)
        {
            if (script == null)
                continue;
            Vector3 targetPosition = Vector3.zero;
            if (otherTargetManager.getTarget() != null)
                targetPosition = otherTargetManager.getTarget().transform.position;
            else
                targetPosition = transform.position + 10000f * (transform.position - m_originCore.transform.position).normalized;
            CubeEntityMovementAbstract newScript = other.GetComponent<CubeEntityMovement>().addMovementComponent(script, otherTargetManager.getTarget(), targetPosition);
            newScript.m_forceFactor = Mathf.Sqrt(m_ressourceFactor); // TODO : maybe change?


            foreach (CubeEntityMovementAbstract ss in followUp)
            {
                ;//newScript.m_followUpMovementScripts.Add(ss);
            }
            script.m_useThis = true;
        }

        IRemoveOnSwitchAffiliationExit[] scripts2 = other.GetComponents<IRemoveOnSwitchAffiliationExit>();
        foreach (IRemoveOnSwitchAffiliationExit script in scripts2)
        {
            script.onSwitchAffiliationRemoveExit(null);
        }

        return;
        //IRemoveOnSwitchAffiliation[] scripts = other.GetComponents<IRemoveOnSwitchAffiliation>();
        //foreach(IRemoveOnSwitchAffiliation script in scripts)
        //{
        //    script.onSwitchAffiliationRemove();
        //}

        //other.GetComponent<CubeEntityParticleSystem>().createOnExitParticleEffects();

        //CubeEntityMovement otherMovementScript = other.GetComponent<CubeEntityMovement>();
        //CubeEntityTargetManager otherTargetManager = other.GetComponent<CubeEntityTargetManager>();

        other.GetComponent<CubeEntityState>().onCopyValues(m_originCore.GetComponent<CubeEntityState>());
        otherTargetManager.setTarget(otherTargetManager.getOrigin());
        otherTargetManager.setOrigin(m_originCore.GetComponent<CubeEntityTargetManager>());

        //foreach (CubeEntityMovementAbstract script in GetComponents<CubeEntityMovementAbstract>())
            //script.getForceVector();

        //List<CubeEntityMovementAbstract> followUp = new List<CubeEntityMovementAbstract>();
        foreach (CubeEntityMovementAbstract script in other.GetComponents<CubeEntityMovementAbstract>())
        {
            if (script == null)
                continue;

            followUp.Add(script);
            script.enabled = false;
        }
        foreach (CubeEntityMovementAbstract script in m_movementScripts)
        {
            if (script == null)
                continue;

            Vector3 targetPosition = Vector3.zero;
            if (otherTargetManager.getTarget() != null)
                targetPosition = otherTargetManager.getTarget().transform.position;
            else
                targetPosition = transform.position + 10000f * (transform.position - m_originCore.transform.position).normalized;
            CubeEntityMovementAbstract newScript = other.GetComponent<CubeEntityMovement>().addMovementComponent(script, otherTargetManager.getTarget(), targetPosition);

            foreach (CubeEntityMovementAbstract ss in followUp)
                newScript.m_followUpMovementScripts.Add(ss);

            script.m_useThis = true;
        }

        //IRemoveOnSwitchAffiliationExit[] scripts2 = other.GetComponents<IRemoveOnSwitchAffiliationExit>();
        //foreach (IRemoveOnSwitchAffiliationExit script in scripts2)
        //{
        //    script.onSwitchAffiliationRemoveExit(null);
        //}
    }

    void manageActivision(GameObject other)
    {
        if(m_maxSwitchesPerFrame <= 0 || m_switchedThisFrame < m_maxSwitchesPerFrame)
        {
            switchAffiliation(other);
            m_switchedThisFrame++;
        }
    }

    private void FixedUpdate()
    {
        m_switchedThisFrame = 0;
    }

    public void OnTriggerEnter(Collider other)
    {
        CubeEntityState otherStateScript = other.GetComponent<CubeEntityState>();
        if (otherStateScript != null)
        {
            if (otherStateScript.m_state == CubeEntityState.s_STATE_ACTIVE && m_originCore.GetComponent<CubeEntityState>().isFoe(otherStateScript) && other.GetComponent<CanNotBeReflected>() == null)
            {
                manageActivision(other.gameObject);
            }
        }
    }
}
