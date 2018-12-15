using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CemBoidRuleAlignment : CemBoidRuleBase, ICopiable, IRemoveOnStateChange
{
    [Header("------- Settings -------")]
    public int m_alignmentPerFrame;
    public float m_alignmentMinPercentPerFrame;
    public float m_alignmentPower;
    public float m_alignmentRadius;
    public int m_alignmentMaxPartners;
    public int m_alignmentMaxPartnerChecks;

    [Header("--- (Leader) ---")]
    public float m_alignmentAffectLeader;

    [Header("--- (Adjust Radius) ---")]
    public bool m_useAdjustRadius;
    public int m_alignmentMinAdjustmentDifference;
    public float m_alignmentMinRadius;
    public float m_alignmentAdjustStep;

    [Header("--- (Line of Sight) ---")]
    public bool m_requireLineOfSight;
    public LayerMask m_layerMask;

    [Header("--- (Angle) ---")]
    public bool m_requireAngle;
    public float m_maxAngle;

    [Header("------- Debug -------")]
    public int m_alignmentCounter;
    Dictionary<GameObject, Vector3> m_alignmentForceVectors;
    Dictionary<GameObject, float> m_alignmentActualRadii;
    public bool m_isInitialized;

    // Use this for initialization
    void Start()
    {
        initializeStuff();
    }
    void initializeStuff()
    {
        m_alignmentForceVectors = new Dictionary<GameObject, Vector3>();
        m_alignmentActualRadii = new Dictionary<GameObject, float>();
    }


    // Update is called once per frame
    void Update ()
    {
        //if(!CemBoidBase.s_calculateInBase && !CemBoidBase.s_calculateInFixedUpdate)
            getInformation();
	}
    private void FixedUpdate()
    {
        if (!CemBoidBase.s_calculateInBase)
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

        int activisionsActually = Mathf.Max(m_alignmentPerFrame, (int)(agents.Count * m_alignmentMinPercentPerFrame));
        if (m_alignmentPerFrame == 0)
            activisionsActually = agents.Count;
        if (m_alignmentPerFrame >= agents.Count)
            activisionsActually = agents.Count;

        for (int i = 0; i < activisionsActually; i++)
        {
            int index = m_alignmentCounter % agents.Count;
            getAlignmentForceVector(agents[index], agents);
            m_alignmentCounter++;
        }
        m_alignmentCounter %= agents.Count;
    }
    public override void getInformation()
    {
        if (m_baseScript == null || !m_useRule)
            return;

        List<GameObject> agents = m_baseScript.m_agents;
        if (agents.Count <= 0)
            return;

        int activisionsActually = Mathf.Max(m_alignmentPerFrame, (int)(agents.Count * m_alignmentMinPercentPerFrame));
        if (m_alignmentPerFrame == 0)
            activisionsActually = agents.Count;
        if (m_alignmentPerFrame >= agents.Count)
            activisionsActually = agents.Count;

        for (int i = 0; i < activisionsActually; i++)
        {
            int index = m_alignmentCounter % agents.Count;
            getAlignmentForceVector(agents[index], agents);
            m_alignmentCounter++;
        }
        m_alignmentCounter %= agents.Count;
    }

    void getAlignmentForceVector(GameObject agent, List<GameObject> agents)
    {
        if (!m_useRule)
            return;

        Vector3 localAverageMovementVector = Vector3.zero;
        int nearAgentsCount = 0;
        int nearObjectsCount = 0;

        Collider[] colliders = Physics.OverlapSphere(agent.transform.position, m_alignmentActualRadii[agent]);
        // adjust radius
        if (m_useAdjustRadius && m_alignmentMaxPartners > 0)
        {
            if (colliders.Length - m_alignmentMaxPartners > m_alignmentMinAdjustmentDifference)
            {
                m_alignmentActualRadii[agent] -= m_alignmentAdjustStep;
            }
            else if (colliders.Length - m_alignmentMaxPartners < -m_alignmentMinAdjustmentDifference)
            {
                m_alignmentActualRadii[agent] += m_alignmentAdjustStep;
            }
            m_alignmentActualRadii[agent] = Mathf.Clamp(m_alignmentActualRadii[agent], m_alignmentMinRadius, m_alignmentRadius);
        }
        foreach (Collider collider in colliders)
        {
            // max partner checks
            if (nearObjectsCount >= m_alignmentMaxPartnerChecks && m_alignmentMaxPartnerChecks > 0)
                break;
            nearObjectsCount++;

            // max partners
            if (nearAgentsCount >= m_alignmentMaxPartners && m_alignmentMaxPartners > 0)
                break;

            // line of sight
            if (m_requireLineOfSight && !isLineOfSight(agent, collider))
                continue;

            // check for angle
            if (m_requireAngle && Utility.getAngle(collider.transform.position - agent.transform.position, agent.transform.forward) > m_maxAngle)
                continue;

            if (collider.GetComponent<CemBoidAttached>() != null && collider.GetComponent<CemBoidAttached>().m_isAttachedToBases.Contains(m_baseScript))
            {
                //float distanceFactor = 1f - (Vector3.Distance(agent.transform.position, collider.transform.position) / m_alignmentRadius);
                if (collider.GetComponent<Rigidbody>() != null)
                    localAverageMovementVector += collider.GetComponent<Rigidbody>().velocity;// * distanceFactor;
                nearAgentsCount++;
            }
        }
        if (nearAgentsCount > 0)
        {
            localAverageMovementVector /= nearAgentsCount;
            m_alignmentForceVectors[agent] = localAverageMovementVector;
            if (agent == m_baseScript.m_leader)
            {
                m_alignmentForceVectors[agent] *= m_alignmentAffectLeader;
            }
        }
        else
        {
            m_alignmentForceVectors[agent] = agent.GetComponent<Rigidbody>().velocity;
        }
    }

    public override void applyRule(List<GameObject> agents)
    {
        if (!m_useRule)
            return;

        foreach (GameObject agent in agents)
        {
            Vector3 velocity = agent.GetComponent<Rigidbody>().velocity;
            agent.GetComponent<Rigidbody>().velocity = Vector3.Lerp(velocity, m_alignmentForceVectors[agent], m_alignmentPower);
        }
    }
    public override void applyRule()
    {
        if (m_baseScript == null || !m_useRule)
            return;

        List<GameObject> agents = m_baseScript.m_agents;
        foreach (GameObject agent in agents)
        {
            Vector3 velocity = agent.GetComponent<Rigidbody>().velocity;            
            agent.GetComponent<Rigidbody>().velocity = Vector3.Lerp(velocity, m_alignmentForceVectors[agent], m_alignmentPower);
        }
    }

    public override void onAddAgent(List<GameObject> agents, GameObject agent)
    {
        if (!m_alignmentForceVectors.ContainsKey(agent))
        {
            m_alignmentForceVectors.Add(agent, Vector3.zero);
            m_alignmentActualRadii.Add(agent, m_alignmentRadius);
        }
        else
            Debug.Log("Warning: Tried to add agent to alignment vectors, but it was already in the list!");
    }
    public override void onRemoveAgent(List<GameObject> agents, GameObject agent)
    {
        if (m_alignmentForceVectors.ContainsKey(agent))
        {
            m_alignmentForceVectors.Remove(agent);
            m_alignmentActualRadii.Remove(agent);
        }
        else
            Debug.Log("Warning: Tried to remove agent from alignment vectors, but it was not in the list!");
    }

    // utility
    bool isLineOfSight(GameObject from, Collider to)
    {
        Vector3 direction = to.transform.position - from.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(from.transform.position, direction, out hit, direction.magnitude, m_layerMask) && hit.collider != to)
        {
            return false;
        }
        return true;
    }
    
    public void resetRadii()
    {
        List<GameObject> agnets = new List<GameObject>(m_alignmentActualRadii.Keys);
        foreach (GameObject agent in agnets)
            m_alignmentActualRadii[agent] = m_alignmentRadius;
    }

    // copy
    public override void setValues(CemBoidRuleBase copyScript)
    {
        if (copyScript == null)
        {
            Debug.Log("Aborted: CemBoidRuleAlignment script was null!");
            return;
        }
        if (copyScript.GetType() != this.GetType())
        {
            Debug.Log("Aborted: Copy script wasn't alignment script!");
            return;
        }

        CemBoidRuleAlignment copyScript2 = (CemBoidRuleAlignment)copyScript;

        m_useRule = copyScript2.m_useRule;

        m_alignmentPerFrame = copyScript2.m_alignmentPerFrame;
        m_alignmentMinPercentPerFrame = copyScript2.m_alignmentMinPercentPerFrame;
        m_alignmentPower = copyScript2.m_alignmentPower;
        m_alignmentRadius = copyScript2.m_alignmentRadius;
        m_alignmentMaxPartners = copyScript2.m_alignmentMaxPartners;
        m_alignmentMaxPartnerChecks = copyScript2.m_alignmentMaxPartnerChecks;

        m_alignmentAffectLeader = copyScript2.m_alignmentAffectLeader;

        m_useAdjustRadius = copyScript2.m_useAdjustRadius;
        m_alignmentMinAdjustmentDifference = copyScript2.m_alignmentMinAdjustmentDifference;
        m_alignmentMinRadius = copyScript2.m_alignmentMinRadius;
        m_alignmentAdjustStep = copyScript2.m_alignmentAdjustStep;

        m_requireLineOfSight = copyScript2.m_requireLineOfSight;
        m_requireAngle = copyScript2.m_requireAngle;
        m_maxAngle = copyScript2.m_maxAngle;

        List<GameObject> agnets = new List<GameObject>(m_alignmentForceVectors.Keys);
        foreach (GameObject agent in agnets)
            m_alignmentForceVectors[agent] = Vector3.zero;

        agnets = new List<GameObject>(m_alignmentActualRadii.Keys);
        foreach (GameObject agent in agnets)
            m_alignmentActualRadii[agent] = m_alignmentRadius;
    }

    // interface
    public void onCopy(ICopiable baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CemBoidRuleBase)baseScript);
    }
    public void onRemove()
    {
        Destroy(this);
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
        
    }*/
}
