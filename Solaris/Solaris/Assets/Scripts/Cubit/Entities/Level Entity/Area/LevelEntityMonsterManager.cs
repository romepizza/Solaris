using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntityMonsterManager : MonoBehaviour
{
    [Header("----- Settings -----")]

    [Header("----- DEBUG -----")]

    public LevelEntityArea m_areaScript;
    public List<GameObject> m_ejectorsAlive;
    public List<GameObject> m_wormsAlive;

    void Start()
    {
        //m_areaScript = gameObject.GetComponent<LevelEntityArea>();
        if (m_areaScript == null)
            Debug.Log("Warning: no LevelEntityArea found!");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            //createEjector();
        }
    }

    void createEjector()
    {
        List<GameObject> potentialCubes = new List<GameObject>();
        for (int i = 0; i < m_areaScript.m_cubesInArea.Count; i++)
        {
            foreach (GameObject cube in m_areaScript.m_cubesInArea[i])
            {
                if (cube.GetComponent<CubeEntityState>().canBeCoreGeneral())
                {
                    potentialCubes.Add(cube);
                }
            }
        }

        GameObject core = null;
        if (potentialCubes.Count > 0)
        {
            int randomIndex = Random.Range(0, potentialCubes.Count);
            core = potentialCubes[randomIndex];
        }
        else
            Debug.Log("Warning: Tried to create Ejector, but no fitting cube was found!");

        if(core != null)
        {
            Debug.Log("Warning: This line of code should not have been reached!");
            return;
            //core.GetComponent<CubeEntitySystem>().setToCoreEjector();
            m_ejectorsAlive.Add(core);
            core.GetComponent<MonsterEntityBase>().m_registeredInManager.Add(this);
        }
    }

    public void deregisterEnemy(GameObject enemyScript)
    {
        if(enemyScript.GetType() == typeof(MonsterEntityEjector))
        {
            m_ejectorsAlive.Remove(enemyScript);
        }
    }
}
