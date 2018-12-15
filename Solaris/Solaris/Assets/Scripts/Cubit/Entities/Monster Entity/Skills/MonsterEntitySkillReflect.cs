using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntitySkillReflect : EntityCopiableAbstract, ICopiable, IPostCopy, IRemoveOnStateChange, IPrepareRemoveOnStateChange
{

    public GameObject m_spawnPrefab;
    public GameObject m_prefabVisualBox;
    public GameObject m_prefabVisualSphere;
    [Header("------- Settings -------")]
    //public bool m_canSpawnMultiple;
    public float m_cooldownOnUseMin;
    public float m_cooldownOnUseMax;
    public float m_cooldownOnDoneMin;
    public float m_cooldownOnDoneMax;
    public float m_durationMin;
    public float m_durationMax;

    [Header("--- (Input) ---")]
    public KeyCode keyCode0;
    public KeyCode keyCode1;
    public KeyCode keyCode2;
    [Header("- Activision -")]
    //public bool m_activateOnKeepPressingOnly;
    [Header("- Uptime -")]
    public bool m_stayActiveIfActive;
    public bool m_stayActiveIfPressed;
    [Header("- Deactivision -")]
    public bool m_deactivateOnNotPressing;
    public float m_deactivateOnMaxTimePressed;

    [Header("--- (Cancel Critera) ---")]
    public int m_maxReflectObjects;

    [Header("--- (Movement) ---")]
    public float m_barrierRotationSpeed;
    public float m_barrierMovementSpeed;
    public List<CubeEntityMovementAbstract> m_movementScripts;
    public List<GameObject> m_onEnterExplosions;


    [Header("--- Ressources) ---")]
    [Header("- Initial -")]
    public float[] m_ressourceCostsInitial;
    public float[] m_ressourcesMinNeededInitial;
    public AnimationCurve m_ressourceCurveInitial;
    [Header("- Uptime -")]
    public float[] m_ressourceCostsUptime;
    public float[] m_ressourcesMinNeededUptime;
    public AnimationCurve m_ressourceCurveUptime;


    [Header("--- (Per Frame) ---")]
    public int m_maxSwitchesPerFrame;
    public float m_afterImmuneTime;

    [Header("--- (Form) ---")]

    [Header("- Box -")]
    public Vector3 m_centerPositionRelativeVectorBox;
    public Vector3 m_boxSizes;
    [Header("- Sphere -")]
    public Vector3 m_centerPositionRelativeVectorSphere;
    public float m_sphereRadius;

    [Header("------- Debug -------")]
    public bool m_isInitialized;
    public bool m_isActive;
    public float m_durationEndTime;
    public bool m_isOnCooldown;
    public float m_cooldownRdyTime;
    List<GameObject> m_cubesHit;
    public BoxCollider m_boxCollider;
    public SphereCollider m_sphereCollider;
    public bool m_isPressing;
    public bool m_pressedThisFrame;
    public bool m_isPositioned;
    //public int m_objectsReflected;

    public bool m_isPressingLong;
    public float m_currentPressingTime;
    public bool m_wasActivatedByPressDown;

    public CubeEntityRessourceManager m_ressourceManager;
    public float[] m_ressourcesUsedInitial;
    public float[] m_ressourceFactorsInitial;
    public float m_ressourceFactorInitial;
    public float[] m_ressourcesUsedUptime;
    public float[] m_ressourceFactorsUptime;
    public float m_ressourceFactorUptime;

    public GameObject m_core;
    

    // Use this for initialization
    void Start()
    {
        if (!m_isInitialized)
            initializeStuff();
    }
    void initializeStuff()
    {
        if (m_core == null)
            m_core = ((MonsterEntityAbstractBase)(Utility.getComponentInParents<MonsterEntityAbstractBase>(transform))).gameObject;

        if (m_ressourceManager == null)
            m_ressourceManager = (CubeEntityRessourceManager)Utility.getComponentInParents<CubeEntityRessourceManager>(transform);
        if (m_ressourceManager == null)
            Debug.Log("Warning: ressourceManager is null!");

        if (m_stayActiveIfActive && m_deactivateOnNotPressing)
            Debug.Log("Warnning: This Combination is reduntant!");

        m_isInitialized = true;
    }

    // Update is called once per frame
    void Update ()
    {
        getInput();
        updateCounter();
        //manageSkill();
    }

    void updateCounter()
    {
        // check deactivision
        bool wasDeactivatedThisFrame = false;
        if (m_isActive)
        {
            // duration
            if (m_durationEndTime < Time.time)
            {
                wasDeactivatedThisFrame = true;
                m_isActive = false;
                deactivateSkill(true);
            }
            // deactivision on not pressing
            else if (!m_isPressing && m_deactivateOnNotPressing)
            {
                wasDeactivatedThisFrame = true;
                m_isActive = false;
                deactivateSkill(true);
            }
            // manually deactivision
            else if(m_isPressingLong)
            {
                wasDeactivatedThisFrame = true;
                m_isActive = false;
                deactivateSkill(true);
            }
            // no ressources
            else 
            {
                getRessourcesUptime();
                if (m_ressourceFactorUptime == 0)
                {
                    wasDeactivatedThisFrame = true;
                    m_isActive = false;
                    deactivateSkill(true);
                }
            }

            // set cooldown
            if (wasDeactivatedThisFrame)
                setOnDoneCooldown();
        }

        // update cooldown
        updateCooldown();

        // check initial activisition
        bool wasActivatedThisFrame = false;
        if (!m_isOnCooldown && !wasDeactivatedThisFrame && !m_isActive)
        {
            // on simple press down
            if (m_pressedThisFrame)
            {
                wasActivatedThisFrame = true;
            }

            // set cooldown and duration
            if (wasActivatedThisFrame)
            {
                getRessourcesInitial();
                if (m_ressourceFactorInitial > 0)
                {
                    activateSkill();
                    setOnUseCooldown();
                    setDuration();
                    m_wasActivatedByPressDown = true;
                    m_isActive = true;
                }
            }
        }

        // check uptime
        if(!wasActivatedThisFrame && m_isActive)
        {
            // on is pressing
            if(m_stayActiveIfPressed && m_isPressing)
            {
                activateSkill();
            }
            // on automaticly
            else if(m_stayActiveIfActive)
            {
                activateSkill();
            }
        }

        return;

        //// check cancel criteria
        ////if(m_maxReflectObjects > 0 && m_objectsReflected >= m_maxReflectObjects)
        ////{
        ////    deactivateSkill(true);
        ////}

        //// check cooldown
        //if (m_isOnCooldown && m_cooldownRdyTime < Time.time)
        //    m_isOnCooldown = false;

        //if (!m_isOnCooldown && m_isPressing && !m_isActive)
        //{
        //    if (m_cooldownOnUseMin != 0 && m_cooldownOnUseMax != 0)
        //    {
        //        m_cooldownRdyTime = Mathf.Max(m_cooldownRdyTime, Time.time + Random.Range(m_cooldownOnUseMin, m_cooldownOnUseMax));
        //        m_isOnCooldown = true;
        //    }
        //    m_isActive = true;
        //    if (m_durationMin != 0 && m_durationMax != 0)
        //        m_durationEndTime = Random.Range(m_durationMin, m_durationMax) + Time.time;
        //    else
        //        m_durationEndTime = float.MaxValue;
        //}

        //// check isActive
        //if (m_isActive && m_durationEndTime < Time.time)
        //{
        //    deactivateSkill(true);
        //}

        //if (m_isActive && (m_isPressing))// || !m_activateOnKeepPressingOnly))
        //{
        //    activateSkill();
        //}
        //else if(m_deactivateOnNotPressing && !m_isPressing)
        //{
        //    deactivateSkill(false);
        //}
    }

    
    // skill
    void activateSkill()
    {
        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnQuaternion = new Quaternion();

        if (gameObject == Constants.getPlayer() || Constants.s_playerLayer == gameObject.layer)
        {
            GameObject target = PlayerEntityAim.aim();
            if(false && target == null)
            {
                spawnPosition = PlayerEntityAim.aim().transform.position;
                spawnQuaternion = Quaternion.LookRotation(transform.position - spawnPosition);
            }
            else
            {
                Vector3 f = Vector3.zero;
                if (m_prefabVisualBox != null)
                    f = m_centerPositionRelativeVectorBox;
                else if (m_prefabVisualSphere != null)
                    f = m_centerPositionRelativeVectorSphere;

                spawnQuaternion = Constants.getMainCamera().transform.rotation;
                spawnPosition = transform.position + spawnQuaternion * f;
            }
        }
        else
        {
            float f = 1;
            if (m_prefabVisualBox != null)
                f = m_centerPositionRelativeVectorBox.z;
            else if (m_prefabVisualSphere != null)
                f = m_centerPositionRelativeVectorSphere.z;

            spawnPosition = transform.position + (m_core.GetComponent<CubeEntityTargetManager>().getTarget().transform.position - transform.position).normalized * f;
            spawnQuaternion = Quaternion.LookRotation(m_core.GetComponent<CubeEntityTargetManager>().getTarget().transform.position - spawnPosition);
        }
        if(spawnPosition == Vector3.zero)
        {
            Debug.Log("Warning: spawnPosition probably not set!");
        }


        if (m_prefabVisualBox != null)
        {
            if (m_boxCollider == null)
            {
                createBoxCollider();
            }

            if (!m_isPositioned)
            {
                m_boxCollider.transform.position = spawnPosition;
                m_boxCollider.transform.rotation = spawnQuaternion;
                m_isPositioned = true;
            }
            else
            {
                m_boxCollider.transform.position = Vector3.Lerp(m_boxCollider.transform.position, spawnPosition, Time.deltaTime * m_barrierMovementSpeed);
                m_boxCollider.transform.rotation = Quaternion.Lerp(m_boxCollider.transform.rotation, spawnQuaternion, Time.deltaTime * m_barrierRotationSpeed);
            }
        }


        if (m_prefabVisualSphere != null)
        {
            if (m_sphereCollider == null)
            {
                createSphereCollider();
            }
            m_sphereCollider.transform.position = spawnPosition;
        }

    }
    public void deactivateSkill(bool resetTimer)
    {
        destroyBoxCollider();
        destroySphereCollider();

        //if (resetTimer)
        //{
        //    if (m_durationEndTime != 0)
        //        m_isActive = false;
        //    if (m_cooldownOnDoneMin != 0 && m_cooldownOnDoneMax != 0)
        //    {
        //        m_cooldownRdyTime = Mathf.Max(m_cooldownRdyTime, Time.time + Random.Range(m_cooldownOnDoneMin, m_cooldownOnDoneMax));
        //        m_isOnCooldown = true;
        //    }
        //}
    }
    // manage
    void createBoxCollider()
    {
        GameObject o = new GameObject();
        o.AddComponent<EntitySystemBase>();
        o.layer = 10;
        o.name = "Reflecting Cube (" + (gameObject == Constants.getPlayer() ? "Player" : m_core.name) + ")";

        SkillEntityIsReflecting script = o.AddComponent<SkillEntityIsReflecting>();
        script.m_originCore = m_core;
        script.m_originSkillGameObject = gameObject;
        script.m_spawnPrefab = m_spawnPrefab;
        script.m_maxSwitchesPerFrame = (int)(m_maxSwitchesPerFrame * m_ressourceFactorInitial);
        script.m_afterImmuneTime = m_afterImmuneTime;
        script.m_maxReflectObjects = (int)(m_maxReflectObjects * m_ressourceFactorInitial);
        script.m_ressourceFactor = m_ressourceFactorInitial;

        script.m_onEnterExplosions = new List<GameObject>();
        foreach (GameObject effect in m_onEnterExplosions)
            script.m_onEnterExplosions.Add(effect);

        GameObject visual = Instantiate(m_prefabVisualBox, o.transform);
        visual.SetActive(true);

        m_boxCollider = o.AddComponent<BoxCollider>();
        m_boxCollider.isTrigger = true;
        m_boxCollider.size = m_boxSizes;

        script.m_movementScripts = new List<CubeEntityMovementAbstract>();
        foreach (CubeEntityMovementAbstract movementScript in m_movementScripts)
        {
            CubeEntityMovementAbstract movementScript2 = null;

            if (movementScript is ICopiable)
                movementScript2 = (CubeEntityMovementAbstract)(o.GetComponent<EntitySystemBase>().copyPasteComponent((ICopiable)movementScript, true));
            else
                Debug.Log("Should not happen!");


            if (movementScript2 == null)
                Debug.Log("Should not happen!");
            else
                movementScript2.pasteScriptButDontActivate(movementScript);

            script.m_movementScripts.Add(movementScript2);
        }
        m_isPositioned = false;
    }
    void destroyBoxCollider()
    {
        if (m_boxCollider != null)
            Destroy(m_boxCollider.gameObject);
        m_boxCollider = null;
    }
    void createSphereCollider()
    {
        GameObject o = new GameObject();
        o.AddComponent<EntitySystemBase>();
        o.layer = 10;
        o.name = "Reflecting Sphere (" + (gameObject == Constants.getPlayer() ? "Player" : m_core.name) + ")";

        SkillEntityIsReflecting script = o.AddComponent<SkillEntityIsReflecting>();
        script.m_spawnPrefab = m_spawnPrefab;
        script.m_originCore = m_core;
        script.m_originSkillGameObject = gameObject;
        script.m_maxSwitchesPerFrame = (int)(m_maxSwitchesPerFrame * m_ressourceFactorInitial);
        script.m_afterImmuneTime = m_afterImmuneTime;
        script.m_maxReflectObjects = (int)(m_maxReflectObjects * m_ressourceFactorInitial);
        script.m_ressourceFactor = m_ressourceFactorInitial;

        GameObject visual = Instantiate(m_prefabVisualSphere, o.transform);
        visual.SetActive(true);

        m_sphereCollider = o.AddComponent<SphereCollider>();
        m_sphereCollider.isTrigger = true;
        m_sphereCollider.radius = m_sphereRadius;

        script.m_movementScripts = new List<CubeEntityMovementAbstract>();
        foreach (CubeEntityMovementAbstract movementScript in m_movementScripts)
        {
            CubeEntityMovementAbstract movementScript2 = null;

            if (movementScript is ICopiable)
                movementScript2 = (CubeEntityMovementAbstract)(o.GetComponent<EntitySystemBase>().copyPasteComponent((ICopiable)movementScript, true));
            else
                Debug.Log("Should not happen!");


            if (movementScript2 == null)
                Debug.Log("Should not happen!");
            else
                movementScript2.pasteScriptButDontActivate(movementScript);

            script.m_movementScripts.Add(movementScript2);
        }
        m_isPositioned = false;
    }
    void destroySphereCollider()
    {
        if (m_sphereCollider != null)
            Destroy(m_sphereCollider.gameObject);
        m_sphereCollider = null;
    }

    // cooldown
    void setOnUseCooldown()
    {
        if (m_cooldownOnUseMin > 0 || m_cooldownOnUseMax > 0)
        {
            m_isOnCooldown = true;
            m_cooldownRdyTime = Mathf.Max(m_cooldownRdyTime, Time.time + Random.Range(m_cooldownOnUseMin, m_cooldownOnUseMax));
        }
        else
        {
            //m_isOnCooldown = false;
            // m_cooldownRdyTime = 0;
        }
    }
    void setOnDoneCooldown()
    {
        if (m_cooldownOnDoneMin > 0 || m_cooldownOnDoneMax > 0)
        {
            m_isOnCooldown = true;
            m_cooldownRdyTime = Mathf.Max(m_cooldownRdyTime, Time.time + Random.Range(m_cooldownOnDoneMin, m_cooldownOnDoneMax));
        }
        else
        {
            // m_isOnCooldown = false;
            // m_cooldownRdyTime = 0;
        }
    }
    void updateCooldown()
    {
        if (m_cooldownRdyTime <= Time.time)
            m_isOnCooldown = false;
    }
    // duration
    void setDuration()
    {
        if(m_durationMin > 0 || m_durationMax > 0)
        {
            m_durationEndTime = Time.time + Random.Range(m_durationMin, m_durationMax) * m_ressourceFactorInitial;
        }
        else
        {
            m_durationEndTime = float.MaxValue;
        }
    }

    // ressources
    void getRessourcesInitial()
    {
        if (m_ressourceManager == null)
        {
            Debug.Log("Warning?");
            m_ressourceFactorInitial = 1;
            return;
        }

        bool hasEnoughRessources = m_ressourceManager.hasEnoughRessurces(m_ressourcesMinNeededInitial);
        if (hasEnoughRessources)
        {
            m_ressourcesUsedInitial = m_ressourceManager.addRessources(m_ressourceCostsInitial);
            m_ressourceFactorsInitial = new float[m_ressourcesUsedInitial.Length];
            for (int i = 0; i < m_ressourceFactorsInitial.Length; i++)
                m_ressourceFactorsInitial[i] = m_ressourceCostsInitial[i] < 0 ? (m_ressourcesUsedInitial[i] / m_ressourceCostsInitial[i]) : 1f;
        }
        else
        {
            //m_ressourcesUsed = m_ressourceManager.addRessources(m_ressourceCosts);
            m_ressourceFactorsInitial = new float[m_ressourcesUsedInitial.Length];
            for (int i = 0; i < m_ressourceFactorsInitial.Length; i++)
                m_ressourceFactorsInitial[i] = 0;
        }

        m_ressourceFactorInitial = m_ressourceCurveInitial.Evaluate(Mathf.Clamp01(Mathf.Min(m_ressourceFactorsInitial)));
    }
    void getRessourcesUptime()
    {
        if (m_ressourceManager == null)
        {
            Debug.Log("Warning?");
            m_ressourceFactorUptime = 1;
            return;
        }

        bool hasEnoughRessources = m_ressourceManager.hasEnoughRessurces(Utility.multiplyFloatArray(m_ressourcesMinNeededUptime, Time.deltaTime));
        if (hasEnoughRessources)
        {
            m_ressourcesUsedUptime = m_ressourceManager.addRessources(Utility.multiplyFloatArray(m_ressourceCostsUptime, Time.deltaTime));
            m_ressourceFactorsUptime = new float[m_ressourcesUsedUptime.Length];
            for (int i = 0; i < m_ressourceFactorsUptime.Length; i++)
                m_ressourceFactorsUptime[i] = m_ressourceCostsUptime[i] < 0 ? (m_ressourcesUsedUptime[i] / m_ressourceCostsUptime[i] * Time.deltaTime) : 1f;
        }
        else
        {
            //m_ressourcesUsed = m_ressourceManager.addRessources(m_ressourceCosts);
            m_ressourceFactorsUptime = new float[m_ressourcesUsedUptime.Length];
            for (int i = 0; i < m_ressourceFactorsUptime.Length; i++)
                m_ressourceFactorsUptime[i] = 0;
        }

        m_ressourceFactorUptime = m_ressourceCurveUptime.Evaluate(Mathf.Clamp01(Mathf.Min(m_ressourceFactorsUptime)));
    }

    // input
    void getInput()
    {
        //if (keyCode0 == KeyCode.None && keyCode1 == KeyCode.None && keyCode2 == KeyCode.None)
        //return;

        if (Input.GetKey(keyCode0) || Input.GetKey(keyCode1) || Input.GetKey(keyCode2) || (m_core != null && m_core.GetComponent<CubeEntityState>().m_affiliation != CubeEntityState.s_AFFILIATION_PLAYER))
        {
            m_isPressing = true;
            m_currentPressingTime += Time.deltaTime;
        }
        else
        {
            m_isPressingLong = false;
            m_isPressing = false;
        }

        if (Input.GetKeyDown(keyCode0) || Input.GetKeyDown(keyCode1) || Input.GetKeyDown(keyCode2) || (m_core != null && m_core.GetComponent<CubeEntityState>().m_affiliation != CubeEntityState.s_AFFILIATION_PLAYER))
        {
            m_pressedThisFrame = true;
            m_currentPressingTime = 0;
        }
        else
        {
            m_pressedThisFrame = false;
        }

        if (Input.GetKeyUp(keyCode0) || Input.GetKeyUp(keyCode1) || Input.GetKeyUp(keyCode2))
        {
            if (!m_wasActivatedByPressDown && m_currentPressingTime <= m_deactivateOnMaxTimePressed)
            {
                m_isPressingLong = true;
            }

            m_wasActivatedByPressDown = false;
        }

        
    }

    // copy
    public void setValuesPlain(MonsterEntitySkillReflect script)
    {
        m_spawnPrefab = script.m_spawnPrefab;
        m_prefabVisualBox = script.m_prefabVisualBox;
        m_prefabVisualSphere = script.m_prefabVisualSphere;


        //m_canSpawnMultiple = script.m_canSpawnMultiple;
        m_cooldownOnDoneMin = script.m_cooldownOnDoneMin;
        m_cooldownOnDoneMax = script.m_cooldownOnDoneMax;

        m_maxReflectObjects = script.m_maxReflectObjects;

        m_cooldownOnUseMin = script.m_cooldownOnUseMin;
        m_cooldownOnUseMax = script.m_cooldownOnUseMax;
        m_cooldownRdyTime = Time.time + Random.Range(m_cooldownOnUseMin, m_cooldownOnUseMax);
        m_durationMin = script.m_durationMin;
        m_durationMax = script.m_durationMax;

        m_maxSwitchesPerFrame = script.m_maxSwitchesPerFrame;
        m_afterImmuneTime = script.m_afterImmuneTime;

        m_sphereRadius = script.m_sphereRadius;

        m_movementScripts = new List<CubeEntityMovementAbstract>();
        foreach (CubeEntityMovementAbstract s in script.m_movementScripts)
            m_movementScripts.Add(s);
        m_onEnterExplosions = new List<GameObject>();
        foreach (GameObject s in script.m_onEnterExplosions)
            m_onEnterExplosions.Add(s);
        m_barrierRotationSpeed = script.m_barrierRotationSpeed;
        m_barrierMovementSpeed = script.m_barrierMovementSpeed;


        m_centerPositionRelativeVectorSphere = script.m_centerPositionRelativeVectorSphere;
        m_centerPositionRelativeVectorBox = script.m_centerPositionRelativeVectorBox;
        m_centerPositionRelativeVectorSphere = script.m_centerPositionRelativeVectorSphere;

        m_boxSizes = script.m_boxSizes;
    }

    // interfaces
    public void onCopy(ICopiable copiable)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValuesPlain((MonsterEntitySkillReflect)copiable);
    }
    public void onPostCopy()
    {
        m_core = ((MonsterEntityAbstractBase)Utility.getComponentInParents<MonsterEntityAbstractBase>(transform)).gameObject;
    }
    public void onStateChangePrepareRemove()
    {
        destroyBoxCollider();
        destroySphereCollider();
    }
    public void onRemove()
    {
        Destroy(this);
    }

}
