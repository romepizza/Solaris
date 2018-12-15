using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityIsParticleEffect : MonoBehaviour
{
    [Header("------- Debug ------")]
    [Header("--- Life Time ---")]
    public float m_lifeTime;
    public float m_destroyDelay;
    public float m_activisionDelay;
    [Header("--- Bools ---")]
    public bool m_unchildFromParentOnPrepareDestroy;
    public bool m_isLoop;
    public bool m_stayOnGameObject;
    public bool m_isTrail;
    public float m_startSpeed;

    [Header("------- Debug ------")]
    public List<CubeEntityParticleSystem> m_registeredInParticleSystems;
    public float m_destroyTime;
    public bool m_isDestroying;
    public float m_activisionTime;
    public bool m_rendererIsActive;
    public bool m_clearAfterReactivision;
    public Dictionary<ParticleSystem, float> m_defaultSimulationSpeeds;
    public GameObject m_mainEffectObject;

	// Update is called once per frame
	void Update ()
    {
        manageActivision();
        manageDestroy();
	}

    // manage lifetime
    void manageDestroy()
    {
        if (m_isDestroying && m_destroyTime < Time.time)
        {
            destroyObject();
            return;
        }

        if (m_destroyTime < Time.time)
        {
            prepareDesptroyObject();
        }
    }

    // manage particle systems
    public void registerParticleSystem(CubeEntityParticleSystem system)
    {
        if (system == null)
            return;

        if(m_registeredInParticleSystems.Contains(system))
        {
            Debug.Log("Aborted: system script was already in the list!");
            return;
        }
        m_registeredInParticleSystems.Add(system);
    }

    public void removeParticleSystem(CubeEntityParticleSystem system)
    {
        if (!m_registeredInParticleSystems.Contains(system))
        {
            Debug.Log("Aborted: system script not in the list!");
            return;
        }

        m_registeredInParticleSystems.Remove(system);
    }
    void deregisterFromParticleSystems()
    {
        foreach (CubeEntityParticleSystem system in m_registeredInParticleSystems)
        {
            system.removeEffectFromList(gameObject);
        }
    }

    // destroy
    public void prepareDesptroyObject()
    {
        if (m_destroyDelay > 0)
        {
            m_destroyTime = m_destroyDelay + Time.time;
            m_isDestroying = true;
            if (m_unchildFromParentOnPrepareDestroy)
            {
                deregisterFromParticleSystems();
                transform.parent = null;
            }
        }
        else
            destroyObject();
    }

    void destroyObject()
    {
        deregisterFromParticleSystems();
        Destroy(this.gameObject);
    }

    // renderer
    void manageActivision()
    {
        if (m_rendererIsActive)
            return;

        if (m_activisionTime <= Time.time)
            activateRenderer();
    }
    public void deactivateRenderer(float activisionTime, bool clearAfterReactivision)
    {
        ParticleSystemRenderer[] particleSystemRenderer = GetComponentsInChildren<ParticleSystemRenderer>();
        foreach (ParticleSystemRenderer renderer in particleSystemRenderer)
        {
            renderer.enabled = false;
        }
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        
        //foreach (ParticleSystem particleSystem in particleSystems)
        {
            //ParticleSystem.MainModule module = particleSystem.main;
            //m_defaultSimulationSpeeds[particleSystem] = module.simulationSpeed;
            //module.simulationSpeed = 0;
        }
        m_rendererIsActive = false;
        m_activisionTime = activisionTime + Time.time;
        m_clearAfterReactivision = clearAfterReactivision;
    }
    public void activateRenderer()
    {
        ParticleSystemRenderer[] particleSystemRenderer = GetComponentsInChildren<ParticleSystemRenderer>();
        foreach (ParticleSystemRenderer renderer in particleSystemRenderer)
        {
            renderer.enabled = true;
        }
        //ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        //foreach (ParticleSystem particleSystem in particleSystems)
        //{
        //    ParticleSystem.MainModule module = particleSystem.main;
        //    module.simulationSpeed = m_defaultSimulationSpeeds[particleSystem];
        //    if (m_clearAfterReactivision)
        //    {
        //        particleSystem.Clear();
        //    }
        //}
        //m_rendererIsActive = true;
    }

    // (de-)activate gameobject
    public void activateGameObject()
    {
        m_mainEffectObject.SetActive(true);
    }
    public void deactivateGameObject()
    {
        m_mainEffectObject.SetActive(false);
    }

    // copy
    public void setValues(CubeEntityParticleSystem system, EntityIsParticleEffect effectScript)
    {
        m_registeredInParticleSystems = new List<CubeEntityParticleSystem>();
        registerParticleSystem(system);
        m_lifeTime = effectScript.m_lifeTime;
        m_destroyDelay = effectScript.m_destroyDelay;
        m_isLoop = effectScript.m_isLoop;
        m_stayOnGameObject = effectScript.m_stayOnGameObject;
        m_startSpeed = effectScript.m_startSpeed;
        m_isTrail = effectScript.m_isTrail;
        m_activisionDelay = effectScript.m_activisionDelay;
        m_unchildFromParentOnPrepareDestroy = effectScript.m_unchildFromParentOnPrepareDestroy;

        m_destroyTime = (m_lifeTime > 0) ? m_lifeTime + Time.time : float.MaxValue;

        //m_defaultSimulationSpeeds = new Dictionary<ParticleSystem, float>();
        //ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        //foreach (ParticleSystem particleSystem in particleSystems)
        //{
        //    m_defaultSimulationSpeeds.Add(particleSystem, particleSystem.main.simulationSpeed);
        //}

        if (m_activisionDelay > 0)
        {
            deactivateRenderer(m_activisionDelay, false);
        }
        else
            m_rendererIsActive = true;


    }
}
