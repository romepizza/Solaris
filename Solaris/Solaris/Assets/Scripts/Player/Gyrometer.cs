using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyrometer : MonoBehaviour
{
    public List<Transform> m_gyrometers;


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Quaternion getRotation()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        if (m_gyrometers.Count == 0)
        {
            Debug.Log("Warning: No gyrometers found!");
            return rotation;
        }

        for(int i = 0; i < m_gyrometers.Count; i++)
        {
            rotation *= m_gyrometers[i].rotation;
        }

        return rotation;
    }
}
