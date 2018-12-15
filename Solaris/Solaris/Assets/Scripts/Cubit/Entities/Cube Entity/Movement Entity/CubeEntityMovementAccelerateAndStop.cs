using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CubeEntityMovementAccelerateAndStop : CubeEntityMovementAbstract, IRemoveOnStateChange, IPrepareRemoveOnStateChange, ICopiable, IOnParticleCollision, IPostCopy
{

    [Space]
    [Space]
    [Space]
    [Space]


    [Header("------- This Setttings -------")]
    public Vector3 m_overrideTargetDirection;
    public float m_checkStateChangeCooldown;
    public float m_updateVectorCooldown;
    public bool m_destroyOnCollision;

    [Header("----- (Accelleration) -----")]
    public bool m_useAcceleration = false;
    public float m_accelerationPowerMin;
    public float m_accelerationPowerMax;
    //public float m_accelerationInitialDelay;

    [Header("--- Aceleration Destroy Trigger ---")]
    [Header("- (Time) -")]
    public float m_accelerationDurationMin;
    public float m_accelerationDurationMax;
    [Header("- (Speed) -")]
    public float m_accelerationSpeedToReachMin;
    public float m_accelerationSpeedToReachMax;
    [Header("- (Distance) -")]
    public float m_accelerationDistanceToTargetFactorMin;
    public float m_accelerationDistanceToTargetFactorMax;
    [Header("- (Collision) -")]
    public float m_accelerationCollisionSpeedMin;
    public float m_accelerationCollisionSpeedMax;
    public bool m_accelerationCollisionWithFoesOnly = false;
    public bool m_accelerationCollisionWithNotFriendly = true;
    //public bool m_accelerationCollisionSetToInactive = true;

    [Space]
    [Header("--- (Idle) ---")]
    public bool m_useIdle = false;

    [Header("--- Idle Destroy Trigger ---")]
    [Header("- (Time) -")]
    public float m_idleDurationMin;
    public float m_idleDurationMax;
    [Header("- (Speed) -")]
    public float m_idleSpeedToReachMin;
    public float m_idleSpeedToReachMax;
    [Header("- (Distance) -")]
    public float m_idleDistanceMin;
    public float m_idleDistanceMax;
    [Header("- (Collision) -")]
    public float m_idleCollisionSpeedMin;
    public float m_idleCollisionSpeedMax;
    public bool m_idleCollisionWithFoesOnly = false;
    public bool m_idleCollisionWithNotFriendly = true;
    //public bool m_idleCollisionWithFoesToInactive = true;

    [Space]
    [Header("--- (Deceleration) ---")]
    public bool m_useDeceleration = false;
    public float m_decelerationPowerMin;
    public float m_decelerationPowerMax;

    [Header("--- Deceleration Destroy Trigger ---")]
    [Header("- (Time) -")]
    public float m_decelerationDurationMin;
    public float m_decelerationDurationMax;
    [Header("- (Speed) -")]
    public float m_decelerationSpeedToReachMin;
    public float m_decelerationSpeedToReachMax;
    [Header("- (Distance) -")]
    public float m_decelerationDistanceMin;
    public float m_decelerationDistanceMax;
    [Header("- (Collision) -")]
    public float m_decelerationCollisionSpeedMin;
    public float m_decelerationCollisionSpeedMax;
    public bool m_decelerationCollisionWithFoesOnly = false;
    public bool m_decelerationCollisionWithNotFriendly = true;

    [Space]
    [Header("------- This Debug -------")]
    public int m_currentMovementState; // 0 = Acceleration, 1 = Idle, 2 = Deceleration
    public Vector3 m_targetDirection;
    public Vector3 m_forceVector;
    public float m_delayRdy;
    public float m_durationEnd;
    public float m_distanceTraveled;
    public float m_collisionSpeedCurrentAcceleration;
    public float m_collisionSpeedCurrentIdle;
    public float m_collisionSpeedCurrentDeceleration;

    public float m_accelerationPower;
    public float m_decelerationPower;

    public float m_accelerationDuration;
    public float m_accelerationSpeedToReach;
    public float m_accelerationDistance;
    public float m_accelerationCollisionSpeed;
    public float m_idleDuration;
    public float m_idleSpeedToReach;
    public float m_idleDistance;
    public float m_idleCollisionSpeed;
    public float m_decelerationSpeedToReach;
    public float m_decelerationDuration;
    public float m_decelerationDistance;
    public float m_decelerationCollisionSpeed;
    public float m_checkStateChangeCooldownRdy;
    public float m_updateVectorCooldownRdy;

    public CubeEntityMovement m_movementScript;
    public CubeEntityParticleSystem m_particleSystem;
    public CubeEntityState m_ownStateScript;
    public Rigidbody m_rb;
    public bool m_isInitialized;


    // Use this for initialization
    void Start()
    {
        if (!m_isInitialized)
            initializeStuff();
    }

    public void initializeStuff()
    {
        m_rb = GetComponent<Rigidbody>();
        if (m_rb == null)
            Debug.Log("RigidBody of " + gameObject.name + " not found");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_useThis)
        {
            if (m_checkStateChangeCooldownRdy < Time.time)
            {
                m_checkStateChangeCooldownRdy = m_checkStateChangeCooldown + Time.time;
                updateState();
            }
            if (m_updateVectorCooldownRdy < Time.time)
            {
                m_updateVectorCooldownRdy = m_updateVectorCooldown + Time.time;
                updateCalculation();
            }
            base.OnFixedUpdate();

            applyForceVector();
        }
    }

    // counter and stuff
    public override bool updateDestroy()
    {
        if (m_currentMovementState > 2)
            return true;
        
        return false;
    }
    bool updateState()
    {
        if (m_currentMovementState == 0)
        {
            m_distanceTraveled += m_rb.velocity.magnitude * Time.deltaTime;

            if (!m_useAcceleration)
            {
                changeState();
            }
            else if(m_accelerationDuration > 0 && m_durationEnd < Time.time)
            {
                changeState();
            }
            else if (m_accelerationSpeedToReach > 0 && m_rb.velocity.magnitude > m_accelerationSpeedToReach)
            {
                changeState();
            }
            else if (m_accelerationDistance > 0 && m_distanceTraveled > m_accelerationDistance)
            {
                changeState();
            }
            else if (m_accelerationCollisionSpeed > 0 && m_collisionSpeedCurrentAcceleration > m_accelerationCollisionSpeed)
            {
                if (m_destroyOnCollision)
                    onDestroy();
                else
                    changeState();

            }
            //bool[] changeConditionsOr = {
            //    !m_useAcceleration,
            //    m_accelerationDuration > 0 && m_durationEnd < Time.time,
            //    (m_accelerationSpeedToReach > 0 && m_rb.velocity.magnitude > m_accelerationSpeedToReach),
            //    m_accelerationDistance > 0 && m_distanceTraveled > m_accelerationDistance,
            //    m_accelerationCollisionSpeed > 0 && m_collisionSpeedCurrent > m_accelerationCollisionSpeed
            //};

            //foreach (bool condition in changeConditionsOr)
            //{
            //    if (condition)
            //    {
            //        changeState();
            //        break;
            //    }
            //}
        }

        if (m_currentMovementState == 1)
        {
            m_distanceTraveled += m_rb.velocity.magnitude * Time.deltaTime;

            if (!m_useIdle)
            {
                changeState();
            }
            else if (m_idleDuration > 0 && m_durationEnd < Time.time)
            {
                changeState();
            }
            else if (m_idleSpeedToReach > 0 && m_rb.velocity.magnitude > m_idleSpeedToReach)
            {
                changeState();
            }
            else if (m_idleDistance > 0 && m_distanceTraveled > m_idleDistance)
            {
                changeState();
            }
            else if (m_idleCollisionSpeed > 0 && m_collisionSpeedCurrentIdle > m_idleCollisionSpeed)
            {
                if (m_destroyOnCollision)
                    onDestroy();
                else
                    changeState();
            }

            //bool[] changeConditionsOr = {
            //    !m_useIdle,
            //    m_idleDuration > 0 && m_durationEnd < Time.time,
            //    (m_idleSpeedToReach > 0 && m_rb.velocity.magnitude > m_idleSpeedToReach),
            //    m_idleDistance > 0 && m_distanceTraveled > m_idleDistance,
            //    m_idleCollisionSpeed > 0 && m_collisionSpeedCurrent > m_idleCollisionSpeed
            //};

            //foreach (bool condition in changeConditionsOr)
            //{
            //    if (condition)
            //    {
            //        changeState();
            //        break;
            //    }
            //}
        }

        if (m_currentMovementState == 2)
        {
            m_distanceTraveled += m_rb.velocity.magnitude * Time.deltaTime;

            if (!m_useDeceleration)
            {
                changeState();
            }
            else if (m_decelerationDuration > 0 && m_durationEnd < Time.time)
            {
                changeState();
            }
            else if (m_decelerationSpeedToReach > 0 && m_rb.velocity.magnitude < m_decelerationSpeedToReach)
            {
                changeState();
            }
            else if (m_decelerationDistance > 0 && m_distanceTraveled > m_decelerationDistance)
            {
                changeState();
            }
            else if (m_decelerationCollisionSpeed > 0 && m_collisionSpeedCurrentDeceleration > m_decelerationCollisionSpeed)
            {
                changeState();
            }

            //bool[] changeConditionsOr = {
            //    !m_useDeceleration,
            //    m_decelerationDuration > 0 && m_durationEnd < Time.time,
            //    (m_decelerationSpeedToReach > 0 && m_rb.velocity.magnitude < m_decelerationSpeedToReach),
            //    m_decelerationDistance > 0 && m_distanceTraveled > m_decelerationDistance,
            //    m_decelerationCollisionSpeed > 0 && m_collisionSpeedCurrent > m_decelerationCollisionSpeed
            //};

            //foreach (bool condition in changeConditionsOr)
            //{
            //    if (condition)
            //    {
            //        changeState();
            //        break;
            //    }
            //}
        }

        return false;
    }
    void changeState()
    {
        m_currentMovementState++;
        resetCounter();
        updateDestroy();
    }

    void resetCounter()
    {
        m_distanceTraveled = 0;
        //m_collisionSpeedCurrentAcceleration = 0;

        if (m_currentMovementState == 1)
        {
            m_durationEnd = m_idleDuration + Time.time;
        }
        if (m_currentMovementState == 2)
        {
            m_durationEnd = m_decelerationDuration + Time.time;
        }
    }

    

    // Movement
    void updateCalculation()
    {
        getForceVector();
    }
    public override void getForceVector()
    {
        if(m_currentMovementState == 0)
        {
            m_forceVector = m_targetDirection.normalized * m_accelerationPower;
        }
        if(m_currentMovementState == 1)
        {
            m_forceVector = Vector3.zero;
        }
        if(m_currentMovementState == 2)
        {
            if (m_rb.velocity.magnitude > 0.01f)
                m_forceVector = -m_rb.velocity.normalized * m_decelerationPower;
            else
                m_forceVector = Vector3.zero;
        }
    }
    void applyForceVector()
    {
        m_rb.AddForce(m_forceVector * m_forceFactor, ForceMode.Acceleration);
    }

    // collision
    public void OnCollisionEnter(Collision collision)
    {
        if (!enabled || !m_useThis || collision.collider.isTrigger)
        {
            return;
        }

        //if (m_duration - (Time.time - m_durationEndTime) < m_minHomingDuration)
        {
            if (!(m_accelerationCollisionSpeed > 0 || m_idleCollisionSpeed > 0 || m_decelerationCollisionSpeed > 0))
            {
                return;
            }
        }


        //CubeEntityState otherStateScript = collision.collider.GetComponent<CubeEntityState>();
        //if (m_accelerationCollisionWithFoesOnly && otherStateScript == null)
        //{
        //    ;// Debug.Log("Aborted: otherStateScript was null!");
        //    return;
        //}

        if (m_currentMovementState == 0)
        {
            if(m_accelerationCollisionWithFoesOnly)
            {
                CubeEntityState otherStateScript = collision.collider.GetComponent<CubeEntityState>();
                if (otherStateScript == null || otherStateScript.isFoe(m_ownStateScript))
                    m_collisionSpeedCurrentAcceleration += m_rb.velocity.magnitude;
            }
            else if(m_accelerationCollisionWithNotFriendly)
            {
                CubeEntityState otherStateScript = collision.collider.GetComponent<CubeEntityState>();
                if(otherStateScript == null || !otherStateScript.isFriendly(m_ownStateScript))
                    m_collisionSpeedCurrentAcceleration += m_rb.velocity.magnitude;
            }
            else
                m_collisionSpeedCurrentAcceleration += m_rb.velocity.magnitude;
        }
        if (m_currentMovementState == 1)
        {
            if (m_idleCollisionWithFoesOnly)
            {
                CubeEntityState otherStateScript = collision.collider.GetComponent<CubeEntityState>();
                if (otherStateScript == null || otherStateScript.isFoe(m_ownStateScript))
                    m_collisionSpeedCurrentIdle += m_rb.velocity.magnitude;
            }
            else if (m_idleCollisionWithNotFriendly)
            {
                CubeEntityState otherStateScript = collision.collider.GetComponent<CubeEntityState>();
                if (otherStateScript == null || !otherStateScript.isFriendly(m_ownStateScript))
                    m_collisionSpeedCurrentIdle += m_rb.velocity.magnitude;
            }
            else
                m_collisionSpeedCurrentIdle += m_rb.velocity.magnitude;

            
        }
        if (m_currentMovementState == 2)
        {
            if (m_decelerationCollisionWithFoesOnly)
            {
                CubeEntityState otherStateScript = collision.collider.GetComponent<CubeEntityState>();
                if (otherStateScript == null || otherStateScript.isFoe(m_ownStateScript))
                    m_collisionSpeedCurrentDeceleration += m_rb.velocity.magnitude;
            }
            else if (m_decelerationCollisionWithNotFriendly)
            {
                CubeEntityState otherStateScript = collision.collider.GetComponent<CubeEntityState>();
                if (otherStateScript == null || !otherStateScript.isFriendly(m_ownStateScript))
                    m_collisionSpeedCurrentDeceleration += m_rb.velocity.magnitude;
            }
            else
                m_collisionSpeedCurrentDeceleration += m_rb.velocity.magnitude;
        }

        updateState();
        if (updateDestroy())
        {
            onDestroy();
        }
    }
    public void onParticleCollision(GameObject other, int otherAffiliation)
    {
        if (!enabled || !m_useThis)
        {
            return;
        }

        {
            if (!(m_accelerationCollisionSpeed > 0 || m_idleCollisionSpeed > 0 || m_decelerationCollisionSpeed > 0))
            {
                return;
            }
        }


        CubeEntityState otherStateScript = other.GetComponent<CubeEntityState>();
        //ParticleEntityCollision otherCollisionScript = null;
        if (otherStateScript == null)
        {
            //otherCollisionScript = other.GetComponent<ParticleEntityCollision>();
            //if (otherCollisionScript == null)
            //{
                Debug.Log("Aborted: otherStateScript was null!");
                return;
            //}
        }

        if (m_currentMovementState == 0)
        {
            if (!m_accelerationCollisionWithFoesOnly || (otherStateScript.isFoe(m_ownStateScript)))
                m_collisionSpeedCurrentAcceleration += m_rb.velocity.magnitude;
        }
        if (m_currentMovementState == 1)
        {
            if (!m_idleCollisionWithFoesOnly || (otherStateScript.isFoe(m_ownStateScript)))
                m_collisionSpeedCurrentIdle += m_rb.velocity.magnitude;
        }
        if (m_currentMovementState == 2)
        {
            if (!m_decelerationCollisionWithFoesOnly || (otherStateScript.isFoe(m_ownStateScript)))
                m_collisionSpeedCurrentDeceleration += m_rb.velocity.magnitude;
        }

        updateState();
        if (updateDestroy())
        {
            onDestroy();
        }
    }

    // copy
    void setValues(CubeEntityMovementAccelerateAndStop copyScript)
    {
        // base
        //m_target = copyScript.m_target;
        base.setValues(copyScript);

        // this
        m_overrideTargetDirection = copyScript.m_overrideTargetDirection;

        m_useAcceleration = copyScript.m_useAcceleration;
        m_useIdle = copyScript.m_useIdle;
        m_useDeceleration = copyScript.m_useDeceleration;

        m_accelerationPowerMin = copyScript.m_accelerationPowerMin;
        m_accelerationPowerMax = copyScript.m_accelerationPowerMax;
        m_accelerationPower = getValue(m_accelerationPowerMin, m_accelerationPowerMax);

        m_accelerationDurationMin = copyScript.m_accelerationDurationMin;
        m_accelerationDurationMax = copyScript.m_accelerationDurationMax;
        m_accelerationDuration = getValue(m_accelerationDurationMin, m_accelerationDurationMax);

        m_accelerationSpeedToReachMin = copyScript.m_accelerationSpeedToReachMin;
        m_accelerationSpeedToReachMax = copyScript.m_accelerationSpeedToReachMax;
        m_accelerationSpeedToReach = getValue(m_accelerationSpeedToReachMin, m_accelerationSpeedToReachMax);

        m_accelerationDistanceToTargetFactorMin = copyScript.m_accelerationDistanceToTargetFactorMin;
        m_accelerationDistanceToTargetFactorMax = copyScript.m_accelerationDistanceToTargetFactorMax;
        m_accelerationDistance = getValue(m_accelerationDistanceToTargetFactorMax, m_accelerationDistanceToTargetFactorMax);

        m_accelerationCollisionSpeedMin = copyScript.m_accelerationCollisionSpeedMin;
        m_accelerationCollisionSpeedMax = copyScript.m_accelerationCollisionSpeedMax;
        m_accelerationCollisionSpeed = getValue(m_accelerationCollisionSpeedMin, m_accelerationCollisionSpeedMax);
        m_accelerationCollisionWithFoesOnly = copyScript.m_accelerationCollisionWithFoesOnly;

        m_idleDurationMin = copyScript.m_idleDurationMin;
        m_idleDurationMax = copyScript.m_idleDurationMax;
        m_idleDuration = getValue(m_idleDurationMin, m_idleDurationMax);

        m_idleSpeedToReachMin = copyScript.m_idleSpeedToReachMin;
        m_idleSpeedToReachMax = copyScript.m_idleSpeedToReachMax;
        m_idleSpeedToReach = getValue(m_idleSpeedToReachMin, m_idleSpeedToReachMax);

        m_idleDistanceMin = copyScript.m_idleDistanceMin;
        m_idleDistanceMax = copyScript.m_idleDistanceMax;
        m_idleDistance = getValue(m_idleDistanceMin, m_idleDistanceMax);

        m_idleCollisionSpeedMin = copyScript.m_idleCollisionSpeedMin;
        m_idleCollisionSpeedMax = copyScript.m_idleCollisionSpeedMax;
        m_idleCollisionSpeed = getValue(m_idleCollisionSpeedMin, m_idleCollisionSpeedMax);
        m_idleCollisionWithFoesOnly = copyScript.m_idleCollisionWithFoesOnly;

        m_decelerationPowerMin = copyScript.m_decelerationPowerMin;
        m_decelerationPowerMax = copyScript.m_decelerationPowerMax;
        m_decelerationPower = getValue(m_decelerationPowerMin, m_decelerationPowerMax);

        m_decelerationDurationMin = copyScript.m_decelerationDurationMin;
        m_decelerationDurationMax = copyScript.m_decelerationDurationMax;
        m_decelerationDuration = getValue(m_decelerationDurationMin, m_decelerationDurationMax);

        m_decelerationSpeedToReachMin = copyScript.m_decelerationSpeedToReachMin;
        m_decelerationSpeedToReachMax = copyScript.m_decelerationSpeedToReachMax;
        m_decelerationSpeedToReach = getValue(m_decelerationSpeedToReachMin, m_decelerationSpeedToReachMax);

        m_decelerationDistanceMin = copyScript.m_decelerationDistanceMin;
        m_decelerationDistanceMax = copyScript.m_decelerationDistanceMax;
        m_decelerationDistance = getValue(m_decelerationDistanceMin, m_decelerationDistanceMax);

        m_decelerationCollisionSpeedMin = copyScript.m_decelerationCollisionSpeedMin;
        m_decelerationCollisionSpeedMax = copyScript.m_decelerationCollisionSpeedMax;
        m_decelerationCollisionSpeed = getValue(m_decelerationCollisionSpeedMin, m_decelerationCollisionSpeedMax);
        m_decelerationCollisionWithFoesOnly = copyScript.m_decelerationCollisionWithFoesOnly;

        m_destroyOnCollision = copyScript.m_destroyOnCollision;

        // Debug
        m_durationEnd = m_accelerationDuration + Time.time;
        m_particleSystem = GetComponent<CubeEntityParticleSystem>();
        //m_destroyRdyTime =  Time.time;
    }

    float getValue(float f1, float f2)
    {
        return Random.Range(f1, f2);
    }

    // interface
    public void onCopy(ICopiable copiable)
    {

    }
    public void onPostCopy()
    {
        m_ownStateScript = (CubeEntityState)Utility.getComponentInParents<CubeEntityState>(transform);// GetComponent<CubeEntityState>();
        if (m_ownStateScript == null)
            Debug.Log("Warning: ownStateScript was null!");
    }
    public void onStateChangePrepareRemove()
    {
        if (GetComponent<CubeEntityMovement>() != null && GetComponent<CubeEntityMovement>().m_movementComponents.Contains(this))
            GetComponent<CubeEntityMovement>().m_movementComponents.Remove(this);
    }
    public void onRemove()
    {
        //if (gameObject != Constants.getPlayer())
            Destroy(this);
    }
    /*
    // abstract
    public override void prepareDestroyScript()
    {
        if (GetComponent<CubeEntityMovement>() != null && GetComponent<CubeEntityMovement>().m_movementComponents.Contains(this))
            GetComponent<CubeEntityMovement>().m_movementComponents.Remove(this);
        if (gameObject != Constants.getPlayer())
            Destroy(this);
    }
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CubeEntityMovementAccelerateAndStop)baseScript);
    }
    public override void onPostCopy()
    {
        m_movementScript = GetComponent<CubeEntityMovement>();
    }*/
    public override void pasteScript(CubeEntityMovementAbstract baseScript, GameObject target, Vector3 targetPosition)
    {
        if (!m_isInitialized)
            initializeStuff();
        m_targetDirection = targetPosition - transform.position;
        setValues((CubeEntityMovementAccelerateAndStop)baseScript);
        base.pasteScript(baseScript, target, targetPosition);
    }

    public override void pasteScriptButDontActivate(CubeEntityMovementAbstract baseScript)
    {
        setValues((CubeEntityMovementAccelerateAndStop)baseScript);
    }
}
