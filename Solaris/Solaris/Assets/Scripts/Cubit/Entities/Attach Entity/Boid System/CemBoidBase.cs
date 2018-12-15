//using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class CemBoidBase : AttachEntityBase, IRemoveOnStateChange, IPrepareRemoveOnStateChange, ICopiable, IPostCopy
{
    //public static bool s_cameraRotated = false;
    //public static float s_cameraRotationSpeed = 3f;
    //public static bool s_moveCameraWhileIdle = true;

    public static bool s_calculateInBase = false;
    public static bool s_calculateInFixedUpdate = false;

    //public GameObject m_spawnPrefab;
    //public GameObject m_showGrabbedObject;
    [Header("------- Settings -------")]
    [Header("--- (Swarm) ---")]
    public int m_maxSwarmSize;
    public float m_maxIndividualSpeed;
    public float m_airResistancePower;
    public bool m_lookInFlightDirection;
    public float m_lookInFlightDirectionPower;
    public float m_affectedByplayerMovementPower;

    [Header("--- (Leader) ---")]
    public bool m_gameObjectIsLeader;
    public GameObject m_leader;
    public bool m_leaderApplyPostMovement;

    [Header("--- (Input) ---")]
    public bool m_allowInput;
    public float m_inputRadius;
    public GameObject m_customAddObject;

    [Header("------- Debug -------")]
    public List<CemBoidRuleBase> m_rules;
    public List<GameObject> m_agents;
    public Vector3 m_averageSwarmPosition;
    public Vector3 m_averageSwarmMovement;
    public bool m_averageSwarmPositionCalculated;
    public bool m_averageSwarmMovementCalculated;
    public bool m_isInitialized;
    //public bool m_isFreeze;
    // Use this for initialization
    void Start ()
    {
        if(!m_isInitialized)
            initializeStuff();
	}

    void initializeStuff()
    {
        m_rules = new List<CemBoidRuleBase>();
        m_agents = new List<GameObject>();


        m_isInitialized = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        updateStuff();
        getRules();
        //if(s_calculateInBase && !s_calculateInFixedUpdate)
            //getMovementInformation();
    }
    void updateStuff()
    {
        m_averageSwarmMovementCalculated = false;
        m_averageSwarmPositionCalculated = false;
    }
    void getRules()
    {
        CemBoidRuleBase[] rules = GetComponents<CemBoidRuleBase>();
        if (rules.Length != m_rules.Count)
        {
            m_rules = new List<CemBoidRuleBase>();
            foreach (CemBoidRuleBase rule in rules)
            {
                m_rules.Add(rule);
                if (rule.m_baseScript == this || rule.m_baseScript == null)
                    rule.m_baseScript = this;
                else
                    Debug.Log("Warning!");
            }
        }
    }
    /*
    void manageCameraRotation()
    {
        if(m_isFreeze && !s_cameraRotated && s_moveCameraWhileIdle)
        {
            Constants.getPlayer().GetComponent<CameraRotation>().currentAngleX += s_cameraRotationSpeed * Time.deltaTime;
            Constants.getPlayer().GetComponent<CameraRotation>().currentAngleY = Mathf.Cos(Time.time * 0.03f) * 50;
            s_cameraRotated = true;
        }
    }
    */
    void FixedUpdate()
    {
        //if (s_calculateInBase)
        //{
        //    if(s_calculateInFixedUpdate)
        //        getMovementInformation();
        //    applyRules();
        //}
        applyPostMovement();
    }
    void applyPostMovement()
    {
        foreach (GameObject agent in m_agents)
        {
            if (!m_leaderApplyPostMovement && agent == m_leader)
                continue;

            Rigidbody rb = agent.GetComponent<Rigidbody>();

            // (air) resistance
            if (rb.velocity.magnitude > 10f && m_airResistancePower > 0)
            {
                rb.AddForce(rb.velocity.normalized * -m_airResistancePower);
            }
            // max speed
            if (rb.velocity.magnitude > m_maxIndividualSpeed)
            {
                rb.velocity = rb.velocity.normalized * m_maxIndividualSpeed;
            }
            // look direction
            if (m_lookInFlightDirection && rb.velocity.magnitude > 0)
            {
                agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, Quaternion.LookRotation(rb.velocity), m_lookInFlightDirectionPower);
            }
        }
    }

    private void LateUpdate()
    {
        //s_cameraRotated = false;
    }

    // swarm movement
    public Vector3 getAverageSwarmPosition()
    {
        if (m_averageSwarmMovementCalculated)
            return m_averageSwarmPosition;
        else
        {
            m_averageSwarmPosition = Vector3.zero;
            foreach (GameObject agent in m_agents)
            {
                m_averageSwarmPosition += agent.transform.position;
            }
            m_averageSwarmPosition /= m_agents.Count;

            m_averageSwarmMovementCalculated = true;
            return m_averageSwarmPosition;
        }
    }
    public Vector3 getAverageSwarmMovement()
    {
        if (m_averageSwarmMovementCalculated)
            return m_averageSwarmMovement;
        else
        {
            m_averageSwarmMovement = Vector3.zero;
            foreach (GameObject agent in m_agents)
            {
                m_averageSwarmMovement += agent.GetComponent<Rigidbody>().velocity;
            }
            m_averageSwarmMovement /= m_agents.Count;

            m_averageSwarmMovementCalculated = true;
            return m_averageSwarmMovement;
        }
    }

    // manage rules
    void getMovementInformation()
    {
        if (m_agents.Count > 0)
        {
            foreach (CemBoidRuleBase rule in m_rules)
            {
                rule.getInformation(m_agents);
            }
        }
    }
    void applyRules()
    {
        if (m_agents.Count > 0)
        {
            foreach (CemBoidRuleBase rule in m_rules)
            {
                rule.applyRule(m_agents);
            }
        }
    }

    // manage agents
    /*
    public void setSwarmSize(int number)
    {
        int tries = 0;
        while(m_agents.Count != number && tries < 10000)
        {
            if(m_agents.Count < number)
            {
                GameObject agent = createCube(this.gameObject);
                Constants.getPlayer().GetComponent<PlayerEntityAttachSystem>().addToGrab(agent);
                agent.GetComponent<CubeEntitySystem>().getPrefapSystem().setToAttachedPlayer();
                agent.GetComponent<CubeEntitySystem>().getMovementComponent().removeAllAccelerationComponents();
                addAgent(agent);

            }
            else if(m_agents.Count >= 1)
            {
                GameObject agent = m_agents[Random.Range(0, m_agents.Count)];
                removeAgent(agent);
                agent.GetComponent<CubeEntitySystem>().getPrefapSystem().setToInactive();
                Destroy(agent);
            }
            tries++;
        }
    }
    */
    public override bool addAgent(GameObject agent)
    {
        if (m_maxSwarmSize < 0 || m_agents.Count < m_maxSwarmSize)
        {
            if (agent != null)
            {
                //bool isLegit = true;
                //CemBoidAttached[] attachedScripts = agent.GetComponents<CemBoidAttached>();
                //foreach (CemBoidAttached attachedScript in attachedScripts)
                //{
                //    if (attachedScript.m_isAttachedToBase == this)
                //    {
                //        isLegit = false;
                //    }
                //}
                CemBoidAttached attachedScript = agent.GetComponent<CemBoidAttached>();
                if (attachedScript == null)
                    attachedScript = agent.AddComponent<CemBoidAttached>();

                if (attachedScript.registerSwarm(this))
                {
                    m_agents.Add(agent);
                    Rigidbody rigidbody = agent.GetComponent<Rigidbody>();
                    if(m_leader != null && rigidbody != null)
                        agent.GetComponent<Rigidbody>().velocity = m_leader.GetComponent<Rigidbody>().velocity;

                    getRules();
                    for (int i = m_rules.Count - 1; i >= 0; i--)
                        m_rules[i].onAddAgent(m_agents, agent);
                    return true;
                }

                //if (isLegit)
                //{
                //    m_agents.Add(agent);

                //    if(m_leader != null)
                //        agent.GetComponent<Rigidbody>().velocity = m_leader.GetComponent<Rigidbody>().velocity;

                //    CemBoidAttached attachedScript = agent.AddComponent<CemBoidAttached>();
                //    attachedScript.m_isAttachedToBase = this;

                //    getRules();
                //    foreach (CemBoidRuleBase rule in m_rules)
                //    {
                //        rule.onAddAgent(m_agents, agent);
                //    }
                //    return true;
                //}
                //else  
                //    Debug.Log("Warning: Tried to add agent that was already in the list!");
            }
        }
        return false;
    }
    public override void removeAgent(GameObject agent)
    {
        if (m_agents.Count > 0)
        {
            if (agent != null)
            {
                getRules();
                for (int i = m_rules.Count - 1; i >= 0; i--)
                    m_rules[i].onRemoveAgent(m_agents, agent);


                agent.GetComponent<CemBoidAttached>().deregisterSwarm(this);
                m_agents.Remove(agent);

                //if (m_agents.Contains(agent))
                //{
                //    getRules();
                //    foreach (CemBoidRuleBase rule in m_rules)
                //    {
                //        rule.onRemoveAgent(m_agents, agent);
                //    }
                //    CemBoidAttached[] attachedScripts = agent.GetComponents<CemBoidAttached>();
                //    foreach (CemBoidAttached attachedScript in attachedScripts)
                //    {
                //        if (attachedScript.m_isAttachedToBase == this)
                //        {
                //            Destroy(attachedScript);
                //        }
                //    }

                //    m_agents.Remove(agent);
                    
                //}
                //else
                //    Debug.Log("Warning: Tried to remove agent that was not in the list!");
            }
            else
                Debug.Log("Warning: Seems like an agent in the list was null!");
        }
    }
    public void removeRandomAgent()
    {
        if(m_agents.Count <= 0)
            return;
        
        GameObject agent = m_agents[Random.Range(0, m_agents.Count)];
        if(agent == null)
        {
            Debug.Log("Aborted: Random Agent was null!");
            return;
        }

        removeAgent(agent);
    }

    // manage cubes
    /*
    public GameObject createCube(GameObject parent)
    {
        GameObject cube = Instantiate(m_spawnPrefab);
        cube.GetComponent<CubeEntitySystem>().addComponentsAtStart();
        cube.GetComponent<CubeEntitySystem>().setToInactive();

        Vector3 randomVector = m_leader.transform.position +  Random.insideUnitSphere * 100f;
        
        cube.transform.position = randomVector;
        cube.transform.SetParent(parent.transform);
        cube.transform.rotation = m_leader.transform.rotation;

        return cube;
    }
    */
    // utility
    

    // copy
    public void setValues(CemBoidBase copyScript, int swarmSize, int numberPredator)
    {
        m_maxSwarmSize = copyScript.m_maxSwarmSize;
        m_maxIndividualSpeed = copyScript.m_maxIndividualSpeed;
        m_airResistancePower = copyScript.m_airResistancePower;
        m_lookInFlightDirection = copyScript.m_lookInFlightDirection;
        m_lookInFlightDirectionPower = copyScript.m_lookInFlightDirectionPower;
        //m_spawnPrefab = copyScript.m_spawnPrefab;
        m_affectedByplayerMovementPower = copyScript.m_affectedByplayerMovementPower;

        //setSwarmSize(swarmSize);

        getRules();
        CemBoidRuleBase[] rules = copyScript.GetComponents<CemBoidRuleBase>();
        foreach(CemBoidRuleBase rule1 in m_rules)
        {
            foreach(CemBoidRuleBase rule2 in rules)
            {
                if (rule1.GetType() == rule2.GetType())
                {
                    rule1.setValues(rule2);

                    // special treatment for predator script
                    if (rule1.GetType() == typeof(CemBoidRulePredator))
                    {
                        CemBoidRulePredator predatorScript = (CemBoidRulePredator)rule1;
                        predatorScript.setValuesPredator(rule2, numberPredator);
                    }
                }
            }
        }
    }
    public void setValuesFlat(CemBoidBase copyScript)
    {
        m_maxSwarmSize = copyScript.m_maxSwarmSize;
        m_maxIndividualSpeed = copyScript.m_maxIndividualSpeed;
        m_airResistancePower = copyScript.m_airResistancePower;
        m_lookInFlightDirection = copyScript.m_lookInFlightDirection;
        m_lookInFlightDirectionPower = copyScript.m_lookInFlightDirectionPower;

        //m_spawnPrefab = copyScript.m_spawnPrefab;
        m_affectedByplayerMovementPower = copyScript.m_affectedByplayerMovementPower;

        m_gameObjectIsLeader = copyScript.m_gameObjectIsLeader;
        //if (m_gameObjectIsLeader)
            //m_leader = ((Rigidbody)Utility.getComponentInParents<Rigidbody>(transform)).gameObject;// gameObject;
            //m_leader = copyScript.m_leader;
        m_leaderApplyPostMovement = copyScript.m_leaderApplyPostMovement;

        m_allowInput = copyScript.m_allowInput;
        m_inputRadius = copyScript.m_inputRadius;
    }

    public override void setValuesByPrefab(GameObject prefab)
    {
        
    }
    // Input Stuff

    /*
    void getInput()
    {
        if (m_allowInput)
        {
            if (Input.GetKey(KeyCode.F))
            {
                //addRandomCube();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                //addAllCubes();
            }

            if (Input.GetKey(KeyCode.UpArrow))// && Input.GetKey(KeyCode.LeftShift))
            {
                setSwarmSize(m_agents.Count + 1);
            }
            else if(Input.GetKeyDown(KeyCode.UpArrow)) // not in use
            {
                setSwarmSize(m_agents.Count + 1);
            }
            if (Input.GetKey(KeyCode.DownArrow))// && Input.GetKey(KeyCode.LeftShift))
            {
                setSwarmSize(m_agents.Count - 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)) // not in use
            {
                setSwarmSize(m_agents.Count - 1);
            }

            if (Input.GetKeyDown(KeyCode.C))
                m_isFreeze = !m_isFreeze;
        }

        if(m_customAddObject != null)
        {
            addAgent(m_customAddObject);
            m_customAddObject = null;
        }
    }
    void addRandomCube()
    {
        if (m_agents.Count < m_maxSwarmSize)
        {
            GameObject cubeAdd = null;
            Collider[] colliders = Physics.OverlapSphere(m_leader.transform.position, m_inputRadius);

            float nearestDist = float.MaxValue;
            for (int i = 0; i < colliders.Length; i++)
            {
                GameObject cubePotential = colliders[i].gameObject;
                if (cubePotential.layer == 8 && cubePotential.GetComponent<CubeEntitySystem>() != null)
                {
                    if (cubePotential.GetComponent<CubeEntitySystem>().getStateComponent() != null && cubePotential.GetComponent<CubeEntitySystem>().getStateComponent().canBeAttachedToPlayer() && cubePotential.GetComponent<CemBoidAttached>() == null)
                    {
                        float dist = Vector3.Distance(m_leader.transform.position, cubePotential.transform.position);
                        if (dist < nearestDist)
                        {
                            nearestDist = dist;
                            cubeAdd = cubePotential;
                        }
                    }
                }
            }

            if (cubeAdd != null)
            {
                Constants.getPlayer().GetComponent<PlayerEntityAttachSystem>().addToGrab(cubeAdd);
                cubeAdd.GetComponent<CubeEntitySystem>().getPrefapSystem().setToAttachedPlayer();
                cubeAdd.GetComponent<CubeEntitySystem>().getMovementComponent().removeAllAccelerationComponents();
                addAgent(cubeAdd);
            }
        }
    }
    void addAllCubes()
    {
        if (m_agents.Count < m_maxSwarmSize)
        {
            GameObject cubeAdd = null;
            Collider[] colliders = Physics.OverlapSphere(m_leader.transform.position, m_inputRadius);

            for (int i = 0; i < colliders.Length; i++)
            {
                GameObject cubePotential = colliders[i].gameObject;
                if (cubePotential.layer == 8 && cubePotential.GetComponent<CubeEntitySystem>() != null)
                {
                    if (cubePotential.GetComponent<CubeEntitySystem>().getStateComponent() != null && cubePotential.GetComponent<CubeEntitySystem>().getStateComponent().canBeAttachedToPlayer())
                    {
                        cubeAdd = cubePotential;
                        if (addAgent(cubeAdd))
                        {
                            Constants.getPlayer().GetComponent<PlayerEntityAttachSystem>().addToGrab(cubeAdd, false, true);
                            cubeAdd.GetComponent<CubeEntitySystem>().getPrefapSystem().setToAttachedPlayer();
                            cubeAdd.GetComponent<CubeEntitySystem>().getMovementComponent().removeAllAccelerationComponents();
                        }
                    }
                }
            }

            
            if (cubeAdd != null)
            {
                Constants.getPlayer().GetComponent<PlayerEntityAttachSystem>().addToGrab(cubeAdd, false, true);
                cubeAdd.GetComponent<CubeEntitySystem>().getPrefapSystem().setToAttachedPlayer();
                cubeAdd.GetComponent<CubeEntitySystem>().getMovementComponent().removeAllAccelerationComponents();
                addAgent(cubeAdd);
            }
        }
    }
    void removeRandomCube()
    {

    }
    */

    // interface
    public void onStateChangePrepareRemove()
    {
        //List<GameObject> agents = new List<GameObject>(m_agents);
        //foreach (GameObject agent in m_agents)
        //{
        //    CemBoidAttached[] scripts = GetComponents<CemBoidAttached>();
        //    foreach (CemBoidAttached script in scripts)
        //    {
        //        if (script.m_isAttachedToBase == this)
        //        {
        //            Destroy(script);
        //        }
        //    }
        //}
        for(int i = m_agents.Count - 1; i >= 0; i--)
        {
            m_agents[i].GetComponent<CemBoidAttached>().deregisterSwarm(this);
        }
    }
    public void onRemove()
    {
        List<CemBoidRuleBase> rules = new List<CemBoidRuleBase>(m_rules);
        foreach (CemBoidRuleBase rule in rules)
        {
            ((IRemoveOnStateChange)rule).onRemove();
        }
        Destroy(this);
    }
    public void onCopy(ICopiable baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValuesFlat((CemBoidBase)baseScript);
    }
    public void onPostCopy()
    {
        if (!m_isInitialized)
            initializeStuff();
        if (m_gameObjectIsLeader)
            m_leader = ((Rigidbody)Utility.getComponentInParents<Rigidbody>(transform)).gameObject;
    }
    /*
    // abstract

    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValuesFlat((CemBoidBase)baseScript);
    }
    public override void prepareDestroyScript()
    {
        List<GameObject> agents = new List<GameObject>(m_agents);
        foreach (GameObject agent in m_agents)
        {
            CemBoidAttached[] scripts = GetComponents<CemBoidAttached>();
            foreach(CemBoidAttached script in scripts)
            {
                if(script.m_isAttachedToBase == this)
                {
                    Destroy(script);
                }
            }
        }
        List<CemBoidRuleBase> rules = new List<CemBoidRuleBase>(m_rules);
        foreach(CemBoidRuleBase rule in m_rules)
        {
            rule.prepareDestroyScript();
        }
        Destroy(this);
    }
    public override void onPostCopy()
    {
        if (!m_isInitialized)
            initializeStuff();
        if (m_gameObjectIsLeader)
            m_leader = this.gameObject;
    }*/
}
