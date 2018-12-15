using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityParticleSystem : MonoBehaviour, IStateOnStateChange, ICopyValues, IRemoveOnStateChange, IRemoveOnSwitchAffiliation
{
    [Header("------- Settings -------")]
    public List<GameObject> m_OnEnterParticleEffects;
    public List<GameObject> m_OnExitParticleEffects;
    public List<GameObject> m_OnChargeLost;

    [Header("------- Debug -------")]
    public List<GameObject> m_activeParticleEffects;
    public List<GameObject> m_loopingParticleEffects;
    public List<GameObject> m_temporaryParticleEffects;
    public List<GameObject> m_trailParticleEffects;

    public List<GameObject> m_collidedWiths;
    public CubeEntityState m_ownStateScript;

    public void Start()
    {
        m_ownStateScript = GetComponent<CubeEntityState>();
    }
    public void LateUpdate()
    {
        m_collidedWiths.Clear();
    }

    // create
    public GameObject createParticleEffect(GameObject particleEffect)
    {
        if(particleEffect == null)
        {
            Debug.Log("Aborted: particle effect was null!");
            return null;
        }

        EntityIsParticleEffect particleScript = particleEffect.GetComponent<EntityIsParticleEffect>();
        if (particleScript == null)
        {
            Debug.Log("Aborted: particle script was null!");
            return null;
        }

        GameObject effect = null;
        if (particleScript.m_stayOnGameObject)
        {
            effect = Instantiate(particleEffect, this.gameObject.transform);
            effect.transform.position = transform.position;
        }
        else
        {
            effect = Instantiate(particleEffect);
            effect.transform.position = transform.position;
            Rigidbody rbEffect = effect.GetComponent<Rigidbody>();
            if (rbEffect != null)
            {
                Rigidbody rbThis = GetComponent<Rigidbody>();
                if (rbThis != null)
                {
                    rbEffect.velocity = rbThis.velocity * particleScript.m_startSpeed;
                }
            }
        }

        EntityIsParticleEffect script = effect.GetComponent<EntityIsParticleEffect>();
        script.setValues(this, particleScript);
        addEffectToLists(effect);
        return effect;
    }
    public static GameObject createParticleEffet(GameObject particleEffect, Vector3 spawnPosition)
    {
        
        return createParticleEffet(particleEffect, spawnPosition, Vector3.zero);
    }
    public static GameObject createParticleEffet(GameObject particleEffect, Vector3 spawnPosition, Vector3 startVelocity)
    {
        if (particleEffect == null)
        {
            Debug.Log("Aborted: particle effect was null!");
            return null;
        }

        EntityIsParticleEffect particleScript = particleEffect.GetComponent<EntityIsParticleEffect>();
        if (particleScript == null)
        {
            Debug.Log("Aborted: particle script was null!");
            return null;
        }

        GameObject effect = null;
        if (particleScript.m_stayOnGameObject || particleScript.m_startSpeed != 0)
        {
            Debug.Log("Warning: This should not happen!");
        }
        else
        {
            effect = Instantiate(particleEffect);
            effect.transform.position = spawnPosition;

            Rigidbody rbEffect = effect.GetComponent<Rigidbody>();
            if (rbEffect != null)
            {
                rbEffect.velocity = startVelocity;
            }
        }

        EntityIsParticleEffect script = effect.GetComponent<EntityIsParticleEffect>();
        script.setValues(null, particleScript);
        return effect;
    }

    // manage particle effects
    public void destroyParticleEffect(GameObject particleEffect)
    {
        if (particleEffect == null)
        {
            Debug.Log("Aborted: particle effect was null!");
            return;
        }

        if (!m_activeParticleEffects.Contains(particleEffect))
        {
            Debug.Log("Aborted: particle effect was not in the list!");
            return;
        }

        EntityIsParticleEffect particleScript = particleEffect.GetComponent<EntityIsParticleEffect>();
        if (particleScript == null)
        {
            Debug.Log("Aborted: particle script system was null!");
            return;
        }

        //removeEffectFromList(particleEffect);
        particleScript.prepareDesptroyObject();
    }

    public void destroyAllParticleEffects()
    {
        if (m_activeParticleEffects == null || m_activeParticleEffects.Count <= 0)
            return;

        for (int i = m_activeParticleEffects.Count - 1; i >= 0; i--)
        {
            GameObject particleEffect = m_activeParticleEffects[i];

            destroyParticleEffect(particleEffect);
        }

        if(m_activeParticleEffects.Count > 0)
        {
            Debug.Log("Oops!");
        }
    }
    public void destroyAllLoopingFromList()
    {
        if (m_loopingParticleEffects == null || m_loopingParticleEffects.Count <= 0)
            return;

        for (int i = m_loopingParticleEffects.Count - 1; i >= 0; i--)
        {
            if(m_loopingParticleEffects[i] != null)
                destroyParticleEffect(m_loopingParticleEffects[i]);
        }
    }
    public void destroyAllTrailsFromList()
    {
        if (m_trailParticleEffects == null || m_trailParticleEffects.Count <= 0)
            return;

        for (int i = m_trailParticleEffects.Count - 1; i >= 0; i--)
        {
            if (m_trailParticleEffects[i] != null)
                destroyParticleEffect(m_trailParticleEffects[i]);
        }
    }

    // manage lists
    void addEffectToLists(GameObject particleEffect)
    {
        if(particleEffect == null)
        {
            Debug.Log("Aborted: particle effect was null!");
            return;
        }
        
        EntityIsParticleEffect particleScript = particleEffect.GetComponent<EntityIsParticleEffect>();
        if (particleScript == null)
        {
            Debug.Log("Aborted: particle script was null!");
            return;
        }

        if (!m_activeParticleEffects.Contains(particleEffect))
            m_activeParticleEffects.Add(particleEffect);

        if (particleScript.m_isLoop && !particleScript.m_isTrail && !m_loopingParticleEffects.Contains(particleEffect))
            m_loopingParticleEffects.Add(particleEffect);
        else if (!particleScript.m_isLoop && !m_temporaryParticleEffects.Contains(particleEffect))
            m_temporaryParticleEffects.Add(particleEffect);
        else if (particleScript.m_isTrail && !m_trailParticleEffects.Contains(particleEffect))
            m_trailParticleEffects.Add(particleEffect);
        else
            Debug.Log("Oops?");

    }
    public void removeEffectFromList(GameObject particleEffect)
    {
        if (particleEffect == null)
        {
            Debug.Log("Aborted: particle effect was null!");
            return;
        }

        //ParticleSystem particleSystem = particleEffect.GetComponent<ParticleSystem>();
        //if (particleSystem)
        //{
        //    Debug.Log("Aborted: particle system was null!");
        //    return;
        //}

        EntityIsParticleEffect particleScript = particleEffect.GetComponent<EntityIsParticleEffect>();
        if(particleScript == null)
        {
            Debug.Log("Aborted: particle script was null!");
            return;
        }

        if (m_activeParticleEffects.Contains(particleEffect))
            m_activeParticleEffects.Remove(particleEffect);

        if (particleScript.m_isLoop && !particleScript.m_isTrail && m_loopingParticleEffects.Contains(particleEffect))
            m_loopingParticleEffects.Remove(particleEffect);
        else if (!particleScript.m_isLoop && m_temporaryParticleEffects.Contains(particleEffect))
            m_temporaryParticleEffects.Remove(particleEffect);
        else if (particleScript.m_isTrail && m_trailParticleEffects.Contains(particleEffect))
            m_trailParticleEffects.Remove(particleEffect);
    }

    // initial stuff
    public void createOnEnterParticleEffects()
    {
        foreach(GameObject particleEffect in m_OnEnterParticleEffects)
        {
            if (particleEffect == null)
            {
                Debug.Log("Aborted: particle effect was null!");
                continue;
            }

            EntityIsParticleEffect particleScript = particleEffect.GetComponent<EntityIsParticleEffect>();
            if (particleScript == null)
            {
                Debug.Log("Aborted: particle script was null!");
                continue;
            }

            createParticleEffect(particleEffect);
        }
    }

    // deathrattle
    public void createOnExitParticleEffects()
    {
        foreach (GameObject particleEffect in m_OnExitParticleEffects)
        {
            if(particleEffect == null)
            {
                Debug.Log("Aborted: particle effect was null!");
                continue;
            }

            EntityIsParticleEffect particleScript = particleEffect.GetComponent<EntityIsParticleEffect>();
            if (particleScript == null)
            {
                Debug.Log("Aborted: particle script was null!");
                continue;
            }
            createParticleEffect(particleEffect);
        }
    }

    // Collision
    public void registerParticleCollision(GameObject other)
    {
        if (m_collidedWiths.Contains(other))
            return;
        m_collidedWiths.Add(other);


        CubeEntityParticleSystem otherParticleSystem = (CubeEntityParticleSystem)Utility.getComponentInParents<CubeEntityParticleSystem>(other.transform);// other.GetComponent<CubeEntityParticleSystem>();
        CubeEntityState otherStateScript = (CubeEntityState)Utility.getComponentInParents<CubeEntityState>(other.transform); //;other.GetComponent<CubeEntityState>();
        if (otherStateScript == null)
            Debug.Log("Warning: otherStateScript was null!");

        if (otherParticleSystem == null)
        {
            Debug.Log("Warning! otherParticleSystem was null!");
        }
        else
            otherParticleSystem.registerParticleCollision(gameObject);

        IOnParticleCollision[] scripts = GetComponentsInChildren<IOnParticleCollision>();
        for (int i = 0; i < scripts.Length; i++)
            scripts[i].onParticleCollision(other, otherStateScript.m_affiliation);
    }
    public void OnParticleCollision(GameObject other)
    {
        registerParticleCollision(other);
    }

    /*
    public void setValues(GameObject prefab)
    {
        CubeEntityParticleSystem script = prefab.GetComponent<CubeEntityParticleSystem>();
        m_OnEnterParticleEffects = script.m_OnEnterParticleEffects;
        m_OnExitParticleEffects = script.m_OnExitParticleEffects;
        m_OnChargeLost = script.m_OnChargeLost;
    }*/
    // interfaces
    public void onSwitchAffiliationRemove()
    {
        //createOnExitParticleEffects();
    }
    public void onCopyValues(ICopyValues copiable)
    {
        CubeEntityParticleSystem script = ((MonoBehaviour)copiable).GetComponent<CubeEntityParticleSystem>();


        m_activeParticleEffects = new List<GameObject>();
        m_loopingParticleEffects = new List<GameObject>();
        m_temporaryParticleEffects = new List<GameObject>();

        m_OnEnterParticleEffects = new List<GameObject>();
        foreach(GameObject o in script.m_OnEnterParticleEffects)
            m_OnEnterParticleEffects.Add(o);

        m_OnExitParticleEffects = new List<GameObject>();
        foreach (GameObject o in script.m_OnExitParticleEffects)
            m_OnExitParticleEffects.Add(o);

        m_OnChargeLost = new List<GameObject>();
        foreach (GameObject o in script.m_OnChargeLost)
            m_OnChargeLost.Add(o);


        m_collidedWiths = new List<GameObject>();
    }
    public void onStateEnter()
    {
        m_ownStateScript = GetComponent<CubeEntityState>();
        createOnEnterParticleEffects();
    }
    public void onStateExit()
    {
        destroyAllLoopingFromList();
        destroyAllTrailsFromList();

        createOnExitParticleEffects();
    }
    public void onRemove()
    {

    }
}
