using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectZone : Zone
{
    [Header("------- Settings -------")]
    public boxType m_boxType;

    [Header("--- Half Extends ---")]
    public Vector3 m_halfExtends;

    [Header("--- Start/End Point ---")]
    public Transform m_startPoint;
    public Transform m_endPoint;

    [Header("------- Debug -------")]
    public Vector3[] m_edges;
    public Vector3 m_size;
    public Vector3 m_center;

    public enum boxType
    {
        halfExtends,
        startEndPoint
    }

    // override
    override public float calculateVolumeSingle()
    {
        if (m_boxType == boxType.halfExtends)
        {
            m_center = transform.position;
            m_size = m_halfExtends * 2f;
        }
        else if (m_boxType == boxType.startEndPoint)
        {
            m_center = (m_startPoint.position + m_endPoint.position) * 0.5f;
            m_size = new Vector3(Mathf.Abs(m_startPoint.position.x - m_endPoint.position.x), Mathf.Abs(m_startPoint.position.y - m_endPoint.position.y), Mathf.Abs(m_startPoint.position.z - m_endPoint.position.z));
        }

        m_volumeSingle = m_size.x * m_size.y * m_size.z;
        return m_volumeSingle;
    }
    override public Vector3 getRandomPoint()
    {
        Vector3 randomVector = m_center + new Vector3(Random.Range(-0.5f * m_size.x, 0.5f * m_size.x), Random.Range(-0.5f * m_size.y, 0.5f * m_size.y), Random.Range(-0.5f * m_size.z, 0.5f * m_size.z));
        return randomVector;
    }
    // Gizmos
    void OnDrawGizmos()
    {
        if (m_drawGizmos || m_isDrawGizmosSub)
        {
            calculateVolumeTotal();
            drawLines();
            calculateVolumeTotal();
        }
    }
    void calculateEdges()
    {
        // near plane: -
        // 0: (+ / + / +)
        // 1: (- / + / +)
        // 2: (- / + / -)
        // 3: (+ / + / -)
        // 4: (+ / - / +)
        // 5: (- / - / +)
        // 6: (- / - / -)
        // 7: (+ / - / -)

        Vector3 startPoint = m_center;
        Vector3 endPoint = m_center;
        if (m_boxType == boxType.halfExtends)
        {
            startPoint += transform.right * m_halfExtends.x;
            startPoint += transform.up * m_halfExtends.y;
            startPoint += transform.forward * m_halfExtends.z;
            endPoint -= transform.right * m_halfExtends.x;
            endPoint -= transform.up * m_halfExtends.y;
            endPoint -= transform.forward * m_halfExtends.z;

           // startPoint = m_center + transform.rotation * m_halfExtends;
            //endPoint = m_center - transform.rotation * m_halfExtends;
        }
        else if (m_boxType == boxType.startEndPoint)
        {
            if (m_startPoint != null)
                startPoint = m_startPoint.position;
            if (m_endPoint != null)
                endPoint = m_endPoint.position;
        }

        m_edges = new Vector3[8];
        m_edges[0] = startPoint;                                            // 0: (+ / + / +)
        m_edges[1] = new Vector3(endPoint.x, startPoint.y, startPoint.z);   // 1: (- / + / +)
        m_edges[2] = new Vector3(endPoint.x, startPoint.y, endPoint.z);     // 2: (- / + / -)
        m_edges[3] = new Vector3(startPoint.x, startPoint.y, endPoint.z);   // 3: (+ / + / -)
        m_edges[4] = new Vector3(startPoint.x, endPoint.y, startPoint.z);   // 4: (+ / - / +)
        m_edges[5] = new Vector3(endPoint.x, endPoint.y, startPoint.z);     // 5: (- / - / +)
        m_edges[6] = endPoint;                                              // 6: (- / - / -)
        m_edges[7] = new Vector3(startPoint.x, endPoint.y, endPoint.z);     // 7: (+ / - / -)

    }
    void drawLines()
    {
        calculateEdges();

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(m_edges[0], m_edges[1]); Gizmos.DrawLine(m_edges[0], m_edges[4]); Gizmos.DrawLine(m_edges[0], m_edges[3]);
        Gizmos.DrawLine(m_edges[1], m_edges[5]); Gizmos.DrawLine(m_edges[1], m_edges[2]);
        Gizmos.DrawLine(m_edges[2], m_edges[3]); Gizmos.DrawLine(m_edges[2], m_edges[6]);
        Gizmos.DrawLine(m_edges[3], m_edges[7]);
        Gizmos.DrawLine(m_edges[4], m_edges[5]); Gizmos.DrawLine(m_edges[4], m_edges[7]);
        Gizmos.DrawLine(m_edges[5], m_edges[6]);
        Gizmos.DrawLine(m_edges[6], m_edges[7]);


        Gizmos.color = Color.magenta;
        for (int i = 0; i < m_edges.Length; i++)
        {
            Gizmos.DrawWireSphere(m_edges[i], 0.3f);
        }
    }
}
