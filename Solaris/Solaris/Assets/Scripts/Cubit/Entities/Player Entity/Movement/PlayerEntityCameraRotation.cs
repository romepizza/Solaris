using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntityCameraRotation : MonoBehaviour
{
    public GameObject m_focus;
    public GameObject m_playerCamera;
    public GameObject m_gyrometer;

    [Header("------- Settings -------")]
    public float m_distance;
    public float m_maxCamAngle;
    public Vector3 m_customRotation;
    // public Vector3 m_lookRotationOffset;
    //public Vector3 m_lookOffset;

    [Header("--- (Position) ---")]
    public Vector3 m_positionOffset;
    public float m_defaultT;
    //public LayerMask m_layerMask;

    [Header("--- (Scroll) ---")]
    public float m_unitsPerScroll;
    public int m_minCamScroll;
    public int m_maxCamScroll;

    [Header("--- (Sensivity) ---")]
    public Vector3 m_sensivityMouseVector;
    public Vector3 m_sensivityJoypadVector;

    [Header("------- Debug -------")]
    //public float m_currentAngleX;
    //public float m_currentAngleY;
    public Vector3 m_currentAngleVector;
    public float m_currentScroll;
    public bool m_isTargetLocked;
    public bool m_m_toggleJoypad;
    //public Vector3 m_lookAtWorldPositionFinal;

    public Vector3 m_lastFocusPosition;
    public Vector3 m_currentFocusPosition;
    public Vector3 m_finalFollowPosition;

    public Vector3 m_lastVelocity;
    public float m_t;

    public Rigidbody m_focusRb;

    void Start()
    {
        m_currentAngleVector.x = 15;
        m_focus = gameObject;
        m_focusRb = m_focus.GetComponent<Rigidbody>();
        m_t = m_defaultT;
    }

    void FixedUpdate()
    {
        if (true)//!GameObject.Find("GeneralScriptObject").GetComponent<Options>().isFreeze)
        {
            bool isPressingUp = Input.GetAxis("ButtonUp") > 0.5f;
            bool isPressingDown = Input.GetAxis("ButtonDown") < -0.5f;
            if (isPressingUp || isPressingDown)
            {
                if (!m_m_toggleJoypad)
                {
                    if (isPressingUp)
                    {
                        m_sensivityJoypadVector.x *= 1.1f;
                        m_sensivityJoypadVector.y *= 1.1f;
                    }
                    else if (isPressingDown)
                    {
                        m_sensivityJoypadVector.x /= 1.1f;
                        m_sensivityJoypadVector.y /= 1.1f;
                    }
                    m_m_toggleJoypad = true;
                }
            }
            else
            {
                m_m_toggleJoypad = false;
            }

            if (Input.GetAxis("RightStickHorizontal") > 0.2f || Input.GetAxis("RightStickHorizontal") < -0.2f)
                m_currentAngleVector.y += Input.GetAxis("RightStickHorizontal") * m_sensivityJoypadVector.y;
            if (Input.GetAxis("RightStickVertical") > 0.2f || Input.GetAxis("RightStickVertical") < -0.2f)
                m_currentAngleVector.x += Input.GetAxis("RightStickVertical") * m_sensivityJoypadVector.x;

            m_currentAngleVector.x = Mathf.Clamp(m_currentAngleVector.x, -m_maxCamAngle, m_maxCamAngle);
        }


        //float speedDifference = (m_lastVelocity - m_focusRb.velocity).magnitude;
        //float t = 1 - ((speedDifference == 0 ? 0.01f : speedDifference) * 70 * Time.fixedDeltaTime);

        //float t2 = Mathf.Lerp(m_t, t, 0.05f);
        //t2 = Mathf.Clamp(t2, 0.1f, 1f);
        //Debug.Log(t);
        //m_t = t2;

        //m_lastVelocity = m_focusRb.velocity;


        Vector3 wantPosition = m_focus.transform.position;// + finalCameraRotationQuaternion * m_positionOffset;

        m_finalFollowPosition = Vector3.Lerp(m_lastFocusPosition, wantPosition, m_t);
        m_lastFocusPosition = m_finalFollowPosition;
    }

    void Update()
    {
        if (true)//!GameObject.Find("GeneralScriptObject").GetComponent<Options>().isFreeze)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                m_sensivityMouseVector.x *= 1.1f;
                m_sensivityMouseVector.y *= 1.1f;
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                m_sensivityMouseVector.x /= 1.1f;
                m_sensivityMouseVector.y /= 1.1f;
            }

            float inputX = Input.GetAxis("Mouse Y") * m_sensivityMouseVector.y;
            float inputY = Input.GetAxis("Mouse X") * m_sensivityMouseVector.x;
            m_currentAngleVector.x -= inputX;// * Time.deltaTime;
            m_currentAngleVector.y += inputY;// * Time.deltaTime;

            if (Input.GetKey(KeyCode.E))
                ;// m_currentAngleVector.z -= m_sensivityMouseVector.z;
            if (Input.GetKey(KeyCode.Q))
                ;// m_currentAngleVector.z += m_sensivityMouseVector.z;

            if (Input.mouseScrollDelta != Vector2.zero)
            {
                m_currentScroll -= Input.mouseScrollDelta[1] * m_currentScroll * 0.1f;
            }
            m_currentScroll = Mathf.Clamp(m_currentScroll, m_minCamScroll, m_maxCamScroll);

            m_currentAngleVector.x = Mathf.Clamp(m_currentAngleVector.x, -m_maxCamAngle, m_maxCamAngle);
        }

    }




    void LateUpdate()
    {
        m_distance = m_currentScroll * m_unitsPerScroll;


        Quaternion tempQuaterion = m_gyrometer.transform.rotation * Quaternion.Euler(m_currentAngleVector) * Quaternion.Euler(m_customRotation);
        Quaternion finalCameraRotationQuaternion = tempQuaterion;

        Vector3 finalCameraPosition = m_finalFollowPosition + finalCameraRotationQuaternion * (new Vector3(0, 0, -m_distance) + m_positionOffset);


        m_playerCamera.transform.rotation = finalCameraRotationQuaternion;
        m_playerCamera.transform.position = finalCameraPosition;



        //Quaternion cameraRotation =  Quaternion.Euler(m_currentAngleVector.y, m_currentAngleVector.x, transform.rotation.eulerAngles.z);
        //Vector3 direction = cameraRotation * new Vector3(0, 0, -m_distance);


        //Quaternion cameraRotationYBodyRotationXZ = Quaternion.Euler(transform.rotation.eulerAngles.x, m_playerCamera.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        //Vector3 positionOffsetWorldActual = m_positionOffset;
        //Vector3 positionOffsetLocalActual = cameraRotationYBodyRotationXZ * m_positionOffset;
        //Vector3 lookAtOffsetWorldActual = m_lookOffset + m_positionOffset * (transform.position - m_playerCamera.transform.position).magnitude / m_distance;
        //Vector3 lookAtOffsetLocalActual = cameraRotationYBodyRotationXZ * (m_lookOffset + m_positionOffset * (transform.position - m_playerCamera.transform.position).magnitude / m_distance);

        //Vector3 cameraPositionFinal = transform.position + cameraRotation * m_positionOffset + direction;
        //Vector3 lookAtWorldPositionFinal = transform.position + cameraRotationYBodyRotationXZ * lookAtOffsetWorldActual;
        ////m_lookAtWorldPositionFinal = lookAtWorldPositionFinal;

        ////m_playerCamera.transform.position = cameraPositionFinal;

        ////m_playerCamera.transform.LookAt(lookAtWorldPositionFinal);
        //Quaternion lookRotationOffsetQuaternion = Quaternion.Euler(m_lookRotationOffset);
        //m_playerCamera.transform.rotation = m_playerCamera.transform.rotation * lookRotationOffsetQuaternion;

        /*
        cameraRotation = Quaternion.Euler(m_currentAngleY, m_currentAngleX, 0);

        m_distance = m_currentScroll * m_unitsPerScroll;
        Vector3 direction = cameraRotation * new Vector3(0, 0, -m_distance);
        Quaternion cameraRotationYBodyRotationXZ = Quaternion.Euler(transform.rotation.eulerAngles.x, playerCamera.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        Vector3 positionOffsetWorldActual = m_positionOffset;
        Vector3 positionOffsetLocalActual = cameraRotationYBodyRotationXZ * m_positionOffset;
        Vector3 lookAtOffsetWorldActual = m_lookOffset + m_positionOffset * (transform.position - playerCamera.transform.position).magnitude / m_distance;
        Vector3 lookAtOffsetLocalActual = cameraRotationYBodyRotationXZ * (m_lookOffset + m_positionOffset * (transform.position - playerCamera.transform.position).magnitude / m_distance);

        Vector3 cameraPositionFinal = transform.position + transform.rotation * positionOffsetWorldActual + direction;
        Vector3 lookAtWorldPositionFinal = transform.position + cameraRotationYBodyRotationXZ * lookAtOffsetWorldActual;
        m_lookAtWorldPositionFinal = lookAtWorldPositionFinal;

        playerCamera.transform.position = cameraPositionFinal;
        playerCamera.transform.LookAt(lookAtWorldPositionFinal);
        */
        //Debug.DrawLine(transform.position, cameraPositionFinal, Color.cyan);
        //Debug.DrawLine(transform.position, lookAtWorldPositionFinal, Color.green);
        //Debug.DrawRay(transform.position, positionOffsetLocalActual, Color.red);
    }
}
