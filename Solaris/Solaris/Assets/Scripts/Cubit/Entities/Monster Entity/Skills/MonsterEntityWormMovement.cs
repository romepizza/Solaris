using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntityWormMovement : EntityCopiableAbstract, ICopiable, IRemoveOnStateChange
{
    public bool m_useRule = true;
    [Header("------- Settings -------")]
    public float m_calculateInterval;
    public float m_grabbedCubesFactor;

    [Header("--- (Movement) ---")]
    public float m_power;
    public float m_maxSpeed;


    [Header("------- Debug -------")]
    public float m_calculatedReadyTime;
    public Vector3 m_forceVector;
    public GameObject m_target;
    bool v;
    Rigidbody rb;
    bool m_isInitialized;

	// Use this for initialization
	void Start ()
    {
        if(!m_isInitialized)
            initializeStuff();
    }

    void initializeStuff()
    {
        rb = GetComponent<Rigidbody>();
        m_target = Constants.getPlayer();
    }
	
	// Update is called once per frame
	void Update ()
    {
        manageMovement();
	}
    private void FixedUpdate()
    {
        applyForceVector();
    }


    // movement
    void manageMovement()
    {
        if (!m_useRule)
            return;

        if (m_calculatedReadyTime <= Time.time)
        {
            m_target = GetComponent<CubeEntityTargetManager>().getTarget();
            if (m_target == null)
                return;

            calculateForceVector();
            m_calculatedReadyTime = m_calculateInterval + Time.time;
        }
    }
    void calculateForceVector()
    {
        Vector3 directionToTarget = m_target.transform.position - transform.position;
        m_forceVector = directionToTarget.normalized * m_power;
    }
    void applyForceVector()
    {
        if (!m_useRule)
            return;

        rb.AddForce(m_forceVector, ForceMode.Acceleration);
        rb.velocity = rb.velocity.normalized * (Mathf.Min(rb.velocity.magnitude, m_maxSpeed));

        // Affect CubeEntityPlayerGrabSystem aswell
        //if (m_grabbedCubesFactor > 0)
        { 
            MonsterEntityAttachSystemNew script = GetComponent<MonsterEntityAttachSystemNew>();
            
            if (script != null && script.m_movementAffectsCubesFactor > 0)
            {
                foreach (GameObject cube in script.m_cubeList)
                {
                    cube.GetComponent<Rigidbody>().AddForce(m_forceVector * script.m_movementAffectsCubesFactor, ForceMode.Acceleration);
                }
            }
        }
    }

    // copy
    public void setValues(MonsterEntityWormMovement copyScript)
    {
        if (copyScript == null)
        {
            Debug.Log("Aborted: CemBoidRuleSeparation script was null!");
            return;
        }
        if (copyScript.GetType() != this.GetType())
        {
            Debug.Log("Aborted: Copy script wasn't separation script!");
            return;
        }

        MonsterEntityWormMovement copyScript2 = (MonsterEntityWormMovement)copyScript;

        m_useRule = copyScript2.m_useRule;
        m_calculateInterval = copyScript2.m_calculateInterval;
        m_power = copyScript2.m_power;
        m_maxSpeed = copyScript2.m_maxSpeed;
        m_grabbedCubesFactor = copyScript2.m_grabbedCubesFactor;
    }

    // interfaces
    public void onCopy(ICopiable copiable)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((MonsterEntityWormMovement)copiable);
    }
    public void onRemove()
    {
        Destroy(this);
    }

    /*
    // abstract
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((MonsterEntityWormMovement)baseScript);
    }
    public override void onPostCopy()
    {
        
    }
    public override void prepareDestroyScript()
    {
        Destroy(this);
    }
    */
}
