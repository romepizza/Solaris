using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAlteration : MonoBehaviour
{
    [Header("------- Settings -------")]
    public bool m_loop;
    [Header("--- Intensty ---")]
    public float m_intensityDuration = -1;
    public float m_intensityDelay;
    public AnimationCurve m_intensityAnimationCurve;

    [Header("------- Debug -------")]
    public Light m_light;

    public bool m_intensityIsAltering;
    public float m_intensityRdy;
    public float m_intensityCurrentDuration;
    public float m_intensityDefault;

    // Use this for initialization
    void Start ()
    {
        initializeStuff();
    }
    void initializeStuff()
    {
        if (m_light == null)
        {
            m_light = GetComponent<Light>();
            if (m_light == null)
                Debug.Log("Warning: Light not found!");
        }

        // intensity
        m_intensityDefault = m_light.intensity;
        if (m_intensityDuration == -1)
        {
            EntityIsParticleEffect e = (EntityIsParticleEffect)Utility.getComponentInParents<EntityIsParticleEffect>(transform);
            if (e != null)
                m_intensityDuration = (e).m_lifeTime;
            else
                m_intensityDuration = 0;
        }
        m_intensityRdy = Time.time + m_intensityDelay;
    }
	
	// Update is called once per frame
	void Update ()
    {
        manageIntensity();
	}

    void manageIntensity()
    {
        if (!m_intensityIsAltering && m_intensityRdy < Time.time)
            m_intensityIsAltering = true;

        if (!m_intensityIsAltering)
            return;

        if (m_intensityDuration == 0)
            return;

        m_intensityCurrentDuration += Time.deltaTime;
        float currentF = Mathf.Clamp01(m_intensityCurrentDuration / m_intensityDuration);
        float intensity = m_intensityDefault * m_intensityAnimationCurve.Evaluate(currentF);
        m_light.intensity = intensity;

        if (currentF == 1)
        {
            if (m_loop)
            {
                m_intensityCurrentDuration = 0;

            }
            else
            {
                m_intensityIsAltering = false;
                m_intensityRdy = float.MaxValue;
            }
        }
    }
}
