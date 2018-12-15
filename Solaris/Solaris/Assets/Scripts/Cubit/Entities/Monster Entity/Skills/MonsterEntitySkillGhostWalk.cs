using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntitySkillGhostWalk : EntityCopiableAbstract
{
    public bool m_useSkill = true;

    [Header("------- Settings -------")]
    public bool m_teleportCubesAswell;

    [Header("--- (Teleport) ---")]
    public float m_teleportDistance;
    public float m_teleportRandomRadius;
    public float m_teleportDotValue;
    public bool m_useConsiderTargetDistance;
    public float m_considerTargetDistance;

    [Header("--- (Trigger) ---")]
    [Header("- (Cooldown) -")]
    public float m_cooldown;
    public float m_cooldownRandomBonus;
    [Header("- (Distance) -")]
    public float m_maxDistance;

    [Header("------- Debug -------")]
    public Vector3 m_teleportPosition;
    public Vector3 m_teleportDirection;
    public float m_cooldownRdy;
    public bool m_isInitialized;
    public AttachSystemBase m_attachScript;

    // Use this for initialization
    void Start()
    {
        if (!m_isInitialized)
            initializeStuff();
    }

    void initializeStuff()
    {
        setCooldown();
        m_attachScript = GetComponent<AttachSystemBase>();

        m_isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_useSkill)
            manageTeleport();
    }

    void manageTeleport()
    {
        if (checkTrigger())
            return;

        getTeleportPosition();
        activateTeleport();

        setCooldown();
    }

    bool checkTrigger()
    {
        bool t1 = m_cooldown > 0 && m_cooldownRdy > Time.time;
        bool t2 = m_maxDistance > 0 && Vector3.Distance(transform.position, Constants.getPlayer().transform.position) < m_maxDistance;

        return t1 || t2;
    }

    void getTeleportPosition()
    {
        Vector3 direction = Constants.getPlayer().transform.position - transform.position;

        int tries = 0;
        do
        {
            m_teleportPosition = transform.position + Random.insideUnitSphere * m_teleportRandomRadius;
            tries++;
        } while (Vector3.Dot(direction, m_teleportPosition - transform.position) < m_teleportDotValue && tries < 1000);

        float m_targetDistanceFactor = 1;
        if(m_considerTargetDistance >= 0)
            m_targetDistanceFactor = Vector3.Distance(transform.position, Constants.getPlayer().transform.position) / m_considerTargetDistance;

        m_teleportPosition += direction.normalized * m_teleportDistance;
        m_teleportDirection = m_teleportPosition - transform.position;
    }

    void activateTeleport()
    {
        List<GameObject> teleportObjects = new List<GameObject>();
        if (m_attachScript != null && m_teleportCubesAswell)
        {
            foreach (GameObject cube in m_attachScript.m_cubeList)
                teleportObjects.Add(cube);
        }

        if (!teleportObjects.Contains(gameObject))
            teleportObjects.Add(gameObject);

        foreach (GameObject cube in teleportObjects)
        {
            cube.transform.position += m_teleportDirection;
        }
    }

    // utility
    void setCooldown()
    {
        m_cooldownRdy = m_cooldown + Random.Range(0, m_cooldownRandomBonus) + Time.time;
    }

    // abstract
    void setValues(MonsterEntitySkillGhostWalk baseScript)
    {
        m_useSkill = baseScript.m_useSkill;

        m_cooldown = baseScript.m_cooldown;
        m_cooldownRandomBonus = baseScript.m_cooldownRandomBonus;
        m_teleportCubesAswell = baseScript.m_teleportCubesAswell;

        m_teleportDistance = baseScript.m_teleportDistance;
        m_teleportRandomRadius = baseScript.m_teleportRandomRadius;
        m_teleportDotValue = baseScript.m_teleportDotValue;

        m_useConsiderTargetDistance = baseScript.m_useConsiderTargetDistance;
        m_considerTargetDistance = baseScript.m_considerTargetDistance;

        m_maxDistance = baseScript.m_maxDistance;
    }

    /*
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((MonsterEntitySkillGhostWalk)baseScript);
    }

    public override void prepareDestroyScript()
    {
        Destroy(this);
    }

    public override void onPostCopy()
    {
        m_attachScript = GetComponent<AttachSystemBase>();
        setCooldown();
    }
    */
}
