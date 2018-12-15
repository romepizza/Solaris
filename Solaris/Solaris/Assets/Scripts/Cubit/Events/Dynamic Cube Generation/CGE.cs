using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGE : MonoBehaviour
{
    public GameObject m_player;
    public GameObject m_prefab;
    public GameObject m_cubeChildObject;
    public CgeTidyUp m_tidyUp;

    [Header("------- Settings -------")]
    public float m_deactivateDistance;
    public int m_deactivisionsPerFrame;
    public int m_poolSize;
    public Vector3 m_inactivePosition;

    [Header("--- (Activate/Deactivate) ---")]
    public bool m_enableDisableCubes;
    [Header("- activision -")]
    public bool m_stopMovementOnActivision;
    public bool m_addRandomMovementOnActivision;
    [Header("- deativision -")]
    public bool m_stopMovementOnDeactivision;
    //public bool m_setToDefaultPositionOnDeactivision;

    [Header("------- Debug -------")]
    public Queue<GameObject> m_inactiveCubes;
    public List<GameObject> m_activeCubes;
    public List<List<GameObject>> m_allCubes;
    public int m_checkedTotal;
    public bool m_spawnCubes;
    public int m_currentNumber;

   // public List<GameObject> m_activeCubes;
    public CGE m_cge;

    //public List<CgeInstance> m_instances;

    void Start()
    {
        if(m_cubeChildObject == null)
        {
            Debug.Log("Warning: No child object for cubes set!");
        }
        initializeInactiveCubes();
    }

    void FixedUpdate()
    {
        checkForDeactivate();
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
            cube.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
            //m_tidyUp.deactivateCube(cube);
            cube.transform.position = m_inactivePosition + Vector3.right * i * 4f;
            deactivateCube(cube);
            cube.name = m_currentNumber + ": " + cube.name;
            m_currentNumber++;
        }
    }

    // Activate // TODO : This is shit!
    public GameObject activateCubeUnsafe(Vector3 targetPosition)
    {
        if (!m_spawnCubes)
            return null;

        if (m_inactiveCubes.Count > 0)
        {
            /*
            GameObject cube = null;
            do
            {
                cube = m_inactiveCubes.Dequeue();
                if(cube.GetComponent<CubeEntityState>().isInactive())
                {
                    m_activeCubes.Add(cube);
                    break;
                }

            } while (m_inactiveCubes.Count > 0);
            */
            GameObject cube = m_inactiveCubes.Dequeue();
            if (!cube.GetComponent<CubeEntityState>().isInactive())
            {
                m_activeCubes.Add(cube);
                if (m_inactiveCubes.Count > 0)
                {
                    cube = m_inactiveCubes.Dequeue();
                    if (!cube.GetComponent<CubeEntityState>().isInactive())
                        cube = null;
                }
            }

            if (cube != null)
            {
                if (m_enableDisableCubes)
                    cube.SetActive(true);

                if (m_stopMovementOnActivision)
                {
                    cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    cube.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }

                cube.transform.position = targetPosition;

                //cube.SetActive(true);
                //cube.transform.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 100f, ForceMode.Acceleration);
                //cube.transform.GetComponent<Rigidbody>().velocity = m_player.GetComponent<Rigidbody>().velocity * 0.5f;
                //cube.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;

                m_activeCubes.Add(cube);
            }

            return cube;
        }
        return null;
    }
    public GameObject activateCubeSafe(Vector3 targetPosition)
    {
        if (!m_spawnCubes)
            return null;

        if (m_inactiveCubes.Count > 0)
        {

            GameObject cube = null;
            int tries = 0;
            do
            {
                cube = m_inactiveCubes.Dequeue();
                if (!cube.GetComponent<CubeEntityState>().isInactive())
                {
                    m_activeCubes.Add(cube);
                    cube = null;
                }
                tries++;
            } while (cube == null && tries < 100);
            if(tries > 10)
            {
                Debug.Log("Warning: no inactive cubes found!");
            }


            if (cube != null)
            {

                if(m_enableDisableCubes)
                    cube.SetActive(true);

                if (m_stopMovementOnActivision)
                {
                    cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    cube.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }

                if(m_addRandomMovementOnActivision)
                {
                    cube.transform.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 100f, ForceMode.Acceleration);
                    cube.transform.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere * 5f;
                }

                cube.transform.position = targetPosition;


                m_activeCubes.Add(cube);
            }
            else
            {
                Debug.Log("Caution");
            }

            return cube;
        }
        return null;
    }

    public void registerActiveCube(GameObject cube)
    {
        //if(!m_activeCubes.Contains(cube))
            m_activeCubes.Add(cube);
        //m_inactiveCubes.Remove(cube);
        //m_inactiveCubes.Dequeue();
    }

    // deactivate
    void checkForDeactivate()
    {
        int checkedThisFrame = 0;
        //int checks = (int)Mathf.Min(m_movesAndScansPerFrame, Mathf.Max(m_cellQueue.Count * 0.1f, 5))
        while (checkedThisFrame < m_deactivisionsPerFrame)
        {
            int index = m_activeCubes.Count - m_checkedTotal - 1;

            if (index < 0)
                index += m_activeCubes.Count;
            if (index < 0)
                index = 0;

            if (m_activeCubes.Count > 0)
            {
                GameObject cube = m_activeCubes[index];

                if (cube != null)
                {
                    bool constraint_2 = cube.GetComponent<CgeDoNotSetToInactive>() == null;
                    bool constraint_0 = Vector3.Distance(m_player.transform.position, cube.transform.position) > m_deactivateDistance;
                    bool constraint_1 = cube.GetComponent<CubeEntitySystem>().getStateComponent().isInactive();

                    if (constraint_0 && constraint_1 && constraint_2)
                    {
                        deactivateCube(cube);
                    }
                }
                else
                    Debug.Log("Warning: Cube was null");
            }

            checkedThisFrame++;
            m_checkedTotal++;
            if (m_checkedTotal > m_activeCubes.Count)
                m_checkedTotal = 0;
        }
    }
    public void deactivateCube(GameObject cube)
    {
        if (m_enableDisableCubes)
            cube.SetActive(false);

        if (m_stopMovementOnDeactivision)
        {
            cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
            cube.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        //if (m_setToDefaultPositionOnDeactivision)
        //{
            //Vector3 position = m_inactivePosition + Vector3.right * m_inactivePositionIndex * 4f;
            //m_inactivePositionIndex++;
            //cube.transform.position = position;
        //}

        //cube.GetComponent<CubeEntitySystem>().setToInactive();

        m_activeCubes.Remove(cube);
        m_inactiveCubes.Enqueue(cube);
    }
}
