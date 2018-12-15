using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntitySkillTeleport : EntityCopiableAbstract, ICopiable, IPostCopy, IRemoveOnStateChange
{
    public bool m_useSkill = true;

    [Header("------- Settings -------")]
    public bool m_teleportCubesAswell;

    [Header("--- (Teleport) ---")]
    public float m_teleportDistance;
    public float m_teleportRandomRadius;
    public float m_teleportDotValue;
    //public bool m_useConsiderTargetDistance;

    [Header("- (Consideration) -")]
    public float m_considerTargetDistance;
    public float m_dampenFactorDistance;
    public float m_considerTargetSpeed;
    public float m_dampenFactorSpeed;


    [Header("--- (Trigger) ---")]
    [Header("- (Cooldown) -")]
    public float m_cooldown;
    public float m_cooldownRandomBonus;
    [Header("- (Distance) -")]
    public float m_maxDistance;
    public float m_minDistance;

    [Header("------- Debug -------")]
    public GameObject m_target;
    public Vector3 m_targetPoint;
    public Vector3 m_teleportPosition;
    public Vector3 m_teleportDirection;
    public float m_cooldownRdy;
    public bool m_isInitialized;
    public AttachSystemBase m_attachScript;

	// Use this for initialization
	void Start ()
    {
        if (!m_isInitialized)
            initializeStuff();
	}

    void initializeStuff()
    {
        setCooldown(1f);
        m_attachScript = GetComponent<AttachSystemBase>();

        m_isInitialized = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(m_useSkill)
            manageTeleport();
	}

    void manageTeleport()
    {
        m_target = GetComponent<CubeEntityTargetManager>().getTarget();

        if (checkTrigger())
            return;

        getTeleportPosition();
        activateTeleport();
    }

    bool checkTrigger()
    {
        bool t1 = m_cooldown > 0 && m_cooldownRdy > Time.time;

        m_targetPoint = transform.forward;
        if (m_target != null)
        {
            m_targetPoint = m_target.transform.position;
        }
        Vector3 direction = m_targetPoint - transform.position;
        bool t2 = m_maxDistance > 0 && Vector3.Distance(transform.position, m_targetPoint) < m_maxDistance;
        bool t3 = m_minDistance > 0 && Vector3.Distance(transform.position, m_targetPoint) > m_minDistance;

        return t1 || t2 ||t3;
    }

    void getTeleportPosition()
    {
        m_targetPoint = transform.position + transform.forward;
        if(m_target != null)
        {
            m_targetPoint = m_target.transform.position;
        }
        Vector3 direction = m_targetPoint - transform.position;


        float targetDistanceFactor = 1;
        if (m_considerTargetDistance > 0)
        {
            targetDistanceFactor = m_dampenFactorDistance + (1 - m_dampenFactorDistance) * Mathf.Clamp01(Vector3.Distance(transform.position, m_targetPoint) / m_considerTargetDistance);
        }


        int tries = 0;
        do {
            m_teleportPosition = transform.position + Random.insideUnitSphere * m_teleportRandomRadius * targetDistanceFactor;
            tries++;
        } while (Vector3.Dot(direction, m_teleportPosition - transform.position) < m_teleportDotValue && tries < 1000);
        if(tries > 900)
            Debug.Log("Warning: Something isn't right about this ...");

        float targetSpeedFactor = 1;
        if (m_considerTargetSpeed > 0)
        {
            targetSpeedFactor = m_dampenFactorSpeed + (1 - m_dampenFactorSpeed) * Mathf.Clamp01(Vector3.Distance(transform.position, m_targetPoint) / m_considerTargetSpeed);
        }
        setCooldown(targetSpeedFactor);

        m_teleportPosition += direction.normalized * m_teleportDistance * targetDistanceFactor;
        m_teleportDirection = m_teleportPosition - transform.position;

    }

    void activateTeleport()
    {
        List<GameObject> teleportObjects = new List<GameObject>();
        if(m_attachScript != null && m_teleportCubesAswell)
        {
            foreach (GameObject cube in m_attachScript.m_cubeList)
                teleportObjects.Add(cube);
        }

        if(!teleportObjects.Contains(gameObject))
            teleportObjects.Add(gameObject);

        foreach(GameObject cube in teleportObjects)
        {
            cube.transform.position += m_teleportDirection;
        }
    }

    // utility
    void setCooldown(float factor)
    {
        m_cooldownRdy = (m_cooldown + Random.Range(0, m_cooldownRandomBonus)) * factor + Time.time;
    }

    // abstract
    void setValues(MonsterEntitySkillTeleport baseScript)
    {
        m_useSkill = baseScript.m_useSkill;

        m_cooldown = baseScript.m_cooldown;
        m_cooldownRandomBonus = baseScript.m_cooldownRandomBonus;
        m_teleportCubesAswell = baseScript.m_teleportCubesAswell;

        m_teleportDistance = baseScript.m_teleportDistance;
        m_teleportRandomRadius = baseScript.m_teleportRandomRadius;
        m_teleportDotValue = baseScript.m_teleportDotValue;

        //m_useConsiderTargetDistance = baseScript.m_useConsiderTargetDistance;
        m_considerTargetDistance = baseScript.m_considerTargetDistance;
        m_dampenFactorDistance = baseScript.m_dampenFactorDistance;
        m_considerTargetSpeed = baseScript.m_considerTargetSpeed;
        m_dampenFactorSpeed = baseScript.m_dampenFactorSpeed;

        m_maxDistance = baseScript.m_maxDistance;
        m_minDistance = baseScript.m_minDistance;
    }

    // interfaces
    public void onCopy(ICopiable copiable)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((MonsterEntitySkillTeleport)copiable);
    }
    public void onPostCopy()
    {
        m_attachScript = GetComponent<AttachSystemBase>();

        setCooldown(1);
    }
    public void onRemove()
    {
        Destroy(this);
    }

    /*
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((MonsterEntitySkillTeleport)baseScript);
    }
    public override void prepareDestroyScript()
    {
        Destroy(this);
    }

    public override void onPostCopy()
    {
        m_attachScript = GetComponent<AttachSystemBase>();

        setCooldown(1);
    }
    */
}
