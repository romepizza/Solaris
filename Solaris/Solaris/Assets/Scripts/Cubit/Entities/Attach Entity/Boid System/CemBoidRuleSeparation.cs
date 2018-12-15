using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CemBoidRuleSeparation : CemBoidRuleBase, ICopiable, IPostCopy
{
    [Header("------- Settings -------")]
    public int m_separationPerFrame;
    public float m_separationMinPercentPerFrame;
    public float m_separationPower;
    public float m_separationRadius;
    public int m_separationMaxPartners;
    public int m_separationMaxPartnerChecks;

    [Header("--- (Leader) ---")]
    public float m_separationAffectLeader;

    [Header("--- (Adjust Radius) ---")]
    public bool m_useAdjustRadius;
    public int m_separationMinAdjustmentDifference;
    public float m_separationMinRadius;
    public float m_separationAdjustStep;

    [Header("--- (Line of Sight) ---")]
    public bool m_requireLineOfSight;
    public LayerMask m_layerMask;

    [Header("--- (Angle) ---")]
    public bool m_requireAngle;
    public float m_maxAngle;

    [Header("------- Debug -------")]
    public int m_separationCounter;
    Dictionary<GameObject, Vector3> m_separationForceVectors;
    Dictionary<GameObject, float> m_separationActualRadii;
    public bool m_isInitialized;


    void Start()
    {
        initializeStuff();
    }
    void initializeStuff()
    {
        m_separationForceVectors = new Dictionary<GameObject, Vector3>();
        m_separationActualRadii = new Dictionary<GameObject, float>();
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

        int activisionsActually = Mathf.Max(m_separationPerFrame, (int)(agents.Count * m_separationMinPercentPerFrame));
        if (m_separationPerFrame == 0)
            activisionsActually = agents.Count;
        if (m_separationPerFrame >= agents.Count)
            activisionsActually = agents.Count;

        for (int i = 0; i < activisionsActually; i++)
        {
            int index = m_separationCounter % agents.Count;
            getSeparationForceVector(agents[index], agents);
            m_separationCounter++;
        }
        m_separationCounter %= agents.Count;
    }
    public override void getInformation()
    {
        if (m_baseScript == null || !m_useRule)
            return;

        List<GameObject> agents = m_baseScript.m_agents;
        if (agents.Count <= 0)
            return;

        int activisionsActually = Mathf.Max(m_separationPerFrame, (int)(agents.Count * m_separationMinPercentPerFrame));
        if (m_separationPerFrame == 0)
            activisionsActually = agents.Count;
        if (m_separationPerFrame >= agents.Count)
            activisionsActually = agents.Count;

        for (int i = 0; i < activisionsActually; i++)
        {
            int index = m_separationCounter % agents.Count;
            getSeparationForceVector(agents[index], agents);
            m_separationCounter++;
        }
        m_separationCounter %= agents.Count;
    }

    void getSeparationForceVector(GameObject agent, List<GameObject> agents)
    {
        if (!m_useRule)
            return;
        
        Vector3 forceVector = Vector3.zero;
        int nearAgentsCount = 0;
        int nearObjectsCount = 0;

        Collider[] colliders = Physics.OverlapSphere(agent.transform.position, m_separationActualRadii[agent]);
        // adjust radius
        if (m_useAdjustRadius && m_separationMaxPartners > 0)
        {
            if (colliders.Length - m_separationMaxPartners > m_separationMinAdjustmentDifference)
            {
                m_separationActualRadii[agent] -= m_separationAdjustStep;
            }
            else if (colliders.Length - m_separationMaxPartners < -m_separationMinAdjustmentDifference)
            {
                m_separationActualRadii[agent] += m_separationAdjustStep;
            }
            m_separationActualRadii[agent] = Mathf.Clamp(m_separationActualRadii[agent], m_separationMinRadius, m_separationRadius);
        }
        foreach (Collider collider in colliders)
        {
            // max partner checks
            if (nearObjectsCount >= m_separationMaxPartnerChecks && m_separationMaxPartnerChecks > 0)
                break;
            nearObjectsCount++;

            // max partners
            if (nearAgentsCount >= m_separationMaxPartners && m_separationMaxPartners > 0)
                break;

            // line of sight
            if (m_requireLineOfSight && !isLineOfSight(agent, collider))
                continue;

            // check for angle
            if (m_requireAngle && Utility.getAngle(collider.transform.position - agent.transform.position, agent.transform.forward) > m_maxAngle)
                continue;



            if (collider.GetComponent<CemBoidAttached>() != null && collider.GetComponent<CemBoidAttached>().m_isAttachedToBases.Contains(m_baseScript))
            {
                float distanceFactor = Mathf.Clamp01(1f - (Vector3.Distance(agent.transform.position, collider.transform.position) / m_separationRadius));
                forceVector += (agent.transform.position - collider.gameObject.transform.position) * distanceFactor;
                nearAgentsCount++;

            }
        }
        m_separationForceVectors[agent] = forceVector.normalized * m_separationPower;
        if (agent == m_baseScript.m_leader)
        {
            m_separationForceVectors[agent] *= m_separationAffectLeader;

        }
    }

    public override void applyRule(List<GameObject> agents)
    {
        if (!m_useRule)
            return;

        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Rigidbody>().AddForce(m_separationForceVectors[agent], ForceMode.Acceleration);
        }
    }
    public override void applyRule()
    {
        if (m_baseScript == null || !m_useRule)
            return;

        List<GameObject> agents = m_baseScript.m_agents;
        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Rigidbody>().AddForce(m_separationForceVectors[agent], ForceMode.Acceleration);
        }
    }

    public override void onAddAgent(List<GameObject> agents, GameObject agent)
    {
        if (!m_separationForceVectors.ContainsKey(agent))
        {
            m_separationForceVectors.Add(agent, Vector3.zero);
            m_separationActualRadii.Add(agent, m_separationRadius);
        }
        else
            Debug.Log("Warning: Tried to add agent to separation vectors, but it was already in the list!");
    }
    public override void onRemoveAgent(List<GameObject> agents, GameObject agent)
    {
        if (m_separationForceVectors.ContainsKey(agent))
        {
            m_separationForceVectors.Remove(agent);
            m_separationActualRadii.Remove(agent);
        }
        else
            ;// Debug.Log("Warning: Tried to remove agent from separation vectors, but it was not in the list!");
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
        List<GameObject> agnets = new List<GameObject>(m_separationActualRadii.Keys);
        foreach (GameObject agent in agnets)
            m_separationActualRadii[agent] = m_separationRadius;
    }

    // copy
    public override void setValues(CemBoidRuleBase copyScript)
    {
        if (copyScript == null)
        {
            Debug.Log("Aborted: CemBoidRuleSeparation script was null!");
            return;
        }
        if (copyScript.GetType() != this.GetType())
        {
            Debug.Log("Aborted: Copy script wasn't separation script!");
            return;
        }

        CemBoidRuleSeparation copyScript2 = (CemBoidRuleSeparation)copyScript;

        m_useRule = copyScript2.m_useRule;

        m_separationPerFrame = copyScript2.m_separationPerFrame;
        m_separationMinPercentPerFrame = copyScript2.m_separationMinPercentPerFrame;
        m_separationPower = copyScript2.m_separationPower;
        m_separationRadius = copyScript2.m_separationRadius;
        m_separationMaxPartners = copyScript2.m_separationMaxPartners;
        m_separationMaxPartnerChecks = copyScript2.m_separationMaxPartnerChecks;

        m_separationAffectLeader = copyScript2.m_separationAffectLeader;

        m_useAdjustRadius = copyScript2.m_useAdjustRadius;
        m_separationMinAdjustmentDifference = copyScript2.m_separationMinAdjustmentDifference;
        m_separationMinRadius = copyScript2.m_separationMinRadius;
        m_separationAdjustStep = copyScript2.m_separationAdjustStep;

        m_requireLineOfSight = copyScript2.m_requireLineOfSight;
        m_requireAngle = copyScript2.m_requireAngle;
        m_maxAngle = copyScript2.m_maxAngle;

        List<GameObject> agnets = new List<GameObject>(m_separationForceVectors.Keys);
        foreach (GameObject agent in agnets)
            m_separationForceVectors[agent] = Vector3.zero;

        agnets = new List<GameObject>(m_separationActualRadii.Keys);
        foreach (GameObject agent in agnets)
            m_separationActualRadii[agent] = m_separationRadius;
    }

    // interfaces
    public void onCopy(ICopiable baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CemBoidRuleBase)baseScript);
    }
    public void onPostCopy()
    {
        m_separationForceVectors = new Dictionary<GameObject, Vector3>();
        m_separationActualRadii = new Dictionary<GameObject, float>();
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
        m_separationForceVectors = new Dictionary<GameObject, Vector3>();
        m_separationActualRadii = new Dictionary<GameObject, float>();
    }
    */
}
