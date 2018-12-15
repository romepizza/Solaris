using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CemBoidRuleSwarmMovement : CemBoidRuleBase, ICopiable
{

    [Header("------- Settings -------")]
    public Vector3 m_swarmMovementOffset;
    //public bool m_useSwarmMovement = true;
    public GameObject m_swarmMovementObject;
    //public int m_swarmMovementPerFrame;
    public float m_swarmMovementPower;

    [Header("--- (Leader) ---")]
    public float m_swarmMovementAffectLeader;

    [Header("------- Debug -------")]
    public int m_swarmMovementCounter;
    Vector3 m_swarmMovementForceVector;
    Vector3 m_swarmMovementTargetPoint;
    public bool m_applyRule;
    public bool m_isInitialized;

    void Start()
    {
        initializeStuff();
    }
    void initializeStuff()
    {
        
    }

    void Update()
    {
        //if(!CemBoidBase.s_calculateInBase && !CemBoidBase.s_calculateInFixedUpdate)
            getInformation();
    }
    void FixedUpdate()
    {
        //if (!CemBoidBase.s_calculateInBase)
        {
            //if (CemBoidBase.s_calculateInFixedUpdate)
                //getInformation();
            applyRule();
        }
    }

    public override void getInformation(List<GameObject> agents)
    {
        if (!m_useRule)
            return;

        if (!getSwarmMovementVector())
        {
            return;
        }
        m_applyRule = true;

        getSwarmMovementForceVector(agents);
    }
    public override void getInformation()
    {
        if (m_baseScript == null || !m_useRule)
            return;

        if (!getSwarmMovementVector())
        {
            return;
        }
        
        m_applyRule = true;

        List<GameObject> agents = m_baseScript.m_agents;
        if (agents.Count <= 0)
            return;
        
        getSwarmMovementForceVector(agents);
    }

    void getSwarmMovementForceVector(List<GameObject> agents)
    {
        float distanceFactor = Mathf.Clamp01((Vector3.Distance(m_baseScript.getAverageSwarmPosition(), m_swarmMovementTargetPoint) / 100f));
        m_swarmMovementForceVector = (m_swarmMovementTargetPoint - m_baseScript.getAverageSwarmPosition()).normalized * m_swarmMovementPower * distanceFactor;
    }

    // manage rules
    public override void applyRule(List<GameObject> agents)
    {
        if (!m_applyRule || !m_useRule)
            return;

        foreach (GameObject agent in agents)
        {
            Vector3 forceVector = m_swarmMovementForceVector;
            if (agent == m_baseScript.m_leader)
            {
                forceVector *= m_swarmMovementAffectLeader;
            }
            agent.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Acceleration);
        }
        
    }
    public override void applyRule()
    {
        if (m_baseScript == null || !m_useRule || !m_applyRule)
            return;

        List<GameObject> agents = m_baseScript.m_agents;
        foreach (GameObject agent in agents)
        {
            Vector3 forceVector = m_swarmMovementForceVector;
            if (agent == m_baseScript.m_leader)
            {
                forceVector *= m_swarmMovementAffectLeader;
            }
            agent.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Acceleration);
        }
    }

    // manage agents
    public override void onAddAgent(List<GameObject> agents, GameObject agent)
    {

    }
    public override void onRemoveAgent(List<GameObject> agents, GameObject agent)
    {

    }

    // utility
    bool getSwarmMovementVector()
    {
        if (m_swarmMovementObject != null)
            m_swarmMovementTargetPoint = m_swarmMovementObject.transform.position;
        else if (m_baseScript.m_leader != null)
            m_swarmMovementTargetPoint = m_baseScript.m_leader.transform.position;
        else
        {
            m_applyRule = false;
            return false;
        }

        Vector3 offset = m_swarmMovementOffset;
        //Vector3 offset = Camera.main.transform.rotation * m_swarmMovementOffset;
        m_swarmMovementTargetPoint += offset;

        return true;
    }

    // copy
    public override void setValues(CemBoidRuleBase copyScript)
    {
        if (copyScript == null)
        {
            Debug.Log("Aborted: CemBoidRuleSwarmMovement script was null!");
            return;
        }
        if (copyScript.GetType() != this.GetType())
        {
            Debug.Log("Aborted: Copy script wasn't swarmMovement script!");
            return;
        }

        CemBoidRuleSwarmMovement copyScript2 = (CemBoidRuleSwarmMovement)copyScript;

        m_useRule = copyScript2.m_useRule;
        m_swarmMovementPower = copyScript2.m_swarmMovementPower;
    }

    // interfaces
    public void onCopy(ICopiable baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CemBoidRuleBase)baseScript);
    }

    /*
    // abstract
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CemBoidRuleBase)baseScript);
    }
    public override void prepareDestroyScript()
    {
        Destroy(this);
    }
    public override void onPostCopy()
    {

    }
    */
}
