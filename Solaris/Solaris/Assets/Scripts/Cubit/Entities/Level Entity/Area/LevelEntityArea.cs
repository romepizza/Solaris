using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntityArea : MonoBehaviour
{
    [Header("----- SETTINGS -----")]

    [Header("----- DEBUG -----")]
    public List<List<GameObject>> m_cubesInArea;
    public List<LevelEntityCubeCluster> m_clusters;
    public List<LevelEntityCubeCluster> m_activeClusters;

    [Header("----- GIZMOS -----")]
    public bool m_showGizmos;

    private LevelEntityLevelManager m_levelManagerScript;
	// Use this for initialization
	void Start ()
    {
        setLevelManager();

        m_cubesInArea = new List<List<GameObject>>();

    }

	
    void setLevelManager()
    {
        GameObject parent = gameObject.transform.parent.gameObject;
        if (parent == null)
        {
            Debug.Log("Warning: LevelEntityAreaManager has no parent! (" + gameObject.name + ")");
            return;
        }
        if (parent.GetComponent<LevelEntityLevelManager>() != null)
        {
            m_levelManagerScript = parent.GetComponent<LevelEntityLevelManager>();
        }
        else
            Debug.Log("Warning: Parent of LevelEntityAreaManager's parent is not a LevelEntityLevelManager!");
    }

    // Register stuff
    public void registerCluster(LevelEntityCubeCluster cluster)
    {
        m_clusters.Add(cluster);
    }

    public void activateCluster(LevelEntityCubeCluster cluster)
    {
        m_activeClusters.Add(cluster);
        m_cubesInArea.Add(new List<GameObject>());
        foreach (GameObject cube in cluster.m_cubes)
        {
            if(cube != null)
                m_cubesInArea[m_clusters.IndexOf(cluster)].Add(cube);
        }
    }
    public void registerActiveCube(GameObject cube, LevelEntityCubeCluster cluster)
    {
        if (m_activeClusters.Contains(cluster))
        {
            int index = m_activeClusters.IndexOf(cluster);
            m_cubesInArea[index].Add(cube);
        }
        else
        {
            Debug.Log("Warning: Tried to add cube to area, that wasn't registered!");
        }
    }
    public void registerActiveCubes(LevelEntityCubeCluster cluster)
    {
        
    }
}
