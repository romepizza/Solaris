using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityAppearance : MonoBehaviour, ICopyValues
{
    
    [Header("----- SETTINGS -----")]
    [Header("--- (Material) ---")]
    public Material m_material;
    //public Color m_materialColor;
    //public float m_materialColorIntensity;
    //public bool m_isEmissive;
    //public float m_emissionIntensity;
    //public Color m_emissionColor;

    [Header("--- (Light) ---")]
    public Color m_lightColor;
    public float m_lightIntensity;
    public float m_lightRange;
    public LightShadows m_lightShadowType;
    [Header("--- (Trail Renderer) ---")]
    public Material m_trailMaterial;
    public float m_trailTime;
    public float m_trailWidth;
    public Color m_trailColor;
    public Gradient m_colorGradient;


    //[Header("----- DEBUG -----")]
    //public CubeEntitySystem m_entitySystemComponent;

    void Start ()
    {
        //m_entitySystemComponent = gameObject.GetComponent<CubeEntitySystem>();
        //if (m_entitySystemComponent == null)
        //    Debug.Log("Error: Tried to add entitySystemComponent, but failed!");
    }
    public void onCopyValues(ICopyValues baseScript)
    {
        
        CubeEntityAppearance appearanceSettingsScript = (CubeEntityAppearance)baseScript;
        if (appearanceSettingsScript != null)
        {
            m_material = appearanceSettingsScript.m_material;

            m_lightColor = appearanceSettingsScript.m_lightColor;
            m_lightIntensity = appearanceSettingsScript.m_lightIntensity;
            m_lightRange = appearanceSettingsScript.m_lightRange;
            m_lightShadowType = appearanceSettingsScript.m_lightShadowType;

            m_trailMaterial = appearanceSettingsScript.m_trailMaterial;
            m_trailTime = appearanceSettingsScript.m_trailTime;
            m_trailWidth = appearanceSettingsScript.m_trailWidth;
            m_trailColor = appearanceSettingsScript.m_trailColor;
            m_colorGradient = appearanceSettingsScript.m_colorGradient;

            applyCubeMaterialProperties();
            applyCubeLight();
            applyCubeTrailRenderer();
            GetComponent<Renderer>().enabled = appearanceSettingsScript.GetComponent<Renderer>().enabled;
        }
    }
    /*
    public void setAppearanceByScript(GameObject prefab)
    {
        CubeEntityAppearance appearanceSettingsScript = prefab.GetComponent<CubeEntityAppearance>();
        if (appearanceSettingsScript != null)
        {
            m_material = appearanceSettingsScript.m_material;

            m_lightColor = appearanceSettingsScript.m_lightColor;
            m_lightIntensity = appearanceSettingsScript.m_lightIntensity;
            m_lightRange = appearanceSettingsScript.m_lightRange;
            m_lightShadowType = appearanceSettingsScript.m_lightShadowType;

            m_trailMaterial = appearanceSettingsScript.m_trailMaterial;
            m_trailTime = appearanceSettingsScript.m_trailTime;
            m_trailWidth = appearanceSettingsScript.m_trailWidth;
            m_trailColor = appearanceSettingsScript.m_trailColor;
            m_colorGradient = appearanceSettingsScript.m_colorGradient;

            applyCubeMaterialProperties();
            applyCubeLight();
            applyCubeTrailRenderer();
            GetComponent<Renderer>().enabled = prefab.GetComponent<Renderer>().enabled;
        }
        
        /*
        CubeEntityFadeEmission fadeScript = prefab.GetComponent<CubeEntityFadeEmission>();
        if(fadeScript != null)
        {
            CubeEntityFadeEmission currentFadeScript = GetComponent<CubeEntityFadeEmission>();
            if(currentFadeScript == null)
                currentFadeScript = gameObject.AddComponent<CubeEntityFadeEmission >();
            currentFadeScript.setScriptByPrefab(prefab, this);
        }
        
    }

    public void setAppearanceMaterialComponent(Material material)
    {
        m_material = material;
    }

    public void setAppearanceMaterialComponent(Material material, Color materialColor, float materialColorIntensity, bool isEmissive, Color emissionColor, float emissionIntensity)
    {
        //if(material != null)
            m_material = material;
        //if(materialColor.r >= 0 && materialColor.g >= 0 && materialColor.b >= 0)
            //m_materialColor = materialColor;
        //if(materialColorIntensity >= 0)
            //m_materialColorIntensity = materialColorIntensity;
        //m_isEmissive = isEmissive;
        //if (emissionColor.r >= 0 && emissionColor.g >= 0 && emissionColor.b >= 0)
            //m_emissionColor = emissionColor;
        //if(emissionIntensity >= 0)
            //m_emissionIntensity = emissionIntensity;
    }

    public void setAppearianceLightComponent(Color lightColor, float lightIntensity, float lightRadius, LightShadows shadowType)
    {
        //if (lightColor.r >= 0 && lightColor.g >= 0 && lightColor.b >= 0)
            m_lightColor = lightColor;
        //if (lightIntensity >= 0)
            m_lightIntensity = lightIntensity;
        //if (lightRadius >= 0)
            m_lightRange = lightRadius;
        m_lightShadowType = shadowType;
    }

    public void setAppearianceTrailComponent(Material trailMaterial, Color trailColor, float trailTime, float trailWidth, Gradient gradient)
    {

        m_trailMaterial = trailMaterial;
        //if (trailColor.r >= 0 && trailColor.g >= 0 && trailColor.b >= 0)
            m_trailColor = trailColor;
        //if (trailTime >= 0)
            m_trailTime = trailTime;
        //if (trailWidth >= 0)
            m_trailWidth = trailWidth;

        m_colorGradient = gradient;
        
    }
    */

    void applyCubeMaterialProperties()
    {
        //if (m_material != null)
        {
            if(m_material != null)
                gameObject.GetComponent<MeshRenderer>().material = m_material;
            //}
            /*
            else
            {
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                mpb.SetColor("_Color", m_materialColor * m_materialColorIntensity);
                if (m_isEmissive)
                    GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                else
                    GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                mpb.SetColor("_EmissionColor", m_emissionColor * m_emissionIntensity);
                GetComponent<Renderer>().SetPropertyBlock(mpb);
            }
            */
        }
    }

    void applyCubeMaterial()
    {
        if (GetComponent<MeshRenderer>() == null)
            gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshRenderer>().material = m_material;
    }

    void applyCubeLight()
    {
        if(m_lightIntensity > 0 && m_lightRange > 0)
        {
            if (gameObject.GetComponent<Light>() == null)
                gameObject.AddComponent<Light>();

            GetComponent<Light>().color = m_lightColor;
            GetComponent<Light>().intensity = m_lightIntensity;
            GetComponent<Light>().range = m_lightRange;
            GetComponent<Light>().shadows = m_lightShadowType;
        }
            
        if (gameObject.GetComponent<Light>() != null && (m_lightIntensity <= 0 || m_lightRange <= 0))
            Destroy(gameObject.GetComponent<Light>());

        
    }

    void applyCubeTrailRenderer()
    {
        if (m_trailMaterial != null && m_trailTime > 0 && m_trailWidth > 0)
        {
            if (gameObject.GetComponent<TrailRenderer>() == null)
                gameObject.AddComponent<TrailRenderer>();

            TrailRenderer tr = GetComponent<TrailRenderer>();

            tr.material = m_trailMaterial;
            tr.colorGradient = m_colorGradient;
            //tr.startColor = m_trailColor;
            //tr.endColor = m_trailColor;
            tr.time = m_trailTime;
            tr.widthMultiplier = m_trailWidth;
            tr.colorGradient = new Gradient();

            Gradient gr = new Gradient();
            gr.SetKeys(
                new GradientColorKey[] { new GradientColorKey(m_colorGradient.colorKeys[0].color, 0.0f), new GradientColorKey(m_colorGradient.colorKeys[1].color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(0.5f, 1.0f) }
            );
            tr.colorGradient = gr;
        }
        else if (gameObject.GetComponent<TrailRenderer>() != null)
            Destroy(gameObject.GetComponent<TrailRenderer>());
        
    }
}
