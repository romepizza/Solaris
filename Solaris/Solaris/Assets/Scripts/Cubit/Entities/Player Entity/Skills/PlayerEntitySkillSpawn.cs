using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntitySkillSpawn : MonoBehaviour
{
    public GameObject m_player;
    public GameObject m_prefab;
    public GameObject m_cubeChildObject;

    [Header("----- SETTINGS -----")]
    public int m_poolSize;
    public Vector3 m_inactivePosition;
    public int m_inactiveCheckPerFrame;

    [Header("--- Detection ---")]
    public float m_flightDistanceForCube;

    [Header("--- Form ---")]
    public float m_totalRadius;
    public float m_cellRadius;

    [Header("----- DEBUG -----")]
    public int m_checkedTotal;
    public List<GameObject> m_activeCubes;
    public Queue<GameObject> m_inactiveCubes;
    public float m_playerFlightDistance;
    public Vector3 m_lastPosition;

    void Start()
    {
        //m_player = Constants.getPlayer();
        initializeInactiveCubes();
        if (m_cubeChildObject == null)
        {
            Debug.Log("Warning: No child object for cubes set!");
        }

        m_lastPosition = m_player.transform.position;
    }

    void FixedUpdate()
    {
        m_playerFlightDistance += Vector3.Distance(m_player.transform.position, m_lastPosition);
        m_lastPosition = m_player.transform.position;

        checkForInactive();
        checkForActive();

    }

    void initializeInactiveCubes()
    {
        m_inactiveCubes = new Queue<GameObject>();
        m_activeCubes = new List<GameObject>();

        for (int i = 0; i < m_poolSize; i++)
        {
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, 1));
            GameObject cube = Instantiate(m_prefab, m_cubeChildObject.transform);
            cube.GetComponent<CubeEntitySystem>().addComponentsAtStart();
            setCubeToInactive(cube);
        }
    }

    void checkForInactive()
    {
        int checkedThisRound = 0;
        while (checkedThisRound < m_inactiveCheckPerFrame)
        {
            Debug.Log("total: " + m_checkedTotal);
            int index = m_activeCubes.Count - m_checkedTotal - 1;

            Debug.Log("index b: " + index);

            if (index < 0)
                index += m_activeCubes.Count;
            if (index < 0)
                index = 0;

            Debug.Log("index a: " + index);
            Debug.Log("count: " + m_activeCubes.Count);
            if (m_activeCubes.Count > 0)
            {
                GameObject cube = m_activeCubes[index];

                if (cube != null)
                {
                    if (Vector3.Distance(m_player.transform.position, cube.transform.position) > 100)
                    {
                        setCubeToInactive(cube);
                    }
                }
                else
                    Debug.Log("Warning: Cube was null");
            }
            checkedThisRound++;
            m_checkedTotal++;
            if (m_checkedTotal > m_activeCubes.Count)
                m_checkedTotal = 0;
        }
    }

    void checkForActive()
    {
        if (m_playerFlightDistance > m_flightDistanceForCube)
        {
            m_playerFlightDistance = 0;
            setCubeToActive(m_player.transform.position + m_player.transform.rotation * new Vector3(0, 5, 20));

            m_playerFlightDistance = 0;
        }
    }

    public void setCubeToInactive(GameObject cube)
    {
        //cube.GetComponent<CubeEntitySystem>().setToInactive();
        cube.SetActive(false);

        m_activeCubes.Remove(cube);
        m_inactiveCubes.Enqueue(cube);
    }

    public void setCubeToActive(Vector3 targetPosition)
    {
        GameObject cube = m_inactiveCubes.Dequeue();

        cube.GetComponent<CubeEntitySystem>().GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
        cube.transform.position = targetPosition;
        cube.transform.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere, ForceMode.Acceleration);
        cube.transform.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere;

        cube.SetActive(true);

        m_activeCubes.Add(cube);
    }

}
