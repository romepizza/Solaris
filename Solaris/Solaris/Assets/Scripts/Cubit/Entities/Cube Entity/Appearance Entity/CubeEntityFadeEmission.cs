using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityFadeEmission : ParentEntityDeleteOnStateChange
{
    [Header("----- SETTINGS -----")]
    public float m_duration;

    [Header("----- DEBUG -----")]
    public float m_defaultLightRadius;
    public float m_durationTimer;
    public float m_checkRdyTime;
    

    private CubeEntityAppearance m_appearanceScript;
    private TrailRenderer m_trailRenderer;

	
	// Update is called once per frame
	void Update ()
    {
        manageFade();
	}

    void manageFade()
    {
        if (m_checkRdyTime < Time.time)
        {
            float factor = m_durationTimer / m_duration;

            //Debug.Log(CubeEntityMaterials.getMaterial(m_appearanceScript, factor).GetColor("_EmissionColor").maxColorComponent);
            // Material
            gameObject.GetComponent<Renderer>().material = CubeEntityMaterials.getMaterial(m_appearanceScript, factor);
            // Light
            if (gameObject.GetComponent<Light>() != null)
                gameObject.GetComponent<Light>().range = m_defaultLightRadius * factor;
            //T Trail Renderer
            if(m_trailRenderer != null)
                m_trailRenderer.time = m_appearanceScript.m_trailTime * factor;
            
            m_durationTimer -= 0.1f;// Time.deltaTime;

            if (m_durationTimer <= 0f)
                Destroy(this);

            m_checkRdyTime = 0.09f + Time.time;
        }
    }


    /*
    public void setScriptByPrefab(GameObject prefab, CubeEntityAppearance appearanceScript)
    {
        m_appearanceScript = appearanceScript;
        m_defaultLightRadius = m_appearanceScript.m_lightRange;

        m_duration = prefab.GetComponent<CubeEntityFadeEmission>().m_duration;
        if (prefab.GetComponent<CubeEntityState>() != null)
        {
            m_duration = prefab.GetComponent<CubeEntityState>().m_duration;
        }

        m_trailRenderer = GetComponent<TrailRenderer>();


        m_durationTimer = m_duration;
    }
    */
}
