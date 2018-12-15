using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRotationToCameraRotation : MonoBehaviour 
{
    public GameObject m_camera;
	// Use this for initialization
	void Start () {
        m_camera = Constants.getMainCamera();
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.rotation = m_camera.transform.rotation;
	}
}
