using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public GameObject m_fps1000;
    public GameObject m_fps100;
    public GameObject m_fps20;
    public GameObject m_fps5;


    public static float m_currentFps1000 = -1f;
    public static float m_currentFps100 = -1f;
    public static float m_currentFps20 = -1f;
    public static float m_currentFps5 = -1f;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(m_fps1000 != null)
            m_currentFps1000 = m_fps1000.GetComponent<ShowFps>().m_finalFps;
        if (m_fps100 != null)
            m_currentFps100 = m_fps100.GetComponent<ShowFps>().m_finalFps;
        if (m_fps20 != null)
            m_currentFps20 = m_fps20.GetComponent<ShowFps>().m_finalFps;
        if (m_fps5 != null)
            m_currentFps5 = m_fps5.GetComponent<ShowFps>().m_finalFps;
    }
}
