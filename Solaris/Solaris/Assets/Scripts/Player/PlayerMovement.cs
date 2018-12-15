using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject visualBody;
    [Header("----- SETTINGS -----")]

    [Header("--- (Cubes) ---")]
    public bool affectCubes;
    public float affectFactor;

    [Header("--- (Movement) ---")]
    public bool m_movePlaneLike;
    public float maxSpeed;
    public float maxSpeedOnBoost;
    [Header("--- (X) ---")]
    public float accelerationPerGradX;
    public float airRestianceXOnButton;
    public float airRestianceXNoButton;
    public float airRestianceXMinSpeedOnButton;
    public float airRestianceXMinSpeedNoButton;
    public float airRestianceXExponentOnButton;
    public float airRestianceXExponentNoButton;
    [Header("--- (Y) ---")]
    public float accelerationUpDown;
    public float airRestianceYOnButton;
    public float airRestianceYNoButton;
    public float airRestianceYMinSpeedOnButton;
    public float airRestianceYMinSpeedNoButton;
    public float airRestianceYExponentOnButton;
    public float airRestianceYExponentNoButton;
    [Header("--- (Z) ---")]
    public float accelerationPerGradZ;
    public float airRestianceZOnButton;
    public float airRestianceZNoButton;
    public float airRestianceZMinSpeedOnButton;
    public float airRestianceZMinSpeedNoButton;
    public float airRestianceZExponentOnButton;
    public float airRestianceZExponentNoButton;

    [Header("----- DEBUG -----")]
    [Header("--- (Movement) ---")]
    public Vector3 forceMovementVectorTotal;
    public Vector3 airRestianceVectorTotal;
    public Vector3 currentMoveDirectionWorld;
    public Vector3 currentMoveDirectionLocal;
    public float movementSpeed;

    [Header("--- (X) ---")]
    public Vector3 forceMovementVectorX;
    public Vector3 airRestianceVectorX;

    [Header("--- (Y) ---")]
    public Vector3 forceMovementVectorY;
    public Vector3 airRestianceVectorY;

    [Header("--- (Z) ---")]
    public Vector3 forceMovementVectorZ;
    public Vector3 airRestianceVectorZ;

    [Header("--- (Buttons) ---")]
    public bool isPressingButtonUp;
    public bool isPressingButtonDown;
    public bool isPressingAnyButton;

    private PlayerRotation playerRotationScript;
    private Rigidbody rb;

    void Start ()
    {
        playerRotationScript = GetComponent<PlayerRotation>();
        rb = GetComponent<Rigidbody>();
    }
	
	void FixedUpdate ()
    {
        updateInformation();
        getInput();
        calculateForceVector();
        calculateAirRestianceVector();
        applyForceVector();
    }

    void updateInformation()
    {
        movementSpeed = rb.velocity.magnitude;
        currentMoveDirectionWorld = rb.velocity;
        if (currentMoveDirectionWorld.x < 0.02 && currentMoveDirectionWorld.x > -0.02)
            currentMoveDirectionWorld.x = 0;
        if (currentMoveDirectionWorld.y < 0.02 && currentMoveDirectionWorld.y > -0.02)
            currentMoveDirectionWorld.y = 0;
        if (currentMoveDirectionWorld.z < 0.02 && currentMoveDirectionWorld.z > -0.02)
            currentMoveDirectionWorld.z = 0;
        rb.velocity = currentMoveDirectionWorld;
        currentMoveDirectionLocal = transform.InverseTransformDirection(rb.velocity);
        if (currentMoveDirectionLocal.x < 0.02 && currentMoveDirectionLocal.x > -0.02)
            currentMoveDirectionLocal.x = 0;
        if (currentMoveDirectionLocal.y < 0.02 && currentMoveDirectionLocal.y > -0.02)
            currentMoveDirectionLocal.y = 0;
        if (currentMoveDirectionLocal.z < 0.02 && currentMoveDirectionLocal.z > -0.02)
            currentMoveDirectionLocal.z = 0;

    }

    void getInput()
    {
        isPressingButtonUp = false;
        isPressingButtonDown = false;
        isPressingAnyButton = false;

        if (Input.GetKey(KeyCode.Space) || Input.GetAxis("RTrigger") < -0.3f)
        {
            isPressingButtonUp = true;
        }
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift) || Input.GetAxis("LTrigger") > 0)
        {
            isPressingButtonDown = true;
        }

        isPressingAnyButton = playerRotationScript.isPressingAnyButtonXZ || isPressingButtonDown || isPressingButtonUp;
    }

    void calculateForceVector()
    {
        forceMovementVectorX = new Vector3(-playerRotationScript.rotationActualZAxis * accelerationPerGradX, 0, 0);
        if(m_movePlaneLike)
            forceMovementVectorZ = new Vector3(0, 0, playerRotationScript.rotationActualXAxis * accelerationPerGradZ);
        else
            forceMovementVectorZ = Camera.main.transform.forward * accelerationPerGradZ * playerRotationScript.rotationActualXAxis;

        forceMovementVectorY = Vector3.zero;
        if (isPressingButtonUp)
            forceMovementVectorY += new Vector3(0, accelerationUpDown, 0);
        if(isPressingButtonDown)
            forceMovementVectorY -= new Vector3(0, accelerationUpDown, 0);
    }

    void calculateAirRestianceVector()
    {
        if((playerRotationScript.isPressingButtonLeft && currentMoveDirectionLocal.x < 0) || playerRotationScript.isPressingButtonRight && currentMoveDirectionLocal.x > 0)
        {
            airRestianceVectorX = (currentMoveDirectionLocal.x <= 0 ? 1 : -1) * new Vector3(airRestianceXOnButton * Mathf.Pow(Mathf.Max(Mathf.Abs(currentMoveDirectionLocal.x), airRestianceXMinSpeedOnButton), airRestianceXExponentOnButton), 0, 0);
        }
        else
        {
            if (currentMoveDirectionLocal.x > 0.02f || currentMoveDirectionLocal.x < -0.02f)
                airRestianceVectorX = (currentMoveDirectionLocal.x <= 0 ? 1 : -1) * new Vector3(airRestianceXNoButton * Mathf.Pow(Mathf.Max(Mathf.Abs(currentMoveDirectionLocal.x), airRestianceXMinSpeedNoButton), airRestianceXExponentNoButton), 0, 0);
            else
                airRestianceVectorX = Vector3.zero;
        }

        if ((isPressingButtonUp && currentMoveDirectionLocal.y > 0) || (isPressingButtonDown && currentMoveDirectionLocal.y < 0))
        {
            airRestianceVectorY = (currentMoveDirectionLocal.y <= 0 ? 1 : -1) * new Vector3(0, airRestianceYOnButton * Mathf.Pow(Mathf.Max(Mathf.Abs(currentMoveDirectionLocal.y), airRestianceYMinSpeedOnButton), airRestianceYExponentOnButton), 0);
        }
        else
        {
            if (currentMoveDirectionLocal.y > 0.02f || currentMoveDirectionLocal.y < -0.02f)
                airRestianceVectorY = (currentMoveDirectionLocal.y <= 0 ? 1 : -1) * new Vector3(0, airRestianceYNoButton * Mathf.Pow(Mathf.Max(Mathf.Abs(currentMoveDirectionLocal.y), airRestianceYMinSpeedNoButton), airRestianceYExponentNoButton), 0);
            else
                airRestianceVectorY = Vector3.zero;
        }

        if ((playerRotationScript.isPressingButtonForward && currentMoveDirectionLocal.z > 0) || (playerRotationScript.isPressingButtonBackward && currentMoveDirectionLocal.z < 0))
        {
            airRestianceVectorZ = (currentMoveDirectionLocal.z <= 0 ? 1 : -1) * new Vector3(0, 0, airRestianceZOnButton * Mathf.Pow(Mathf.Max(Mathf.Abs(currentMoveDirectionLocal.z), airRestianceZMinSpeedOnButton), airRestianceZExponentOnButton));
        }
        else
        {
            if (currentMoveDirectionLocal.z > 0.02f || currentMoveDirectionLocal.z < -0.02f)
                airRestianceVectorZ = (currentMoveDirectionLocal.z <= 0 ? 1 : -1) * new Vector3(0, 0, airRestianceZNoButton * Mathf.Pow(Mathf.Max(Mathf.Abs(currentMoveDirectionLocal.z), airRestianceZMinSpeedNoButton), airRestianceZExponentNoButton));
            else
                airRestianceVectorZ = Vector3.zero;
        }
    }

    void applyForceVector()
    {
        if(m_movePlaneLike)
            forceMovementVectorTotal = transform.rotation * (forceMovementVectorX + forceMovementVectorZ + forceMovementVectorY);
        else
            forceMovementVectorTotal = forceMovementVectorZ + transform.rotation * (forceMovementVectorX + forceMovementVectorY);
        rb.AddForce(forceMovementVectorTotal, ForceMode.Acceleration);
        //Debug.DrawRay(transform.position + Vector3.up * 2f, forceMovementVectorTotal, Color.green);

        airRestianceVectorTotal = transform.TransformDirection(airRestianceVectorX + airRestianceVectorY + airRestianceVectorZ);
        rb.AddForce(airRestianceVectorTotal, ForceMode.Acceleration);
       // Debug.DrawRay(transform.position + Vector3.up * 2f, airRestianceVectorTotal, Color.blue);

        rb.velocity = rb.velocity.normalized * Mathf.Clamp(rb.velocity.magnitude, 0, maxSpeed);
       // Debug.DrawRay(transform.position + Vector3.up * 2f, rb.velocity, Color.yellow);

        // Affect GrabSystem cubes aswell
        
        // Affect CubeEntityPlayerGrabSystem aswell
        if (GetComponent<PlayerEntityAttachSystem>() != null && GetComponent<PlayerEntityAttachSystem>().m_movementAffectsCubesFactor != 0)
        {
            foreach (GameObject cube in GetComponent<PlayerEntityAttachSystem>().m_cubeList)
            {
                cube.GetComponent<Rigidbody>().AddForce((forceMovementVectorTotal + airRestianceVectorTotal) * GetComponent<PlayerEntityAttachSystem>().m_movementAffectsCubesFactor, ForceMode.Acceleration);
            }
        }

        CemBoidBase boidScript = Constants.getBoidSystem();
        if (boidScript != null)
        {
            if (boidScript.m_affectedByplayerMovementPower > 0)
            {
                foreach (GameObject agent in boidScript.m_agents)
                {
                    agent.GetComponent<Rigidbody>().AddForce((forceMovementVectorTotal + airRestianceVectorTotal) * boidScript.m_affectedByplayerMovementPower, ForceMode.Acceleration);
                }
            }
        }
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
