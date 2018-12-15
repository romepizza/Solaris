using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityTargetManager : MonoBehaviour, IPrepareRemoveOnStateChange
{
    public static int m_setTargetsThisFrame = 0;

    [Header("------- Settings -------")]


    [Header("------- Debug -------")]
    public CubeEntityTargetManager m_origin;
    public CubeEntityTargetManager m_target;
    public List<CubeEntityTargetManager> m_targetedByCore;
    public List<CubeEntityTargetManager> m_targetedByActive;


    public void updateTarget()
    {
        CubeEntityState stateScript = GetComponent<CubeEntityState>();
        if (stateScript == null)
        {
            Debug.Log("Aborted: state script was null!");
            return;
        }

        if (gameObject == Constants.getPlayer())
        {
            if (PlayerEntityAim.aim() != null)
                setTarget(PlayerEntityAim.aim().GetComponent<CubeEntityTargetManager>());
            else
                setTarget((CubeEntityTargetManager)null);
        }
        else if (stateScript.m_affiliation == CubeEntityState.s_AFFILIATION_PLAYER_ALLY && stateScript.m_state == CubeEntityState.s_STATE_CORE)
        {
            List<GameObject> enemyList = Constants.getMainCge().GetComponent<CgeMonsterManager>().m_monstersAlive;
            if (enemyList != null && enemyList.Count > 0)
            {
                setTarget(enemyList[Random.Range(0, enemyList.Count)].GetComponent<CubeEntityTargetManager>());
            }
            else
            {
                setTarget((CubeEntityTargetManager)null);
            }
        }
        else if (stateScript.m_affiliation == CubeEntityState.s_AFFILIATION_ENEMY_1 && stateScript.m_state == CubeEntityState.s_STATE_CORE)
        {
            setTarget( Constants.getPlayer().GetComponent<CubeEntityTargetManager>());
        }
        else
        {
            setTarget((CubeEntityTargetManager)null);
        }
        //setTarget(m_target);
    }
    public void LateUpdate()
    {
        m_setTargetsThisFrame = 0;
    }

    // getter & setter
    public GameObject getTarget()
    {
        if ((gameObject.layer == Constants.s_playerLayer) || m_target == null)
            updateTarget();

        if (m_target == null)
        {
            //Debug.Log("Caution!");
            return null;
        }

        return m_target.gameObject;
    }
    //public CubeEntityTargetManager getTargetScript()
    //{
    //    if ((gameObject.layer == Constants.s_playerLayer) || m_target == null)
    //        updateTarget();

    //    if (m_target == null)
    //    {
    //        //Debug.Log("Caution!");
    //        return null;
    //    }

    //    return m_target;
    //}
    public void setTarget(GameObject target)
    {
        CubeEntityTargetManager targetManager = null; 
        if (target == null)
            targetManager = null;
        else
            targetManager = target.GetComponent<CubeEntityTargetManager>();

        //if (targetManager == null)
        //{
        //    Debug.Log("Aborted: targets targetManager was null!");
        //    return;
        //}
        setTarget(targetManager);

        //if(m_setTargetsThisFrame > 10)
        //{
        //    Debug.Log("Warning: m_setTargetsThisFrame = " + m_setTargetsThisFrame);
        //}
        //if(m_setTargetsThisFrame > 100)
        //{
        //    Debug.Log("Aborted: m_setTargetsThisFrame > " + m_setTargetsThisFrame);
        //    return;
        //}
        //m_setTargetsThisFrame++;

        //if (m_target == target)
        //    return;

        //deregisterAllTargetedBy();
        //if (target == null)
        //    m_target = null;
        //else
        //{
        //    m_target = target.GetComponent<CubeEntityTargetManager>();
        //    m_target.registerTargetedBy(this);
        //}
    }
    //public void setTarget(GameObject target, GameObject origin)
    //{
    //    Debug.Log("Aborted: Yet to be implemented!");
    //    return;

    //    //if (m_setTargetsThisFrame > 10)
    //    //{
    //    //    Debug.Log("Warning: m_setTargetsThisFrame = " + m_setTargetsThisFrame);
    //    //}
    //    //if (m_setTargetsThisFrame > 100)
    //    //{
    //    //    Debug.Log("Aborted: m_setTargetsThisFrame > " + m_setTargetsThisFrame);
    //    //    return;
    //    //}
    //    //m_setTargetsThisFrame++;

    //    //deregisterAllTargetedBy();
    //    //if (target == null)
    //    //    m_target = null;
    //    //else
    //    //{
    //    //    m_target = target.GetComponent<CubeEntityTargetManager>();
    //    //    m_target.registerTargetedBy(this);
    //    //}
    //    //setOrigin(origin);
    //}
    //public void setTarget(GameObject target, CubeEntityTargetManager origin)
    //{
    //    Debug.Log("Aborted: Yet to be implemented!");
    //    return;

    //    //if (m_setTargetsThisFrame > 10)
    //    //{
    //    //    Debug.Log("Warning: m_setTargetsThisFrame = " + m_setTargetsThisFrame);
    //    //}
    //    //if (m_setTargetsThisFrame > 100)
    //    //{
    //    //    Debug.Log("Aborted: m_setTargetsThisFrame > " + m_setTargetsThisFrame);
    //    //    return;
    //    //}
    //    //m_setTargetsThisFrame++;

    //    //deregisterAllTargetedBy();
    //    //if (target == null)
    //    //    m_target = null;
    //    //else
    //    //{
    //    //    m_target = target.GetComponent<CubeEntityTargetManager>();
    //    //    m_target.registerTargetedBy(this);
    //    //}
    //    //setOrigin(origin);
    //}
    public void setTarget(CubeEntityTargetManager target)
    {
        if (m_setTargetsThisFrame > 10)
        {
            Debug.Log("Warning: m_setTargetsThisFrame = " + m_setTargetsThisFrame);
        }
        if (m_setTargetsThisFrame > 100)
        {
            Debug.Log("Aborted: m_setTargetsThisFrame > " + m_setTargetsThisFrame);
            return;
        }
        m_setTargetsThisFrame++;

        if (m_target == target)
        {
            return;
        }


        if (m_target != null)
        {
            m_target.deregisterTargetedBy(this);
        }

        if (target == null)
        {
            m_target = null;
        }
        else
        {
            m_target = target;
            m_target.registerTargetedBy(this);
        }

        IOnTargetChange[] onTargetChanges = GetComponentsInChildren<IOnTargetChange>();
        for (int i = 0; i < onTargetChanges.Length; i++)
            onTargetChanges[i].onTargetChange(m_target);
    }
    //public void setTarget(CubeEntityTargetManager target, CubeEntityTargetManager origin)
    //{
    //    Debug.Log("Aborted: Yet to be implemented!");
    //    return;

    //    //if (m_setTargetsThisFrame > 10)
    //    //{
    //    //    Debug.Log("Warning: m_setTargetsThisFrame = " + m_setTargetsThisFrame);
    //    //}
    //    //if (m_setTargetsThisFrame > 100)
    //    //{
    //    //    Debug.Log("Aborted: m_setTargetsThisFrame > " + m_setTargetsThisFrame);
    //    //    return;
    //    //}
    //    //m_setTargetsThisFrame++;

    //    //deregisterAllTargetedBy();
    //    //if (target == null)
    //    //    m_target = null;
    //    //else
    //    //{
    //    //    m_target = target;
    //    //    m_target.registerTargetedBy(this);
    //    //}
    //    //setOrigin(origin);
    //}
    public void setTarget(CubeEntityTargetManager target, GameObject origin)
    {
        setTarget(target);
        setOrigin(origin);
        return;

        if (m_setTargetsThisFrame > 10)
        {
            Debug.Log("Warning: m_setTargetsThisFrame = " + m_setTargetsThisFrame);
        }
        if (m_setTargetsThisFrame > 100)
        {
            Debug.Log("Aborted: m_setTargetsThisFrame > " + m_setTargetsThisFrame);
            return;
        }
        m_setTargetsThisFrame++;

        deregisterAllTargetedBy();
        if (target == null)
            m_target = null;
        else
        {
            m_target = target;
            m_target.registerTargetedBy(this);
        }
        setOrigin(origin);
    }
    public CubeEntityTargetManager getOrigin()
    {
        return m_origin;
    }
    public void setOrigin(CubeEntityTargetManager origin)
    {
        m_origin = origin;
    }
    public void setOrigin(GameObject origin)
    {
        CubeEntityTargetManager targetManager = origin.GetComponent<CubeEntityTargetManager>();
        if (targetManager == null)
        {
            Debug.Log("Warning: targetManager was null!");
        }
        m_origin = targetManager;
    }
    public void resetScript()
    {
        deregisterAllTargetedBy();
        m_target = null;
        m_origin = null;
    }


    // manage targeted by
    public void registerTargetedBy(CubeEntityTargetManager targetedBy)
    {

        if (targetedBy == null)
        {
            Debug.Log("Aborted: targetedBy was null!");
            return;
        }
        if (m_targetedByCore.Contains(targetedBy))
        {
            Debug.Log("Aborted: targetedBy was already in the list!");
            return;
        }


        if (targetedBy.GetComponent<CubeEntityState>().m_state == CubeEntityState.s_STATE_ACTIVE)
        {
            m_targetedByActive.Add(targetedBy);

            IOnTargetedBy[] onTargetedBys = GetComponentsInChildren<IOnTargetedBy>();
            for (int i = 0; i < onTargetedBys.Length; i++)
            {
                onTargetedBys[i].onTargetedByActive(targetedBy);
            }
        }
        else if (targetedBy.GetComponent<CubeEntityState>().m_state == CubeEntityState.s_STATE_CORE)
        {
            m_targetedByCore.Add(targetedBy);

            IOnTargetedBy[] onTargetedBys = GetComponentsInChildren<IOnTargetedBy>();
            for (int i = 0; i < onTargetedBys.Length; i++)
            {
                onTargetedBys[i].onTargetedByCore(targetedBy);
            }
        }
        else
            Debug.Log("Warning: This should not have happend!");

        
    }
    void deregisterTargetedBy(CubeEntityTargetManager targetedBy)
    {
        if (targetedBy == null)
        {
            Debug.Log("Aborted: targetedBy was null!");
            return;
        }
        //if (!m_targetedByCore.Contains(targetedBy))
        //{
        //    Debug.Log("Warning: targetedBy was not in the list!");
        //    return;
        //}

        if (targetedBy.GetComponent<CubeEntityState>().m_state == CubeEntityState.s_STATE_ACTIVE)
            m_targetedByActive.Remove(targetedBy);
        else if (targetedBy.GetComponent<CubeEntityState>().m_state == CubeEntityState.s_STATE_CORE)
            m_targetedByCore.Remove(targetedBy);
        else
             Debug.Log("Warning: This should not have happend!");

    }
    void deregisterAllTargetedBy()
    {
        deregisterAllTargetedByActive();
        deregisterAllTargetedByCore();
    }

    void deregisterAllTargetedByActive()
    {
        for (int i = m_targetedByActive.Count - 1; i >= 0; i--)
        {
            deregisterTargetedBy(m_targetedByActive[i]);
        }
    }

    void deregisterAllTargetedByCore()
    {
        for (int i = m_targetedByCore.Count - 1; i >= 0; i--)
        {
            deregisterTargetedBy(m_targetedByCore[i]);
        }
    }

    // interface
    public void onStateChangePrepareRemove()
    {
        //deregisterAllTargetedBy();
        if (m_target != null)
        {
            m_target.deregisterTargetedBy(this);
        }
        resetScript();
    }
}
