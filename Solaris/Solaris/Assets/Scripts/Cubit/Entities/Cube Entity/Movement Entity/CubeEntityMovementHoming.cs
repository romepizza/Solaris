using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityMovementHoming : CubeEntityMovementAbstract, IRemoveOnStateChange, IPrepareRemoveOnStateChange, ICopiable, IOnParticleCollision, IPostCopy
{
    [Space]
    [Space]
    [Space]
    [Space]

    [Header("------- This Setttings -------")]
    public float m_accelerationPower;
    public float m_randomOffset;

    [Header("--- (Deviation) ---")]
    public float m_deviationPower;
    public float m_maxDegree;
    public float m_deviationDecreaseRange;

    [Space]
    [Header("--- (Calculation) ---")]
    public float m_calculationCooldown;
    public float m_initialCooldown;
    public bool m_reducePower = false;
    public float m_calculationCooldownMinRandom;
    public float m_calculationCooldownMaxRandom;

    [Header("--- (Aim Helper) ---")]
    public float m_aimPower;
    public float m_aimInFutureMin;
    public float m_aimInFutureMax;

    [Space]
    [Header("--- (Destroy Trigger) ---")]
    [Header("- Min Homing Duration -")]
    public float m_minHomingDuration;
    [Header("- Duration -")]
    public float m_duration;
    [Header("- Angle -")]
    public float m_maxAngle;
    public float m_maxAngleMinDistance;
    [Header("- Collision -")]
    public float m_collisionSpeed;
    public bool m_collisionWithFoeOnly = true;

    [Header("------- This Debug -------")]
    public float m_durationEndTime;
    public Vector3 m_targetDirection;
    //public Vector3 m_targetPosition;
    //public CubeEntitySystem m_entitySystemScript;
    public float m_currentCollisionSpeed;
    //public CubeEntityMovement m_movementScript;
    public Rigidbody m_rb;
    public Vector3 m_forceVectorAcceleration;
    public Vector3 m_forceVectorDeviation;
    public float m_calculationRdy;
    public bool m_isInitialized;
    public float m_isActiveTime;
    public float m_initialCooldownRdy;
    public bool m_isInitialCooldown;
    public Vector3 m_randomOffsetFinal;
    public Rigidbody m_targetRigidbody;
    public CubeEntityState m_ownStateScript;

    // Use this for initialization
    void Start()
    {
        if(!m_isInitialized)
            initializeStuff();
    }

    public void initializeStuff()
    {
        m_rb = GetComponent<Rigidbody>();
        if (m_rb == null)
            Debug.Log("RigidBody of " + gameObject.name + " not found");

        m_durationEndTime = m_duration + Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_useThis)
        {
            updateCalculation();

            m_isActiveTime += Time.deltaTime;
            base.OnFixedUpdate();
            /*
            if (updateDestroy())
            {
                activateFollowUpScripts();
                prepareDestroyScript();// m_movementScript.removeComponent(this);
            }
            */
            applyForceVector();
        }
    }
    
    // Movement
    void updateCalculation()
    {
        if(m_isInitialCooldown)
        {
            if(m_initialCooldownRdy <= Time.time)
            {
                m_calculationRdy = Time.time;
                m_isInitialCooldown = false;
            }
        }

        if (m_calculationRdy <= Time.time && !m_isInitialCooldown)
        {
            getForceVector();
            m_calculationRdy = Time.time + (m_calculationCooldown * Random.Range(m_calculationCooldownMinRandom, m_calculationCooldownMaxRandom));
        }
        //applyForceVector();
    }
    public override void getForceVector()
    {
        Vector3 futureBonus = Vector3.zero;

        if (m_target == null)
        {
            m_forceVectorDeviation = Vector3.zero;
            m_forceVectorAcceleration = Vector3.zero;
        }
        else
        {
            m_targetPosition = m_target.transform.position;
            
            if (m_targetRigidbody != null)
            {
                float distance = Vector3.Distance(m_targetPosition, transform.position);
                float timeScale = Mathf.Clamp((distance / m_rb.velocity.magnitude) * m_aimPower, m_aimInFutureMin, m_aimInFutureMax);
                futureBonus = m_targetRigidbody.velocity * timeScale;
            }
            
        }

        Vector3 targetPosition = m_targetPosition + m_randomOffsetFinal + futureBonus;

        Vector3 direction = targetPosition - transform.position;
        float angle = Utility.getAngle(direction, m_rb.velocity);
        m_forceVectorAcceleration = direction.normalized * m_accelerationPower;

        float angleFactor = 1;
        if(m_maxDegree > 0)
            angleFactor = Mathf.Clamp(angle / m_maxDegree, 0.2f, 1f);

        m_forceVectorDeviation = Vector3.zero;

        if (m_rb.velocity.magnitude > 0.01f)
        {
            Vector3 starePosition = transform.position + m_rb.velocity.normalized * direction.magnitude;
            Vector3 deviationDirection = targetPosition - starePosition;
            if (deviationDirection.magnitude > 0.01f)
            {
                float distanceFactor = Mathf.Clamp(Vector3.Distance(transform.position, targetPosition) / m_deviationDecreaseRange, 0.4f, 1f);
                m_forceVectorDeviation = deviationDirection.normalized * angleFactor * m_deviationPower * distanceFactor;

            }
        }
        else
        {
            
        }
    }
    void applyForceVector()
    {
        Vector3 reducedDeviationFactor = m_forceVectorDeviation;
        if (m_reducePower)
        {
            reducedDeviationFactor *= Mathf.Clamp01((m_calculationRdy - Time.time) / m_calculationCooldown);
        }

        Vector3 finalForceVector = reducedDeviationFactor;
        if (m_rb.velocity.magnitude < m_maxSpeed)
            finalForceVector += m_forceVectorAcceleration;

        //Debug.DrawRay(transform.position, m_forceVectorAcceleration, Color.blue);
        //Debug.DrawRay(transform.position, m_forceVectorDeviation, Color.green);
        //Debug.DrawRay(transform.position + new Vector3(2f, 2f, 2f), finalForceVector, Color.magenta);
        finalForceVector *= m_forceFactor;
        //Debug.DrawRay(transform.position, m_forceVectorAcceleration, Color.white);
        m_rb.AddForce(finalForceVector, ForceMode.Acceleration);
    }

    // destroy
    public override bool updateDestroy()
    {
        if(m_target != null /*&& m_target.GetComponent<MonsterEntityBase>() == null*/ && m_target != Constants.getPlayer())
        {
            //Debug.Log("0");
            //return true;
        }

        if (m_duration > 0 && m_durationEndTime < Time.time)
        {
            return true;
        }

        if (m_isActiveTime > m_minHomingDuration && !m_isInitialCooldown)
        {
            if (m_maxAngle > 0 && (m_maxAngleMinDistance <= 0 || Vector3.Distance(transform.position, m_targetPosition) < m_maxAngleMinDistance) && Utility.getAngle(m_targetPosition - transform.position, m_rb.velocity) > m_maxAngle)
            {
                return true;
            }
        }

        return false;
    }
    /*
     *void activateFollowUpScripts()
    {
        foreach (CubeEntityMovementAbstract script in m_followUpMovementScripts)
        {
            if (script == null)
                continue;

            if(m_target != null)
                m_targetPosition = m_target.transform.position;
            GetComponent<CubeEntitySystem>().getMovementComponent().addMovementComponent(script, m_target, m_targetPosition);

            script.m_useThis = true;
        }
    }
    */
    // collision
    public void OnCollisionEnter(Collision collision)
    {
        if (!enabled || !m_useThis || collision.collider.isTrigger)
            return;

        if (m_collisionSpeed <= 0)
        {
            return;
        }

        if (!((Time.time - m_durationEndTime) >= m_minHomingDuration))
        {
            return;
        }

        if (m_collisionWithFoeOnly)
        {
            CubeEntityState otherStateScript = collision.collider.GetComponent<CubeEntityState>();
            if (otherStateScript == null)
            {
                ;// Debug.Log("Aborted: otherStateScript was null!");
                return;
            }
            if (!otherStateScript.isFoe(m_ownStateScript))
            {
                return;
            }
        }

        m_currentCollisionSpeed += collision.impulse.magnitude;

        if (m_currentCollisionSpeed >= m_collisionSpeed)
        {
            onDestroy();
        }

        
    }
    // collision
    public void onParticleCollision(GameObject other, int otherAffiliation)
    {
        if (!enabled || !m_useThis)
        {
            return;
        }

        if (m_collisionSpeed <= 0)
            return;

        if (!((Time.time - m_durationEndTime) < m_minHomingDuration))
        {
            return;
        }

        if(m_collisionWithFoeOnly)
        {
            //ParticleEntityCollision otherCollisionScript = other.GetComponent<ParticleEntityCollision>();
            //if (otherCollisionScript == null)
            //{
                CubeEntityState otherStateScript = other.GetComponent<CubeEntityState>();
                if (otherStateScript == null)
                {
                    Debug.Log("Aborted: otherStateScript was null!");
                    return;
                }
                else if (!otherStateScript.isFoe(m_ownStateScript))
                {
                    return;
                }
            //}
           // else if (!m_ownStateScript.isFoe(otherCollisionScript.m_affiliation))
            //{
            //    return;
           // }
        }

        m_currentCollisionSpeed += m_rb.velocity.magnitude; // TODO : (accAndStop too)

        if (m_currentCollisionSpeed >= m_collisionSpeed)
        {
            onDestroy();
        }
    }

    // copy
    void setValues(CubeEntityMovementHoming copyScript)
    {
        // base
        //m_maxSpeed = copyScript.m_maxSpeed;
        //m_setInactiveOnDestroy = copyScript.m_setInactiveOnDestroy;
        //m_useThis = copyScript.m_useThis;
        //m_forceFactor = copyScript.m_forceFactor;

        //m_followUpMovementScripts = new List<CubeEntityMovementAbstract>();
        //foreach (CubeEntityMovementAbstract script in copyScript.m_followUpMovementScripts)
        //{
        //    m_followUpMovementScripts.Add(script);
        //}
        //m_startMovements = new List<CubeEntityMovementStartSpeed>();
        //foreach (CubeEntityMovementStartSpeed script in copyScript.m_startMovements)
        //{
        //    m_startMovements.Add(script);
        //}
        base.setValues(copyScript);

        // this
        m_accelerationPower = copyScript.m_accelerationPower;
        m_randomOffset = copyScript.m_randomOffset;

        m_deviationPower = copyScript.m_deviationPower;
        m_maxDegree = copyScript.m_maxDegree;
        m_deviationDecreaseRange = copyScript.m_deviationDecreaseRange;


        m_minHomingDuration = copyScript.m_minHomingDuration;
        m_initialCooldown = copyScript.m_initialCooldown;
        m_calculationCooldown = copyScript.m_calculationCooldown;
        m_reducePower = copyScript.m_reducePower;
        m_calculationCooldownMinRandom = copyScript.m_calculationCooldownMinRandom;
        m_calculationCooldownMaxRandom = copyScript.m_calculationCooldownMaxRandom;

        m_aimPower = copyScript.m_aimPower;
        m_aimInFutureMin = copyScript.m_aimInFutureMin;
        m_aimInFutureMax = copyScript.m_aimInFutureMax;

        m_duration = copyScript.m_duration;
        m_maxAngle = copyScript.m_maxAngle;
        m_maxAngleMinDistance = copyScript.m_maxAngleMinDistance;
        m_collisionSpeed = copyScript.m_collisionSpeed;
        m_collisionWithFoeOnly = copyScript.m_collisionWithFoeOnly;

        m_isInitialCooldown = true;
        m_initialCooldownRdy = Time.time + m_initialCooldown;

        m_randomOffsetFinal = Random.insideUnitSphere * m_randomOffset;
    }

    // interface
    public void onCopy(ICopiable copiable)
    {
        setValues((CubeEntityMovementHoming)copiable);
    }
    public void onPostCopy()
    {
        if (!m_collisionWithFoeOnly)
            return;
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
        //Debug.Log(m_movementScript);
        //if (m_movementScript.m_movementComponents.Contains(this))
        //m_movementScript.m_movementComponents.Remove(this);
        if (GetComponent<CubeEntityMovement>() != null && GetComponent<CubeEntityMovement>().m_movementComponents.Contains(this))
            GetComponent<CubeEntityMovement>().m_movementComponents.Remove(this);
        if (gameObject != Constants.getPlayer())
            Destroy(this);
    }
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CubeEntityMovementHoming)baseScript);
    }
    public override void onPostCopy()
    {
        //m_movementScript = GetComponent<CubeEntityMovement>();
    }*/
    public override void pasteScript(CubeEntityMovementAbstract baseScript, GameObject target, Vector3 targetPosition)
    {

        if (!m_isInitialized)
            initializeStuff();
        setValues((CubeEntityMovementHoming)baseScript);
        base.pasteScript(baseScript, target, targetPosition);
        if (m_target != null)
            m_targetRigidbody = m_target.GetComponent<Rigidbody>();
        getForceVector();

    }
    
    public override void pasteScriptButDontActivate(CubeEntityMovementAbstract baseScript)
    {
        setValues((CubeEntityMovementHoming)baseScript);
    }
    
}
