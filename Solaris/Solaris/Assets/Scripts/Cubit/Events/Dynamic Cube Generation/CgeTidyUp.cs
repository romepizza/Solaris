using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CgeTidyUp : MonoBehaviour
{
    public CGE m_cge;

    [Header("------- Settings -------")]
    public float m_deactivateDistance;
    public int m_deactivisionsPerFrame;

    [Header("------- Debug -------")]
    public List<List<GameObject>> m_allCubes;
    public int m_checkedTotal;

    // Use this for initialization
    void Start()
    {
        m_cge = GetComponent<CGE>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //checkForDeactivate();
    }

    
}
