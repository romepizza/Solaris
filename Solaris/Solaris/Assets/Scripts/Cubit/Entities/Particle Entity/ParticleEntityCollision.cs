using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEntityCollision : MonoBehaviour, IOnParticleCollision
{
    [Header("------- Settings -------")]
    //public int m_affiliation;
    public bool m_effectOnFoeOnly;
    public bool m_loseMaxParticleOnCollision;

    [Header("------- Debug -------")]
    public ParticleSystem m_particleSystem;
    public List<ParticleCollisionEvent> m_collisionEvents;
    public CubeEntityParticleSystem m_particleSystemScript;
    public CubeEntityState m_ownStateScript;
    public CubeEntityCharge m_ownChargeScript;

    public bool m_isInitialized;


	// Use this for initialization
	void Start ()
    {
        if (!m_isInitialized)
            initializeStuff();
    }

    void initializeStuff()
    {
        if (m_particleSystem == null)
            m_particleSystem = GetComponent<ParticleSystem>();
        if (m_particleSystem == null)
            Debug.Log("Warning: particle system is null!");

        m_particleSystem = (ParticleSystem)Utility.getComponentInParents<ParticleSystem>(transform);

        m_collisionEvents = new List<ParticleCollisionEvent>();
        m_particleSystemScript = (CubeEntityParticleSystem)Utility.getComponentInParents<CubeEntityParticleSystem>(transform);
        m_ownStateScript = (CubeEntityState)Utility.getComponentInParents<CubeEntityState>(transform);
        if (m_ownStateScript == null)
            Debug.Log(gameObject.name + " Warning: ownStateScript is null!");

        m_ownChargeScript = (CubeEntityCharge)Utility.getComponentInParents<CubeEntityCharge>(transform);

        m_isInitialized = true;
    }

    //public void OnParticleCollision(GameObject other)
    //{
    //m_particleSystemScript.registerParticleCollision(other);

    //main.maxParticles = main.maxParticles - 1;
    //}
    public void onParticleCollision(GameObject other, int otherAffiliation)
    {
        if (m_effectOnFoeOnly && !m_ownStateScript.isFoe(otherAffiliation))
        {
            return;
        }

        if (m_ownChargeScript != null && m_ownChargeScript.m_chargePower > 0)
        {
            CubeEntityCharge cubeEntityCharge = other.GetComponent<CubeEntityCharge>();
            if (cubeEntityCharge != null)
            {
                cubeEntityCharge.evaluateDischarge(m_ownChargeScript.m_chargePower, m_ownStateScript.m_affiliation, CubeEntityState.s_STATE_ACTIVE, true);
            }
        }

        int numCollisionEvents = m_particleSystem.GetCollisionEvents(other, m_collisionEvents);
        if (numCollisionEvents == 0)
            return;

        if (m_loseMaxParticleOnCollision)
        {
            ParticleSystem.MainModule main = m_particleSystem.main;
            main.maxParticles = Mathf.Clamp(main.maxParticles - 1, 0, int.MaxValue);
        }
    }

    // interfaces
    public void onDestoryEffect()
    {

    }

}
