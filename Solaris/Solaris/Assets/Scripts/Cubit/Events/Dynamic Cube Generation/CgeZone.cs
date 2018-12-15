using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CgeZone
{
    [Header("----- SETTINGS -----")]
    public Vector3 m_cellSize;
    public bool m_useStrechCells;

    [Header("----- DEBUG -----")]
    public Vector3 m_cellSizeStreched;
    public List<CgeCell> m_cells;
    public Vector3 m_center;
    public Vector3[] m_edgesPreFrame;
    public Vector3[] m_edgesPostFrame;
    public Vector3 m_diameter;
    public Vector3 m_normal;
    public Vector3 m_cellNumbers;
    public int m_faceIndex;
    public CgeForm m_form;

    // Constructor
    public CgeZone(Vector3 size, Vector3 start, Vector3 end, int faceIndex, bool useStrechCells, CgeForm form)
    {
        m_cellSize = size;
        m_faceIndex = faceIndex;
        m_useStrechCells = useStrechCells;
        m_form = form;

        calculateEdges(start, end);
        calculateNormals();
        createCells();


        /*
        Debug.Log(faceIndex);
        Debug.Log(m_edgesPostFrame[0]);
        Debug.Log(m_edgesPostFrame[1]);
        Debug.Log(m_edgesPostFrame[2]);
        Debug.Log(m_edgesPostFrame[3]);
        */

        /*
        Debug.DrawLine(m_edgesPostFrame[0], m_edgesPostFrame[1], Color.yellow, 10f);
        Debug.DrawLine(m_edgesPostFrame[1], m_edgesPostFrame[2], Color.yellow, 10f);
        Debug.DrawLine(m_edgesPostFrame[2], m_edgesPostFrame[3], Color.yellow, 10f);
        Debug.DrawLine(m_edgesPostFrame[3], m_edgesPostFrame[0], Color.yellow, 10f);
        */

        /*
        m_edgesPreFrame = new Vector3[8];
        m_edgesPreFrame[0] = startPoint;                                            // 0: (+ / + / +)
        m_edgesPreFrame[1] = new Vector3(endPoint.x, startPoint.y, startPoint.z);   // 1: (- / + / +)
        m_edgesPreFrame[2] = new Vector3(endPoint.x, startPoint.y, endPoint.z);     // 2: (- / + / -)
        m_edgesPreFrame[3] = new Vector3(startPoint.x, startPoint.y, endPoint.z);   // 3: (+ / + / -)
        m_edgesPreFrame[4] = new Vector3(startPoint.x, endPoint.y, startPoint.z);   // 4: (+ / - / +)
        m_edgesPreFrame[5] = new Vector3(endPoint.x, endPoint.y, startPoint.z);     // 5: (- / - / +)
        m_edgesPreFrame[6] = endPoint;                                              // 6: (- / - / -)
        m_edgesPreFrame[7] = new Vector3(startPoint.x, endPoint.y, endPoint.z);     // 7: (+ / - / -)
        */

        /*
        m_edgesPostFrame = new Vector3[8];
        m_edgesPostFrame[0] = startPoint;                                            // 0: (+ / + / +)
        m_edgesPostFrame[1] = new Vector3(endPoint.x, startPoint.y, startPoint.z);   // 1: (- / + / +)
        m_edgesPostFrame[2] = new Vector3(endPoint.x, startPoint.y, endPoint.z);     // 2: (- / + / -)
        m_edgesPostFrame[3] = new Vector3(startPoint.x, startPoint.y, endPoint.z);   // 3: (+ / + / -)
        m_edgesPostFrame[4] = new Vector3(startPoint.x, endPoint.y, startPoint.z);   // 4: (+ / - / +)
        m_edgesPostFrame[5] = new Vector3(endPoint.x, endPoint.y, startPoint.z);     // 5: (- / - / +)
        m_edgesPostFrame[6] = endPoint;                                              // 6: (- / - / -)
        m_edgesPostFrame[7] = new Vector3(startPoint.x, endPoint.y, endPoint.z);     // 7: (+ / - / -)
        */
    }
    void calculateEdges(Vector3 start, Vector3 end)
    {
        m_edgesPreFrame = new Vector3[8];
        m_edgesPostFrame = new Vector3[4];

        m_center = (start + end) * 0.5f;

        Vector3 startPoint = start;
        Vector3 endPoint = end;
        m_diameter = start - end;

        //Debug.Log(m_faceIndex + ": " + m_diameter);


        m_edgesPostFrame = new Vector3[4];
        m_edgesPostFrame[0] = startPoint;
        m_edgesPostFrame[2] = endPoint;
        if (m_faceIndex >= 0 && m_faceIndex < 4)
        {
            int sign = start.y > end.y ? -1 : 1;

            m_edgesPostFrame[1] = start + sign * new Vector3(0, m_diameter.y, 0);
            m_edgesPostFrame[3] = end - sign * new Vector3(0, m_diameter.y, 0);
        }
        if (m_faceIndex >= 4 && m_faceIndex < 6)
        {
            int sign = start.x > end.x ? -1 : 1;
            m_edgesPostFrame[1] = start + sign * new Vector3(m_diameter.x, 0, 0);
            m_edgesPostFrame[3] = end - sign * new Vector3(m_diameter.x, 0, 0);
        }
    }
    void calculateNormals()
    {
        if (m_diameter.x == 0)
            m_normal = Vector3.right * ((m_diameter.z < 0) ? 1 : -1);
        else if (m_diameter.z == 0)
            m_normal = Vector3.forward * ((m_diameter.x < 0) ? -1 : 1);
        else if (m_faceIndex == 4 || m_faceIndex == 5)
            m_normal = Vector3.up * ((m_faceIndex == 4) ? 1 : -1);
        else
            Debug.Log("Warning: Unwanted Assertion!");
    }
    void createCells()
    {
        m_cellNumbers = Vector3.zero;

        m_cellNumbers.x = Mathf.Abs(m_diameter.x / m_cellSize.x);
        m_cellNumbers.y = Mathf.Abs(m_diameter.y / m_cellSize.y);
        m_cellNumbers.z = Mathf.Abs(m_diameter.z / m_cellSize.z);
        //Debug.Log("Numbers: " + m_cellNumbers);
        m_cellSizeStreched = m_cellSize;
        
        if (m_useStrechCells)
        {
            float factorX = (int)m_cellNumbers.x * m_cellSize.x;
            float factorY = (int)m_cellNumbers.y * m_cellSize.y;
            float factorZ = (int)m_cellNumbers.z * m_cellSize.z;

            if(factorX != 0)
                m_cellSizeStreched.x *=  factorX;
            if (factorY != 0)
                m_cellSizeStreched.y *= factorY;
            if (factorZ != 0)
                m_cellSizeStreched.z *=  factorZ;
        }
        //Debug.Log("d: " + m_diameter);
        //Debug.Log("cN: " + (int)m_cellNumbers.x);
        //Debug.Log("Ergebnis: " + ((int)m_cellNumbers.x * m_cellSize.x));
        //Debug.Log("Size 2: " + m_cellSizeStreched);
        //int numberCells = Mathf.Max((int)(cellSizeActual.x), 1) * Mathf.Max((int)(cellSizeActual.y), 1) * Mathf.Max((int)(cellSizeActual.z), 1);
        m_cells = new List<CgeCell>();

        for (int x = 0; x <= m_cellNumbers.x; x++)
        {
            for(int y = 0; y <= m_cellNumbers.y; y++)
            {
                for(int z = 0; z <= m_cellNumbers.z; z++)
                {
                    Vector3 center = m_edgesPostFrame[0] - new Vector3(m_cellSizeStreched.x * x * ((m_diameter.x < 0) ? -1 : 1), m_cellSizeStreched.y * y, m_cellSizeStreched.z * z * ((m_diameter.z < 0) ? -1 : 1));
                    //Debug.DrawLine(Vector3.zero, center, Color.white, 100f);
                    CgeCell cell = new CgeCell(center, m_cellSizeStreched, m_form, m_normal);
                    m_cells.Add(cell);
                    m_form.addCell(cell);
                }
            }
        }

        //Debug.Log(m_cells.Count);
    }
    
    public void moveZone(Vector3 distance)
    {
        for(int i = 0; i < m_edgesPostFrame.Length; i++)
        {
            m_edgesPostFrame[i] += distance;
        }
        /*
        Debug.DrawLine(m_edgesPostFrame[0], m_edgesPostFrame[1], Color.red);
        Debug.DrawLine(m_edgesPostFrame[1], m_edgesPostFrame[2], Color.red);
        Debug.DrawLine(m_edgesPostFrame[2], m_edgesPostFrame[3], Color.red);
        Debug.DrawLine(m_edgesPostFrame[3], m_edgesPostFrame[0], Color.red);
        */
        /*
        Debug.DrawLine(m_edgesPostFrame[0], m_edgesPostFrame[1]); Debug.DrawLine(m_edgesPostFrame[0], m_edgesPostFrame[4]); Debug.DrawLine(m_edgesPostFrame[0], m_edgesPostFrame[3]);
        Debug.DrawLine(m_edgesPostFrame[1], m_edgesPostFrame[5]); Debug.DrawLine(m_edgesPostFrame[1], m_edgesPostFrame[2]);
        Debug.DrawLine(m_edgesPostFrame[2], m_edgesPostFrame[3]); Debug.DrawLine(m_edgesPostFrame[2], m_edgesPostFrame[6]);
        Debug.DrawLine(m_edgesPostFrame[3], m_edgesPostFrame[7]);
        Debug.DrawLine(m_edgesPostFrame[4], m_edgesPostFrame[5]); Debug.DrawLine(m_edgesPostFrame[4], m_edgesPostFrame[7]);
        Debug.DrawLine(m_edgesPostFrame[5], m_edgesPostFrame[6]);
        Debug.DrawLine(m_edgesPostFrame[6], m_edgesPostFrame[7]);
        */
        /*
        Debug.DrawLine(m_edgesPreFrame[0], m_edgesPreFrame[1]); Debug.DrawLine(m_edgesPreFrame[0], m_edgesPreFrame[4]); Debug.DrawLine(m_edgesPreFrame[0], m_edgesPreFrame[3]);
        Debug.DrawLine(m_edgesPreFrame[1], m_edgesPreFrame[5]); Debug.DrawLine(m_edgesPreFrame[1], m_edgesPreFrame[2]);
        Debug.DrawLine(m_edgesPreFrame[2], m_edgesPreFrame[3]); Debug.DrawLine(m_edgesPreFrame[2], m_edgesPreFrame[6]);
        Debug.DrawLine(m_edgesPreFrame[3], m_edgesPreFrame[7]);
        Debug.DrawLine(m_edgesPreFrame[4], m_edgesPreFrame[5]); Debug.DrawLine(m_edgesPreFrame[4], m_edgesPreFrame[7]);
        Debug.DrawLine(m_edgesPreFrame[5], m_edgesPreFrame[6]);
        Debug.DrawLine(m_edgesPreFrame[6], m_edgesPreFrame[7]);
        */

        /*
        for (int i = 0; i < m_edgesPreFrame.Length; i++)
        {
            m_edgesPreFrame[i] += distance;
        }
        */

    }
}
