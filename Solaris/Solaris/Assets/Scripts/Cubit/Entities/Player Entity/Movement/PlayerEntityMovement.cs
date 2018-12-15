using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntityMovement : MonoBehaviour
{
    //public GameObject m_visualBody;
    [Header("----- SETTINGS -----")]

    [Header("--- (Cubes) ---")]
    public bool m_affectCubes;
    public float m_affectFactor;

    [Header("--- (Movement) ---")]
    public bool m_movePlaneLike;
    public float m_maxSpeed;
    public Vector3 m_accelerationValues;
    public float m_airRestistance;
    //public float m_maxSpeedOnBoost;
    /*
    [Header("--- (X) ---")]
    [Space]
    public float m_accelerationPerGradX;
    public float m_airRestianceXOnButton;
    public float m_airRestianceXNoButton;
    public float m_airRestianceXMinSpeedOnButton;
    public float m_airRestianceXMinSpeedNoButton;
    public float m_airRestianceXExponentOnButton;
    public float m_airRestianceXExponentNoButton;
    [Header("--- (Y) ---")]
    public float m_accelerationUpDown;
    public float m_airRestianceYOnButton;
    public float m_airRestianceYNoButton;
    public float m_airRestianceYMinSpeedOnButton;
    public float m_airRestianceYMinSpeedNoButton;
    public float m_airRestianceYExponentOnButton;
    public float m_airRestianceYExponentNoButton;
    [Header("--- (Z) ---")]
    public float m_accelerationPerGradZ;
    public float m_airRestianceZOnButton;
    public float m_airRestianceZNoButton;
    public float m_airRestianceZMinSpeedOnButton;
    public float m_airRestianceZMinSpeedNoButton;
    public float m_airRestianceZExponentOnButton;
    public float m_airRestianceZExponentNoButton;
    */
    [Header("----- DEBUG -----")]
    [Header("--- (Movement) ---")]
    public Vector3 m_forceMovementVectorTotal;
    public Vector3 m_airRestianceVectorTotal;
    public Vector3 m_currentMoveDirectionWorld;
    public Vector3 m_currentMoveDirectionLocal;
    public float m_movementSpeed;

    [Header("--- (X) ---")]
    public Vector3 m_forceMovementVectorX;
    public Vector3 m_airRestianceVectorX;

    [Header("--- (Y) ---")]
    public Vector3 m_forceMovementVectorY;
    public Vector3 m_airRestianceVectorY;

    [Header("--- (Z) ---")]
    public Vector3 m_forceMovementVectorZ;
    public Vector3 m_airRestianceVectorZ;


    [Header("--- (Buttons) ---")]
    public bool m_isPressingButtonUp;
    public bool m_isPressingButtonDown;
    public bool m_isPressingAnyButton;
    public bool m_isPressingButtonForward;
    public bool m_isPressingButtonBackward;
    public bool m_isPressingButtonLeft;
    public bool m_isPressingButtonRight;
    public bool m_isPressingAnyButtonXZ;

    //private PlayerRotation m_playerRotationScript;
    private Rigidbody m_rb;

    void Start()
    {
        //m_playerRotationScript = GetComponent<PlayerRotation>();
        m_rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        updateInformation();
        getInput();
        calculateForceVector();
        calculateAirRestianceVector();
        applyForceVector();
    }

    void updateInformation()
    {
        m_movementSpeed = m_rb.velocity.magnitude;
        m_currentMoveDirectionWorld = m_rb.velocity;
        if (m_currentMoveDirectionWorld.x < 0.02 && m_currentMoveDirectionWorld.x > -0.02)
            m_currentMoveDirectionWorld.x = 0;
        if (m_currentMoveDirectionWorld.y < 0.02 && m_currentMoveDirectionWorld.y > -0.02)
            m_currentMoveDirectionWorld.y = 0;
        if (m_currentMoveDirectionWorld.z < 0.02 && m_currentMoveDirectionWorld.z > -0.02)
            m_currentMoveDirectionWorld.z = 0;
        m_rb.velocity = m_currentMoveDirectionWorld;
        m_currentMoveDirectionLocal = transform.InverseTransformDirection(m_rb.velocity);
        if (m_currentMoveDirectionLocal.x < 0.02 && m_currentMoveDirectionLocal.x > -0.02)
            m_currentMoveDirectionLocal.x = 0;
        if (m_currentMoveDirectionLocal.y < 0.02 && m_currentMoveDirectionLocal.y > -0.02)
            m_currentMoveDirectionLocal.y = 0;
        if (m_currentMoveDirectionLocal.z < 0.02 && m_currentMoveDirectionLocal.z > -0.02)
            m_currentMoveDirectionLocal.z = 0;
    }

    void getInput()
    {
        m_isPressingButtonUp = false;
        m_isPressingButtonDown = false;
        m_isPressingButtonForward = false;
        m_isPressingButtonBackward = false;
        m_isPressingButtonLeft = false;
        m_isPressingButtonRight = false;
        m_isPressingAnyButtonXZ = false;
        m_isPressingAnyButton = false;


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
        if (Input.GetKey(KeyCode.Space) || Input.GetAxis("RTrigger") < -0.3f)
        {
            m_isPressingButtonUp = true;
        }
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift) || Input.GetAxis("LTrigger") > 0)
        {
            m_isPressingButtonDown = true;
        }


        m_isPressingAnyButtonXZ = m_isPressingButtonBackward || m_isPressingButtonForward || m_isPressingButtonLeft || m_isPressingButtonRight;
        m_isPressingAnyButton = m_isPressingAnyButtonXZ || m_isPressingButtonDown || m_isPressingButtonUp;
    }

    void calculateForceVector()
    {
        m_forceMovementVectorX = Vector3.zero;
        m_forceMovementVectorY = Vector3.zero;
        m_forceMovementVectorZ = Vector3.zero;


        // X
        if (m_isPressingButtonLeft)
            m_forceMovementVectorX += new Vector3(-m_accelerationValues.x, 0, 0);
        if (m_isPressingButtonRight)
            m_forceMovementVectorX += new Vector3(m_accelerationValues.x, 0, 0);

        // Z
        if (m_isPressingButtonForward)
        {
            if (m_movePlaneLike)
                m_forceMovementVectorZ += new Vector3(0, 0, m_accelerationValues.z);
            else
                m_forceMovementVectorZ += Camera.main.transform.forward * m_accelerationValues.z;
        }
        if (m_isPressingButtonBackward)
        {
            if(m_movePlaneLike)
                m_forceMovementVectorZ += new Vector3(0, 0, -m_accelerationValues.z);
            else
                m_forceMovementVectorZ += Camera.main.transform.forward * -m_accelerationValues.z;
        }

        // Y
        if (m_isPressingButtonUp)
            m_forceMovementVectorY += new Vector3(0, m_accelerationValues.y, 0);
        if (m_isPressingButtonDown)
            m_forceMovementVectorY += new Vector3(0, -m_accelerationValues.y, 0);

        /*
        forceMovementVectorX = new Vector3(-playerRotationScript.rotationActualZAxis * accelerationPerGradX, 0, 0);
        if (m_movePlaneLike)
            forceMovementVectorZ = new Vector3(0, 0, playerRotationScript.rotationActualXAxis * accelerationPerGradZ);
        else
            forceMovementVectorZ = Camera.main.transform.forward * accelerationPerGradZ * playerRotationScript.rotationActualXAxis;

        forceMovementVectorY = Vector3.zero;
        if (isPressingButtonUp)
            forceMovementVectorY += new Vector3(0, accelerationUpDown, 0);
        if (isPressingButtonDown)
            forceMovementVectorY -= new Vector3(0, accelerationUpDown, 0);
            */
    }

    void calculateAirRestianceVector()
    {


        /*
        if ((m_isPressingButtonLeft && m_currentMoveDirectionLocal.x < 0) || m_isPressingButtonRight && m_currentMoveDirectionLocal.x > 0)
        {
            m_airRestianceVectorX = (m_currentMoveDirectionLocal.x <= 0 ? 1 : -1) * new Vector3(m_airRestianceXOnButton * Mathf.Pow(Mathf.Max(Mathf.Abs(m_currentMoveDirectionLocal.x), m_airRestianceXMinSpeedOnButton), m_airRestianceXExponentOnButton), 0, 0);
        }
        else
        {
            if (m_currentMoveDirectionLocal.x > 0.02f || m_currentMoveDirectionLocal.x < -0.02f)
                m_airRestianceVectorX = (m_currentMoveDirectionLocal.x <= 0 ? 1 : -1) * new Vector3(m_airRestianceXNoButton * Mathf.Pow(Mathf.Max(Mathf.Abs(m_currentMoveDirectionLocal.x), m_airRestianceXMinSpeedNoButton), m_airRestianceXExponentNoButton), 0, 0);
            else
                m_airRestianceVectorX = Vector3.zero;
        }

        if ((m_isPressingButtonUp && m_currentMoveDirectionLocal.y > 0) || (m_isPressingButtonDown && m_currentMoveDirectionLocal.y < 0))
        {
            m_airRestianceVectorY = (m_currentMoveDirectionLocal.y <= 0 ? 1 : -1) * new Vector3(0, m_airRestianceYOnButton * Mathf.Pow(Mathf.Max(Mathf.Abs(m_currentMoveDirectionLocal.y), m_airRestianceYMinSpeedOnButton), m_airRestianceYExponentOnButton), 0);
        }
        else
        {
            if (m_currentMoveDirectionLocal.y > 0.02f || m_currentMoveDirectionLocal.y < -0.02f)
                m_airRestianceVectorY = (m_currentMoveDirectionLocal.y <= 0 ? 1 : -1) * new Vector3(0, m_airRestianceYNoButton * Mathf.Pow(Mathf.Max(Mathf.Abs(m_currentMoveDirectionLocal.y), m_airRestianceYMinSpeedNoButton), m_airRestianceYExponentNoButton), 0);
            else
                m_airRestianceVectorY = Vector3.zero;
        }

        if ((m_isPressingButtonForward && m_currentMoveDirectionLocal.z > 0) || (m_isPressingButtonBackward && m_currentMoveDirectionLocal.z < 0))
        {
            m_airRestianceVectorZ = (m_currentMoveDirectionLocal.z <= 0 ? 1 : -1) * new Vector3(0, 0, m_airRestianceZOnButton * Mathf.Pow(Mathf.Max(Mathf.Abs(m_currentMoveDirectionLocal.z), m_airRestianceZMinSpeedOnButton), m_airRestianceZExponentOnButton));
        }
        else
        {
            if (m_currentMoveDirectionLocal.z > 0.02f || m_currentMoveDirectionLocal.z < -0.02f)
                m_airRestianceVectorZ = (m_currentMoveDirectionLocal.z <= 0 ? 1 : -1) * new Vector3(0, 0, m_airRestianceZNoButton * Mathf.Pow(Mathf.Max(Mathf.Abs(m_currentMoveDirectionLocal.z), m_airRestianceZMinSpeedNoButton), m_airRestianceZExponentNoButton));
            else
                m_airRestianceVectorZ = Vector3.zero;
        }
        */
    }

    void applyForceVector()
    {
        if (m_movePlaneLike)
            m_forceMovementVectorTotal =  transform.rotation * (m_forceMovementVectorX + m_forceMovementVectorZ + m_forceMovementVectorY);
        else
            m_forceMovementVectorTotal = m_forceMovementVectorZ + transform.rotation * (m_forceMovementVectorX + m_forceMovementVectorY);
        m_rb.AddForce(m_forceMovementVectorTotal, ForceMode.Acceleration);
        //Debug.DrawRay(transform.position + Vector3.up * 2f, forceMovementVectorTotal, Color.green);

        m_airRestianceVectorTotal = m_rb.velocity.magnitude > 0 ? -m_rb.velocity.normalized * m_airRestistance : Vector3.zero;
        //m_airRestianceVectorTotal = transform.TransformDirection(m_airRestianceVectorX + m_airRestianceVectorY + m_airRestianceVectorZ);
        m_rb.AddForce(m_airRestianceVectorTotal, ForceMode.Acceleration);
        //Debug.DrawRay(transform.position + Vector3.up * 2f, airRestianceVectorTotal, Color.blue);
        if (m_rb.velocity.magnitude <= 0.5f)
            m_rb.velocity = Vector3.zero;

        m_rb.velocity = m_rb.velocity.normalized * Mathf.Clamp(m_rb.velocity.magnitude, 0, m_maxSpeed);
        //Debug.DrawRay(transform.position + Vector3.up * 2f, m_forceMovementVectorTotal, Color.yellow);








        // Affect GrabSystem cubes aswell
        //if (m_affectCubes && GetComponent<GrabSystem>() != null)
        //{
        //    foreach (GameObject cube in GetComponent<GrabSystem>().grabbedCubes)
        //    {
        //        cube.GetComponent<Rigidbody>().AddForce((m_forceMovementVectorTotal + m_airRestianceVectorTotal) * m_affectFactor, ForceMode.Acceleration);
        //    }
        //}

        // Affect CubeEntityPlayerGrabSystem aswell
        AttachSystemBase[] attachSystems = GetComponentsInChildren<AttachSystemBase>();
        foreach (AttachSystemBase script in attachSystems)
        {
            if (script.m_movementAffectsCubesFactor != 0)
            {
                foreach (GameObject cube in script.m_cubeList)
                {
                    cube.GetComponent<Rigidbody>().AddForce((m_forceMovementVectorTotal + m_airRestianceVectorTotal) * script.m_movementAffectsCubesFactor, ForceMode.Acceleration);
                }
            }
        }

        //CemBoidBase boidScript = Constants.getBoidSystem();
        //if (boidScript != null)
        //{
        //    if (boidScript.m_affectedByplayerMovementPower > 0)
        //    {
        //        foreach (GameObject agent in boidScript.m_agents)
        //        {
        //            agent.GetComponent<Rigidbody>().AddForce((m_forceMovementVectorTotal + m_airRestianceVectorTotal) * boidScript.m_affectedByplayerMovementPower, ForceMode.Acceleration);
        //        }
        //    }
        //}

        /* OLD
        // Affect CGEBoidSystem aswell
        CEMBoidSystem boidScript = Constants.getBoidSystem();
        if(boidScript.m_useIsAffectedByPlayerMovement)
        {
            foreach(GameObject agent in boidScript.m_agents)
            {
                agent.GetComponent<Rigidbody>().AddForce((forceMovementVectorTotal + airRestianceVectorTotal) , ForceMode.Acceleration);
            }
        }
        */
    }
}
