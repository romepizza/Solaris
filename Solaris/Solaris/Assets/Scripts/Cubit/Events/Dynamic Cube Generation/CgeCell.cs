using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CgeCell
{
    [Header("----- SETTINGS -----")]
    public bool m_showDebug;

    [Header("----- DEBUG -----")]
    public GameObject m_player;
    public Vector3 m_center;
    public Vector3 m_size;
    public Vector3[] m_edges;
    public CgeForm m_form;
    public Vector3 m_normal;

    // Constructor
    public CgeCell(Vector3 center, Vector3 size, CgeForm form, Vector3 normal)
    {
        m_showDebug = true;
        m_player = Constants.getPlayer();

        m_center = center;
        m_size = size;
        m_form = form;
        m_normal = normal;

        // Debug
        if (m_showDebug)
        {
            m_edges = new Vector3[8];
            m_edges[0] = center + new Vector3(m_size.x, m_size.y, m_size.z) * 0.5f;   // 0: (+ / + / +)
            m_edges[1] = center + new Vector3(-m_size.x, m_size.y, m_size.z) * 0.5f;   // 1: (- / + / +)
            m_edges[2] = center + new Vector3(-m_size.x, m_size.y, -m_size.z) * 0.5f;   // 2: (- / + / -)
            m_edges[3] = center + new Vector3(m_size.x, m_size.y, -m_size.z) * 0.5f;   // 3: (+ / + / -)

            m_edges[4] = center + new Vector3(m_size.x, -m_size.y, m_size.z) * 0.5f;   // 4: (+ / - / +)
            m_edges[5] = center + new Vector3(-m_size.x, -m_size.y, m_size.z) * 0.5f;   // 5: (- / - / +)
            m_edges[6] = center + new Vector3(-m_size.x, -m_size.y, -m_size.z) * 0.5f;   // 6: (- / - / -)
            m_edges[7] = center + new Vector3(m_size.x, -m_size.y, -m_size.z) * 0.5f;   // 7: (+ / - / -)
        }
        

        //Debug.Log(m_size);

        //Debug.DrawLine(Constants.getPlayer().transform.position, m_center);

        /*
        Debug.DrawLine(center, edges[0], Color.blue, 100f);
        Debug.DrawLine(center, edges[1], Color.cyan, 100f);               
        Debug.DrawLine(center, edges[2], Color.green, 100f);
        Debug.DrawLine(center, edges[3], Color.yellow, 100f);
        Debug.DrawLine(center, edges[4], Color.gray, 100f);
        Debug.DrawLine(center, edges[5], Color.black, 100f);
        Debug.DrawLine(center, edges[6], Color.magenta, 100f);
        Debug.DrawLine(center, edges[7], Color.white, 100f);
        */

        /*
        Debug.DrawLine(m_edges[0], m_edges[1], Color.cyan, 100f); Debug.DrawLine(m_edges[0], m_edges[4], Color.cyan, 100f); Debug.DrawLine(m_edges[0], m_edges[3], Color.cyan, 100f);
        Debug.DrawLine(m_edges[1], m_edges[5], Color.cyan, 100f); Debug.DrawLine(m_edges[1], m_edges[2], Color.cyan, 100f);
        Debug.DrawLine(m_edges[2], m_edges[3], Color.cyan, 100f); Debug.DrawLine(m_edges[2], m_edges[6], Color.cyan, 100f);
        Debug.DrawLine(m_edges[3], m_edges[7], Color.cyan, 100f);
        Debug.DrawLine(m_edges[4], m_edges[5], Color.cyan, 100f); Debug.DrawLine(m_edges[4], m_edges[7], Color.cyan, 100f);
        Debug.DrawLine(m_edges[5], m_edges[6], Color.cyan, 100f);
        Debug.DrawLine(m_edges[6], m_edges[7], Color.cyan, 100f);
        */
    }

    // Ops
    public void moveAndScanCell(Vector3 direction)
    {
        m_center += direction;

        if (m_showDebug)
        {
            for (int i = 0; i < m_edges.Length; i++)
                m_edges[i] += direction;
        }

        //if (Vector3.Dot(direction.normalized, (m_center - m_player.transform.position).normalized) > 0.5f)
        //if (Utility.getAngle(direction.normalized, (m_center - m_player.transform.position).normalized) > 0.5f)
        {
            Collider[] cols = Physics.OverlapBox(m_center, m_size * 0.5f);
            if (cols.Length == 0)
            {
                Vector3 position = getSpawnPosition();
                m_form.addCubeToActivisionQueue(this, position);
            }
        }
    }

    public void moveCell(Vector3 direction)
    {

        m_center += direction;

        // Debug
        if (m_showDebug)
        {
            for (int i = 0; i < m_edges.Length; i++)
                m_edges[i] += direction;
        }

        /*
        for(int i = 0; i < m_edges.Length; i++)
            m_edges[i] += direction;

        Debug.DrawLine(m_edges[0], m_edges[1]); Debug.DrawLine(m_edges[0], m_edges[4]); Debug.DrawLine(m_edges[0], m_edges[3]);
        Debug.DrawLine(m_edges[1], m_edges[5]); Debug.DrawLine(m_edges[1], m_edges[2]);
        Debug.DrawLine(m_edges[2], m_edges[3]); Debug.DrawLine(m_edges[2], m_edges[6]);
        Debug.DrawLine(m_edges[3], m_edges[7]);
        Debug.DrawLine(m_edges[4], m_edges[5]); Debug.DrawLine(m_edges[4], m_edges[7]);
        Debug.DrawLine(m_edges[5], m_edges[6]);
        Debug.DrawLine(m_edges[6], m_edges[7]);
        */
    }
    public void scanForActivision(Vector3 direction)
    {
        if (Vector3.Dot(direction, m_normal) >= 0.4f)
        {
            Collider[] cols = Physics.OverlapBox(m_center, m_size * 0.5f);
            if (cols.Length == 0)
            {
                Vector3 position = getSpawnPosition();
                m_form.addCubeToActivisionQueue(this, position);
            }
        }
    }
    public Vector3 getSpawnPosition()
    {
        return m_center + new Vector3(m_size.x  * Random.Range(-0.9f, 0.9f), m_size.y * Random.Range(-0.9f, 0.9f), m_size.z * Random.Range(-0.9f, 0.9f)) * 0.5f;
        //return m_center + m_size * 0.5f * Random.Range(-0.9f, 0.9f);
    }



    // Debug
    public void drawCell()
    {
        Color color = new Color(1f, 1f, 1f, 0.1f);
        Debug.DrawLine(m_edges[0], m_edges[1], color); Debug.DrawLine(m_edges[0], m_edges[4], color); Debug.DrawLine(m_edges[0], m_edges[3], color);
        Debug.DrawLine(m_edges[1], m_edges[5], color); Debug.DrawLine(m_edges[1], m_edges[2], color);
        Debug.DrawLine(m_edges[2], m_edges[3], color); Debug.DrawLine(m_edges[2], m_edges[6], color);
        Debug.DrawLine(m_edges[3], m_edges[7], color);
        Debug.DrawLine(m_edges[4], m_edges[5], color); Debug.DrawLine(m_edges[4], m_edges[7], color);
        Debug.DrawLine(m_edges[5], m_edges[6], color);
        Debug.DrawLine(m_edges[6], m_edges[7], color);
    }
}
