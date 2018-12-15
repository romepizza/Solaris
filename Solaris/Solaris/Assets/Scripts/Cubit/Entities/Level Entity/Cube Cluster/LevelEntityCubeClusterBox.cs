using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntityCubeClusterBox : LevelEntityCubeCluster
{
    [Header("---------- CHILD ----------")]

    [Header("----- SETTINGS -----")]
    public devisionType devideBy;

    [Header("----- DEBUG -----")]
    public Vector3 m_areaSize;

    [Header("--- (Cells) ---")]
    public Vector3 m_cellNumbers;
    public Vector3 m_forcedPadding;
    public Vector3 m_forcedPaddingPerCell;
    public Vector3 m_orientationVector;
    
    public int m_currentCreateX;
    public int m_currentCreateY;
    public int m_currentCreateZ;
    public int m_currentCreateIndex;



    [Header("----- GIZMO -----")]
    public bool m_calculateEdgesB;
    public Vector3[] m_edges;

    private Cell[,,] m_cells; // m_cells[x][y][z]

    // strucs
    public class Cell
    {
        public Vector3 m_center;
        public Vector3[] m_edges;
    }
    public enum devisionType
    {
        bySize,
        byNumber
    }

    // Create Cubes
    public override void createCubes()
    {
        createCells();
        createChildObject();
        m_cubes = new GameObject[m_cubeAmount];

        if (m_createCubesPerFrame <= 0)
        {
            int cubeIndex = 0;
            for (int x = 0; x < (int)m_cellNumbers.x; x++)
            {
                for (int y = 0; y < (int)m_cellNumbers.y; y++)
                {
                    for (int z = 0; z < (int)m_cellNumbers.z; z++)
                    {
                        createCubeInsideCell(x, y, z, cubeIndex);
                        cubeIndex++;
                    }
                }
            }
        }
        else
        {
            m_isCreatingCubes = true;
        }
    }
    public override void createCubesOverTime()
    {
        if (m_isCreatingCubes)
        {
            int createdThisFrame = 0;

            while (m_currentCreateX < (int)m_cellNumbers.x)
            {
                while (m_currentCreateY < (int)m_cellNumbers.y)
                {
                    while (m_currentCreateZ < (int)m_cellNumbers.z)
                    {
                        GameObject cube = createCubeInsideCell(m_currentCreateX, m_currentCreateY, m_currentCreateZ, m_currentCreateIndex);
                        m_areaScript.registerActiveCube(cube, this);
                        m_currentCreateIndex++;
                        createdThisFrame++;
                        m_currentCreateZ++;
                        if (createdThisFrame >= m_createCubesPerFrame)
                            return;
                    }
                    m_currentCreateY++;
                    m_currentCreateZ = 0;
                }
                m_currentCreateX++;
                m_currentCreateY = 0;
            }
            m_isCreatingCubes = false;
        }

    }
    GameObject createCubeInsideCell(int x, int y, int z, int cubeIndex)
    {
        GameObject cube = Instantiate(m_spawnPrefab);
        cube.GetComponent<CubeEntitySystem>().addComponentsAtStart();
        cube.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);

        Vector3 randomVector = Vector3.zero;
        randomVector.x = Random.Range(-m_cellSize.x + m_cubePadding.x, m_cellSize.x - m_cubePadding.x) / 2;
        randomVector.y = Random.Range(-m_cellSize.y + m_cubePadding.y, m_cellSize.y - m_cubePadding.y) / 2;
        randomVector.z = Random.Range(-m_cellSize.z + m_cubePadding.z, m_cellSize.z - m_cubePadding.z) / 2;


        cube.transform.position = m_cells[x, y, z].m_center + randomVector;
        cube.transform.SetParent(m_childObjectOfCubes.transform);

        //int index = (x * y * z) + (y * z) + z;
        m_cubes[cubeIndex] = cube;
        
        return cube;
    }
	
    // Generate shape
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

        Vector3 startPoint = Vector3.zero;
        if(m_startPoint != null)
            startPoint = m_startPoint.transform.position;

        Vector3 endPoint = Vector3.zero;
        if(m_endPoint != null)
            endPoint = m_endPoint.transform.position;

        m_edges = new Vector3[8];
        m_edges[0] = startPoint;                                            // 0: (+ / + / +)
        m_edges[1] = new Vector3(endPoint.x, startPoint.y, startPoint.z);   // 1: (- / + / +)
        m_edges[2] = new Vector3(endPoint.x, startPoint.y, endPoint.z);     // 2: (- / + / -)
        m_edges[3] = new Vector3(startPoint.x, startPoint.y, endPoint.z);   // 3: (+ / + / -)
        m_edges[4] = new Vector3(startPoint.x, endPoint.y, startPoint.z);   // 4: (+ / - / +)
        m_edges[5] = new Vector3(endPoint.x, endPoint.y, startPoint.z);     // 5: (- / - / +)
        m_edges[6] = endPoint;                                              // 6: (- / - / -)
        m_edges[7] = new Vector3(startPoint.x, endPoint.y, endPoint.z);     // 7: (+ / - / -)

        m_areaSize = new Vector3(Mathf.Abs(startPoint.x - endPoint.x), Mathf.Abs(startPoint.y - endPoint.y), Mathf.Abs(startPoint.z - endPoint.z));
    }
    void createCells()
    {
        if (devideBy == devisionType.bySize)
        {
            getSizeInfo();

            m_cells = new Cell[(int)m_cellNumbers.x, (int)m_cellNumbers.y, (int)m_cellNumbers.z];

            for (int x = 0; x < (int)m_cellNumbers.x; x++)
            {
                for (int y = 0; y < (int)m_cellNumbers.y; y++)
                {
                    for (int z = 0; z < (int)m_cellNumbers.z; z++)
                    {
                        m_cells[x, y, z] = createSingleCell(x, y, z);
                    }
                }
            }
        }
    }
    void getSizeInfo()
    {
        m_cellNumbers.x = m_areaSize.x / m_cellSize.x;
        m_cellNumbers.y = m_areaSize.y / m_cellSize.y;
        m_cellNumbers.z = m_areaSize.z / m_cellSize.z;



        m_forcedPadding = m_areaSize - new Vector3((int)m_cellNumbers.x * m_cellSize.x, (int)m_cellNumbers.y * m_cellSize.y, (int)m_cellNumbers.z * m_cellSize.z);
        m_forcedPaddingPerCell.x = m_forcedPadding.x / m_cellNumbers.x;
        m_forcedPaddingPerCell.y = m_forcedPadding.y / m_cellNumbers.y;
        m_forcedPaddingPerCell.z = m_forcedPadding.z / m_cellNumbers.z;

        m_orientationVector = new Vector3();
        m_orientationVector.x = (m_startPoint.transform.position.x - m_endPoint.transform.position.x < 0) ? (1) : (-1);
        m_orientationVector.y = (m_startPoint.transform.position.y - m_endPoint.transform.position.y < 0) ? (1) : (-1);
        m_orientationVector.z = (m_startPoint.transform.position.z - m_endPoint.transform.position.z < 0) ? (1) : (-1);

        m_cubeAmount = (int)m_cellNumbers.x * (int)m_cellNumbers.y * (int)m_cellNumbers.z;
        
        m_cubePadding = new Vector3(m_spawnPrefab.transform.localScale.x * 2f, m_spawnPrefab.transform.localScale.y * 2f, m_spawnPrefab.transform.localScale.z * 2f);
    }
    Cell createSingleCell(int x, int y, int z)
    {
        Cell cell = new Cell();
        // center point
        Vector3 xOffset = new Vector3((x + 0.5f) * (m_cellSize.x + m_forcedPaddingPerCell.x) * m_orientationVector.x, 0, 0);
        Vector3 yOffset = new Vector3(0, (y + 0.5f) * (m_cellSize.y + m_forcedPaddingPerCell.y) * m_orientationVector.y, 0);
        Vector3 zOffset = new Vector3(0, 0, (z + 0.5f) * (m_cellSize.z + m_forcedPaddingPerCell.z) * m_orientationVector.z);
        cell.m_center = m_startPoint.transform.position + xOffset + yOffset + zOffset;

        // edges
        Vector3[] edges = new Vector3[8];
        edges[0] = cell.m_center + new Vector3(m_cellSize.x, m_cellSize.y, m_cellSize.z) / 2f;
        edges[1] = cell.m_center + new Vector3(-m_cellSize.x, m_cellSize.y, m_cellSize.z) / 2f;
        edges[2] = cell.m_center + new Vector3(-m_cellSize.x, m_cellSize.y, -m_cellSize.z) / 2f;
        edges[3] = cell.m_center + new Vector3(m_cellSize.x, m_cellSize.y, -m_cellSize.z) / 2f;
        edges[4] = cell.m_center + new Vector3(m_cellSize.x, -m_cellSize.y, m_cellSize.z) / 2f;
        edges[5] = cell.m_center + new Vector3(-m_cellSize.x, -m_cellSize.y, m_cellSize.z) / 2f;
        edges[6] = cell.m_center + new Vector3(-m_cellSize.x, -m_cellSize.y, -m_cellSize.z) / 2f;
        edges[7] = cell.m_center + new Vector3(m_cellSize.x, -m_cellSize.y, -m_cellSize.z) / 2f;
        cell.m_edges = edges;

        return cell;
    }

    // Gizmo stuff
    void drawLines()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(m_edges[0], m_edges[1]); Gizmos.DrawLine(m_edges[0], m_edges[4]); Gizmos.DrawLine(m_edges[0], m_edges[3]);
        Gizmos.DrawLine(m_edges[1], m_edges[5]); Gizmos.DrawLine(m_edges[1], m_edges[2]);
        Gizmos.DrawLine(m_edges[2], m_edges[3]); Gizmos.DrawLine(m_edges[2], m_edges[6]);
        Gizmos.DrawLine(m_edges[3], m_edges[7]);
        Gizmos.DrawLine(m_edges[4], m_edges[5]); Gizmos.DrawLine(m_edges[4], m_edges[7]);
        Gizmos.DrawLine(m_edges[5], m_edges[6]);
        Gizmos.DrawLine(m_edges[6], m_edges[7]);

        for (int i = 0; i < m_edges.Length; i++)
        {
            Gizmos.DrawWireSphere(m_edges[i], 0.3f);
        }
        
        Gizmos.color = Color.magenta;

        for (int x = 0; x < (int)m_cellNumbers.x; x++)
        {
            for (int y = 0; y < (int)m_cellNumbers.y; y++)
            {
                for (int z = 0; z < (int)m_cellNumbers.z; z++)
                {
                    try
                    {
                        Gizmos.DrawLine(m_cells[x, y, z].m_edges[0], m_cells[x, y, z].m_edges[1]); Gizmos.DrawLine(m_cells[x, y, z].m_edges[0], m_cells[x, y, z].m_edges[4]); Gizmos.DrawLine(m_cells[x, y, z].m_edges[0], m_cells[x, y, z].m_edges[3]);
                        Gizmos.DrawLine(m_cells[x, y, z].m_edges[1], m_cells[x, y, z].m_edges[5]); Gizmos.DrawLine(m_cells[x, y, z].m_edges[1], m_cells[x, y, z].m_edges[2]);
                        Gizmos.DrawLine(m_cells[x, y, z].m_edges[2], m_cells[x, y, z].m_edges[3]); Gizmos.DrawLine(m_cells[x, y, z].m_edges[2], m_cells[x, y, z].m_edges[6]);
                        Gizmos.DrawLine(m_cells[x, y, z].m_edges[3], m_cells[x, y, z].m_edges[7]);
                        Gizmos.DrawLine(m_cells[x, y, z].m_edges[4], m_cells[x, y, z].m_edges[5]); Gizmos.DrawLine(m_cells[x, y, z].m_edges[4], m_cells[x, y, z].m_edges[7]);
                        Gizmos.DrawLine(m_cells[x, y, z].m_edges[5], m_cells[x, y, z].m_edges[6]);
                        Gizmos.DrawLine(m_cells[x, y, z].m_edges[6], m_cells[x, y, z].m_edges[7]);
                    }
                    catch
                    {
                        calculateEdges();
                        createCells();
                    }
                    //Gizmos.DrawWireCube(m_cells[x, y, z].m_center, new Vector3(2, 2, 2));
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        // 0: (0, 1),   (0, 4),     (0, 3)    
        // 1: -         (1, 5),     (1, 2)     
        // 2: (2, 3),   (2, 6)
        // 3: (3, 7)
        // 4: (4, 5),   (4, 7)
        // 5: (5, 6)
        // 6: (6, 7)
        if (m_cellSize.x > 0 && m_cellSize.y > 0 && m_cellSize.z > 0)
        {
            if (m_calculateEdgesB)
            {
                calculateEdges();
                //m_prefabSize = Mathf.Max(m_spawnPrefab.transform.localScale.x, m_spawnPrefab.transform.localScale.y, m_spawnPrefab.transform.localScale.z) / 2f;
                createCells();
            }
            if (m_areaScript == null)
                setAreaManager();
            if (m_showGizmos || m_areaScript.m_showGizmos)
                drawLines();
        }
        else
            Debug.Log("Warning: Do not make m_cellSize's values smaller equal to zero!");
    }
}
