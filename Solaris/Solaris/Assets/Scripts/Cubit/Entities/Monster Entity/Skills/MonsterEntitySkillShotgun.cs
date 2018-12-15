using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntitySkillShotgun : EntityCopiableAbstract, ICopiable, IPostCopy, IRemoveOnStateChange
{
    public bool m_useSkill = true;
    public GameObject m_spawnPrefab;
    [Header("------- Input -------")]
    public KeyCode keyCode0;
    public KeyCode keyCode1;
    public KeyCode keyCode2;

    [Header("----- Settings -----")]
    //public float m_cooldown;
    //public float m_bonusRandomCooldown;
    //public float m_shootDurationFrames;
    //public int m_shotsPerFrame;

    [Header("--- Cooldown ---")]
    public float m_burstCooldownMin;
    public float m_burstCooldownMax;
    public int m_burstNumberMin;
    public int m_burstNumberMax;
    [Space]
    public float m_isBurstingCooldownMin;
    public float m_isBurstingCooldownMax;
    public int m_isBurstingShotsPerFrameMin;
    public int m_isBurstingShotsPerFrameMax;

    [Header("--- Delay ---")]
    public float m_burstDelay;
    public List<GameObject> m_onDelayEffects;
    public Vector3 m_onDelayEffectsSpawnPositionLocal;
    public bool m_onDelayEffectAlignWithCamera;

    [Header("--- Constraints ---")]
    public float m_requireLos;
    public LayerMask m_requireLosLayermask;
    [Space]
    public float m_requireDistance;


    [Header("--- Ressources) ---")]
    public float[] m_ressourceCosts;
    public float[] m_ressourcesMinNeeded;
    public AnimationCurve m_ressourceCurve;

    [Header("--- (Movement) ---")]
    public List<CubeEntityMovementAbstract> m_movementScripts;
    public List<CubeEntityMovementStartSpeed> m_startSpeedScripts;
    public List<GameObject> m_spawnEffects;
    public List<CubeEntityMovementAbstract> m_movementScriptsSelf;
    //public float m_startSpeed;

    [Header("--- (SpawnPosition) ---")]
    public float m_spawnPositionVectorZMin;
    public Vector3 m_spawnPositionVectorMin;
    public Vector3 m_spawnPositionVectorMax;
    public float m_angleStep;

    [Header("--- Conditions ---")]
    public int m_minCubes;
    public float m_minDistanceToCore;

    [Header("--- (Aim) ---")]
    public bool m_useAimHelp = true;
    public float m_rangeIfNoTarget;
    public float m_randomRadius;
    public float m_shootInFlightDirectionMin;
    public float m_shootInFlightDirectionMax;

    [Header("--- (Cube Selection) ---")]
    public bool m_reduceMaxGrabbed;
    [Header("- From Grabbed -")]
    public bool m_selectNearestFirst;
    public bool m_fromGrabbedOnly;
    public bool m_fromGrabbedFirst;
    [Header("- Created -")]
    public bool m_createIfNoneFound;
    public bool m_createOnly;

    [Header("------- Debug -------")]
    public float m_shootingReady;
    public bool m_isInitialized;
    public List<GameObject> m_cubesToShoot;
    public Dictionary<GameObject, Vector3> m_targetPositions;
    //public int m_framesShot;
    public GameObject m_target;
    public Vector3 m_targetPosition;
    public GameObject m_core;
    public AttachSystemBase m_attachSystem;
    public float m_shootPositionIndex;
    public List<GameObject> potentialCubes;
    public List<GameObject> m_onDelayEffectsActives;

    public int m_maxShotsThisburst;
    public int m_thisBurstCubesShotNumber;

    public CubeEntityRessourceManager m_ressourceManager;
    public float m_burstReady;
    public CubeEntityMovement m_selfMovmentEntity;
    public bool m_isBursting;
    public bool m_isDelay;
    public float m_delayRdy;
    //public float m_burstEndTime;

    public float m_isBurstingReady;
    public bool m_shotThisFrame;
    //public float m_isBurstingEndTime;
    public float[] m_ressourcesUsed;
    public float[] m_ressourceFactors;
    public float m_ressourceFactor;
    public bool selfMovementScriptApplied = false;
    public CubeEntityParticleSystem m_particleScript;
    public float m_checkCooldownRdy;

    // Use this for initialization
    void Start ()
    {
        if(!m_isInitialized)
            initializeStuff();
	}
    void initializeStuff()
    {
        //m_cooldownReady = m_cooldown + Time.time + Random.Range(0, m_bonusRandomCooldown);
        
        if (m_core == null)
            m_core = ((MonsterEntityAbstractBase)Utility.getComponentInParents<MonsterEntityAbstractBase>(transform)).gameObject;

        if (m_ressourceManager == null)
            m_ressourceManager = (CubeEntityRessourceManager)Utility.getComponentInParents<CubeEntityRessourceManager>(transform);
        if (m_ressourceManager == null)
            Debug.Log("Warning: ressourceManager is null!");


        if (m_particleScript == null)
            m_particleScript = (CubeEntityParticleSystem)Utility.getComponentInParents<CubeEntityParticleSystem>(transform);

        m_attachSystem = (AttachSystemBase)Utility.getComponentInParents<AttachSystemBase>(transform);
        m_selfMovmentEntity = (CubeEntityMovement)Utility.getComponentInParents<CubeEntityMovement>(transform);

        m_cubesToShoot = new List<GameObject>();
        potentialCubes = new List<GameObject>();
        m_targetPositions = new Dictionary<GameObject, Vector3>();
        m_onDelayEffectsActives = new List<GameObject>();

        m_isInitialized = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!m_useSkill)//|| m_attachSystem == null)
            return;

        manageCounter();
        manageShot();
	}

    void manageCounter()
    {
        // check if start bursting
        if (isConstraints())
        {
            getRessources();
            if (m_ressourceFactor > 0)
            {
                createDelayEffects();
                m_isDelay = true;
                m_delayRdy = m_burstDelay + Time.time;
            }
        }

        // check delay
        if(m_isDelay)
        {
            if(m_delayRdy <= Time.time)
            {
                selfMovementScriptApplied = false;
                m_thisBurstCubesShotNumber = 0;
                m_maxShotsThisburst = Random.Range(m_burstNumberMin, m_burstNumberMax);
                m_isBursting = true;
                m_isDelay = false;
            }
            moveDelayEffects();
        }

        // check if stop bursting
        if(m_isBursting)
        {
            if (m_thisBurstCubesShotNumber >= m_maxShotsThisburst)
            {
                m_burstReady = Random.Range(m_burstCooldownMin, m_burstCooldownMax) * Mathf.Clamp(1f / m_ressourceFactor, 1f, 10f) + Time.time;
                m_isBursting = false;
                m_ressourceFactor = 1;
            }
        }

        // check cooldown while bursting
        if(m_isBursting)
        {
            if (m_isBurstingReady <= Time.time)
            {
                m_isBurstingReady = Random.Range(m_isBurstingCooldownMin, m_isBurstingCooldownMax) + Time.time;
                m_shotThisFrame = true;
            }
            else
            {
                m_shotThisFrame = false;
            }
        }
        else
        {
            m_shotThisFrame = false;
        }
    }
    bool isConstraints()
    {
        if (m_checkCooldownRdy > Time.time)
            return false;

        m_target = m_core.GetComponent<CubeEntityTargetManager>().getTarget();
        if (m_useAimHelp && m_target != null)
            m_targetPosition = m_target.transform.position;
        else
        {
            if (gameObject.layer == Constants.s_playerLayer)
                m_targetPosition = transform.position + Constants.getMainCamera().transform.forward * m_rangeIfNoTarget;
            else
            {
                Debug.Log("Caution");
                m_targetPosition = transform.forward * m_rangeIfNoTarget;
            }
        }

        Vector3 direction = m_targetPosition - transform.position;
        bool isDistance = true;
        if (m_requireDistance > 0)
        {
            float sqrDist = Vector3.SqrMagnitude(direction);
            if (sqrDist > m_requireDistance * m_requireDistance)
            {
                isDistance = false;
                m_checkCooldownRdy = Time.time + 0.5f;
            }
        }

        bool isLos = true;
        if (m_requireLos > 0)
        {
            //Debug.DrawRay(transform.position, m_targetPosition - transform.position, Color.red, 1f);
            if (Physics.Raycast(transform.position, m_targetPosition - transform.position, Mathf.Min(direction.magnitude * 0.95f, m_requireLos), m_requireLosLayermask))
            {
                isLos = false;
                m_checkCooldownRdy = Time.time + 0.5f;
            }
        }



        return !m_isDelay && !m_isBursting && m_burstReady <= Time.time && isPressingKey() && isLos && isDistance;
    }

    /*
void manageShot()
{
    if (!m_isBursting)
        return;


    if (m_framesShot >= m_shootDurationFrames)
    {
        m_cooldownReady = m_cooldown + Random.Range(0, m_bonusRandomCooldown) + Time.time;
        m_isBursting = false;
        return;
    }

    //Debug.Log(m_core.GetComponent<MonsterEntityBase>());

    m_target = m_core.GetComponent<CubeEntityTargetManager>().getTarget();
    if (m_target != null)
    {
        selectCubes();
        if (m_cubesToShoot.Count > 0)
        {
            getTargetPositions();
            shootCubes();
        }
    }

    m_framesShot++;
}
    */
    void manageShot()
    {
        if (!m_isBursting || !m_shotThisFrame)
            return;
        
        m_target = m_core.GetComponent<CubeEntityTargetManager>().getTarget();
        if (m_useAimHelp && m_target != null)
            m_targetPosition = m_target.transform.position;
        else
        {
            if (gameObject.layer == Constants.s_playerLayer)
                m_targetPosition = transform.position + Constants.getMainCamera().transform.forward * m_rangeIfNoTarget;
            else
            {
                Debug.Log("Caution");
                m_targetPosition = transform.forward * m_rangeIfNoTarget;
            }
        }

        //if (m_target != null)
        {
            selectCubes();
            if (m_cubesToShoot.Count > 0)
            {
                getTargetPositions();
                shootCubes();
            }
        }
    }


    void selectCubes()
    {
        m_cubesToShoot.Clear();
        potentialCubes = new List<GameObject>();

        int cubeShotNumberThisFrame = Random.Range(m_isBurstingShotsPerFrameMin, m_isBurstingShotsPerFrameMax);
        cubeShotNumberThisFrame = Mathf.Min(cubeShotNumberThisFrame, m_maxShotsThisburst - m_thisBurstCubesShotNumber);
        if (cubeShotNumberThisFrame == 0)
            return;

        if (m_fromGrabbedOnly)
        {
            potentialCubes = m_attachSystem.m_cubeList;

            if (m_selectNearestFirst)
            {
                Debug.Log("Caution: Feature not implemented yet!");
                potentialCubes = sortList(potentialCubes, m_targetPosition);
            }
            //for (int i = 0; i < m_shotsPerFrame; i++)
            for (int i = 0; i < cubeShotNumberThisFrame; i++)
            {
                if (i >= potentialCubes.Count)
                    break;

                m_cubesToShoot.Add(potentialCubes[i]);
            }

            /*
            foreach (GameObject cube in potentialCubes)
            {
                if (m_cubesToShoot.Count >= m_shotsPerFrame)
                    break;

                float dist = Vector3.Distance(cube.transform.position, transform.position);
                if (dist > 0 & dist <= m_minDistanceToCore)
                {
                    Debug.Log("SAD");
                    m_cubesToShoot.Add(cube);
                }
            }
            */
        }

        if(m_createOnly)
        {
            m_cubesToShoot.Clear();
            while (m_cubesToShoot.Count < cubeShotNumberThisFrame && Constants.getMainCge().m_inactiveCubes.Count > 0)
            {
                //Vector3 spawnPosition = transform.position + (m_targetPosition - transform.position).normalized * 15f + Random.insideUnitSphere * 10f;
                //Vector3 spawnPosition = transform.position + Random.insideUnitSphere.normalized * 15f;

                float angle = 0;
                if (m_angleStep < 0)
                    angle = Random.Range(0, 360f);
                else
                {
                    m_shootPositionIndex++;
                    if (m_shootPositionIndex >= 180)
                    {
                        angle = (360 - (m_shootPositionIndex - 180)) * Mathf.PI / 180;
                    }
                    else
                    {
                        angle = m_shootPositionIndex * Mathf.PI / 180;
                    }
                    m_shootPositionIndex += m_angleStep;
                    m_shootPositionIndex %= 360;
                }

                Vector3 direction = m_targetPosition - transform.position;
                Vector3 directionX = Vector3.Cross(direction, Vector3.up).normalized * (angle >= 0 ? Mathf.Sin(angle) : 1);// * Random.Range(-1f, 1f);
                Vector3 directionY = Vector3.Cross(directionX, direction).normalized * (angle >= 0 ? Mathf.Cos(angle) : 1);//* Random.Range(-1f, 1f);
                float range = Random.Range(m_spawnPositionVectorMin.z, m_spawnPositionVectorMax.z);
                if (m_target != null && m_spawnPositionVectorZMin > 0)
                    range = Mathf.Min(range, (m_targetPosition - transform.position).magnitude - m_spawnPositionVectorZMin);
                Vector3 directionZ = direction.normalized * range;// Mathf.Min(Random.Range(m_spawnPositionVectorMin.z, m_spawnPositionVectorMax.z), (m_targetPosition - transform.position).magnitude - 20f);

                //Debug.DrawRay(transform.position, directionX, Color.blue, m_cooldown);
                //Debug.DrawRay(transform.position, directionY, Color.green, m_cooldown);
                //Debug.DrawRay(transform.position, directionZ, Color.red, m_cooldown);

                Vector3 finalDirection = (directionX * Random.Range(m_spawnPositionVectorMin.x, m_spawnPositionVectorMax.x) + directionY * Random.Range(m_spawnPositionVectorMin.y, m_spawnPositionVectorMax.y)) + directionZ;

                Vector3 spawnPosition = transform.position + finalDirection;
                GameObject cube = Constants.getMainCge().activateCubeSafe(spawnPosition);
                //if(cube.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
                //{
                    //cube.GetComponent<Rigidbody>().velocity = (spawnPosition - transform.position).normalized * m_startSpeed;
                //}
                m_cubesToShoot.Add(cube);
            }
        }

        if(m_fromGrabbedFirst)
        {
            potentialCubes = m_attachSystem.m_cubeList;

            if (m_selectNearestFirst)
            {
                Debug.Log("Caution: Feature not implemented yet!");
                potentialCubes = sortList(potentialCubes, m_targetPosition);
            }

            foreach (GameObject cube in potentialCubes)
            {
                if (m_cubesToShoot.Count >= cubeShotNumberThisFrame)
                    break;

                float dist = Vector3.Distance(cube.transform.position, transform.position);
                if (dist > 0 & dist <= m_minDistanceToCore)
                {
                    m_cubesToShoot.Add(cube);
                }
            }

            if(m_createIfNoneFound)
            {
                while (m_cubesToShoot.Count < cubeShotNumberThisFrame)
                {
                    Vector3 spawnPosition = transform.position + (m_targetPosition - transform.position).normalized * 15f + Random.insideUnitSphere * 10f;
                    GameObject cube = Constants.getMainCge().activateCubeSafe(spawnPosition);
                    if (cube.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
                    {
                        cube.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere * 3f;
                    }
                    m_cubesToShoot.Add(cube);
                }
            }
        }
    }
    List<GameObject> sortList(List<GameObject> list, Vector3 targetPosition)
    {
        return list;
    }
    void getRessources()
    {
        if(m_ressourceManager == null)
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

    void getTargetPositions()
    {
        foreach (GameObject cube in m_cubesToShoot)
        {
            if (m_target != null && (m_shootInFlightDirectionMin > 0 || m_shootInFlightDirectionMax > 0) && m_movementScripts.Count > 0)// && false)
            {
                Vector3 targetDirection = m_target.GetComponent<Rigidbody>().velocity;
                float dist = Vector3.Distance(m_targetPosition, cube.transform.position);
                //Debug.Log(m_targetPositions[cube]);
                m_targetPositions[cube] = m_targetPosition + targetDirection * (dist / m_movementScripts[0].m_maxSpeed) * Random.Range(m_shootInFlightDirectionMin, m_shootInFlightDirectionMax) + Random.insideUnitSphere * m_randomRadius;
            }
            else
            {
                //Debug.Log(m_targetPositions[cube]);
                m_targetPositions[cube] = m_targetPosition;
            }
        }
    }

    void shootCubes()
    {
        foreach(GameObject cube in m_cubesToShoot)
        {
            //if (m_attachSystem.m_cubeList.Count <= m_minCubes)
            //    break;
            if (!m_createOnly)   // TODO : m_createIfNoneFound case
            {
                //m_attachSystem.deregisterCube(cube);
                if(m_reduceMaxGrabbed)
                {
                    m_attachSystem.reduceMaxGrabbed(1);
                }
            }
            if (m_createIfNoneFound) // TODO : ^ look above ^
                Debug.Log("Warning: Code was not prepared for this case yet!"); // reason: will try to remove cube from attach system, but maybe the cube was not attached at all but created.
                                                                                //         Trying to get along without an "if(attachSystem.m_cubes.Contains(agent))" call.

            //cube.GetComponent<CubeEntityMovement>().removeComponents(typeof(CubeEntityMovementAbstract));

            if (m_spawnPrefab != null)
                cube.GetComponent<CubeEntityPrefapSystem>().setToPrefab(m_spawnPrefab);
            else
            {
                Debug.Log("Caution!");
                cube.GetComponent<CubeEntitySystem>().setActiveDynamicly(m_core.GetComponent<CubeEntityState>());
            }
            cube.GetComponent<CubeEntityCharge>().applyFactor(m_ressourceFactor);

            foreach(GameObject effect in m_spawnEffects)
            {
                CubeEntityParticleSystem.createParticleEffet(effect, cube.transform.position);
            }

            // set affiliation manually
            cube.GetComponent<CubeEntityState>().m_affiliation = m_core.GetComponent<CubeEntityState>().m_affiliation;
            
            // apply start speed
            foreach (CubeEntityMovementStartSpeed script in m_startSpeedScripts)
            {
                if (script == null)
                    continue;

                Vector3[] customCoordinateSystem = new Vector3[3];
                customCoordinateSystem[2] = m_targetPosition - cube.transform.position;
                customCoordinateSystem[1] = cube.transform.position - transform.position;
                customCoordinateSystem[0] = Vector3.Cross(customCoordinateSystem[2], customCoordinateSystem[1]);
                customCoordinateSystem[1] = Vector3.Cross(customCoordinateSystem[0], customCoordinateSystem[2]);

                script.applyMovement(cube, m_targetPositions[cube], m_core.transform.position, (gameObject.layer == 9 ? Constants.getMainCamera().transform.rotation : m_core.transform.rotation), customCoordinateSystem, m_ressourceFactor);
            }

            // apply movement scripts
            foreach (CubeEntityMovementAbstract script in m_movementScripts)
            {
                if (script == null)
                    continue;

                CubeEntityMovementAbstract s = cube.GetComponent<CubeEntityMovement>().addMovementComponent(script, m_target, m_targetPositions[cube]);
                s.m_forceFactor = Mathf.Sqrt(m_ressourceFactor);
            }

            // apply movement scripts self
            if (!selfMovementScriptApplied)
            {
                selfMovementScriptApplied = true;
                foreach (CubeEntityMovementAbstract script in m_movementScriptsSelf)
                {
                    if (script == null)
                        continue;

                    CubeEntityMovementAbstract s = m_selfMovmentEntity.addMovementComponent(script, m_target, m_targetPositions[cube]);
                    s.m_forceFactor = Mathf.Sqrt(m_ressourceFactor);
                }
            }

            cube.GetComponent<CubeEntityTargetManager>().setTarget(m_target);
            cube.GetComponent<CubeEntityTargetManager>().setOrigin((CubeEntityTargetManager)Utility.getComponentInParents<CubeEntityTargetManager>(transform));

            cube.transform.rotation = Quaternion.LookRotation((m_targetPositions[cube]));

            m_thisBurstCubesShotNumber++;
        }
    }

    // effects
    void createDelayEffects()
    {
        foreach (GameObject effect in m_onDelayEffects)
        {
            GameObject e = m_particleScript.createParticleEffect(effect);
            if (m_onDelayEffectAlignWithCamera)
            {
                e.transform.position += Constants.getMainCamera().transform.rotation * m_onDelayEffectsSpawnPositionLocal;
            }
            else
            {
                e.transform.position += transform.rotation * m_onDelayEffectsSpawnPositionLocal;
            }
            m_onDelayEffectsActives.Add(e);
        }
    }
    void moveDelayEffects()
    {
        for(int i = m_onDelayEffectsActives.Count - 1; i >= 0; i--)
        {
            GameObject effect = m_onDelayEffectsActives[i];
            if (effect == null)
            {
                m_onDelayEffectsActives.RemoveAt(i);
                continue;
            }
            if (m_onDelayEffectAlignWithCamera)
            {
                effect.transform.position = transform.position + Constants.getMainCamera().transform.rotation * m_onDelayEffectsSpawnPositionLocal;
            }
        }
    }

    // input
    bool isPressingKey()
    {
        return gameObject.layer != Constants.s_playerLayer || Input.GetKey(keyCode0) || Input.GetKey(keyCode1) || Input.GetKey(keyCode2);
    }

    // copy
    void setValues(MonsterEntitySkillShotgun baseScript)
    {
        m_useSkill = baseScript.m_useSkill;
        m_spawnPrefab = baseScript.m_spawnPrefab;

        m_reduceMaxGrabbed = baseScript.m_reduceMaxGrabbed;
        //m_cooldown = baseScript.m_cooldown;
        //m_bonusRandomCooldown = baseScript.m_bonusRandomCooldown;
        //m_shootDurationFrames = baseScript.m_shootDurationFrames;
        //m_shotsPerFrame = baseScript.m_shotsPerFrame;
        m_burstCooldownMin = baseScript.m_burstCooldownMin;
        m_burstCooldownMax = baseScript.m_burstCooldownMax;
        m_burstNumberMin = baseScript.m_burstNumberMin;
        m_burstNumberMax = baseScript.m_burstNumberMax;

        m_isBurstingCooldownMin = baseScript.m_isBurstingCooldownMin;
        m_isBurstingCooldownMax = baseScript.m_isBurstingCooldownMax;
        m_isBurstingShotsPerFrameMin = baseScript.m_isBurstingShotsPerFrameMin;
        m_isBurstingShotsPerFrameMax = baseScript.m_isBurstingShotsPerFrameMax;

        m_burstDelay = baseScript.m_burstDelay;
        m_requireLos = baseScript.m_requireLos;
        m_requireLosLayermask = baseScript.m_requireLosLayermask;
        m_requireDistance = baseScript.m_requireDistance;
        m_onDelayEffects = new List<GameObject>();
        foreach (GameObject o in baseScript.m_onDelayEffects)
            m_onDelayEffects.Add(o);
        m_onDelayEffectsSpawnPositionLocal = baseScript.m_onDelayEffectsSpawnPositionLocal;
        m_onDelayEffectAlignWithCamera = baseScript.m_onDelayEffectAlignWithCamera;

        m_rangeIfNoTarget = baseScript.m_rangeIfNoTarget;

        m_selectNearestFirst = baseScript.m_selectNearestFirst;

        m_movementScripts = new List<CubeEntityMovementAbstract>();
        foreach (CubeEntityMovementAbstract o in baseScript.m_movementScripts)
            m_movementScripts.Add(o);
        m_spawnEffects = new List<GameObject>();
        foreach (GameObject o in baseScript.m_spawnEffects)
            m_spawnEffects.Add(o);
        m_movementScriptsSelf = new List<CubeEntityMovementAbstract>();
        foreach (CubeEntityMovementAbstract o in baseScript.m_movementScriptsSelf)
            m_movementScriptsSelf.Add(o);
        //m_startSpeed = baseScript.m_startSpeed;
        


        m_fromGrabbedOnly = baseScript.m_fromGrabbedOnly;
        m_fromGrabbedFirst = baseScript.m_fromGrabbedFirst;
        m_createIfNoneFound = baseScript.m_createIfNoneFound;
        m_createOnly = baseScript.m_createOnly;

        m_minCubes = baseScript.m_minCubes;
        m_minDistanceToCore = baseScript.m_minDistanceToCore;

        m_randomRadius = baseScript.m_randomRadius;
        m_shootInFlightDirectionMin = baseScript.m_shootInFlightDirectionMin;
        m_shootInFlightDirectionMax = baseScript.m_shootInFlightDirectionMax;
    }

    // interface
    public void onCopy(ICopiable copiable)
    {
        if(!m_isInitialized)
            initializeStuff();
        setValues((MonsterEntitySkillShotgun)copiable);
    }
    public void onPostCopy()
    {
        m_attachSystem = (AttachSystemBase)Utility.getComponentInParents<AttachSystemBase>(transform);
        m_core = ((MonsterEntityAbstractBase)Utility.getComponentInParents<MonsterEntityAbstractBase>(transform)).gameObject;
        m_burstReady = Random.Range(m_burstCooldownMin, m_burstCooldownMax) + Time.time;
    }
    public void onRemove()
    {
        Destroy(this);
    }

    /*
    // abstract
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((MonsterEntitySkillShotgun)baseScript);
    }
    public override void prepareDestroyScript()
    {
        Destroy(this);
    }
    public override void onPostCopy()
    {
        m_attachSystem = (AttachSystemBase)Utility.getComponentInParents<AttachSystemBase>(transform);
        m_core = Utility.getCoreObjectInParents(transform);
        m_cooldownReady = m_cooldown + Time.time + Random.Range(0, m_bonusRandomCooldown);
    }*/
}
