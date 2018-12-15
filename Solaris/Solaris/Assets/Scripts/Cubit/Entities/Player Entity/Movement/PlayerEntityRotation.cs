using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntityRotation : MonoBehaviour
{
    public Camera m_playerCamera;
    public GameObject m_visualBody;
    public GameObject m_visualWingLeft;
    public GameObject m_visualWingRight;
    public GameObject m_gyrometer;

    [Header("----- SETTINGS -----")]
    public bool m_lerpRotation;
    public float m_rotateBodyBy;
    [Header("--- (Axis) ---")]
    public Vector3 m_maxRotationAxisVector;
    public float m_maxRotationWingsUpDown;
    public Vector3 m_rotationSpeedVector;

    [Header("--- (X/Z) ---")]
    //public float m_maxRotationXAxis;
    //public float m_maxRotationZAxis;
    //public float m_rotationSpeedXZ;
    [Header("--- (Y) ---")]
    public bool m_rotateYWings;
    //public float m_maxRotationY;
    //public float m_rotationSpeedY;

    [Header("----- DEBUG -----")]
    [Header("--- (Actual Stuff) ---")]
    //public float m_maxRotationXAxisActual;
    //public float m_maxRotationZAxisActual;
    //public float m_rotationSpeedXZActual;
    //public float m_rotationSpeedYActual;
    //public float m_rotationActualXAxis;
    //public float m_rotationActualZAxis;

    public Vector3 m_maxRotationAxisActualVector;
    public Vector3 m_rotationSpeedActualVector;
    public Vector3 m_rotationActualAxisVector;

    [Header("--- (Rotation) ---")]
    //public float m_wantRotationXAxis;
    //public float m_wantRotationYAxis;
    //public float m_wantRotationZAxis;
    public float m_yRotationDifference;

    public Vector3 m_wantRotationAxisVector;

    [Header("--- (Buttons) ---")]
    public bool m_isPressingButtonForward;
    public bool m_isPressingButtonBackward;
    public bool m_isPressingButtonLeft;
    public bool m_isPressingButtonRight;
    public bool m_isPressingButtonUp;
    public bool m_isPressingButtonDown;
    public bool m_isPressingAnyButtonXZ;

    [Header("--- (Misc) ---")]
    public bool m_isBoosting;

    [Header("--- (Quaternions) ---")]
    public Quaternion m_defaultRotationBody;
    public Quaternion m_defaultRotationWingLeft;
    public Quaternion m_defaultRotationWingRight;
    public Quaternion m_defaultRotationGyrometer;

    public Quaternion m_wantQuaternionGameObject;
    public Quaternion m_wantQuaternionBody;
    public Quaternion m_wantQuaternionWingLeft;
    public Quaternion m_wantQuaternionWingRight;
    public Quaternion m_wantQuaternionGyrometer;

    public Quaternion m_lerpedQuaternionGameObject;
    public Quaternion m_lerpedQuaternionBody;
    public Quaternion m_lerpedQuaternionWingLeft;
    public Quaternion m_lerpedQuaternionWingRight;
    public Quaternion m_lerpedQuaternionGyrometer;

    public Quaternion m_finalQuaternionGameObject;
    public Quaternion m_finalQuaternionBody;
    public Quaternion m_finalQuaternionWingLeft;
    public Quaternion m_finalQuaternionWingRight;
    public Quaternion m_finalQuaternionGyrometer;


    public PlayerEntityCameraRotation m_cameraRotationScript;
    void Start()
    {
        m_defaultRotationBody = m_visualBody.transform.rotation;
        m_defaultRotationWingLeft = m_visualWingLeft.transform.rotation;
        m_defaultRotationWingRight = m_visualWingRight.transform.rotation;
        m_defaultRotationGyrometer = m_gyrometer.transform.rotation;

        if (m_cameraRotationScript == null)
            m_cameraRotationScript = GetComponent<PlayerEntityCameraRotation>();
    }

    void FixedUpdate()
    {
        updateInformation();
        getInput();
        calculateWantRotation();
        calculateLerpedRotation();
        applyRotation();
    }

    void updateInformation()
    {
        //m_maxRotationXAxisActual = m_maxRotationXAxis;
        //m_maxRotationZAxisActual = m_maxRotationZAxis;
        m_maxRotationAxisActualVector = m_maxRotationAxisVector;
        //m_rotationSpeedXZActual = m_rotationSpeedXZ;
        //m_rotationSpeedYActual = m_rotationSpeedY;
        m_rotationSpeedActualVector = m_rotationSpeedVector;
    }

    void getInput()
    {
        m_isPressingButtonForward = false;
        m_isPressingButtonBackward = false;
        m_isPressingButtonLeft = false;
        m_isPressingButtonRight = false;
        m_isPressingButtonUp = false;
        m_isPressingButtonDown = false;
        m_isPressingAnyButtonXZ = false;


        if (Input.GetKey(KeyCode.W) || Input.GetAxis("LeftStickVertical") < 0)
        {
            m_isPressingButtonForward = true;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetAxis("LeftStickVertical") > 0)
        {
            m_isPressingButtonBackward = true;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetAxis("LeftStickHorizontal") < 0)
        {
            m_isPressingButtonLeft = true;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetAxis("LeftStickHorizontal") > 0)
        {
            m_isPressingButtonRight = true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            m_isPressingButtonUp = true;
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            m_isPressingButtonDown = true;
        }

        m_isPressingAnyButtonXZ = m_isPressingButtonBackward || m_isPressingButtonForward || m_isPressingButtonLeft || m_isPressingButtonRight;
    }

    void calculateWantRotation()
    {
        m_wantQuaternionBody = m_defaultRotationBody;
        m_wantQuaternionWingLeft = m_defaultRotationWingLeft;
        m_wantQuaternionWingRight = m_defaultRotationWingRight;
        m_wantQuaternionGyrometer = m_defaultRotationGyrometer;

        //m_wantRotationXAxis = 0;// m_playerCamera.transform.rotation.eulerAngles.x;
        //m_wantRotationYAxis = m_playerCamera.transform.rotation.eulerAngles.y;
        //m_wantRotationZAxis = 0;
        float angle = m_playerCamera.transform.rotation.eulerAngles.y;

        m_wantRotationAxisVector = new Vector3(0, angle, 0);

        if (m_isPressingButtonForward)
            m_wantRotationAxisVector.x += m_maxRotationAxisActualVector.x;
        if (m_isPressingButtonBackward)
            m_wantRotationAxisVector.x -= m_maxRotationAxisActualVector.x;
        if (m_isPressingButtonRight)
            m_wantRotationAxisVector.z -= m_maxRotationAxisActualVector.z;
        if (m_isPressingButtonLeft)
            m_wantRotationAxisVector.z += m_maxRotationAxisActualVector.z;
        float m_wingRotationUpDown = 0;
        if (m_isPressingButtonUp)
            m_wingRotationUpDown += m_maxRotationWingsUpDown;
        if (m_isPressingButtonDown)
            m_wingRotationUpDown -= m_maxRotationWingsUpDown;

        if (m_rotateYWings)
        {
            m_yRotationDifference = m_wantRotationAxisVector.y - transform.rotation.eulerAngles.y;
            if (m_yRotationDifference > 180)
                m_yRotationDifference -= 360;
            if (m_yRotationDifference < -180)
                m_yRotationDifference += 360;

            m_yRotationDifference = Mathf.Clamp(m_yRotationDifference, -m_maxRotationAxisVector.y, m_maxRotationAxisVector.y);
        }
        else
            m_yRotationDifference = 0;

        //Debug.Log(m_wantRotationAxisVector);

        //m_gyrometer.transform.position = transform.position;

        m_wantQuaternionGameObject =    m_gyrometer.transform.rotation *  Quaternion.Euler(0, m_cameraRotationScript.m_currentAngleVector.y, 0);
        //m_playerCamera.transform.rotation = Quaternion.Euler(0, -m_wantRotationAxisVector.y, 0) * m_playerCamera.transform.rotation;
        m_wantQuaternionBody =          m_wantQuaternionGameObject * m_defaultRotationBody * Quaternion.Euler(/*m_wantRotationAxisVector.x + */m_cameraRotationScript.m_currentAngleVector.x - 25, 0, m_rotateBodyBy * m_wantRotationAxisVector.z); // * Quaternion.Euler(m_wantRotationAxisVector);//Quaternion.Euler(m_defaultRotationBody.eulerAngles.x + m_wantRotationAxisVector.x,                                  m_wantRotationAxisVector.y,         m_defaultRotationBody.eulerAngles.z + m_wantRotationAxisVector.z + transform.rotation.eulerAngles.z);
        m_wantQuaternionWingLeft =      m_wantQuaternionGameObject * m_defaultRotationWingLeft * Quaternion.Euler(m_wantRotationAxisVector.x, 0, m_wantRotationAxisVector.z + m_wingRotationUpDown) * Quaternion.Euler(m_yRotationDifference, 0, 0);// * Quaternion.Euler(m_wingRotationUpDown, 0, 0);// Quaternion.Euler(m_defaultRotationWingLeft.eulerAngles.x + m_wantRotationAxisVector.x + m_yRotationDifference,      m_wantRotationAxisVector.y,         m_defaultRotationWingLeft.eulerAngles.z + m_wantRotationAxisVector.z + transform.rotation.eulerAngles.z);
        m_wantQuaternionWingRight = m_wantQuaternionGameObject * m_defaultRotationWingRight * Quaternion.Euler(m_wantRotationAxisVector.x, 0, -m_wantRotationAxisVector.z + m_wingRotationUpDown) * Quaternion.Euler(-m_yRotationDifference, 0, 0);// * Quaternion.Euler(m_wingRotationUpDown, 0, 0);// Quaternion.Euler(m_defaultRotationWingRight.eulerAngles.x - m_wantRotationAxisVector.x + m_yRotationDifference,     m_wantRotationAxisVector.y + 180f,  m_defaultRotationWingRight.eulerAngles.z - m_wantRotationAxisVector.z - transform.rotation.eulerAngles.z);
        //m_wantQuaternionGyrometer =     Quaternion.Euler(m_defaultRotationGyrometer.eulerAngles.x + m_wantRotationAxisVector.x,                             m_wantRotationAxisVector.y,         m_defaultRotationGyrometer.eulerAngles.z + m_wantRotationAxisVector.z + transform.rotation.eulerAngles.z);
        /*
        if (Input.GetKey(KeyCode.Space))
        {
            m_wantQuaternionWingLeft = Quaternion.Euler(m_wantQuaternionWingLeft.x, m_wantQuaternionWingLeft.y, m_wantQuaternionWingLeft.z + m_maxRotationZAxisActual);
            m_wantQuaternionWingRight = Quaternion.Euler(m_wantQuaternionWingRight.x, m_wantQuaternionWingRight.y, m_wantQuaternionWingRight.z + m_maxRotationZAxisActual);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_wantQuaternionWingLeft = Quaternion.Euler(m_wantQuaternionWingLeft.x, m_wantQuaternionWingLeft.y, m_wantQuaternionWingLeft.z + m_maxRotationZAxisActual);
            m_wantQuaternionWingRight = Quaternion.Euler(m_wantQuaternionWingRight.x, m_wantQuaternionWingRight.y, m_wantQuaternionWingRight.z + m_maxRotationZAxisActual);
        }*/
    }

    void calculateLerpedRotation()
    {
        //Debug.Log(m_wantQuaternionGameObject.eulerAngles);
        m_lerpedQuaternionGameObject = Quaternion.Lerp(transform.rotation, m_wantQuaternionGameObject, m_rotationSpeedActualVector.y * Time.deltaTime);
        m_lerpedQuaternionBody = Quaternion.Lerp(m_visualBody.transform.rotation, m_wantQuaternionBody, (m_rotationSpeedActualVector.x + m_rotationSpeedActualVector.z) * Time.deltaTime);
        m_lerpedQuaternionWingLeft = Quaternion.Lerp(m_visualWingLeft.transform.rotation, m_wantQuaternionWingLeft, (m_rotationSpeedActualVector.x + m_rotationSpeedActualVector.z) * Time.deltaTime);
        m_lerpedQuaternionWingRight = Quaternion.Lerp(m_visualWingRight.transform.rotation, m_wantQuaternionWingRight, (m_rotationSpeedActualVector.x + m_rotationSpeedActualVector.z) * Time.deltaTime);
        //m_lerpedQuaternionGyrometer = Quaternion.Lerp(m_gyrometer.transform.rotation, m_wantQuaternionGyrometer, (m_rotationSpeedActualVector.x + m_rotationSpeedActualVector.z) * Time.deltaTime);

        m_rotationActualAxisVector.x = m_lerpedQuaternionGyrometer.eulerAngles.x;
        if (m_rotationActualAxisVector.x > 180)
            m_rotationActualAxisVector.x -= 360;
        m_rotationActualAxisVector.z = m_lerpedQuaternionGyrometer.eulerAngles.z;
        if (m_rotationActualAxisVector.z > 180)
            m_rotationActualAxisVector.z -= 360;
    }

    void applyRotation()
    {
        if (m_lerpRotation)
        {
            m_finalQuaternionGameObject = m_lerpedQuaternionGameObject;
            m_finalQuaternionBody = m_lerpedQuaternionBody;
            m_finalQuaternionWingLeft = m_lerpedQuaternionWingLeft;
            m_finalQuaternionWingRight = m_lerpedQuaternionWingRight;
            //m_finalQuaternionGyrometer = m_lerpedQuaternionGyrometer;
        }
        else
        {
            m_finalQuaternionGameObject = m_wantQuaternionGameObject;
            m_finalQuaternionBody = m_wantQuaternionBody;
            m_finalQuaternionWingLeft = m_wantQuaternionWingLeft;
            m_finalQuaternionWingRight = m_wantQuaternionWingRight;
            //m_finalQuaternionGyrometer = m_wantQuaternionGyrometer;
        }

        transform.rotation = m_finalQuaternionGameObject;
        
        m_visualBody.transform.rotation = m_finalQuaternionBody;
        m_visualWingLeft.transform.rotation = m_finalQuaternionWingLeft;
        m_visualWingRight.transform.rotation = m_finalQuaternionWingRight;
        //m_gyrometer.transform.rotation = m_finalQuaternionGyrometer;
        
    }
}
