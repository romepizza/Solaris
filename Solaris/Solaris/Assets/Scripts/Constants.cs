using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public GameObject m_player;
    public GameObject m_mainCamera;
    public GameObject m_playerGyrometer;
    public GameObject m_mainCge;
    public CemBoidBase m_boidSystem;
    public GameObject m_planet;
    

    private static GameObject s_player;
    private static GameObject s_mainCamera;
    private static GameObject s_playerGyrometer;
    private static CGE s_mainCge;
    private static CemBoidBase s_boidSystem;
    private static GameObject s_planet;

    public static int s_playerLayer = 9;

	// Use this for initialization
	void Start ()
    {
        s_player = m_player;
        s_mainCamera = m_mainCamera;
        s_playerGyrometer = m_playerGyrometer;
        s_boidSystem = m_boidSystem;
        s_planet = m_planet;


        if (m_mainCge != null)
            s_mainCge = m_mainCge.GetComponent<CGE>();
        else
            Debug.Log("Warning: No main CGE set!");
	}
    
    public static GameObject getPlayer()
    {
        return s_player;
    }

    public static GameObject getMainCamera()
    {
        return s_mainCamera;
    }

    public static GameObject getPlayerGyrometer()
    {
        return s_playerGyrometer;
    }

    public static CGE getMainCge()
    {
        return s_mainCge;
    }

    public static CemBoidBase getBoidSystem()
    {
        return s_boidSystem;
    }

    public static GameObject getPlanet()
    {
        return s_planet;
    }
}
