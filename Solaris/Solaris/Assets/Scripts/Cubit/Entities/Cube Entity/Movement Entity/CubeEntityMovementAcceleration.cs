using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityMovementAcceleration : MonoBehaviour
{
    /*
    [Header("----- SETTINGS -----")]
    public float m_duration;
    public float m_power;
    public Vector3 m_targetDirection;
    public Vector3 m_targetPoint;

    [Header("----- DEBUG -----")]
    public float m_durationEndTime;
    //public CubeEntitySystem m_entitySystemScript;

    public CubeEntityMovement m_movementScript;
    private Rigidbody m_rb;
    public bool m_isInitialized;

    // Use this for initialization
    void Start ()
    {
        Debug.Log("Caution: These lines of code are out of date!");

        if (!m_isInitialized)
            initializeStuff();
	}
    void initializeStuff()
    {
        m_rb = GetComponent<Rigidbody>();
        if (m_rb == null)
            Debug.Log("RigidBody of " + gameObject.name + " not found");
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        updateDuration();
        if(m_useThis)
            updateAcceleration();
    }


    void updateDuration()
    {
        if(m_duration >= 0 && m_durationEndTime < Time.time)
        {
            Debug.Log("Caution! This code is outdated!");
            //m_movementScript.removeComponent(this);
        }
    }

    void updateAcceleration()
    {
        if(m_rb.velocity.magnitude < m_maxSpeed)
        {
            m_rb.AddForce(m_targetDirection.normalized * m_power, ForceMode.Acceleration);
        }
    }

    public override void getForceVector()
    {

    }
    // setter
    void setValues(CubeEntityMovementAcceleration copyScript)
    {
        m_duration = copyScript.m_duration;
        m_power = copyScript.m_power;
        m_maxSpeed = copyScript.m_maxSpeed;
        m_targetPoint = copyScript.m_targetPoint;
        m_targetDirection = copyScript.m_targetDirection;
        m_useThis = copyScript.m_useThis;
    }

    void setValues(CubeEntityMovementAcceleration copyScript, Vector3 targetPosition)
    {
        m_duration = copyScript.m_duration;
        m_power = copyScript.m_power;
        m_maxSpeed = copyScript.m_maxSpeed;
        m_targetPoint = targetPosition;

        m_targetDirection = m_targetPoint - transform.position;
        m_useThis = copyScript.m_useThis;
    }
    /*
    // abstract
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CubeEntityMovementAcceleration)baseScript);
    }
    public override void prepareDestroyScript()
    {
        Destroy(this);
    }
    public override void onPostCopy()
    {
        m_movementScript = GetComponent<CubeEntityMovement>();
        m_durationEndTime = m_duration + Time.time;
    }
    
    public override void pasteScript(CubeEntityMovementAbstract baseScript, GameObject target, Vector3 targetPosition)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CubeEntityMovementAcceleration)baseScript, targetPosition);
    }

    public override bool updateDestroy()
    {
        throw new System.NotImplementedException();
    }
    */
}
