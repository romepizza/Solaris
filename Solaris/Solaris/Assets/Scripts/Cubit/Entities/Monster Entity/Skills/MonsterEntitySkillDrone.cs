using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntitySkillDrone : EntityCopiableAbstract, ICopiable, IRemoveOnStateChange
{
    public bool m_allowInput;
    [Header("----- SETTINGS -----")]
    public int m_maxDrones;

    [Header("--- (Activision) ---")]
    [Header("- Cooldown -")]
    public float m_cooldown;

    [Header("--- (Cubes) ---")]

    [Header("----- DEBUG -----")]
    public bool m_isInitialized;
    public float m_cooldownRdy;
    public GameObject m_chosenCube;
    public List<GameObject> m_drones;

    // Use this for initialization
    void Start ()
    {
        if (!m_isInitialized)
            initializeStuff();

    }

    void initializeStuff()
    {
        m_drones = new List<GameObject>();

        m_isInitialized = true;
    }


    // Update is called once per frame
    void Update()
    {
        getInput();
    }

    void activateSkill()
    {
        m_chosenCube = Constants.getMainCge().activateCubeSafe(transform.position + Random.insideUnitSphere.normalized * 50f);
        if (m_chosenCube == null)
        {
            Debug.Log("Warning: Too less cubes in cge!");
            return;
        }
        else
        {
            if (m_drones.Count >= m_maxDrones)
                destroyDrone(m_drones[Random.Range(0, m_drones.Count)]);

            createDrone();
        }

        changeTarget(PlayerEntityAim.aim());

        m_cooldownRdy = m_cooldown + Time.time;
    }

    void createDrone()
    {
        m_chosenCube.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_coreDronePrefab);
        m_drones.Add(m_chosenCube);
        m_chosenCube.GetComponent<CubeEntityTargetManager>().setTarget(PlayerEntityAim.aim().GetComponent<CubeEntityTargetManager>());
    }

    public void destroyDrone(GameObject drone)
    {
        if (drone == null)
        {
            Debug.Log("Aborted: drone to remove was null!");
            return;
        }

        if (m_drones.Contains(drone))
        {
            //drone.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
            drone.GetComponent<MonsterEntityBase>().die();
            removeDrone(drone);
        }
        else
            Debug.Log("Warning: drone to remove was not in the list!");
    }

    public void removeDrone(GameObject drone)
    {
        if(drone == null)
        {
            Debug.Log("Aborted: drone to remove was null!");
            return;
        }

        if (m_drones.Contains(drone))
        {
            m_drones.Remove(drone);
        }
        else
            Debug.Log("Warning: drone to remove was not in the list!");
    }

    // intern
    void getInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && m_cooldownRdy < Time.time)
        {
            activateSkill();
        }
    }
    void changeTarget(GameObject target)
    {
        foreach(GameObject drone in m_drones)
        {
            drone.GetComponent<CubeEntityTargetManager>().setTarget(target);
        }
    }
    // copy
    void setValuesPlain(MonsterEntitySkillDrone baseScript)
    {
        m_allowInput = baseScript.m_allowInput;
        m_cooldown = baseScript.m_cooldown;
    }

    // interfaces
    public void onCopy(ICopiable copiable)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValuesPlain((MonsterEntitySkillDrone)copiable);
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
        setValuesPlain((MonsterEntitySkillDrone)baseScript);
    }
    public override void prepareDestroyScript()
    {
        Destroy(this);
    }
    public override void onPostCopy()
    {
        
    }
    */
}
