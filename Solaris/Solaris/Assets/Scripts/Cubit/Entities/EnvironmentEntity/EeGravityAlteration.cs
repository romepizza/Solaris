using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EeGravityAlteration : EntityCopiableAbstract, ICopiable, IPostCopy, IRemoveOnStateChange, IPrepareRemoveOnStateChange
{
    public static List<EeGravityAlteration> s_globalGravitationFields;

    [Header("------- Settings -------")]
    public bool m_isGlobal;
    public AnimationCurve m_attractPlayerCurve;

    [Header("--- Force ---")]
    public float m_gravity;
    public float m_maxDistance;
    public AnimationCurve m_curve;
    public float m_ignoreRadius;
    public float m_distanceRemoveFactor;

    [Header("--- Calculation ---")]
    public int m_calcsPerCalculation;
    public float m_calcsPerCalculationMinPercent;
    public float m_calcCooldown;

    [Header("--- Gather ---")]
    public int m_gatherPerFrame;
    public float m_gatherPerFrameMinPercent;
    public float m_gatherCooldown;

    [Header("--- Gather Search ---")]
    public float m_gatherSearchCooldown;

    [Header("--- Remove Interval ---")]
    public int m_removePerRemove;
    public float m_removePerRemoveMinPercent;
    public float m_removeCooldown;

    [Header("------- Debug --------")]
    public List<CubeEntityRegisterManager> m_affectedAgents;
    //public Dictionary<CubeEntityRegisterManager, AffectedAgent> m_dict;
    public Dictionary<CubeEntityRegisterManager, Rigidbody> m_dictRigidbodies;
    public Dictionary<CubeEntityRegisterManager, Vector3> m_dictForces;
    public Dictionary<CubeEntityRegisterManager, bool> m_dictSkip;
    public Transform m_center;
    public bool m_isInitialized;
    public CubeEntityRegisterManager m_cacheAgent;

    public float m_removeCooldownRdy;
    public float m_calcCooldownRdy;
    public float m_gatherCooldownRdy;
    public float m_gatherSearchCooldownRdy;
    public int m_removeIndex;

    public Queue<Collider> m_queue;
    public Collider m_cacheCollider;


    public int m_forceVectorIndex;
    
    void Start()
    {
        if (!m_isInitialized)
            initializeStuff();
    }
    void initializeStuff()
    {
        if (m_center == null)
            m_center = transform;

        m_affectedAgents = new List<CubeEntityRegisterManager>();
        m_dictForces = new Dictionary<CubeEntityRegisterManager, Vector3>();
        m_dictRigidbodies = new Dictionary<CubeEntityRegisterManager, Rigidbody>();
        m_dictSkip = new Dictionary<CubeEntityRegisterManager, bool>();
        m_queue = new Queue<Collider>();

        if (s_globalGravitationFields == null)
            s_globalGravitationFields = new List<EeGravityAlteration>();

        if(m_isGlobal)
            EeGravityAlteration.registerGlobalGravitationField(this);

        //if(m_attractPlayerCurve > 0)
            registerAgent(Constants.getPlayer().GetComponent<CubeEntityRegisterManager>());

        m_isInitialized = true;
    }

    void Update()
    {
        manageRemove();
        fillQueue();
        checkAgents();
        calculateForces();
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        applyAllForces();
	}

    // remove
    public void manageRemove()
    {
        if (m_removeCooldownRdy > Time.time)
            return;
        m_removeCooldownRdy = m_removeCooldown + Time.time;

        if (m_isGlobal)
            return;

        int removeThisFrame = Mathf.Min(m_affectedAgents.Count, Mathf.Max(m_removePerRemove, (int)(m_affectedAgents.Count * m_removePerRemoveMinPercent)));
        checkRemove(removeThisFrame);
    }
    public void checkRemove(int checkNumber)
    {
        if (checkNumber <= 0)
            return;

        for(int i = 0; i < checkNumber; i++)
        {
            m_removeIndex %= m_affectedAgents.Count;
            checkRemove(m_affectedAgents[m_removeIndex]);
            m_removeIndex--;
            if (m_removeIndex < 0)
                m_removeIndex += m_affectedAgents.Count;
        }
    }
    public void checkRemove(CubeEntityRegisterManager agent)
    {
        if ((agent.transform.position - m_center.transform.position).sqrMagnitude > (m_maxDistance * m_maxDistance * m_distanceRemoveFactor * m_distanceRemoveFactor))
            agent.deregisterGravitationField(this);
    }

    // get surrounding 
    void checkAgents()
    {
        if (m_gatherCooldownRdy > Time.time)
            return;
        m_gatherCooldownRdy = m_gatherCooldown + Time.time;

        if (m_isGlobal)
            return;

        int gatherThisFrame = Mathf.Min(m_queue.Count, Mathf.Max(m_gatherPerFrame, (int)(m_queue.Count * m_gatherPerFrameMinPercent)));
        computeQueue(gatherThisFrame);

        //m_checkAgentsCooldownRdy = m_gatherCheckCooldown + Time.time;

    }

    // queue
    public void fillQueue()
    {
        if(m_gatherSearchCooldownRdy > Time.time)
            return;
        m_gatherSearchCooldownRdy = m_gatherSearchCooldown + Time.time;

        Collider[] colliders = Physics.OverlapSphere(m_center.position, m_maxDistance, (int)(Mathf.Pow(2, 8) + 0.1)); // TODO : Layer consistancy

        for (int i = 0; i < colliders.Length; i++)
        {
            m_queue.Enqueue(colliders[i]);
        }
    }
    public void computeQueue(int numberCompute)
    {
        if (numberCompute <= 0)
            return;
        
        int found = 0;
        do
        {
            m_cacheCollider = m_queue.Dequeue();

            if (m_cacheCollider == null)
                continue;

            if (m_cacheCollider.gameObject == gameObject)
                continue;

            if (!(m_cacheCollider.GetComponent<CubeEntityState>().m_state == CubeEntityState.s_STATE_ACTIVE))
                continue;

            CubeEntityRegisterManager registerManager = m_cacheCollider.GetComponent<CubeEntityRegisterManager>();
            if (registerManager == null)
                continue;

            if (m_isGlobal && registerManager.m_isDefyGlobalCurrent)
                continue;

            if (!m_isGlobal && registerManager.m_isDefyLocalCurrent)
                continue;

            if (m_cacheCollider.GetComponent<CubeEntityRegisterManager>().registerGravitationField(this))
                found++;

        } while (found < numberCompute && m_queue.Count > 0);
    }

    // calculate force
    void calculateForces()
    {
        if (m_calcCooldownRdy > Time.time)
            return;
        m_calcCooldownRdy = m_calcCooldown + Time.time;

        if (m_affectedAgents.Count <= 0)
            return;

        int calcsThisFrame = Mathf.Min(m_affectedAgents.Count, Mathf.Max(m_calcsPerCalculation, (int)(m_affectedAgents.Count * m_calcsPerCalculationMinPercent)));
         
        for (int i = 0; i < m_calcsPerCalculation; i++)
        {
            m_forceVectorIndex %= m_affectedAgents.Count;
            calculateForce(m_affectedAgents[m_forceVectorIndex]);
            m_forceVectorIndex++;
        }
    }
    void calculateForce(CubeEntityRegisterManager agent)
    {
        float playerFactor = 1;
        if (agent.gameObject == Constants.getPlayer())
            playerFactor = m_attractPlayerCurve.Evaluate((agent.transform.position - transform.position).magnitude / m_maxDistance);

        Vector3 centerPosition = m_center.position;
        Vector3 upDirection = agent.transform.position - centerPosition;
        float forcePower = playerFactor * m_gravity * m_curve.Evaluate((upDirection.sqrMagnitude - m_ignoreRadius * m_ignoreRadius) / (m_maxDistance * m_maxDistance));
        Vector3 force = upDirection.normalized * forcePower;

        m_dictForces[agent] = force;
    }

    // apply force
    void applyAllForces()
    {
        for(int i = 0; i < m_affectedAgents.Count; i++)
        {
            applyForce(m_affectedAgents[i]);
        }
    }
    void applyForce(CubeEntityRegisterManager agent)
    {
        if(!m_dictSkip[agent])
            m_dictRigidbodies[agent].AddForce(m_dictForces[agent]);
    }

    // manage gameGbjects
    public void registerAgent(CubeEntityRegisterManager agent)
    {
        if(agent == null)
        {
            Debug.Log("Aborted: gravityAlteration was null!");
            return;
        }

        if (m_affectedAgents.Contains(agent)) // TODO : Peformance? list proberbly isn't big
        {
            Debug.Log("Aborted: gravityAlteration already was in the list!");
            return;
        }

        //AffectedAgent agent = new AffectedAgent();
        //agent.rb = gravityAlteration.GetComponent<Rigidbody>();
        //m_dict.Add(gravityAlteration, agent);
        m_affectedAgents.Add(agent);
        m_dictRigidbodies.Add(agent, agent.GetComponent<Rigidbody>());
        m_dictForces.Add(agent, Vector3.zero);
        m_dictSkip.Add(agent, false);
    }
    public void deregisterAgent(CubeEntityRegisterManager agent)
    {
        if (!m_affectedAgents.Contains(agent)) // TODO : Peformance? list proberbly isn't big
        {
            Debug.Log("Aborted: gravityAlteration was not in the list!");
            return;
        }
        m_affectedAgents.Remove(agent);
        m_dictRigidbodies.Remove(agent);
        m_dictForces.Remove(agent);
        m_dictSkip.Remove(agent);
    }
    public void deregisterAllAgents()
    {
        for (int i = m_affectedAgents.Count - 1; i >= 0; i--) // TODO : Performance, use indices
        {
            m_affectedAgents[i].deregisterGravitationField(this);// deregisterGravitationField(m_affectedAgents[i]);
        }
    }

    // skip
    public void registerSkip(CubeEntityRegisterManager agent, bool isSkip)
    {
        if(!m_dictSkip.ContainsKey(agent)) // TODO : Delete
        {
            Debug.Log("Aborted: agent was not in the dict!");
            return;
        }
        m_dictSkip[agent] = isSkip;
    }

    
    // static stuff
    public static void registerAgentStatic(CubeEntityRegisterManager agent)
    {
        if (s_globalGravitationFields == null)
        {
            // Debug.Log("Warning: s_globalGravitationFields was null!");
            s_globalGravitationFields = new List<EeGravityAlteration>();
        }

        for(int i = 0; i < s_globalGravitationFields.Count; i++)
        {
            agent.GetComponent<CubeEntityRegisterManager>().registerGravitationField(s_globalGravitationFields[i]);
        }
    }
    public static void registerGlobalGravitationField(EeGravityAlteration gravityAlteration)
    {
        if (s_globalGravitationFields == null)
        {
            Debug.Log("Warning: s_globalGravitationFields was null!");
            s_globalGravitationFields = new List<EeGravityAlteration>();
        }

        if (s_globalGravitationFields.Contains(gravityAlteration))
        {
            Debug.Log("Warning: list already contained gravityField!");
            return;
        }

        s_globalGravitationFields.Add(gravityAlteration);
    }
    public static void deregisterGlobalGravitaionField(EeGravityAlteration gravityAlteration)
    {
        if (s_globalGravitationFields == null)
        {
            Debug.Log("Warning: s_globalGravitationFields was null!");
            s_globalGravitationFields = new List<EeGravityAlteration>();
        }

        if (!s_globalGravitationFields.Contains(gravityAlteration))
        {
            Debug.Log("Warning: gravityField was not in the list!");
            return;
        }

        s_globalGravitationFields.Remove(gravityAlteration);
    }

    // copy
    void setValues(EeGravityAlteration copyScript)
    {
        m_isGlobal = copyScript.m_isGlobal;
        m_gravity = copyScript.m_gravity;
        m_maxDistance = copyScript.m_maxDistance;
        m_curve = copyScript.m_curve;

        m_ignoreRadius = copyScript.m_ignoreRadius;

        m_calcsPerCalculation = copyScript.m_calcsPerCalculation;
        m_calcsPerCalculationMinPercent = copyScript.m_calcsPerCalculationMinPercent;
        m_calcCooldown = copyScript.m_calcCooldown;

        m_gatherPerFrame = copyScript.m_gatherPerFrame;
        m_gatherPerFrameMinPercent = copyScript.m_gatherPerFrameMinPercent;
        m_gatherCooldown = copyScript.m_gatherCooldown;

        m_gatherSearchCooldown = copyScript.m_gatherSearchCooldown;

        m_distanceRemoveFactor = copyScript.m_distanceRemoveFactor;
        m_removePerRemove = copyScript.m_removePerRemove;
        m_removePerRemoveMinPercent = copyScript.m_removePerRemoveMinPercent;
        m_removeCooldown = copyScript.m_removeCooldown;

        m_attractPlayerCurve = copyScript.m_attractPlayerCurve;
}

    // interfaces
    public void onCopy(ICopiable copiable)
    {
        setValues((EeGravityAlteration)copiable);
        if (!m_isInitialized)
            initializeStuff();
    }
    public void onPostCopy()
    {
        if (m_isGlobal)
        {
            EeGravityAlteration.registerGlobalGravitationField(this);
        }
        fillQueue();
    }
    public void onStateChangePrepareRemove()
    {
        deregisterAllAgents();
        if (m_isGlobal)
            EeGravityAlteration.deregisterGlobalGravitaionField(this);
    }
    public void onRemove()
    {
        Destroy(this);
    }
}
