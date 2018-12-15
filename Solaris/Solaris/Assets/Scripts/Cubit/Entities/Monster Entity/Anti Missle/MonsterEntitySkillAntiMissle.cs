using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntitySkillAntiMissle : EntityCopiableAbstract, ICopiable, IPostCopy, IRemoveOnStateChange, IOnTargetedBy
{
    public bool m_useSkill = true;
    public GameObject m_spawnPrefab;

    [Header("------- Input -------")]
    public KeyCode keyCode0;
    public KeyCode keyCode1;
    public KeyCode keyCode2;

    [Header("------- Settings ------")]
    public float m_cooldown;
    public float m_spawnDistance;

    [Header("--- (Passive) ---")]
    public float m_passiveCooldown;

    [Header("--- (Movement) ---")]
    public List<CubeEntityMovementAbstract> m_movementScripts;
    public List<CubeEntityMovementStartSpeed> m_startSpeedScripts;
    public List<GameObject> m_spawnEffects;
    public float m_startSpeed;

    [Header("--- (Idle Movement) ---")]
    public MonsterEntityAttachSystemNew m_attachSystem;

    [Header("--- (Aim) ---")]
    public float m_rangeIfNoTarget;
    public float m_randomRadius;
    public float m_shootInFlightDirectionMin;
    public float m_shootInFlightDirectionMax;

    [Header("--- (On Targeted By) ---")]
    public float m_onTargetedBySetTargetChange;
    public float m_onTargetedByCreateChance;

    [Header("--- Ressources) ---")]
    public float[] m_ressourceCosts;
    public float[] m_ressourcesMinNeeded;
    public AnimationCurve m_ressourceCurve;

    [Header("------- Debug -------")]
    public bool m_isInitialized;
    public GameObject m_core;
    public GameObject m_target;
    public CubeEntityTargetManager m_targetScript;
    public GameObject m_cubeToToss;
    public Vector3 m_targetPosition;
    public float m_cooldownRdy;

    public CubeEntityRessourceManager m_ressourceManager;
    public float[] m_ressourcesUsed;
    public float[] m_ressourceFactors;
    public float m_ressourceFactor;
    public CubeEntityState m_ownStateScript;

    public float m_passiveCooldownRdy;

    // Use this for initialization
    void Start ()
    {
        initializeStuff();
    }
	
    void initializeStuff()
    {
        if (m_isInitialized)
            return;

        if (m_core == null)
            m_core = ((MonsterEntityAbstractBase)Utility.getComponentInParents<MonsterEntityAbstractBase>(transform)).gameObject;
        if(m_ownStateScript == null)
            m_ownStateScript = m_core.GetComponent<CubeEntityState>();


        m_isInitialized = true;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!m_useSkill)
            return;

        if (m_attachSystem != null && m_attachSystem.m_cubeList.Count == m_attachSystem.m_maxCubesGrabbed)
            return;

        manageSkillActive();
        manageSkillPassive();
    }

    // manage Skill
    void manageSkillActive()
    {
        if(!(m_cooldownRdy <= Time.time && isPressingKey()))
            return;
        m_cooldownRdy = m_cooldown + Time.time;

        m_target = m_core.GetComponent<CubeEntityTargetManager>().getTarget();

        //getTargetPosition();
        shootCube(null);
        //setCubeScript();
    }
    void manageSkillPassive()
    {
        if (m_passiveCooldown <= 0)
            return;
        if (m_passiveCooldownRdy <= Time.time)
            return;

        shootCube(null);
    }
    void selectCube()
    {
        Vector3 spawnPosition = transform.position + /*Constants.getMainCamera().transform.rotation * Vector3.forward * 30f +*/ Random.insideUnitSphere.normalized * m_spawnDistance;
        m_cubeToToss = Constants.getMainCge().activateCubeSafe(spawnPosition);
    }
    void getTargetPosition()
    {
        if (m_target != null && (m_shootInFlightDirectionMin > 0 || m_shootInFlightDirectionMax > 0) && m_movementScripts.Count > 0)// && false)
        {
            Vector3 targetDirection = m_target.GetComponent<Rigidbody>().velocity;
            float dist = Vector3.Distance(m_targetPosition, m_cubeToToss.transform.position);
            //Debug.Log(m_targetPositions[cube]);
            m_targetPosition = m_targetPosition + targetDirection * (dist / m_movementScripts[0].m_maxSpeed) * Random.Range(m_shootInFlightDirectionMin, m_shootInFlightDirectionMax) + Random.insideUnitSphere * m_randomRadius;
        }
    }
    void shootCube(CubeEntityTargetManager target)
    {
        if (!checkValidity(target))
            return;


        selectCube();
        if (m_cubeToToss == null)
        {
            Debug.Log("Caution?");
            return;
        }

        getTargetPosition();
        performShoot();
        setCubeScript();

        if (target != null)
        {
            if (!m_cubeToToss.GetComponent<MonsterEntityAntiMissle>().setTarget(target))
                Debug.Log("Warning: This should not have happened!");
        }
    }
    void performShoot()
    {
        if (m_spawnPrefab != null)
            m_cubeToToss.GetComponent<CubeEntityPrefapSystem>().setToPrefab(m_spawnPrefab);
        else
        {
            Debug.Log("Caution!");
            m_cubeToToss.GetComponent<CubeEntitySystem>().setActiveDynamicly(m_core.GetComponent<CubeEntityState>());
        }

        // set affiliation manually
        m_cubeToToss.GetComponent<CubeEntityState>().m_affiliation = m_core.GetComponent<CubeEntityState>().m_affiliation;

        if (m_targetPosition == Vector3.zero)
            m_targetPosition = m_cubeToToss.transform.position + (m_cubeToToss.transform.position - m_core.transform.position).normalized * 1000f;

        foreach (CubeEntityMovementStartSpeed script in m_startSpeedScripts)
        {
            if (script == null)
                continue;

            Vector3[] customCoordinateSystem = new Vector3[3];
            customCoordinateSystem[2] = m_targetPosition - m_cubeToToss.transform.position;
            customCoordinateSystem[1] = m_cubeToToss.transform.position - transform.position;
            customCoordinateSystem[0] = Vector3.Cross(customCoordinateSystem[2], customCoordinateSystem[1]);
            customCoordinateSystem[1] = Vector3.Cross(customCoordinateSystem[0], customCoordinateSystem[2]);

            //script.applyMovement(m_cubeToToss, m_targetPosition, m_core.transform.position, (gameObject.layer == 9 ? Constants.getMainCamera().transform.rotation : m_core.transform.rotation), customCoordinateSystem, m_ressourceFactor);
            script.applyMovement(m_cubeToToss, m_cubeToToss.transform.position, m_core.transform.position, (gameObject.layer == 9 ? Constants.getMainCamera().transform.rotation : m_core.transform.rotation), customCoordinateSystem, 1f);// m_ressourceFactor);
        }

        foreach (CubeEntityMovementAbstract script in m_movementScripts)
        {
            if (script == null)
                continue;

            CubeEntityMovementAbstract s = m_cubeToToss.GetComponent<CubeEntityMovement>().addMovementComponent(script, m_target, m_targetPosition);
            //s.m_forceFactor = Mathf.Sqrt(m_ressourceFactor);
        }

        m_cubeToToss.GetComponent<CubeEntityTargetManager>().setTarget(m_target);
        m_cubeToToss.GetComponent<CubeEntityTargetManager>().setOrigin((CubeEntityTargetManager)Utility.getComponentInParents<CubeEntityTargetManager>(transform));

        if(m_attachSystem != null)
            m_attachSystem.registerToGrab(m_cubeToToss);
    }
    void setCubeScript()
    {
        MonsterEntityAntiMissle script = m_cubeToToss.GetComponent<MonsterEntityAntiMissle>();
        if(script == null)
        {
            Debug.Log("Aborted: MonsterEntityAntiMissle script was null!");
            return;
        }

        script.setValues(this, null);// m_target.GetComponent<CubeEntityTargetManager>());
    }

    bool checkValidity(CubeEntityTargetManager target)
    {
        if(target == null)
        {
            return true;
        }

        CubeEntityState otherStateScript = target.GetComponent<CubeEntityState>();
        if (otherStateScript == null)
        {
            Debug.Log("Warning!");
            return false;
        }
        if (m_ownStateScript == null)
        {
            Debug.Log("Warning! " + name);
            return false;
        }

        if (!(otherStateScript.m_state == CubeEntityState.s_STATE_ACTIVE && otherStateScript.m_affiliation != m_ownStateScript.m_affiliation && otherStateScript.m_type != CubeEntityState.s_TYPE_ANTI_MISSLE))
            return false;

        MonsterEntityAntiMissle missleScript = m_spawnPrefab.GetComponent<MonsterEntityAntiMissle>();
        if (missleScript.m_onlyOnTargetCore && m_core != null)
        {
            if (target.m_target != m_core.GetComponent<CubeEntityTargetManager>())
            {
                return false;
            }
        }
        if (target.m_targetedByActive.Count > 0)
        {
            return false;
        }
        

        return true;
    }
    // set target
    void setRandomCubeTarget(CubeEntityTargetManager targetedBy)
    {
        if (m_attachSystem == null)
            return;

        if (m_attachSystem.m_cubeList.Count == 0)
            return;

        if (m_attachSystem.m_cubeList[Random.Range(0, m_attachSystem.m_cubeList.Count - 1)].GetComponent<MonsterEntityAntiMissle>().setTarget(targetedBy))
            ;// Debug.Log("Warning: This should not have happened!");
    }

    // ressources
    void getRessources()
    {
        if (m_ressourceManager == null)
        {
            Debug.Log("Warning?");
            m_ressourceFactor = 1;
            return;
        }

        bool hasEnoughRessources = m_ressourceManager.hasEnoughRessurces(m_ressourcesMinNeeded);
        if (hasEnoughRessources)
        {
            m_ressourcesUsed = m_ressourceManager.addRessources(m_ressourceCosts);
            m_ressourceFactors = new float[m_ressourcesUsed.Length];
            for (int i = 0; i < m_ressourceFactors.Length; i++)
                m_ressourceFactors[i] = m_ressourceCosts[i] < 0 ? (m_ressourcesUsed[i] / m_ressourceCosts[i]) : 1f;
        }
        else
        {
            //m_ressourcesUsed = m_ressourceManager.addRessources(m_ressourceCosts);
            m_ressourceFactors = new float[m_ressourcesUsed.Length];
            for (int i = 0; i < m_ressourceFactors.Length; i++)
                m_ressourceFactors[i] = 0;
        }
        m_ressourceFactor = m_ressourceCurve.Evaluate(Mathf.Clamp01(Mathf.Min(m_ressourceFactors)));
    }

    // copy
    void setValues(MonsterEntitySkillAntiMissle copyScript)
    {
        m_useSkill = copyScript.m_useSkill;
        m_spawnPrefab = copyScript.m_spawnPrefab;


        m_movementScripts = new List<CubeEntityMovementAbstract>();
        foreach (CubeEntityMovementAbstract o in copyScript.m_movementScripts)
            m_movementScripts.Add(o);
        m_spawnEffects = new List<GameObject>();
        foreach (GameObject o in copyScript.m_spawnEffects)
            m_spawnEffects.Add(o);
        m_startSpeed = copyScript.m_startSpeed;

        m_cooldown = copyScript.m_cooldown;

        m_movementScripts = copyScript.m_movementScripts;
        m_startSpeedScripts = copyScript.m_startSpeedScripts;
        m_spawnEffects = copyScript.m_spawnEffects;          

        m_rangeIfNoTarget = copyScript.m_rangeIfNoTarget; 
        m_randomRadius = copyScript.m_randomRadius; 
        m_shootInFlightDirectionMin = copyScript.m_shootInFlightDirectionMin; 
        m_shootInFlightDirectionMax = copyScript.m_shootInFlightDirectionMax; 

        m_ressourceCosts = copyScript.m_ressourceCosts;
        m_ressourcesMinNeeded = copyScript.m_ressourcesMinNeeded;
        m_ressourceCurve = copyScript.m_ressourceCurve;

        m_onTargetedByCreateChance = copyScript.m_onTargetedByCreateChance;
        m_onTargetedBySetTargetChange = copyScript.m_onTargetedBySetTargetChange;
    }

    // input
    bool isPressingKey()
    {
        return gameObject.layer != 9 || Input.GetKey(keyCode0) || Input.GetKey(keyCode1) || Input.GetKey(keyCode2); // TODO : Layer consitency
    }

    // interface
    public void onCopy(ICopiable copiable)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((MonsterEntitySkillAntiMissle)copiable);
    }
    public void onPostCopy()
    {
        m_core = ((MonsterEntityAbstractBase)Utility.getComponentInParents<MonsterEntityAbstractBase>(transform)).gameObject;
        m_cooldownRdy = m_cooldown + Time.time;
        m_ownStateScript = m_core.GetComponent<CubeEntityState>();
    }
    public void onRemove()
    {
        Destroy(this);
    }
    public void onTargetedByActive(CubeEntityTargetManager targetedBy)
    {
        if (m_onTargetedByCreateChance > 0 && m_onTargetedByCreateChance > Random.Range(0f, 1f))
            shootCube(targetedBy);

        if (m_onTargetedBySetTargetChange > 0 && m_onTargetedBySetTargetChange > Random.Range(0f, 1f))
            setRandomCubeTarget(targetedBy);
    }
    public void onTargetedByCore(CubeEntityTargetManager targetedBy)
    {
       
    }
}
