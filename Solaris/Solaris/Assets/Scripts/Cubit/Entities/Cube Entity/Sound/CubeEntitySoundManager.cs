using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntitySoundManager : MonoBehaviour, IStateOnStateChange, ICopyValues, IPrepareRemoveOnStateChange
{
    [Header("------- Settings -------")]
    public List<GameObject> m_onEnterSoundEffects;
    public List<GameObject> m_onExitSoundEffects;


    [Header("------- Debug -------")]
    public List<GameObject> m_activeSoundEffects;
    public List<GameObject> m_loopingSoundEffects;
    public List<GameObject> m_temprarySoundEffects;

    // create
    void createSoundeffect(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.Log("Aborted: soundEffect was null!");
            return;
        }

        CubeEntitySoundInstance instanceScript = prefab.GetComponent<CubeEntitySoundInstance>();
        if (instanceScript == null)
        {
            Debug.Log("Aborted: instance was null!");
            return;
        }

        GameObject instance = null;
        if (instanceScript.m_stayOnGameObject)
        {
            instance = Instantiate(prefab, this.gameObject.transform);
            instance.transform.position = transform.position;
        }
        else
        {
            instance = Instantiate(prefab);
            instance.transform.position = transform.position;
            Rigidbody rbEffect = instance.GetComponent<Rigidbody>();
            if (rbEffect != null)
            {
                Rigidbody rbThis = GetComponent<Rigidbody>();
                if (rbThis != null)
                {
                    //rbEffect.velocity = rbThis.velocity * particleScript.m_startSpeed;
                }
            }
        }

        CubeEntitySoundInstance script = instance.GetComponent<CubeEntitySoundInstance>();
        if(script == null)
        {
            Debug.Log("Aborted: soundInstanceScript was null!");
            return;
        }
        script.initialize();
        script.registerSoundManager(this);
        //script.addSoundToObject(this, particleScript);
        addEffectToLists(instance);
    }

    // destroy
    void destroySoundEffect(GameObject effect)
    {
        if (effect == null)
        {
            Debug.Log("Aborted: particle effect was null!");
            return;
        }

        if (!m_activeSoundEffects.Contains(effect))
        {
            Debug.Log("Aborted: particle effect was not in the list!");
            return;
        }

        CubeEntitySoundInstance instanceScript = effect.GetComponent<CubeEntitySoundInstance>();
        if (instanceScript == null)
        {
            Debug.Log("Aborted: instanceScript system was null!");
            return;
        }

        //removeEffectFromList(effect);
        instanceScript.removeSound();
    }
    void destroyAllSoundEffects()
    {
        if (m_activeSoundEffects == null || m_activeSoundEffects.Count <= 0)
            return;

        for (int i = m_activeSoundEffects.Count - 1; i >= 0; i--)
        {
            if (m_activeSoundEffects[i] != null)
                destroySoundEffect(m_activeSoundEffects[i]);//.GetComponent<CubeEntitySoundInstance>().removeSound();
        }
    }
    void destroyAllLoopingFromList()
    {
        if (m_loopingSoundEffects == null || m_loopingSoundEffects.Count <= 0)
            return;

        for (int i = m_loopingSoundEffects.Count - 1; i >= 0; i--)
        {
            if (m_loopingSoundEffects[i] != null)
            {
                destroySoundEffect(m_loopingSoundEffects[i]);
            }
        }
    }
    // add & remove from lists
    void addEffectToLists(GameObject effect)
    {
        if(effect == null)
        {
            Debug.Log("Aborted: effect was null!");
            return;
        }
        if(m_activeSoundEffects.Contains(effect))
        {
            Debug.Log("Aborted: effect already in the list");
            return;
        }
        m_activeSoundEffects.Add(effect);

        CubeEntitySoundInstance instanceScript = effect.GetComponent<CubeEntitySoundInstance>();
        if (instanceScript != null && instanceScript.m_isLoop)
        {
            m_loopingSoundEffects.Add(effect);
        }
    }
    public void removeEffectFromList(GameObject effect)
    {
        if (effect == null)
        {
            Debug.Log("Aborted: effect was null!");
            return;
        }
        if (!m_activeSoundEffects.Contains(effect))
        {
            //Debug.Log("Aborted: effect was not in the list");
            return;
        }
        //Debug.Log("ASD");
        m_activeSoundEffects.Remove(effect);
        if (m_loopingSoundEffects.Contains(effect))
            m_loopingSoundEffects.Remove(effect);
    }
    void removeAllEffectsFromList()
    {
        for (int i = m_activeSoundEffects.Count - 1; i >= 0; i--)
        {
            removeEffectFromList(m_activeSoundEffects[i]);
        }
    }

    void createOnEnterSoundEffects()
    {
        foreach (GameObject soundEffect in m_onEnterSoundEffects)
        {
            if (soundEffect == null)
            {
                Debug.Log("Aborted: sound effect was null!");
                continue;
            }

            CubeEntitySoundInstance soundScript = soundEffect.GetComponent<CubeEntitySoundInstance>();
            if (soundEffect == null)
            {
                Debug.Log("Aborted: sound script was null!");
                continue;
            }

            createSoundeffect(soundEffect);
        }
    }

    void createOnExitSoundEffects()
    {
        foreach (GameObject soundEffect in m_onExitSoundEffects)
        {
            if (soundEffect == null)
            {
                Debug.Log("Aborted: sound effect was null!");
                continue;
            }

            CubeEntitySoundInstance soundScript = soundEffect.GetComponent<CubeEntitySoundInstance>();
            if (soundScript == null)
            {
                Debug.Log("Aborted: sound script was null!");
                continue;
            }

            createSoundeffect(soundEffect);
        }
    }


    public void onCopyValues(ICopyValues copiable)
    {
        CubeEntitySoundManager script = ((MonoBehaviour)copiable).GetComponent<CubeEntitySoundManager>();

        m_onEnterSoundEffects = new List<GameObject>();
        m_onExitSoundEffects = new List<GameObject>();

        m_onEnterSoundEffects = new List<GameObject>();
        foreach (GameObject o in script.m_onEnterSoundEffects)
            m_onEnterSoundEffects.Add(o);

        m_onExitSoundEffects = new List<GameObject>();
        foreach (GameObject o in script.m_onExitSoundEffects)
            m_onExitSoundEffects.Add(o);

        m_activeSoundEffects = new List<GameObject>();
        m_loopingSoundEffects = new List<GameObject>();
        m_temprarySoundEffects = new List<GameObject>();
    }

    public void onStateEnter()
    {
        createOnEnterSoundEffects();
    }
    public void onStateExit()
    {
        //removeAllEffectsFromList();
        createOnExitSoundEffects();
    }
    public void onStateChangePrepareRemove()
    {
        destroyAllLoopingFromList();
    }
}
