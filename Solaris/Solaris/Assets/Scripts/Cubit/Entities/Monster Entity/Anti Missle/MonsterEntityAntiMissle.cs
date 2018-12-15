using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntityAntiMissle : EntityCopiableAbstract, ICopiable, IRemoveOnStateChange//, IOnTargetedBy
{
    [Header("------- Settings ------")]
    public bool m_changeTargetsTarget;
    public bool m_onlyOnTargetCore;

    [Header("--- (Explosion) ---")]
    public float m_explosionDmg;
    public float m_explosionRadius;
    public float m_explosionCooldown;

    [Header("--- (Detection) ---")]
    public float m_detectionRadius;
    public float m_detectionCooldown;
    public int m_detectionQueuePerFrame;

    [Header("--- (Movement) ---")]
    public List<CubeEntityMovementAbstract> m_movementScripts;
    public List<CubeEntityMovementStartSpeed> m_startSpeedScripts;
    public List<GameObject> m_spawnEffects;

    [Header("--- (Effects On Target Set) ---")]
    public List<GameObject> m_effectsOnTargetSet;

    [Header("------- Debug -------")]
    public bool m_isInitialized;
    public CubeEntityTargetManager m_target;
    public MonsterEntitySkillAntiMissle m_originScript;
    public Queue<Collider> m_detectionQueue;
    //public Collider[] m_colliders;

    public bool m_hadTarget;

    public float m_detectionCooldownRdy;
    public float m_explosionCooldownRdy;

    public CubeEntityState m_ownStateScript;

    // Use this for initialization
    void Start ()
    {
        initializeStuff();
	}

    void initializeStuff()
    {
        if (m_isInitialized)
            return;

        m_detectionQueue = new Queue<Collider>();
        //m_colliders = new Collider[500];
        m_ownStateScript = GetComponent<CubeEntityState>();

        m_isInitialized = true;
    }

    // Update is called once per frame
    void Update ()
    {
        manageDetection();
        manageExplosion();
        manageStop();
	}

    // target
    public bool setTarget(CubeEntityTargetManager target)
    {
        CubeEntityState otherStateScript = target.GetComponent<CubeEntityState>();

        if (!(otherStateScript.m_state == CubeEntityState.s_STATE_ACTIVE && otherStateScript.m_affiliation != m_ownStateScript.m_affiliation && otherStateScript.m_type != CubeEntityState.s_TYPE_ANTI_MISSLE))
            return false;
        //CubeEntityPrefapSystem

        m_target = target;
        if (m_onlyOnTargetCore && m_originScript != null && m_originScript.m_core != null)
        {
            if (m_target.m_target != m_originScript.m_core.GetComponent<CubeEntityTargetManager>())
            {
                m_target = null;
                return false;
            }
        }
        if (m_target.m_targetedByActive.Count > 0)
        {
            m_target = null;
            return false;
        }


        m_target = target;


        
        CubeEntityTargetManager targetManager = GetComponent<CubeEntityTargetManager>();
        targetManager.setTarget(m_target);
        if (m_changeTargetsTarget)
        {
            target.GetComponent<CubeEntityTargetManager>().setTarget(targetManager);
        }

        CubeEntityAttached attachedScript = GetComponent<CubeEntityAttached>();
        if (attachedScript != null)
            attachedScript.deregisterAttach();


        manageMovement();
        manageEffects();

        return true;
        //m_origin.m_attachSystem.deregisterCube(gameObject);
    }
    void manageMovement()
    {
        GetComponent<CubeEntityMovement>().removeComponents(typeof(CubeEntityMovementAbstract));
        Vector3 targetPosition = Vector3.zero;
        Vector3 thisPosition = transform.position;
        foreach (CubeEntityMovementStartSpeed script in m_startSpeedScripts)
        {
            if (script == null)
                continue;

            Vector3[] customCoordinateSystem = new Vector3[3];
            customCoordinateSystem[0] = Vector3.zero;
            customCoordinateSystem[1] = customCoordinateSystem[0];
            customCoordinateSystem[2] = customCoordinateSystem[1];
            //customCoordinateSystem[2] = targetPosition - m_cubeToToss.transform.position;
            //customCoordinateSystem[1] = m_cubeToToss.transform.position - transform.position;
            //customCoordinateSystem[0] = Vector3.Cross(customCoordinateSystem[2], customCoordinateSystem[1]);
            //customCoordinateSystem[1] = Vector3.Cross(customCoordinateSystem[0], customCoordinateSystem[2]);

            script.applyMovement(gameObject, targetPosition, thisPosition, new Quaternion(), customCoordinateSystem, 1);
        }

        foreach (CubeEntityMovementAbstract script in m_movementScripts)
        {
            if (script == null)
                continue;

            CubeEntityMovementAbstract s = GetComponent<CubeEntityMovement>().addMovementComponent(script, m_target.gameObject, targetPosition);
            //s.m_forceFactor = Mathf.Sqrt(m_ressourceFactor);
        }


        //GetComponent<CubeEntityTargetManager>().setOrigin((CubeEntityTargetManager)Utility.getComponentInParents<CubeEntityTargetManager>(transform));
    }
    void manageEffects()
    {
        CubeEntityParticleSystem particleSystem = GetComponent<CubeEntityParticleSystem>();
        for(int i = 0; i < m_effectsOnTargetSet.Count; i++)
        {
            if (m_effectsOnTargetSet[i] == null)
                continue;

            particleSystem.createParticleEffect(m_effectsOnTargetSet[i]);
        }
    }

    // manage activision
    void manageDetection()
    {
        if (m_target != null)
            return;

        if (m_detectionRadius <= 0)
            return;

        manageDetectionGather();
        manageSetTargetCheck();
    }
    void manageDetectionGather()
    {
        if (m_detectionCooldownRdy > Time.time)
            return;
        m_detectionCooldownRdy = m_detectionCooldown + Time.time;

        m_detectionQueue.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_detectionRadius,(int)(Mathf.Pow(2, 8) + 0.1f));

        if (colliders.Length <= 0)
            return;

        for (int i = 0; i < colliders.Length; i++)
            m_detectionQueue.Enqueue(colliders[i]);
    }
    void manageSetTargetCheck()
    {
        int checksThisFrame = Mathf.Min(m_detectionQueue.Count, m_detectionQueuePerFrame);

        for(int i = 0; i < checksThisFrame; i++)
        {
            Collider collider = m_detectionQueue.Dequeue();

            CubeEntityState otherStateScript = collider.GetComponent<CubeEntityState>();

            if (!(otherStateScript.m_state == CubeEntityState.s_STATE_ACTIVE && otherStateScript.m_affiliation != m_ownStateScript.m_affiliation))
                continue;
            //CubeEntityPrefapSystem
            
            m_target = collider.GetComponent<CubeEntityTargetManager>();
            if (m_onlyOnTargetCore && m_originScript != null && m_originScript.m_core != null)
            {
                if (m_target.m_target != m_originScript.m_core.GetComponent<CubeEntityTargetManager>())
                {
                    m_target = null;
                    continue;
                }
            }
            if (m_target.m_targetedByActive.Count > 0)
            {
                m_target = null;
                continue;
            }

            if (!setTarget(m_target))
                Debug.Log("Warning: This should not have happened!");
            
            break;
        }
    }

    // explosion
    void manageExplosion()
    {
        if (m_target == null)
            return;

        if (m_explosionCooldownRdy > Time.time)
            return;
        m_explosionCooldownRdy = m_explosionCooldown + Time.time;



        Vector3 direction = m_target.transform.position - transform.position;
        if (direction.sqrMagnitude < m_explosionRadius * m_explosionRadius)
            explode();
    }
    void explode()
    {
        m_target.GetComponent<CubeEntityCharge>().evaluateDischarge(-m_explosionDmg, GetComponent<CubeEntityState>().m_affiliation, CubeEntityState.s_STATE_ACTIVE, true);
        GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
    }

    // stop
    void manageStop()
    {
        bool hasTarget = m_target != null && m_target.m_target != null;
        if(m_hadTarget && !hasTarget)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);

        if (hasTarget)
            m_hadTarget = true;
    }

    // copy
    public void setValues(MonsterEntitySkillAntiMissle script, CubeEntityTargetManager target)
    {
        m_originScript = script;
        m_target = target;
    }
    void setValues(MonsterEntityAntiMissle copyScript)
    {
        m_explosionDmg = copyScript.m_explosionDmg;
        m_explosionRadius = copyScript.m_explosionRadius;
        m_detectionRadius = copyScript.m_detectionRadius;
        m_detectionCooldown = copyScript.m_detectionCooldown;
        m_detectionQueuePerFrame = copyScript.m_detectionQueuePerFrame;
        m_changeTargetsTarget = copyScript.m_changeTargetsTarget;
        m_onlyOnTargetCore = copyScript.m_onlyOnTargetCore;

        m_movementScripts = new List<CubeEntityMovementAbstract>();
        for (int i = 0; i < copyScript.m_movementScripts.Count; i++)
            m_movementScripts.Add(copyScript.m_movementScripts[i]);

        m_startSpeedScripts = new List<CubeEntityMovementStartSpeed>();
        for (int i = 0; i < copyScript.m_startSpeedScripts.Count; i++)
            m_startSpeedScripts.Add(copyScript.m_startSpeedScripts[i]);

        m_effectsOnTargetSet = new List<GameObject>();
        for(int i = 0; i < copyScript.m_effectsOnTargetSet.Count; i++)
            m_effectsOnTargetSet.Add(copyScript.m_effectsOnTargetSet[i]);

        //EntitySystemBase entitySystemBaseScript = GetComponent<EntitySystemBase>();
        //m_movementScripts = new List<CubeEntityMovementAbstract>();
        //foreach (CubeEntityMovementAbstract movementScript in copyScript.m_movementScripts)
        //{
        //    CubeEntityMovementAbstract script = null;

        //    if (movementScript is ICopiable)
        //        script = (CubeEntityMovementAbstract)(GetComponent<EntitySystemBase>().copyPasteComponent((ICopiable)movementScript, true));
        //    else
        //        Debug.Log("Should not happen!");


        //    if (script == null)
        //        Debug.Log("Should not happen!");
        //    else
        //        script.pasteScriptButDontActivate(movementScript);


        //    if (script != null)
        //        m_movementScripts.Add(script);
        //}

        //m_startSpeedScripts = new List<CubeEntityMovementStartSpeed>();
        //if (m_startSpeedScripts != null)
        //{
        //    foreach (CubeEntityMovementStartSpeed e in m_startSpeedScripts)
        //    {
        //        if (e == null)
        //            continue;

        //        CubeEntityMovementStartSpeed script = null;

        //        if (e is ICopiable)
        //            script = (CubeEntityMovementStartSpeed)(entitySystemBaseScript.copyPasteComponent(e, false));
        //        else
        //            Debug.Log("Should not happen!");

        //        if (script == null)
        //            Debug.Log("Should not happen!");


        //        m_startSpeedScripts.Add(script);
        //    }
        //}
    }

    // interface
    public void onCopy(ICopiable copiable)
    {
        //m_target = GetComponent<CubeEntityTargetManager>();
        initializeStuff();
        setValues((MonsterEntityAntiMissle)copiable);
    }
    public void onRemove()
    {
        Destroy(this);
    }
    //public void onTargetedBy(CubeEntityTargetManager targetedBy)
    //{

    //}

    //public override void pasteScript(CubeEntityMovementAbstract baseScript, GameObject target, Vector3 targetPosition)
    //{
    //    initializeStuff();
    //    setValues((CubeEntityMovementAccelerateAndStop)baseScript);
    //    base.pasteScript(baseScript, target, targetPosition);
    //}

    //public override void pasteScriptButDontActivate(CubeEntityMovementAbstract baseScript)
    //{
    //    setValues((CubeEntityMovementAccelerateAndStop)baseScript);
    //}

    //public override bool updateDestroy()
    //{
    //    return true;
    //}
    //public override void getForceVector()
    //{

    //}
}
