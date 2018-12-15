using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelEntityCubeCluster : MonoBehaviour
{
    [Header("---------- PARENT ----------")]
    [Header("----- INFOS -----")]
    public int m_cubeAmount;
    [Header("----- PREFABS -----")]
    public GameObject m_spawnPrefab;
    [Header("----- SETTINGS -----")]
    public string m_cubesName = "Cubes";
    public Vector3 m_cellSize = new Vector3(15, 15, 15);
    public float m_checkForActivisionTime = 1;
    public int m_createCubesPerFrame = 100;

    [Header("----- DEBUG -----")]
    public GameObject[] m_cubes;
    public GameObject m_startPoint;
    public GameObject m_endPoint;
    public GameObject m_childObjectOfCubes;
    public Vector3 m_cubePadding;
    public LevelEntityArea m_areaScript;
    public float m_checkForActivisionFinishTime;
    public bool m_isActive;
    public bool m_isCreatingCubes;

    [Header("----- GIZMOS -----")]
    public bool m_showGizmos;

    void Start()
    {
        setAreaManager();
        registerClusterToParentArea();

        //m_checkForActivisionFinishTime = m_checkForActivisionTime * Random.Range(1f, 1.1f) + Time.time;
    }

    void Update()
    {
        manageTimer();
        createCubesOverTime();
    }

    public void setAreaManager()
    {
        GameObject parent = gameObject.transform.parent.gameObject;
        if (parent == null)
        {
            Debug.Log("Warning: LevelEntityCreateCubeArea has no parent! (" + gameObject.name + ")");
            return;
        }
        if (parent.GetComponent<LevelEntityArea>() != null)
        {
            m_areaScript = parent.GetComponent<LevelEntityArea>();
        }
        else
            Debug.Log("Warning: Parent of LevelEntityCreateArea's parent is not a LevelEntityAreaManager!");

        
    }

    protected void createChildObject()
    {
        if (m_childObjectOfCubes == null)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                if (gameObject.transform.GetChild(i).name == m_cubesName)
                {
                    m_childObjectOfCubes = gameObject.transform.GetChild(i).gameObject;
                    return;
                }
            }

            GameObject cubes = new GameObject(m_cubesName);
            cubes.transform.SetParent(this.gameObject.transform);
            m_childObjectOfCubes = cubes;
        }
    }

    public abstract void createCubes();
    public abstract void createCubesOverTime();

    // Management
    void registerClusterToParentArea()
    {
        m_areaScript.registerCluster(this);
    }
    public void registerCubesToParentArea()
    {
        foreach(GameObject cube in m_cubes)
        {
            m_areaScript.registerActiveCube(cube, this);
        }
    }
    public void activateCluster()
    {
        createCubes();
        m_areaScript.activateCluster(this);
        m_isActive = true;
    }
    void checkForActivisionTimer()
    {
        if (!m_isActive && m_checkForActivisionFinishTime < Time.time)
        {
            checkForActivision();
            m_checkForActivisionFinishTime = m_checkForActivisionTime * Random.Range(1f, 1.1f) + Time.time;
        }
    }
    void checkForActivision()
    {
        if (true)
            activateCluster();
    }

    // Timer
    void manageTimer()
    {
        checkForActivisionTimer();
    }
}
