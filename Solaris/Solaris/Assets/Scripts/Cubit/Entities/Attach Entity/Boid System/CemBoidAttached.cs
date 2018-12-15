using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CemBoidAttached : MonoBehaviour
{
    public List<CemBoidBase> m_isAttachedToBases;
    public CemBoidBase m_isAttachedToBase;
    public List<CemBoidRuleBase> m_predatorBaseScripts;


    void Awake()
    {
        m_isAttachedToBases = new List<CemBoidBase>();
        m_predatorBaseScripts = new List<CemBoidRuleBase>();
    }

    public void removeFromSwarm()
    {
        if(m_isAttachedToBase == null)
        {
            Debug.Log("Aborted: swarmScript was null!");
            return;
        }

        m_isAttachedToBase.removeAgent(gameObject);

        if (m_predatorBaseScripts.Count == 0)
            Destroy(this);
    }
    public void deregisterSwarm(CemBoidBase swarmScript)
    {
        m_isAttachedToBases.Remove(swarmScript);
        if(m_isAttachedToBases.Count <= 0)
        {
            Destroy(this);
        }
    }
    public bool registerSwarm(CemBoidBase swarmScript)
    {
        if(!m_isAttachedToBases.Contains(swarmScript))
        {
            m_isAttachedToBases.Add(swarmScript);
            return true;
        }
        return false;
    }
}
