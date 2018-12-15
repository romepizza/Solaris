using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntitySkillEject : EntityCopiableAbstract, ICopiable, IPostCopy, IRemoveOnStateChange
{
    public CubeEntityMovementAbstract[] m_movementScripts;
    [Header("----- SETTINGS -----")]
    public float m_cooldown;
    //public float m_duration;
    //public float m_power;
    //public float m_maxSpeed;
    public bool m_selectNearestCube;
    public int m_minCubes;
    public float m_minDistanceToCore;

    [Header("--- (Shoot Help) ---")]
    public float m_minShootInDirection;
    public float m_maxShootInDirection;

    [Header("----- DEBUG -----")]
    public GameObject m_cubeToShoot;
    public List<GameObject> m_potentialCubes;
    public bool m_isShooting;
    public float m_cooldownFinishTime;
    public GameObject m_target;
    public Vector3 m_targetPosition;

    public AttachSystemBase m_attachSystem;
    public MonsterEntityBase m_base;
    public bool m_isInitialized;

	// Use this for initialization
	void Start ()
    {
        if (!m_isInitialized)
            initializeStuff();
    }
    void initializeStuff()
    {
        m_potentialCubes = new List<GameObject>();
        m_target = Constants.getPlayer();

        m_isInitialized = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        manageShot();
	}

    void manageShot()
    {
        if (m_isShooting && m_cooldownFinishTime < Time.time)
        {
            m_target = GetComponent<CubeEntityTargetManager>().getTarget();
            selectCube();
            if (m_cubeToShoot != null)
            {
                getTargetPosition();
                shoot();
            }
        }
    }


    void getTargetPosition()
    {
        Vector3 targetDirection = m_target.GetComponent<Rigidbody>().velocity;
        float dist = Vector3.Distance(m_target.transform.position, m_cubeToShoot.transform.position);
        m_targetPosition = m_target.transform.position + targetDirection * (dist / m_movementScripts[0].m_maxSpeed) * Random.Range(m_minShootInDirection, m_maxShootInDirection);
    }

    void selectCube()
    {
        m_potentialCubes = m_attachSystem.m_cubeList;

        float minDistance = float.MaxValue;
        foreach (GameObject cube in m_potentialCubes)
        {
            float distToCore = Vector3.Distance(cube.transform.position, transform.position);
            float dist = Vector3.Distance(cube.transform.position, m_target.transform.position);
            if (dist < minDistance && distToCore < m_minDistanceToCore)
            {
                m_cubeToShoot = cube;
                minDistance = dist;
            }
        }
    }

    void shoot()
    {
        m_cooldownFinishTime = m_cooldown + Time.time;

        //m_attachSystem.deregisterCube(m_cubeToShoot);
        //m_cubeToShoot.GetComponent<Rigidbody>().velocity = Vector3.zero;// m_cubeToShoot.GetComponent<Rigidbody>().velocity.normalized * Mathf.Sqrt(m_cubeToShoot.GetComponent<Rigidbody>().velocity.magnitude);

        m_cubeToShoot.GetComponent<CubeEntitySystem>().setActiveDynamicly(GetComponent<CubeEntityState>());

        m_cubeToShoot.GetComponent<CubeEntitySystem>().getMovementComponent().removeComponents(typeof(CubeEntityMovementAbstract));
        //m_cubeToShoot.GetComponent<CubeEntitySystem>().getMovementComponent().addAccelerationComponent(m_targetPosition, m_duration, m_power, m_maxSpeed);
        if(m_movementScripts.Length < 1)
        {
            Debug.Log("Aborted: m_movementScript was null!");
            return;
        }
        foreach (CubeEntityMovementAbstract script in m_movementScripts)
        {
            if (script == null)
                continue;
            m_cubeToShoot.GetComponent<CubeEntitySystem>().getMovementComponent().addMovementComponent(script, m_target, m_targetPosition);
        }
    }

    public void setValuesByScript(GameObject prefab)
    {
        MonsterEntitySkillEject script = prefab.GetComponent<MonsterEntitySkillEject>();
        m_cooldown = script.m_cooldown;
        m_cooldownFinishTime = m_cooldown + Time.time;
        m_minCubes = script.m_minCubes;
        m_minShootInDirection = script.m_minShootInDirection;
        m_maxShootInDirection = script.m_maxShootInDirection;
        m_minCubes = script.m_minCubes;
        m_minDistanceToCore = script.m_minDistanceToCore;

        // Debug
        m_isShooting = script.m_isShooting;
    }

    public void setValuesPlain(MonsterEntitySkillEject script)
    {
        m_cooldown = script.m_cooldown;
        m_cooldownFinishTime = m_cooldown + Time.time;
        m_minCubes = script.m_minCubes;
        m_minShootInDirection = script.m_minShootInDirection;
        m_maxShootInDirection = script.m_maxShootInDirection;
        m_minCubes = script.m_minCubes;
        m_minDistanceToCore = script.m_minDistanceToCore;
        m_movementScripts = script.m_movementScripts;

        // Debug
        m_isShooting = script.m_isShooting;
    }

    // interfaces
    public void onCopy(ICopiable copiable)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValuesPlain((MonsterEntitySkillEject)copiable);
    }
    public void onPostCopy()
    {
        m_attachSystem = GetComponent<AttachSystemBase>();
        m_base = GetComponent<MonsterEntityBase>();
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
        setValuesPlain((MonsterEntitySkillEject)baseScript);
    }
    public override void prepareDestroyScript()
    {
        Destroy(this);
    }
    public override void onPostCopy()
    {
        m_attachSystem = GetComponent<AttachSystemBase>();
        m_base = GetComponent<MonsterEntityBase>();
    }*/
}
