using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitGyrometer : MonoBehaviour {

    public KeyCode m_keyCode;
    public KeyCode m_keyCodeFlip;
    [Header("------- Settings -------")]
    public GameObject m_centerObject;
    public Vector3 m_customRotation;
    
    [Header("------- Debug -------")]
    public Transform m_centerTransform;
    public bool m_playerIsAffected;
    public Transform m_playerTransform;
    public Transform m_gyrometer;

    // public PlayerEntityCameraRotation m_playerCameraScript;

    // Use this for initialization
    void Start()
    {
        initializeStuff();
    }
    void initializeStuff()
    {
        if (m_centerObject == null)
        {
            m_centerObject = gameObject;
        }
        m_centerTransform = m_centerObject.transform;
        m_playerTransform = Constants.getPlayer().transform;
        m_gyrometer = Constants.getPlayerGyrometer().transform;
    }

    // Update is called once per frame
    void Update()
    {
        manageInput();
        calculateNewGyrometerQuaternion();
    }


    void calculateNewGyrometerQuaternion()
    {
        if (m_playerIsAffected)
        {
            Vector3 direction = m_playerTransform.position - m_centerTransform.position;

            Quaternion wantQuaternion = Quaternion.LookRotation(-direction, m_gyrometer.forward);
            wantQuaternion = wantQuaternion * Quaternion.Euler(-90f, 0, 0) * Quaternion.Euler(m_customRotation);
            m_gyrometer.rotation = Quaternion.RotateTowards(m_gyrometer.rotation, wantQuaternion, 135f * Time.deltaTime);
        }
        else
        {
            m_gyrometer.rotation = Quaternion.RotateTowards(m_gyrometer.rotation, Quaternion.Euler(0, 0, 0), 135f * Time.deltaTime);
        }
    }
    // input
    void manageInput()
    {
        if (Input.GetKeyDown(m_keyCode))
        {
            m_playerIsAffected = !m_playerIsAffected;
        }
        if(Input.GetKeyDown(m_keyCodeFlip))
        {
            m_customRotation.z = (m_customRotation.z + 180) % 360;
        }
    }
}
