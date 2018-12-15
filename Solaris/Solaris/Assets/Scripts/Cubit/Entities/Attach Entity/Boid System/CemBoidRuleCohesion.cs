using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CemBoidRuleCohesion : CemBoidRuleBase, ICopiable
{
    [Header("------- Settings -------")]
    public int m_cohesionPerFrame;
    public float m_cohesionMinPercentPerFrame;
    public float m_cohesionPower;
    public float m_cohesionRadius;
    public int m_cohesionMaxPartners;
    public int m_cohesionMaxPartnerChecks;

    [Header("--- (Leader) ---")]
    public float m_cohesionAffectLeader;

    [Header("--- (Adjust Radius) ---")]
    public bool m_useAdjustRadius;
    public int m_cohesionMinAdjustmentDifference;
    public float m_cohesionMinRadius;
    public float m_cohesionAdjustStep;

    [Header("--- (Line of Sight) ---")]
    public bool m_requireLineOfSight;
    public LayerMask m_layerMask;

    [Header("--- (Angle) ---")]
    public bool m_requireAngle;
    public float m_maxAngle;

    [Header("------- Debug -------")]
    public int m_cohesionCounter;
    Dictionary<GameObject, Vector3> m_cohesionForceVectors;
    Dictionary<GameObject, float> m_cohesionActualRadii;
    public bool m_isInitialized;

    // Use this for initialization
    void Start ()
    {
        initializeStuff();
    }
    void initializeStuff()
    {
        m_cohesionForceVectors = new Dictionary<GameObject, Vector3>();
        m_cohesionActualRadii = new Dictionary<GameObject, float>();
    }

    private void Update()
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

        int activisionsActually = Mathf.Max(m_cohesionPerFrame, (int)(agents.Count * m_cohesionMinPercentPerFrame));
        if (m_cohesionPerFrame == 0)
            activisionsActually = agents.Count;
        if (m_cohesionPerFrame >= agents.Count)
            activisionsActually = agents.Count;

        for (int i = 0; i < activisionsActually; i++)
        {
            int index = m_cohesionCounter % agents.Count;
            getCohesionForceVector(agents[index], agents);
            m_cohesionCounter++;
        }
        m_cohesionCounter %= agents.Count;
    }
    public override void getInformation()
    {
        if (m_baseScript == null || !m_useRule)
            return;

        List<GameObject> agents = m_baseScript.m_agents;
        if (agents.Count <= 0)
            return;

        int activisionsActually = Mathf.Max(m_cohesionPerFrame, (int)(agents.Count * m_cohesionMinPercentPerFrame));
        if (m_cohesionPerFrame == 0)
            activisionsActually = agents.Count;
        if (m_cohesionPerFrame >= agents.Count)
            activisionsActually = agents.Count;

        for (int i = 0; i < activisionsActually; i++)
        {
            int index = m_cohesionCounter % agents.Count;
            getCohesionForceVector(agents[index], agents);
            m_cohesionCounter++;
        }
        m_cohesionCounter %= agents.Count;
    }

    void getCohesionForceVector(GameObject agent, List<GameObject> agents)
    {
        if (!m_useRule)
            return;

        Vector3 localCenter = Vector3.zero;
        int nearAgentsCount = 0;
        int nearObjectsCount = 0;
        
        if (m_cohesionRadius <= 0)
        {
            foreach (GameObject agent2 in agents)
            {
                // line of sight
                if (m_requireLineOfSight && !isLineOfSight(agent, agent2))
                    continue;

                // check for angle
                if (m_requireAngle && Utility.getAngle(agent2.transform.position - agent.transform.position, agent.transform.forward) > m_maxAngle)
                    continue;

                localCenter += agent2.transform.position;
                nearAgentsCount++;
            }
        }
        else
        {
            Collider[] colliders = Physics.OverlapSphere(agent.transform.position, m_cohesionActualRadii[agent]);
            // adjust radius
            if (m_useAdjustRadius && m_cohesionMaxPartners > 0)
            {
                if (colliders.Length - m_cohesionMaxPartners > m_cohesionMinAdjustmentDifference)
                {
                    m_cohesionActualRadii[agent] -= m_cohesionAdjustStep;
                }
                else if(colliders.Length - m_cohesionMaxPartners < -m_cohesionMinAdjustmentDifference)
                {
                    m_cohesionActualRadii[agent] += m_cohesionAdjustStep;
                }
                m_cohesionActualRadii[agent] = Mathf.Clamp(m_cohesionActualRadii[agent], m_cohesionMinRadius, m_cohesionRadius);
            }
            foreach (Collider collider in colliders)
            {
                // check max partner
                if (nearObjectsCount >= m_cohesionMaxPartnerChecks && m_cohesionMaxPartnerChecks > 0)
                    break;
                nearObjectsCount++;

                // check max partners
                if (nearAgentsCount >= m_cohesionMaxPartners && m_cohesionMaxPartners > 0)
                    break;
                
                // line of sight
                if (m_requireLineOfSight && !isLineOfSight(agent, collider))
                    continue;

                // check for angle
                if (m_requireAngle && Utility.getAngle(collider.transform.position - agent.transform.position, agent.transform.forward) > m_maxAngle)
                    continue;

                if (!(collider.GetComponent<CemBoidAttached>() != null && collider.GetComponent<CemBoidAttached>().m_isAttachedToBases.Contains(m_baseScript)))
                    return;
                
                localCenter += collider.gameObject.transform.position;

                nearAgentsCount++;
                
            }
        }

        if (nearAgentsCount > 0)
        {
            localCenter /= nearAgentsCount;
            m_cohesionForceVectors[agent] = (localCenter - agent.transform.position).normalized * m_cohesionPower;
            if(agent == m_baseScript.m_leader)
            {
                m_cohesionForceVectors[agent] *= m_cohesionAffectLeader;
            }
        }
        else
        {
            m_cohesionForceVectors[agent] = Vector3.zero;
        }
    }

    public override void applyRule(List<GameObject> agents)
    {
        if (!m_useRule)
            return;

        foreach(GameObject agent in agents)
        {
            agent.GetComponent<Rigidbody>().AddForce(m_cohesionForceVectors[agent], ForceMode.Acceleration);
        }
    }
    public override void applyRule()
    {
        if (m_baseScript == null || !m_useRule)
            return;

        List<GameObject> agents = m_baseScript.m_agents;
        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Rigidbody>().AddForce(m_cohesionForceVectors[agent], ForceMode.Acceleration);
        }
    }

    // on action
    public override void onAddAgent(List<GameObject> agents, GameObject agent)
    {
        if (!m_cohesionForceVectors.ContainsKey(agent))
        {
            m_cohesionForceVectors.Add(agent, Vector3.zero);
            m_cohesionActualRadii.Add(agent, m_cohesionRadius);
        }
        else
            Debug.Log("Warning: Tried to add agent to cohesion vectors, but it was already in the list!");
    }
    public override void onRemoveAgent(List<GameObject> agents, GameObject agent)
    {
        if (m_cohesionForceVectors.ContainsKey(agent))
        {
            m_cohesionForceVectors.Remove(agent);
            m_cohesionActualRadii.Remove(agent);
        }
        else
            Debug.Log("Warning: Tried to remove agent from cohesion vectors, but it was not in the list!");
    }

    // utility
    bool isLineOfSight(GameObject from, GameObject to)
    {
        Vector3 direction = to.transform.position - from.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(from.transform.position, direction, out hit, direction.magnitude, m_layerMask) && hit.collider.gameObject != to)
        {
            return false;
        }
        return true;
    }
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
        List<GameObject> agents = new List<GameObject>(m_cohesionActualRadii.Keys);
        foreach (GameObject agent in agents)
            m_cohesionActualRadii[agent] = m_cohesionRadius;
    }

    // copy
    public override void setValues(CemBoidRuleBase copyScript)
    {
        if (copyScript == null)
        {
            Debug.Log("Aborted: CemBoidRuleCohesion script was null!");
            return;
        }
        if(copyScript.GetType() != this.GetType())
        {
            Debug.Log("Aborted: Copy script wasn't cohesion script!");
            return;
        }

        CemBoidRuleCohesion copyScript2 = (CemBoidRuleCohesion)copyScript;

        m_useRule = copyScript2.m_useRule;

        m_cohesionPerFrame = copyScript2.m_cohesionPerFrame;
        m_cohesionMinPercentPerFrame = copyScript2.m_cohesionMinPercentPerFrame;
        m_cohesionPower = copyScript2.m_cohesionPower;
        m_cohesionRadius = copyScript2.m_cohesionRadius;
        m_cohesionMaxPartners = copyScript2.m_cohesionMaxPartners;
        m_cohesionMaxPartnerChecks = copyScript2.m_cohesionMaxPartnerChecks;

        m_cohesionAffectLeader = copyScript2.m_cohesionAffectLeader;

        m_useAdjustRadius = copyScript2.m_useAdjustRadius;
        m_cohesionMinAdjustmentDifference = copyScript2.m_cohesionMinAdjustmentDifference;
        m_cohesionMinRadius = copyScript2.m_cohesionMinRadius;
        m_cohesionAdjustStep = copyScript2.m_cohesionAdjustStep;

        m_requireLineOfSight = copyScript2.m_requireLineOfSight;

        m_requireAngle = copyScript2.m_requireAngle;
        m_maxAngle = copyScript2.m_maxAngle;

        List<GameObject> agents = new List<GameObject>(m_cohesionForceVectors.Keys);
        foreach (GameObject agent in agents)
            m_cohesionForceVectors[agent] = Vector3.zero;

        agents = new List<GameObject>(m_cohesionActualRadii.Keys);
        foreach (GameObject agent in agents)
            m_cohesionActualRadii[agent] = m_cohesionRadius;
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
