using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CgeFormRect : CgeForm
{
    //public bool m_showZones;
    public bool m_showCells;
    [Header("------- Settings -------")]
    public Vector3 m_detectionSize;
    public Vector3 m_cellSize;
    public bool m_useStrechCells;
    public int m_movesAndScansPerFrame;

    [Header("------- Debug -------")]
    public Vector3[] m_edges;
    public CgeZone[] m_zones;
    public Queue<CgeCell> m_cellQueue;
    public Queue<Vector3> m_positionQueue;
    public Vector3 m_lastPlayerMovement;
    // Use this for initialization
    public override void Start ()
    {
        base.Start();
        initializeStuff();
        m_cellQueue = new Queue<CgeCell>();
        m_positionQueue = new Queue<Vector3>();
    }
	
	// Update is called once per frame
    public override void FixedUpdate ()
    {
        base.FixedUpdate();

        Vector3 direction = managePlayerMovement();
        //Debug.Log(direction);
        if (direction != Vector3.zero)
            moveForm(direction);

        manageQueues();
        
        if(m_showCells)
        {
            for (int i = 0; i < m_cells.Count; i++)
                m_cells[i].drawCell();
        }
        
    }

    // Management
    Vector3 managePlayerMovement()
    {
        Vector3 direction = Vector3.zero;
        // right
        if(m_playerMovementVector.x > m_cellSize.x * 0.5f)
        {
            direction += new Vector3(m_cellSize.x, 0, 0);
            m_playerMovementVector.x -= m_cellSize.x;
        }
        // left
        if (m_playerMovementVector.x < -m_cellSize.x * 0.5f)
        {
            direction += new Vector3(-m_cellSize.x, 0, 0);
            m_playerMovementVector.x += m_cellSize.x;
        }
        // up
        if (m_playerMovementVector.z > m_cellSize.z * 0.5f)
        {
            direction += new Vector3(0, 0, m_cellSize.z);
            m_playerMovementVector.z -= m_cellSize.z;
        }
        // down
        if (m_playerMovementVector.z < -m_cellSize.z * 0.5f)
        {
            direction += new Vector3(0, 0, -m_cellSize.z);
            m_playerMovementVector.z += m_cellSize.z;
        }
        // forward
        if (m_playerMovementVector.y > m_cellSize.y * 0.5f)
        {
            direction += new Vector3(0, m_cellSize.y, 0);
            m_playerMovementVector.y -= m_cellSize.y;
        }
        // backward
        if (m_playerMovementVector.y < -m_cellSize.y * 0.5f)
        {
            direction += new Vector3(0, -m_cellSize.y, 0);
            m_playerMovementVector.y += m_cellSize.y;
        }

        return direction;
    }
    void moveForm(Vector3 direction)
    {
        for (int i = 0; i < m_cells.Count; i++)
        {
            if (m_cells[i] != null)
                registerCubeMovement(m_cells[i], direction);// registerCellCheck(m_cells[i], direction);
        }
        
    }

    // Queue
    void registerCubeMovement(CgeCell cell, Vector3 direction)
    {
        m_cellQueue.Enqueue(cell);
        m_positionQueue.Enqueue(direction);
    }
    void manageQueues()
    {
        int checks = m_movesAndScansPerFrame;// (int)Mathf.Min(m_movesAndScansPerFrame, Mathf.Max(m_cellQueue.Count * 0.1f, 5));
        //Debug.Log("C: " + checks);
        for(int i = 0; i < checks; i++)
        {
            if (m_cellQueue.Count <= 0)
                break;
            CgeCell cell = m_cellQueue.Dequeue();
            cell.moveAndScanCell(m_positionQueue.Dequeue());
        }
    }

    // Construction
    void initializeStuff()
    {
        m_zones = new CgeZone[6];
        m_edges = new Vector3[8];
        Vector3 startPoint = m_player.transform.position + 0.5f * m_detectionSize;
        Vector3 endPoint = m_player.transform.position - 0.5f * m_detectionSize;

        m_edges = new Vector3[8];
        m_edges[0] = startPoint;                                            // 0: (+ / + / +)
        m_edges[1] = new Vector3(endPoint.x, startPoint.y, startPoint.z);   // 1: (- / + / +)
        m_edges[2] = new Vector3(endPoint.x, startPoint.y, endPoint.z);     // 2: (- / + / -)
        m_edges[3] = new Vector3(startPoint.x, startPoint.y, endPoint.z);   // 3: (+ / + / -)
        m_edges[4] = new Vector3(startPoint.x, endPoint.y, startPoint.z);   // 4: (+ / - / +)
        m_edges[5] = new Vector3(endPoint.x, endPoint.y, startPoint.z);     // 5: (- / - / +)
        m_edges[6] = endPoint;                                              // 6: (- / - / -)
        m_edges[7] = new Vector3(startPoint.x, endPoint.y, endPoint.z);     // 7: (+ / - / -)

        /*
        Debug.DrawLine(m_edges[0], m_edges[1], Color.cyan, 10.0f); Debug.DrawLine(m_edges[0], m_edges[4], Color.cyan, 10.0f); Debug.DrawLine(m_edges[0], m_edges[3], Color.cyan, 10.0f);
        Debug.DrawLine(m_edges[1], m_edges[5], Color.cyan, 10.0f); Debug.DrawLine(m_edges[1], m_edges[2], Color.cyan, 10.0f);
        Debug.DrawLine(m_edges[2], m_edges[3], Color.cyan, 10.0f); Debug.DrawLine(m_edges[2], m_edges[6], Color.cyan, 10.0f);
        Debug.DrawLine(m_edges[3], m_edges[7], Color.cyan, 10.0f);
        Debug.DrawLine(m_edges[4], m_edges[5], Color.cyan, 10.0f); Debug.DrawLine(m_edges[4], m_edges[7], Color.cyan, 10.0f);
        Debug.DrawLine(m_edges[5], m_edges[6], Color.cyan, 10.0f);
        Debug.DrawLine(m_edges[6], m_edges[7], Color.cyan, 10.0f);
        */

        // destroy previous tones, if 


        // Create zones
        for (int i = 0; i < 6; i++)
        {
            Vector3 start = Vector3.zero;
            Vector3 end = Vector3.zero;


            if (i == 0)
            {
                start = m_edges[0];
                end = m_edges[5];
            }
            else if (i == 1)
            {
                start = m_edges[1];
                end = m_edges[6];
            }
            else if (i == 2)
            {
                start = m_edges[2];
                end = m_edges[7];
            }
            else if (i == 3)
            {
                start = m_edges[3];
                end = m_edges[4];
            }
            else if (i == 4)
            {
                start = m_edges[0];
                end = m_edges[2];
            }
            else if (i == 5)
            {
                start = m_edges[4];
                end = m_edges[6];
            }


            CgeZone zone = new CgeZone(m_cellSize, start, end, i, m_useStrechCells, this);
            m_zones[i] = zone;
        }
    }
}
