using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntitySoundInstance : MonoBehaviour
{
    public const float s_c = 30000f;

    public AudioSource m_audioSource;
    [Header("------- Settings -------")]
    public bool m_destroyAfterwards;
    public bool m_stayOnGameObject;
    public bool m_isLoop;
    public float m_delay;
    public float m_duration;

    [Header("--- (Audio) ---")]
    public float m_volumeFactorMin = 1f;
    public float m_volumeFactorMax = 1f;
    public float m_pitchFactorMin = 1f;
    public float m_pitchFactorMax = 1f;

    [Header("-------- Debug --------")]
    public List<CubeEntitySoundManager> m_registeredIn;
    public float m_removeTime;
    public float m_delayRdy;

    // Use this for initialization
    public void initialize()
    {
        m_registeredIn = new List<CubeEntitySoundManager>();
        m_removeTime = float.MaxValue;

        m_delayRdy = m_delay + Time.time;
        if (m_delay < 0)
        {
            m_delayRdy = (transform.position - Constants.getPlayer().transform.position).magnitude / s_c + Time.time;
        }

        setSoundValues();
    }

    void setSoundValues()
    {
        if(m_audioSource == null)
        {
            Debug.Log("Warning: audioSource was null!");
            m_audioSource = GetComponent<AudioSource>();
            if(m_audioSource == null)
            {
                Debug.Log("Warning: audioSource was null!");
                return;
            }
        }
        m_audioSource.volume *= Random.Range(m_volumeFactorMin, m_volumeFactorMax);
        m_audioSource.pitch *= Random.Range(m_pitchFactorMin, m_pitchFactorMax);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_delayRdy <= Time.time)
        {
            if (m_duration <= 0)
                m_removeTime = (m_audioSource.loop == true) ? float.MaxValue : m_audioSource.clip.length + Time.time;
            else
                m_removeTime = m_duration + Time.time;

            m_delayRdy = float.MaxValue;
            playSoundEffect();
        }

        if (m_removeTime <= Time.time)
            removeSound();
    }

    public void removeSound()
    {
        for(int i = m_registeredIn.Count - 1; i >= 0; i--)
        {
            m_registeredIn[i].removeEffectFromList(gameObject);
        }
        m_audioSource.Stop();
        Destroy(m_audioSource);
        Destroy(this);

        if (m_destroyAfterwards)
        {
            if (GetComponent<CubeEntitySystem>() == null)
            {
                Destroy(this.gameObject);
            }
            else
                Debug.Log("Aborted: Tried to delete sound gameObject, but it had a CubeEntitySystem script attached to it!");
        }
    }

    public void playSoundEffect()
    {
        m_audioSource.enabled = true;
        m_audioSource.Play();
    }

    public void registerSoundManager(CubeEntitySoundManager manager)
    {
        m_registeredIn.Add(manager);
    }

    // Old
    public void addSoundToObject(AudioSource source, float duration, bool destroyAfterwards, float delay)
    {
        m_duration = duration;
        m_destroyAfterwards = destroyAfterwards;
        m_delay = delay;
        m_delayRdy = m_delay + Time.time;


        //if (m_duration <= 0)
        //m_duration = source.clip.length;
        //m_finishTime = m_duration + Time.time;
        createAudioSource(source);
    }

    void createAudioSource(AudioSource original)
    {
        m_audioSource = gameObject.AddComponent<AudioSource>();

        m_audioSource.clip                          = original.clip;
        m_audioSource.outputAudioMixerGroup         = original.outputAudioMixerGroup;
        m_audioSource.mute                          = original.mute;
        m_audioSource.bypassEffects                 = original.bypassEffects;
        m_audioSource.bypassListenerEffects         = original.bypassListenerEffects;
        m_audioSource.bypassReverbZones             = original.bypassReverbZones;
        m_audioSource.playOnAwake                   = false;//m_audioSource.playOnAwake = original.playOnAwake;
        m_audioSource.loop                          = original.loop;
        m_audioSource.priority                      = original.priority;
        m_audioSource.volume                        = original.volume;
        m_audioSource.pitch                         = original.pitch;
        m_audioSource.panStereo                     = original.panStereo;
        m_audioSource.spatialBlend                  = original.spatialBlend;
        m_audioSource.reverbZoneMix                 = original.reverbZoneMix;
        m_audioSource.dopplerLevel                  = original.dopplerLevel;
        m_audioSource.spread                        = original.spread;
        m_audioSource.minDistance                   = original.minDistance;
        m_audioSource.maxDistance                   = original.maxDistance;
        m_audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, original.GetCustomCurve(AudioSourceCurveType.CustomRolloff));
        m_audioSource.rolloffMode = AudioRolloffMode.Custom;
    }
}
