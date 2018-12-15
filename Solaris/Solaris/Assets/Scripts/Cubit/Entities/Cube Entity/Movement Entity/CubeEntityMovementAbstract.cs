using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CubeEntityMovementAbstract : EntityCopiableAbstract, IRemoveOnSwitchAffiliationExit, IOnTargetChange
{
    public GameObject m_target;
    [Header("------- Base Settings -------")]
    public bool m_useThis = true;
    public bool m_defyGravityGlobal;
    public bool m_defyGravityLocal;
    public bool m_setTargetToNullOnDestroy;
    public float m_maxSpeed;
    public float m_destroyCheckIntervalMin;
    public float m_destroyCheckIntervalMax;
    public bool m_setInactiveOnDestroy;
    public float m_forceFactor = 1f;

    [Header("--- Follow Up ---")]
    public List<CubeEntityMovementAbstract> m_followUpMovementScripts;
    public List<CubeEntityMovementStartSpeed> m_startMovements;

    [Header("------- Base Debug -------")]
    public Vector3 m_targetPosition;
    public float m_destroyRdyTime;

    public void OnFixedUpdate()
    {
        if (m_destroyCheckIntervalMin > 0 || m_destroyCheckIntervalMax > 0)
        {
            if (m_destroyRdyTime < Time.time)
            {
                if (updateDestroy())
                    onDestroy();
                else
                    m_destroyRdyTime = Random.Range(m_destroyCheckIntervalMin, m_destroyCheckIntervalMax) + Time.time;
            }
        }
        else
        {
            if (updateDestroy())
                onDestroy();
        }
    }
    // follow up
    public virtual void onDestroy()
    {

        if (m_defyGravityGlobal && !m_setInactiveOnDestroy)
            GetComponent<CubeEntityRegisterManager>().registerSkipGlobal(this, false);
        if (m_defyGravityLocal)
            GetComponent<CubeEntityRegisterManager>().registerSkipLocal(this, false);

        if (m_setTargetToNullOnDestroy)
        {
            m_target = null;
            GetComponent<CubeEntityTargetManager>().setTarget((CubeEntityTargetManager)null);
        }

        activateFollowUpScripts();
        //prepareDestroyScript();
        if (this is IPrepareRemoveOnStateChange)
            ((IPrepareRemoveOnStateChange)this).onStateChangePrepareRemove();
        if (this is IRemoveOnStateChange)
            ((IRemoveOnStateChange)this).onRemove();

        //Debug.Log(GetComponent<>)
        if (m_setInactiveOnDestroy)
        {
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
        }
    }
    public void activateFollowUpScripts()
    {
        if (m_followUpMovementScripts == null)
            return;
        foreach (CubeEntityMovementAbstract script in m_followUpMovementScripts)
        {
            if (script == null)
                continue;

            GetComponent<CubeEntitySystem>().getMovementComponent().addMovementComponent(script, m_target, m_targetPosition);

            script.m_useThis = true;
        }
    }

    // copy
    public void setValues(CubeEntityMovementAbstract copyScript)
    {
        m_useThis = copyScript.m_useThis;
        m_defyGravityGlobal = copyScript.m_defyGravityGlobal;
        m_maxSpeed = copyScript.m_maxSpeed;
        m_forceFactor = copyScript.m_forceFactor;
        m_destroyCheckIntervalMin = copyScript.m_destroyCheckIntervalMin;
        m_destroyCheckIntervalMax = copyScript.m_destroyCheckIntervalMax;
        m_setInactiveOnDestroy = copyScript.m_setInactiveOnDestroy;
        m_setTargetToNullOnDestroy = copyScript.m_setTargetToNullOnDestroy;
        m_defyGravityLocal = copyScript.m_defyGravityLocal;

        m_destroyRdyTime = Random.Range(m_destroyCheckIntervalMin, m_destroyCheckIntervalMax) + Time.time;

        m_followUpMovementScripts = new List<CubeEntityMovementAbstract>();
        if (copyScript.m_followUpMovementScripts != null)
        {
            foreach (CubeEntityMovementAbstract script in copyScript.m_followUpMovementScripts)
            {
                m_followUpMovementScripts.Add(script);
            }
        }
        m_startMovements = new List<CubeEntityMovementStartSpeed>();
        if (copyScript.m_startMovements != null)
        {
            foreach (CubeEntityMovementStartSpeed script in copyScript.m_startMovements)
            {
                m_startMovements.Add(script);
            }
        }
    }

    // interfaces
    public void onSwitchAffiliationRemoveExit(IRemoveOnSwitchAffiliationExit otherScript)
    {
        getForceVector();
    }
    public virtual void pasteScript(CubeEntityMovementAbstract baseScript, GameObject target, Vector3 targetPosition)
    {
        // start movement
        foreach (CubeEntityMovementStartSpeed startMovement in baseScript.m_startMovements)
        {
            Debug.Log("Caution!");
            if (startMovement == null)
                continue;
            startMovement.applyMovement(gameObject, targetPosition, baseScript.transform.position, baseScript.transform.rotation, null, 1);
        }
        
        m_target = target;
        m_targetPosition = targetPosition;

        if (m_defyGravityGlobal)
            GetComponent<CubeEntityRegisterManager>().registerSkipGlobal(this, true);
        if (m_defyGravityLocal)
            GetComponent<CubeEntityRegisterManager>().registerSkipLocal(this, true);
    }
    public abstract bool updateDestroy();
    public abstract void getForceVector();
    public abstract void pasteScriptButDontActivate(CubeEntityMovementAbstract baseScript);
    public void onTargetChange(CubeEntityTargetManager target)
    {
        if (target == null)
            target = null;
        else
            m_target = target.gameObject;
    }

    
}
