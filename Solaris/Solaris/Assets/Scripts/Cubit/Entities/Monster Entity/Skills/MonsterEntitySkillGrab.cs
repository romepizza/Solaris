using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntitySkillGrab : EntityCopiableAbstract, ICopiable, IPostCopy, IRemoveOnStateChange
{
    [Header("----- SETTINGS -----")]
    [Header("--- (Skill) ---")]
    public bool m_grabNearestCube = true;
    public float m_grabCooldown;
    public float m_grabRadius;
    public float m_grabRadiusIncrease;

    [Header("--- (Selection) ---")]
    public bool m_createCubesIfNoneFound;
    public bool m_createCubesOnly;

    [Header("----- DEBUG -----")]

    [Header("--- (Timer) ---")]
    public float m_grabFinishTime;

    public MonsterEntityAttachSystem m_attachSystemScript;
    public MonsterEntityAttachSystemNew m_attachSystemScriptNew;
    public MonsterEntityBase m_baseScript;

    void Start()
    {
        m_attachSystemScript = gameObject.GetComponent<MonsterEntityAttachSystem>();
        

        m_attachSystemScriptNew = gameObject.GetComponent<MonsterEntityAttachSystemNew>();
        if (m_attachSystemScriptNew == null)
            Debug.Log("Warning: Attach system new not found!");
    }

    // Updates
    void Update()
    {
        manageSkill();
    }

    

    // Skill specific
    void manageSkill()
    {
        if (m_grabFinishTime < Time.time && m_attachSystemScriptNew.m_cubeList.Count < m_attachSystemScriptNew.m_currentMaxCubesGrabbed)
        {
            activateSkill();
        }
    }
    void activateSkill()
    {
        // chose cube to add to attached
        GameObject cubeAdd = null;

        if (false)
        {
            if (m_grabRadiusIncrease <= 0)
            {
                Debug.Log("Aborted: m_grabRadiusIncrease was less than zero!");
                return;
            }
            for (float sphereRadius = m_grabRadiusIncrease; sphereRadius <= m_grabRadius; sphereRadius += m_grabRadiusIncrease)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius);
                for (int col = 0; col < colliders.Length; col++)
                {
                    GameObject potentialCube = colliders[col].gameObject;
                    if (potentialCube.GetComponent<CubeEntitySystem>() != null && potentialCube.GetComponent<CubeEntitySystem>().getStateComponent() != null && potentialCube.GetComponent<CubeEntitySystem>().getStateComponent().canBeAttachedToEnemy())
                    {
                        cubeAdd = potentialCube;
                        break;
                    }
                }
                if (cubeAdd != null)
                    break;
            }
        }
        else if (!m_createCubesOnly)
        {
            // chose cube to add to attached
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_grabRadius);

            float nearestDist = float.MaxValue;

            for (int i = 0; i < colliders.Length; i++)
            {
                GameObject cubePotential = colliders[i].gameObject;
                if (cubePotential.GetComponent<CubeEntitySystem>() != null)
                {
                    if (cubePotential.GetComponent<CubeEntitySystem>().getStateComponent() != null && cubePotential.GetComponent<CubeEntitySystem>().getStateComponent().canBeAttachedToEnemy())
                    {
                        if (m_grabNearestCube)
                        {
                            float dist = Vector3.Distance(transform.position, cubePotential.transform.position);
                            if (dist < nearestDist)
                            {
                                nearestDist = dist;
                                cubeAdd = cubePotential;
                            }
                        }
                        else
                        {
                            cubeAdd = cubePotential;
                            break;
                        }
                    }
                }
            }
        }

        if(m_createCubesOnly || (m_createCubesIfNoneFound && cubeAdd == null))
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere.normalized * Random.Range(10f, 20f);
            cubeAdd = Constants.getMainCge().activateCubeSafe(spawnPosition);
        }

        // add cube to attached
        if (cubeAdd != null)
        {
            if (cubeAdd.GetComponent<CubeEntityState>().m_state == CubeEntityState.s_STATE_ATTACHED)
                Debug.Log("Caution!");

            if (m_attachSystemScriptNew != null)
                m_attachSystemScriptNew.registerToGrab(cubeAdd);
        }

        m_grabFinishTime = m_grabCooldown + Time.time;
    }


    // Setter
    /*
    public void setValuesByScript(GameObject prefab, MonsterEntityBase baseScript)
    {
        MonsterEntitySkillGrab script = prefab.GetComponent<MonsterEntitySkillGrab>();
        if (script != null)
        {
            m_grabNearestCube = script.m_grabNearestCube;
            m_grabCooldown = script.m_grabCooldown;
            m_grabRadius = script.m_grabRadius;
            m_grabRadiusIncrease = script.m_grabRadiusIncrease;
            m_createCubesIfNoneFound = script.m_createCubesIfNoneFound;
            m_baseScript = baseScript;
        }
        else
            Debug.Log("Warning: Tried to copy values of MonsterEntitySkillGrab from prefab, that didn't have the script attached!");
    }
    */
    public void setValuesPlain(MonsterEntitySkillGrab script)
    {
        if (script != null)
        {
            m_grabNearestCube = script.m_grabNearestCube;
            m_grabCooldown = script.m_grabCooldown;
            m_grabRadius = script.m_grabRadius;
            m_grabRadiusIncrease = script.m_grabRadiusIncrease;
            m_createCubesIfNoneFound = script.m_createCubesIfNoneFound;
            m_createCubesOnly = script.m_createCubesOnly;
        }
        else
            Debug.Log("Warning: Tried to copy values of MonsterEntitySkillGrab from prefab, that didn't have the script attached!");
    }

    public void onCopy(ICopiable copiable)
    {
        setValuesPlain((MonsterEntitySkillGrab)copiable);
    }
    public void onPostCopy()
    {
        m_attachSystemScript = gameObject.GetComponent<MonsterEntityAttachSystem>();

        m_attachSystemScriptNew = gameObject.GetComponent<MonsterEntityAttachSystemNew>();
        if (m_attachSystemScriptNew == null)
            Debug.Log("Warning: Attach system new not found!");

        m_baseScript = GetComponent<MonsterEntityBase>();
        if (m_baseScript == null)
            Debug.Log("Warning: m_attachSystemScript was null");
    }
    public void onRemove()
    {
        Destroy(this);
    }

    /*

    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        setValuesPlain((MonsterEntitySkillGrab)baseScript);
    }
    
    public override void prepareDestroyScript()
    {
        Destroy(this);
    }

    public override void onPostCopy()
    {
        m_attachSystemScript = gameObject.GetComponent<MonsterEntityAttachSystem>();

        m_attachSystemScriptNew = gameObject.GetComponent<MonsterEntityAttachSystemNew>();
        if (m_attachSystemScriptNew == null)
            Debug.Log("Warning: Attach system new not found!");

        m_baseScript = GetComponent<MonsterEntityBase>();
        if (m_baseScript == null)
            Debug.Log("Warning: m_attachSystemScript was null");
    }
    */
}
