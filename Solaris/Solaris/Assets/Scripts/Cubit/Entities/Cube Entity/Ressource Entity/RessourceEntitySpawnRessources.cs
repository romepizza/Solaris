using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceEntitySpawnRessources : MonoBehaviour
{
    [Header("------- Settings -------")]
    public GameObject[] m_spawningPrefabs;

    [Header("--- (Spawn Over Distance) ---")]
    public float m_distanceForSpawn;

    [Header("--- (Spawn Position) ---")]
    public float m_spawnDistanceMin;
    public float m_spawnDistanceMax;
    public float m_minAngle;

    [Header("------- Debug -------")]


    public Vector3 m_lastPositon;
    public float m_currentDistance;

    public Rigidbody m_rb;


	// Use this for initialization
	void Start ()
    {
        initializeStuff();
	}

    void initializeStuff()
    {
        m_rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        updateMovement();
        checkSpawn();
	}

    void updateMovement()
    {
        Vector3 currentPosition = transform.position;
        m_currentDistance += Vector3.Distance(m_lastPositon, currentPosition);
        m_lastPositon = currentPosition;
    }

    void checkSpawn()
    {
        if (m_currentDistance >= m_distanceForSpawn)
        {
            spawnRessource();
            m_currentDistance = 0;
        }
    }

    void spawnRessource()
    {
        GameObject spawnPrefab = m_spawningPrefabs[Random.Range(0, m_spawningPrefabs.Length)];
        Vector3 spawnPosition = Vector3.zero;
        float spawnDistance = Random.Range(m_spawnDistanceMin, m_spawnDistanceMax);
        int tries = 0;
        do
        {
            spawnPosition = transform.position + Random.insideUnitSphere.normalized * spawnDistance;
            tries++;
        } while (tries < 25 && Utility.getAngle(m_rb.velocity.normalized, (spawnPosition - transform.position).normalized) > 75);
        if (tries > 20)
            Debug.Log("Caution: tries > 20");


        GameObject cube = Constants.getMainCge().activateCubeSafe(spawnPosition);
        if (cube.activeSelf == false)
            Debug.Log("Warning: cube was inactive!");

        cube.GetComponent<CubeEntityPrefapSystem>().setToPrefab(spawnPrefab);
    }
}
