using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CgeForm : MonoBehaviour
{
    [Header("----- SETTINGS -----")]
    public int m_activisionsPerFrame;
    public GameObject m_spawnObject;

    [Header("----- Debug Base -----")]
    public GameObject m_player;
    public List<CgeCell> m_cells;

    public float m_playerFlightDistance;
    public Vector3 m_lastPlayerPosition;
    public Vector3 m_vectorToLastPosition;
    public float m_flightDistanceForCube;
    public Vector3 m_playerMovementVector;

    public Queue<CgeCell> m_cellsAboutToBeScanned;
    public Queue<Vector3> m_cellChangePositions;

    public CGE m_cge;

    public virtual void Start()
    {
        m_cells = new List<CgeCell>();
        m_cge = Constants.getMainCge();//GetComponent<CGE>();
        m_lastPlayerPosition = m_player.transform.position;
        m_playerMovementVector = Vector3.zero;
        m_cellsAboutToBeScanned = new Queue<CgeCell>();
        m_cellChangePositions = new Queue<Vector3>();
    }


    public virtual void FixedUpdate()
    {
        updatePlayerMovement();
        updateCubeActivision();
    }

    void updatePlayerMovement()
    {
        m_vectorToLastPosition = m_player.transform.position - m_lastPlayerPosition;
        //m_playerFlightDistance += Vector3.Distance(m_player.transform.position, m_lastPlayerPosition);
        m_playerMovementVector += m_vectorToLastPosition;
        m_lastPlayerPosition = m_player.transform.position;
    }

    public void updateCubeActivision()
    {
        for (int cubesActivated = 0; cubesActivated < m_activisionsPerFrame; cubesActivated++)
        {
            if (m_cellChangePositions.Count > 0)
            {
                Vector3 sd = m_cellChangePositions.Dequeue();
                activateCubeInsideCell(sd);
            }
            else
                break;
        }
    }

    public void addCubeToActivisionQueue(CgeCell cell, Vector3 direction) // CgeCell to be deleted
    {
        m_cellsAboutToBeScanned.Enqueue(cell);
        m_cellChangePositions.Enqueue(direction);
    }

    public void activateCubeInsideCell(Vector3 position)
    {
        if (m_spawnObject == null)
            m_cge.activateCubeUnsafe(position);
        else
        {
            if (m_spawnObject.GetComponent<EntitySystemBase>() != null)
            {
                GameObject o = m_cge.activateCubeSafe(position);
                if (o.activeSelf == false)
                    Debug.Log("Shit!");
                o.GetComponent<CubeEntityPrefapSystem>().setToPrefab(m_spawnObject);
            }
            else
            {
                GameObject gameObject = Instantiate(m_spawnObject);
                gameObject.transform.position = position;
            }
        }
    }

    public void addCell(CgeCell cell)
    {
        m_cells.Add(cell);
    }

    // abstract
}
