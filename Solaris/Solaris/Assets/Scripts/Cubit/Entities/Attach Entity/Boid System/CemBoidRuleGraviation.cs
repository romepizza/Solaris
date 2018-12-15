using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CemBoidRuleGraviation : CemBoidRuleBase, ICopiable
{

    [Header("------- Settings -------")]
    public Vector3 m_gravitationCenterOffset;
    //public bool m_useGravitation = true;
    public bool m_gravitationUseSwamCenter;
    public GameObject m_gravitationCenterObject; // If equal to null: gravitation center equals center of swarm
    public int m_gravitationPerFrame;
    public float m_gravitationMinPercentPerFrame;
    public float m_gravitationPower;
    public float m_gravitationMaxSpeed;
    public float m_gravitationDampenRadius;

    [Header("--- (Leader) ---")]
    public float m_gravitationAffectLeader;

    [Header("------- Debug -------")]
    public int m_gravitationCounter;
    Dictionary<GameObject, Vector3> m_gravitationForceVectors;
    public Vector3 m_gravitationCenter;
    public bool m_isInitialized;

    void Start()
    {
        initializeStuff();
    }
    void initializeStuff()
    {
        m_gravitationForceVectors = new Dictionary<GameObject, Vector3>();
    }

    void Update()
    {
        //if(!CemBoidBase.s_calculateInBase && !CemBoidBase.s_calculateInFixedUpdate)
            getInformation();
    }
    private void FixedUpdate()
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

        // get gravitation center
        getGravitationCenter();

        // determine, which force vectors of which agents should be calculated
        int activisionsActually = Mathf.Max(m_gravitationPerFrame, (int)(agents.Count * m_gravitationMinPercentPerFrame));
        if (m_gravitationPerFrame == 0)
            activisionsActually = agents.Count;
        if (m_gravitationPerFrame >= agents.Count)
            activisionsActually = agents.Count;

        for (int i = 0; i < activisionsActually; i++)
        {
            int index = m_gravitationCounter % agents.Count;
            getGravitationForceVector(agents[index], agents);
            m_gravitationCounter++;
        }
        m_gravitationCounter %= agents.Count;
    }
    public override void getInformation()
    {
        if (m_baseScript == null || !m_useRule)
            return;

        List<GameObject> agents = m_baseScript.m_agents;
        if (agents.Count <= 0)
            return;

        // get gravitation center
        getGravitationCenter();

        int activisionsActually = Mathf.Max(m_gravitationPerFrame, (int)(agents.Count * m_gravitationMinPercentPerFrame));
        if (m_gravitationPerFrame == 0)
            activisionsActually = agents.Count;
        if (m_gravitationPerFrame >= agents.Count)
            activisionsActually = agents.Count;

        for (int i = 0; i < activisionsActually; i++)
        {
            int index = m_gravitationCounter % agents.Count;
            getGravitationForceVector(agents[index], agents);
            m_gravitationCounter++;
        }
        m_gravitationCounter %= agents.Count;
    }

    void getGravitationForceVector(GameObject agent, List<GameObject> agents)
    {
        if (!m_useRule)
            return;


        Vector3 direction = (m_gravitationCenter - agent.transform.position);
        Vector3 flightDirection = agent.GetComponent<Rigidbody>().velocity;
        float angle = Vector3.Dot(direction.normalized, flightDirection.normalized);

        if (m_gravitationMaxSpeed <= 0  || angle < 0 || flightDirection.magnitude < m_gravitationMaxSpeed)
        {
            float distanceFactor = Mathf.Clamp01((Vector3.Distance(agent.transform.position, m_gravitationCenter) / m_gravitationDampenRadius));
            
            m_gravitationForceVectors[agent] = direction.normalized * m_gravitationPower * distanceFactor;
            if (agent == m_baseScript.m_leader)
            {
                m_gravitationForceVectors[agent] *= m_gravitationAffectLeader;
            }
        }
    }

    // manage rule
    public override void applyRule(List<GameObject> agents)
    {
        if (!m_useRule)
            return;

        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Rigidbody>().AddForce(m_gravitationForceVectors[agent], ForceMode.Acceleration);
        }
    }
    public override void applyRule()
    {
        if (m_baseScript == null || !m_useRule)
            return;

        List<GameObject> agents = m_baseScript.m_agents;
        foreach (GameObject agent in agents)
        {
            try
            {
                agent.GetComponent<Rigidbody>().AddForce(m_gravitationForceVectors[agent], ForceMode.Acceleration);
            }
            catch (System.Exception e)
            {

            }
        }
    }

    // manage agent
    public override void onAddAgent(List<GameObject> agents, GameObject agent)
    {
        if (!m_gravitationForceVectors.ContainsKey(agent))
            m_gravitationForceVectors.Add(agent, Vector3.zero);
        else
            Debug.Log("Warning: Tried to add agent to gravitation vectors, but it was already in the list!");
    }
    public override void onRemoveAgent(List<GameObject> agents, GameObject agent)
    {
        if (m_gravitationForceVectors.ContainsKey(agent))
            m_gravitationForceVectors.Remove(agent);
        else
            ;// Debug.Log("Warning: Tried to remove agent from gravitation vectors, but it was not in the list!");
    }

    // utility
    public void getGravitationCenter()
    {
        if (m_gravitationUseSwamCenter)
        {
            m_gravitationCenter = m_baseScript.getAverageSwarmPosition();
        }
        else if (m_gravitationCenterObject != null)
        {
            m_gravitationCenter = m_gravitationCenterObject.transform.position;
        }
        else if (m_baseScript.m_leader != null)
        {
            m_gravitationCenter = m_baseScript.m_leader.transform.position;
        }
        else
        {
            m_gravitationCenter = m_baseScript.getAverageSwarmPosition();
        }

        Vector3 offset = m_gravitationCenterOffset;
        //Vector3 offset = Camera.main.transform.rotation * m_gravitationCenterOffset;
        m_gravitationCenter += offset;
    }

    // copy
    public override void setValues(CemBoidRuleBase copyScript)
    {
        if (copyScript == null)
        {
            Debug.Log("Aborted: CemBoidRuleGraviation script was null!");
            return;
        }
        if (copyScript.GetType() != this.GetType())
        {
            Debug.Log("Aborted: Copy script wasn't gravitation script!");
            return;
        }

        CemBoidRuleGraviation copyScript2 = (CemBoidRuleGraviation)copyScript;

        m_useRule = copyScript2.m_useRule;

        m_gravitationPerFrame = copyScript2.m_gravitationPerFrame;
        m_gravitationMinPercentPerFrame = copyScript2.m_gravitationMinPercentPerFrame;
        m_gravitationPower = copyScript2.m_gravitationPower;
        m_gravitationUseSwamCenter = copyScript2.m_gravitationUseSwamCenter;
        m_gravitationMaxSpeed = copyScript2.m_gravitationMaxSpeed;
        m_gravitationDampenRadius = copyScript2.m_gravitationDampenRadius;
        m_gravitationAffectLeader = copyScript2.m_gravitationAffectLeader;
        m_gravitationCenterOffset = copyScript2.m_gravitationCenterOffset;

        List<GameObject> agnets = new List<GameObject>(m_gravitationForceVectors.Keys);
        foreach (GameObject agent in agnets)
            m_gravitationForceVectors[agent] = Vector3.zero;
    }

    // interface
    public void onCopy(ICopiable baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CemBoidRuleBase)baseScript);
    }

    // abstract
    /*
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
        m_gravitationForceVectors = new Dictionary<GameObject, Vector3>();
    }
    */
}
