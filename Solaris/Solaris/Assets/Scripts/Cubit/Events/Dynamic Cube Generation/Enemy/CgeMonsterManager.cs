using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CgeMonsterManager : MonoBehaviour
{
    [Header("----- Settings -----")]
    public bool m_spawnNewCube;
    [Space]
    public float m_spawnDistanceMin;
    public float m_spawnDistanceMax;

    [Header("----- DEBUG -----")]
    public bool m_toggle;

    public List<GameObject> m_ejectorsAlive;
    public List<GameObject> m_wormsAlive;
    public List<GameObject> m_morphersAlive;
    public List<GameObject> m_monstersAlive;
    public List<GameObject> m_swarmsAlive;

    private CGE m_cge;

    void Start()
    {
        //m_areaScript = gameObject.GetComponent<LevelEntityArea>();
        m_cge = GetComponent<CGE>();
    }

    void Update()
    {
        if (Input.GetButton("ButtonX"))
        {
            if (!m_toggle)
            {
                createEjector();
                m_toggle = true;
            }
        }
        else
        {
            m_toggle = false;
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            createEjector();
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            createWorm();
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            createMorpher();
        }

        if (Input.GetKeyUp(KeyCode.F1))
        {
            createSwarm();
        }
    }

    void createEjector()
    {
        GameObject core = null;

        if (m_spawnNewCube)
        {
            core = Constants.getMainCge().activateCubeSafe(getSpawnPosition());
        }
        else
        {
            List<GameObject> potentialCubes = new List<GameObject>();
            foreach (GameObject cube in m_cge.m_activeCubes)
            {
                if (cube.GetComponent<CubeEntityState>().canBeCoreGeneral() && cube.GetComponent<MonsterEntityBase>() == null)
                {
                    potentialCubes.Add(cube);
                }
            }

            if (potentialCubes.Count > 0)
            {
                int randomIndex = Random.Range(0, potentialCubes.Count);
                core = potentialCubes[randomIndex];
            }
            else
                Debug.Log("Warning: Tried to create Ejector, but no fitting cube was found!");
        }
        if (core != null)
        {
            core.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_coreEnemyEjector);
            m_ejectorsAlive.Add(core);
            m_monstersAlive.Add(core);
            //core.GetComponent<MonsterEntityBase>().m_registeredInManager.Add(this);
        }
    }
    void createWorm()
    {
        GameObject core = null;

        if (m_spawnNewCube)
        {
            core = Constants.getMainCge().activateCubeSafe(getSpawnPosition());
        }
        else
        { 
            List<GameObject> potentialCubes = new List<GameObject>();

            foreach (GameObject cube in m_cge.m_activeCubes)
            {
                if (cube.GetComponent<CubeEntityState>().canBeCoreGeneral() && cube.GetComponent<MonsterEntityBase>() == null)
                {
                    potentialCubes.Add(cube);
                }
            }

            if (potentialCubes.Count > 0)
            {
                int randomIndex = Random.Range(0, potentialCubes.Count);
                core = potentialCubes[randomIndex];
            }
            else
                Debug.Log("Warning: Tried to create Worm, but no fitting cube was found!");
        }
        if (core != null)
        {
            core.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_coreEnemyWorm);
            m_wormsAlive.Add(core);
            m_monstersAlive.Add(core);
            //core.GetComponent<MonsterEntityBase>().m_registeredInManager.Add(this);
        }
    }
    void createMorpher()
    {
        GameObject core = null;

        if (m_spawnNewCube)
        {
            core = Constants.getMainCge().activateCubeSafe(getSpawnPosition());
        }
        else
        {
            List<GameObject> potentialCubes = new List<GameObject>();

            foreach (GameObject cube in m_cge.m_activeCubes)
            {
                if (cube.GetComponent<CubeEntityState>().canBeCoreGeneral() && cube.GetComponent<MonsterEntityBase>() == null)
                {
                    potentialCubes.Add(cube);
                }
            }

            if (potentialCubes.Count > 0)
            {
                int randomIndex = Random.Range(0, potentialCubes.Count);
                core = potentialCubes[randomIndex];
            }
            else
                Debug.Log("Warning: Tried to create Worm, but no fitting cube was found!");
        }
        if (core != null)
        {
            core.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_coreEnemyMorpher);
            m_morphersAlive.Add(core);
            m_monstersAlive.Add(core);
            //core.GetComponent<MonsterEntityBase>().m_registeredInManager.Add(this);
        }
    }
    void createSwarm()
    {
        GameObject core = null;

        if (m_spawnNewCube)
        {
            core = Constants.getMainCge().activateCubeSafe(getSpawnPosition());
        }
        else
        {
            List<GameObject> potentialCubes = new List<GameObject>();

            foreach (GameObject cube in m_cge.m_activeCubes)
            {
                if (cube.GetComponent<CubeEntityState>().canBeCoreGeneral() && cube.GetComponent<MonsterEntityBase>() == null)
                {
                    potentialCubes.Add(cube);
                }
            }

            if (potentialCubes.Count > 0)
            {
                int randomIndex = Random.Range(0, potentialCubes.Count);
                core = potentialCubes[randomIndex];
            }
            else
                Debug.Log("Warning: Tried to create Worm, but no fitting cube was found!");
        }
        if (core != null)
        {
            core.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_coreEnemySwarm);
            m_swarmsAlive.Add(core);
            m_monstersAlive.Add(core);
            //core.GetComponent<MonsterEntityBase>().m_registeredInManager.Add(this);
        }
    }


    Vector3 getSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        //spawnPosition = Constants.getPlayer().transform.position + Random.insideUnitSphere.normalized * Random.Range(250, 500);
        spawnPosition = Constants.getPlayer().transform.position + Camera.main.transform.forward * Random.Range(m_spawnDistanceMin, m_spawnDistanceMax);

        return spawnPosition;
    }
    public void deregisterEnemy(MonsterEntityAbstractBase enemyScript)
    {
        if (enemyScript.GetType() == typeof(MonsterEntityEjector))
        {
            m_ejectorsAlive.Remove(enemyScript.gameObject);
        }

        if(enemyScript.GetType() == typeof(MonsterEntityWorm))
        {
            m_wormsAlive.Remove(enemyScript.gameObject);
        }

        if(enemyScript.GetType() == typeof(MonsterEntityMorpher))
        {
            m_morphersAlive.Remove(enemyScript.gameObject);
        }

        if(enemyScript.GetType() == typeof(MonsterEntitySwarm))
        {
            m_swarmsAlive.Remove(enemyScript.gameObject);
        }

        m_monstersAlive.Remove(enemyScript.gameObject);
    }
}
