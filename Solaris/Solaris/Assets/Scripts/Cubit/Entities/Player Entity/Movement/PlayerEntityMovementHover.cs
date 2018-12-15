using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntityMovementHover : MonoBehaviour {

    public GameObject m_gyroscope;
    [Header("----- SETTINGS -----")]
    public bool m_movePlaneLike;

    [Header("--- Movement ---")]
    public Vector3 m_accelerationValues;
    public float m_crouchHight;

    [Header("--- Hover ---")]
    public float m_hoverPowerUp;
    public float m_hoverPowerDown;
    public float m_targetHeight;
    public float m_gravity;
    //public float m_belowFlyingDownFactor;
    //public float m_aboveFlyingUpFactor;
    [Header("- Stopping -")]
    public float m_hoverStopPower;

    [Header("- Push Up -")]
    public float m_hoverPushUpPower;
    public float m_hoverPushUpPercent;

    [Header("- Ground Detection -")]
    public bool m_gravityIfNoGround;
    public float m_rayLengthGround;
    public LayerMask m_layerMask;
    public Transform[] m_groundDetectionTransforms;

    [Header("- Collision Detection -")]
    public float m_collisionUpForce;
    public float m_additionAngle;
    public float m_rayLengthCollisionMin;
    public float m_rayLengthCollisionMax;
    public float m_rayLengthCollisionFactor;

    [Header("--- Air Resistance ---")]
    public float m_maxSpeed;
    public Vector3 m_airRestistanceIdle;
    public Vector3 m_airRestistancePressing;
    //public AnimationCurve m_airRestistanceCurveX
    public Vector3 m_airRestistanceStopping;


    [Header("----- DEBUG -----")]
    public float m_distanceToGround;
    public float m_distToTargetHeight;
    //public float m_timeTillTargetPosition;
    public float m_power;
    public float m_targetHeightActual;

    [Header("----")]
    public Vector3 m_currentMoveDirectionWorld;
    public Vector3 m_currentMoveDirectionLocal;
    public float m_movementSpeed;
    public Vector3 m_forceMovementVectorTotal;
    public Vector3 m_airRestianceVectorTotal;

    [Header("--- (X) ---")]
    public Vector3 m_forceMovementVectorX;
    //public Vector3 m_airRestianceVectorX;

    [Header("--- (Y) ---")]
    public Vector3 m_forceMovementVectorY;
    //public Vector3 m_airRestianceVectorY;

    [Header("--- (Z) ---")]
    public Vector3 m_forceMovementVectorZ;
    //public Vector3 m_airRestianceVectorZ;

    [Header("--- (Buttons) ---")]
    public bool m_isPressingButtonUp;
    public bool m_isPressingButtonDown;
    public bool m_isPressingAnyButton;
    public bool m_isPressingButtonForward;
    public bool m_isPressingButtonBackward;
    public bool m_isPressingButtonLeft;
    public bool m_isPressingButtonRight;
    public bool m_isPressingAnyButtonXZ;

    private Rigidbody m_rb;
    private Vector3 m_hitPosition;


    // Use this for initialization
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
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

    // calculation
    void calculateForceVector()
    {
        m_forceMovementVectorX = Vector3.zero;
        m_forceMovementVectorY = Vector3.zero;
        m_forceMovementVectorZ = Vector3.zero;


        // X
        if (m_isPressingButtonLeft)
            m_forceMovementVectorX.x += -m_accelerationValues.x;
        if (m_isPressingButtonRight)
            m_forceMovementVectorX.x += m_accelerationValues.x;

        // Z
        if (m_isPressingButtonForward)
        {
            if (m_movePlaneLike)
                m_forceMovementVectorZ.z += m_accelerationValues.z;
            else
                m_forceMovementVectorZ += Camera.main.transform.forward * m_accelerationValues.z;
        }
        if (m_isPressingButtonBackward)
        {
            if (m_movePlaneLike)
                m_forceMovementVectorZ.z += -m_accelerationValues.z;
            else
                m_forceMovementVectorZ += Camera.main.transform.forward * -m_accelerationValues.z;
        }


        // Y, pressing
        if (m_isPressingButtonDown)
        {
            // if above target height
            if (m_distToTargetHeight < -0.5f) // TODO : m_distToTargetHeight currently is the value from the last frame
            {
                m_forceMovementVectorY.y += -m_accelerationValues.y;
            }
            else
                m_targetHeightActual = m_crouchHight;
        }
        else
            m_targetHeightActual = m_targetHeight;

        if (m_isPressingButtonUp)
            m_forceMovementVectorY.y += m_accelerationValues.y;

        // Y hover and gravity
        m_forceMovementVectorY.y += evaluateYHover();

        // Y collision
       // m_forceMovementVectorY.y += evaluateYCollision();

    }
    float evaluateYHover()
    {
        // ground detection
        m_distanceToGround = float.MaxValue;
        bool groundHit = false;
        m_hitPosition = Vector3.zero;
        RaycastHit hit;

        // default ray if no transforms are specified
        if (m_groundDetectionTransforms == null || m_groundDetectionTransforms.Length == 0)
        {
            if (Physics.Raycast(transform.position, -m_gyroscope.transform.up, out hit, m_rayLengthGround, m_layerMask))
            {
                groundHit = true;
                Vector3 hitPosition = hit.point;
                float dist = (transform.position - hitPosition).magnitude;
                if (dist < m_distanceToGround)
                {
                    m_distanceToGround = dist;
                    m_hitPosition = hitPosition;
                }
            }
        }
        // search for lowest distance to ground
        else
        {
            //Transform t = null;
            foreach (Transform l_transform in m_groundDetectionTransforms)
            {
                if (Physics.Raycast(l_transform.position, -m_gyroscope.transform.up, out hit, m_rayLengthGround, m_layerMask))
                {
                    groundHit = true;
                    Vector3 hitPosition2 = hit.point;
                    float dist = (l_transform.position - hitPosition2).magnitude;
                    if (dist < m_distanceToGround)
                    {
                        m_distanceToGround = dist;
                        m_hitPosition = hitPosition2;
                        //t = l_transform;
                    }
                }
            }
            //if (t != null)
            //Debug.DrawLine(t.position, m_hitPosition, Color.red);
        }
        m_power = 0;

        // collision detection
        if (!(m_isPressingButtonUp || m_isPressingButtonDown))
        {
            float rayLength = Mathf.Clamp(m_currentMoveDirectionWorld.magnitude * m_rayLengthCollisionFactor, m_rayLengthCollisionMin, m_rayLengthCollisionMax);
            Vector3 rayDirection = Quaternion.AngleAxis(-m_additionAngle, Vector3.Cross(m_currentMoveDirectionWorld, m_gyroscope.transform.up)) * m_currentMoveDirectionWorld;
            if (Physics.Raycast(transform.position, rayDirection, out hit, rayLength, m_layerMask))
            {
                Vector3 hitPosition = hit.point;
                float distanceToCollisionPoint = (transform.position - hitPosition).magnitude;
                float distanceFactor = Mathf.Clamp((m_currentMoveDirectionWorld.magnitude * m_rayLengthCollisionFactor) / distanceToCollisionPoint, 1f, 3f);
                m_power += m_collisionUpForce * distanceFactor;
                //Debug.DrawRay(transform.position, m_currentMoveDirectionWorld.normalized * rayLength, Color.green);
                //Debug.DrawRay(transform.position, rayDirection.normalized * rayLength, Color.yellow);
            }
            else
            {
                //Debug.DrawRay(transform.position, rayDirection.normalized * rayLength, Color.magenta);
                //Debug.DrawRay(transform.position, m_currentMoveDirectionWorld.normalized * rayLength, Color.red);
            }
        }

        if (!m_gravityIfNoGround && m_distanceToGround >= float.MaxValue)
            return 0;

        // evaluate force
        //if (!collisionHit)
        {
            if (groundHit)
            {
                m_distToTargetHeight = m_targetHeightActual - m_distanceToGround;
                // below target height and not pressing down
                if (m_distToTargetHeight > 0)
                {
                    //if (!m_isPressingButtonUp)
                    {
                        float distanceTerm = 1;
                        if (m_distanceToGround / m_targetHeightActual < m_hoverPushUpPercent)
                            distanceTerm = m_hoverPushUpPower * Mathf.Clamp(m_targetHeightActual / m_distanceToGround, 1f, 10f);
                        float alignTerm = m_hoverPowerUp * (m_distToTargetHeight - m_currentMoveDirectionLocal.y);
                        m_power += alignTerm + distanceTerm;
                    }
                    // flying down, decelerate fast
                    if (m_currentMoveDirectionLocal.y < 0)
                    {
                        float speedFactor = Mathf.Clamp(m_currentMoveDirectionLocal.y, 1f, 10f);
                        float distanceFactor = Mathf.Clamp((m_targetHeightActual * m_targetHeightActual) / (m_distanceToGround * m_distanceToGround), 1f, 10f);
                        m_power = m_hoverStopPower * speedFactor * distanceFactor;
                    }
                }
                // above target height
                else if (m_distToTargetHeight <= 0)
                {
                    // flying up (NOT)
                    if (m_distToTargetHeight < -0.5f && !m_isPressingButtonUp)
                    {
                        m_power += -m_gravity;
                    }
                    // flying down and too fast
                    if (m_currentMoveDirectionLocal.y <= 0 && (m_distToTargetHeight - m_currentMoveDirectionLocal.y) > 0)
                    {
                        m_power += m_gravity + m_hoverPowerDown * (m_distToTargetHeight - m_currentMoveDirectionLocal.y);
                    }
                }
                //Debug.DrawRay(transform.position + new Vector3(0, 0, 1), m_rb.velocity, Color.blue);
                //Debug.DrawRay(transform.position + new Vector3(0, 0, 1.5f), m_gyroscope.transform.up * m_distToTargetHeight, Color.green);
            }
            else if (!m_isPressingButtonUp)
                m_power -= m_gravity;
        }

        //Debug.DrawRay(transform.position + new Vector3(0, 0, 0.5f), m_gyroscope.transform.up * m_power, Color.yellow);
        return m_power;
    }
    void calculateAirRestianceVector()
    {
        m_airRestianceVectorTotal = Vector3.zero;
        //m_airRestianceVectorX = Vector3.zero;
        //m_airRestianceVectorY = Vector3.zero;
        //m_airRestianceVectorZ = Vector3.zero;

        float speedFactor = Mathf.Clamp(m_currentMoveDirectionLocal.magnitude / m_maxSpeed, 0f, 1f);
        // X
        if (m_isPressingButtonRight || m_isPressingButtonLeft && !(m_isPressingButtonRight && m_isPressingButtonLeft)) // only if ONE of buttons left or right is clicked
        {
            // pressing right
            if (m_isPressingButtonRight)
            {
                // flying right
                if (m_currentMoveDirectionLocal.x >= 0)
                {
                    m_airRestianceVectorTotal.x += m_airRestistancePressing.x * (Mathf.Clamp(Mathf.Abs(m_currentMoveDirectionLocal.x) / m_maxSpeed, 0f, 10f));
                }
                else // flying left
                {
                    m_airRestianceVectorTotal.x -= m_airRestistanceStopping.x * Mathf.Clamp((m_currentMoveDirectionLocal.x * m_currentMoveDirectionLocal.x) / (m_maxSpeed * m_maxSpeed), 0f, 5f);
                }
            }
            // pressing left
            if (m_isPressingButtonLeft)
            {
                // flying right
                if (m_currentMoveDirectionLocal.x >= 0)
                {
                    m_airRestianceVectorTotal.x += m_airRestistanceStopping.x * Mathf.Clamp((m_currentMoveDirectionLocal.x * m_currentMoveDirectionLocal.x) / (m_maxSpeed * m_maxSpeed), 0f, 5f);
                }
                else // flying left
                {
                    m_airRestianceVectorTotal.x -= m_airRestistancePressing.x * (Mathf.Clamp(Mathf.Abs(m_currentMoveDirectionLocal.x) / m_maxSpeed, 0f, 10f));
                }
            }
        }
        else // not pressing either
        {
            m_airRestianceVectorTotal.x += Mathf.Sign(m_currentMoveDirectionLocal.x) * m_airRestistanceIdle.x * Mathf.Clamp((m_currentMoveDirectionLocal.x * m_currentMoveDirectionLocal.x) / (m_maxSpeed * m_maxSpeed), 0.1f, 5f);
        }

        // Y
        if (m_isPressingButtonUp || m_isPressingButtonDown && !(m_isPressingButtonUp && m_isPressingButtonDown)) // only if ONE of buttons Down or Up is clicked
        {
            // pressing Up
            if (m_isPressingButtonUp)
            {
                // flying Up
                if (m_currentMoveDirectionLocal.y >= 0)
                {
                    m_airRestianceVectorTotal.y += m_airRestistancePressing.y * Mathf.Clamp(Mathf.Abs(m_currentMoveDirectionLocal.y) / m_maxSpeed, 0f, 10f);
                }
                else // flying Down
                {
                    m_airRestianceVectorTotal.y -= m_airRestistanceStopping.y * Mathf.Clamp((m_currentMoveDirectionLocal.y * m_currentMoveDirectionLocal.y) / (m_maxSpeed * m_maxSpeed), 0f, 5f);
                }
            }
            // pressing Down
            if (m_isPressingButtonDown)
            {
                // flying Up
                if (m_currentMoveDirectionLocal.y >= 0)
                {
                    m_airRestianceVectorTotal.y += m_airRestistanceStopping.y * Mathf.Clamp((m_currentMoveDirectionLocal.y * m_currentMoveDirectionLocal.y) / (m_maxSpeed * m_maxSpeed), 0f, 5f);
                }
                else // flying Down
                {
                    m_airRestianceVectorTotal.y -= m_airRestistancePressing.y * (Mathf.Clamp(Mathf.Abs(m_currentMoveDirectionLocal.y) / m_maxSpeed, 0f, 10f));
                }
            }
        }
        else // not pressing either
        {
            m_airRestianceVectorTotal.y += Mathf.Sign(m_currentMoveDirectionLocal.y) * m_airRestistanceIdle.y * Mathf.Clamp((m_currentMoveDirectionLocal.y * m_currentMoveDirectionLocal.y) / (m_maxSpeed * m_maxSpeed), 0.1f, 5f);
        }

        // Z
        if (m_isPressingButtonForward || m_isPressingButtonBackward && !(m_isPressingButtonForward && m_isPressingButtonBackward)) // only if ONE of buttons Backward or Forward is clicked
        {
            // pressing Forward
            if (m_isPressingButtonForward)
            {
                // flying Forward
                if (m_currentMoveDirectionLocal.z >= 0)
                {
                    m_airRestianceVectorTotal.z += m_airRestistancePressing.z * (Mathf.Clamp(Mathf.Abs(m_currentMoveDirectionLocal.z) / m_maxSpeed, 0f, 10f));
                }
                else // flying Backward
                {
                    m_airRestianceVectorTotal.z -= m_airRestistanceStopping.z * Mathf.Clamp((m_currentMoveDirectionLocal.z * m_currentMoveDirectionLocal.z) / (m_maxSpeed * m_maxSpeed), 0f, 5f);// (Mathf.Clamp(Mathf.Abs(m_currentMoveDirectionLocal.z) / m_maxSpeed, 0f, 1f)) * (Mathf.Clamp(Mathf.Abs(m_currentMoveDirectionLocal.z) / m_maxSpeed, 0f, 1f));
                }
            }
            // pressing Backward
            if (m_isPressingButtonBackward)
            {
                // flying Forward
                if (m_currentMoveDirectionLocal.z >= 0)
                {
                    m_airRestianceVectorTotal.z += m_airRestistanceStopping.z * Mathf.Clamp((m_currentMoveDirectionLocal.z * m_currentMoveDirectionLocal.z) / (m_maxSpeed * m_maxSpeed), 0f, 5f);
                }
                else // flying Backward
                {
                    m_airRestianceVectorTotal.z -= m_airRestistancePressing.z * (Mathf.Clamp(Mathf.Abs(m_currentMoveDirectionLocal.z) / m_maxSpeed, 0f, 10f));
                }
            }
        }
        else // not pressing either
        {
            m_airRestianceVectorTotal.z += Mathf.Sign(m_currentMoveDirectionLocal.z) * m_airRestistanceIdle.z * Mathf.Clamp((m_currentMoveDirectionLocal.z * m_currentMoveDirectionLocal.z) / (m_maxSpeed * m_maxSpeed), 0.1f, 5f);// * m_airRestistanceCurveX.Evaluate();
        }
    }

    // apply force
    void applyForceVector()
    {
        if (m_movePlaneLike)
            m_forceMovementVectorTotal = transform.rotation * (m_forceMovementVectorX + m_forceMovementVectorZ + m_forceMovementVectorY);
        else
            m_forceMovementVectorTotal = m_forceMovementVectorZ + transform.rotation * (m_forceMovementVectorX + m_forceMovementVectorY);
        m_rb.AddForce(m_forceMovementVectorTotal, ForceMode.Acceleration);


        m_airRestianceVectorTotal = -transform.TransformDirection(m_airRestianceVectorTotal);
        m_rb.AddForce(m_airRestianceVectorTotal, ForceMode.Acceleration);

        if (m_rb.velocity.magnitude <= 0.01f)
            m_rb.velocity = Vector3.zero;


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

    }


    // get players input
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
}
