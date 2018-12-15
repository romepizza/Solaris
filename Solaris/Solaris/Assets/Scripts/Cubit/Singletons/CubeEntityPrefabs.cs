using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityPrefabs : MonoBehaviour
{
    [Header("----- SETTINGS -----")]
    [Header("--- (Neutral) ---")]
    public GameObject s_inactivePrefab;
    public GameObject s_activeNeutralPrefab;

    [Header("--- (Player) ---")]
    public GameObject s_activePlayerPrefab;
    public GameObject s_attachedPlayerPrefab;
    [Header("- (Skill Drone) -")]
    public GameObject s_activeDronePrefab;
    public GameObject s_attachedDronePrefab;
    public GameObject s_coreDronePrefab;

    [Header("--- (Enemy) ---")]

    [Header("- (Ejector) -")]
    public GameObject s_activeEnemyEjector;
    public GameObject s_attachedEnemyEjector;
    public GameObject s_coreEnemyEjector;
    [Header("- (Worm) -")]
    public GameObject s_attachedEnemyWorm;
    public GameObject s_coreEnemyWorm;
    [Header("- (Morpher) -")]
    public GameObject s_activeEnemyMorpher;
    public GameObject s_attachedEnemyMorpher;
    public GameObject s_coreEnemyMorpher;
    [Header("- (Swarm) -")]
    public GameObject s_coreEnemySwarm;
    public GameObject s_attachedEnemySwarm;


    private static CubeEntityPrefabs s_Instance = null;
    private string m_objectName = "CubeScriptObject";

	// Use this for initialization
	void Start ()
    {
        m_objectName = this.gameObject.name;
	}
	

    public static CubeEntityPrefabs getInstance()
    {
        if(s_Instance == null)
        {
            s_Instance = FindObjectOfType(typeof(CubeEntityPrefabs)) as CubeEntityPrefabs;
        }
        
        if(s_Instance == null)
        {
            Debug.Log("Singleton of CubeEntityPrefabs not working properly!");
        }
        return s_Instance;
    }
}
