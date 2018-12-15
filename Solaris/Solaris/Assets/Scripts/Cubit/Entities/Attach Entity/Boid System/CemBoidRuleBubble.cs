using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CemBoidRuleBubble : CemBoidRuleBase, ICopiable
{
    [Header("------- Settings -------")]
    //public bool m_useBubble;
    public bool m_bubbleUseSwamCenter;
    public GameObject m_bubbleCenterObject;
    public int m_bubblePerFrame;
    public float m_bubbleMinPercentPerFrame;
    public float m_bubblePower;
    public float m_bubbleMaxDistance;
    public float m_bubbleMaxSpeed;

    [Header("--- (Leader) ---")]
    public float m_bubbleAffectLeader;

    [Header("------- Debug -------")]
    public Vector3 m_bubbleCenter;
    public int m_bubbleCounter;
    Dictionary<GameObject, Vector3> m_bubbleForceVectors;
    public bool m_isInitialized;

    void Start()
    {
        initializeStuff();
    }
    void initializeStuff()
    {
        m_bubbleForceVectors = new Dictionary<GameObject, Vector3>();
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

        // get bubble center
        if(m_bubbleUseSwamCenter)
        {
            m_bubbleCenter = m_baseScript.getAverageSwarmPosition();
        }
        else if (m_bubbleCenterObject != null)
        {
            m_bubbleCenter = m_bubbleCenterObject.transform.position;
        }
        else
        {
            m_bubbleCenter = m_baseScript.getAverageSwarmPosition();
        }

        int activisionsActually = Mathf.Max(m_bubblePerFrame, (int)(agents.Count * m_bubbleMinPercentPerFrame));
        if (m_bubblePerFrame == 0)
            activisionsActually = agents.Count;
        if (m_bubblePerFrame >= agents.Count)
            activisionsActually = agents.Count;

        for (int i = 0; i < activisionsActually; i++)
        {
            int index = m_bubbleCounter % agents.Count;
            getSeparationForceVector(agents[index], agents);
            m_bubbleCounter++;
        }
        m_bubbleCounter %= agents.Count;
    }
    public override void getInformation()
    {
        if (m_baseScript == null || !m_useRule)
            return;

        List<GameObject> agents = m_baseScript.m_agents;
        if (agents.Count <= 0)
            return;

        // get bubble center
        if (m_bubbleUseSwamCenter)
        {
            m_bubbleCenter = m_baseScript.getAverageSwarmPosition();
        }
        else if (m_bubbleCenterObject != null)
        {
            m_bubbleCenter = m_bubbleCenterObject.transform.position;
        }
        else if (m_baseScript.m_leader != null)
        {
            m_bubbleCenter = m_baseScript.m_leader.transform.position;
        }
        else
        {
            m_bubbleCenter = m_baseScript.getAverageSwarmPosition();
        }



        int activisionsActually = Mathf.Max(m_bubblePerFrame, (int)(agents.Count * m_bubbleMinPercentPerFrame));
        if (m_bubblePerFrame == 0)
            activisionsActually = agents.Count;
        if (m_bubblePerFrame >= agents.Count)
            activisionsActually = agents.Count;

        for (int i = 0; i < activisionsActually; i++)
        {
            int index = m_bubbleCounter % agents.Count;
            getSeparationForceVector(agents[index], agents);
            m_bubbleCounter++;
        }
        m_bubbleCounter %= agents.Count;
    }

    void getSeparationForceVector(GameObject agent, List<GameObject> agents)
    {
        if (!m_useRule)
            return;

        m_bubbleForceVectors[agent] = Vector3.zero;
        float distance = Vector3.Distance(agent.transform.position, m_bubbleCenter);
        if (distance > m_bubbleMaxDistance)
        {
            Vector3 direction = (m_bubbleCenter - agent.transform.position);
            Vector3 flightDirection = agent.GetComponent<Rigidbody>().velocity;
            float angle = Vector3.Dot(direction.normalized, flightDirection.normalized);

            if (!(/*angle > 0.5f && */flightDirection.magnitude > m_bubbleMaxSpeed))
            {
                float distanceFactor = Mathf.Clamp01((distance - m_bubbleMaxDistance) / m_bubbleMaxDistance * 0.2f);
                m_bubbleForceVectors[agent] = direction.normalized * m_bubblePower * distanceFactor;
                if (agent == m_baseScript.m_leader)
                {
                    m_bubbleForceVectors[agent] *= m_bubbleAffectLeader;
                }
            }
        }
    }

    public override void applyRule(List<GameObject> agents)
    {
        if (!m_useRule)
            return;

        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Rigidbody>().AddForce(m_bubbleForceVectors[agent], ForceMode.Acceleration);
        }
    }
    public override void applyRule()
    {
        if (m_baseScript == null || !m_useRule)
            return;

        List<GameObject> agents = m_baseScript.m_agents;
        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Rigidbody>().AddForce(m_bubbleForceVectors[agent], ForceMode.Acceleration);
        }
    }

    public override void onAddAgent(List<GameObject> agents, GameObject agent)
    {
        if (!m_bubbleForceVectors.ContainsKey(agent))   
            m_bubbleForceVectors.Add(agent, Vector3.zero);
        else
            Debug.Log("Warning: Tried to add agent to bubble vectors, but it was already in the list!");
    }
    public override void onRemoveAgent(List<GameObject> agents, GameObject agent)
    {
        if (m_bubbleForceVectors.ContainsKey(agent))
            m_bubbleForceVectors.Remove(agent);
        else
            Debug.Log("Warning: Tried to remove agent from bubble vectors, but it was not in the list!");
    }

    // copy
    public override void setValues(CemBoidRuleBase copyScript)
    {
        if (copyScript == null)
        {
            Debug.Log("Aborted: CemBoidRuleBubble script was null!");
            return;
        }
        if (copyScript.GetType() != this.GetType())
        {
            Debug.Log("Aborted: Copy script wasn't bubble script!");
            return;
        }

        CemBoidRuleBubble copyScript2 = (CemBoidRuleBubble)copyScript;

        m_useRule = copyScript2.m_useRule;

        m_bubblePerFrame = copyScript2.m_bubblePerFrame;
        m_bubbleMinPercentPerFrame = copyScript2.m_bubbleMinPercentPerFrame;
        m_bubblePower = copyScript2.m_bubblePower;
        m_bubbleUseSwamCenter = copyScript2.m_bubbleUseSwamCenter;
        m_bubbleMaxDistance = copyScript2.m_bubbleMaxDistance;
        m_bubbleMaxSpeed = copyScript2.m_bubbleMaxSpeed;

        List<GameObject> agnets = new List<GameObject>(m_bubbleForceVectors.Keys);
        foreach (GameObject agent in agnets)
            m_bubbleForceVectors[agent] = Vector3.zero;
    }

    // interface
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

    }*/
}
