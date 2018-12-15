using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityMaterials : MonoBehaviour
{

    [Header("----- SETTINGS -----")]
    [Header("--- (Neutral) ---")]
    // 0
    public Material m_MATERIAL_INACTIVE;
    private static Material[] s_MATERIALS_INACTIVE;
    // 1
    public Material m_MATERIAL_ACTIVE_NEUTRAL;
    public static Material[] s_MATERIALS_ACTIVE_NEUTRAL;

    [Header("--- (Player) ---")]
    // 2
    public Material m_MATERIAL_ACTIVE_PLAYER;
    public static Material[] s_MATERIALS_ACTIVE_PLAYER;
    // 3
    public Material m_MATERIAL_ATTACHED_PLAYER;
    public static Material[] s_MATERIALS_ATTACHED_PLAYER;

    [Header("--- (Drone) ---")]
    public Material m_MATERIAL_ACTIVE_DRONE;
    public static Material[] s_MATERIALS_ACTIVE_DRONE;

    public Material m_MATERIAL_ATTACHED_DRONE;
    public static Material[] s_MATERIALS_ATTACHED_DRONE;

    public Material m_MATERIAL_CORE_DRONE;
    public static Material[] s_MATERIALS_CORE_DRONE;

    [Header("--- (Enemy) ---")]
    [Header("- (Ejector) -")]
    // 4
    public Material m_MATERIAL_ACTIVE_ENEMY_EJECTOR;
    public static Material[] s_MATERIALS_ACTIVE_ENEMY_EJECTOR;
    // 5
    public Material m_MATERIAL_ATTACHED_ENEMY_EJECTOR;
    public static Material[] s_MATERIALS_ATTACHED_ENEMY_EJECTOR;
    // 6
    public Material m_MATERIAL_CORE_ENEMY_EJECTOR;
    public static Material[] s_MATERIALS_CORE_ENEMY_EJECTOR;

    [Header("- (Worm) -")]
    // 7
    public Material m_MATERIAL_ATTACHED_ENEMY_WORM;
    public static Material[] s_MATERIALS_ATTACHED_ENEMY_WORM;
    // 8
    public Material m_MATERIAL_CORE_ENEMY_WORM;
    public static Material[] s_MATERIALS_CORE_ENEMY_WORM;

    [Header("- (Morpher) -")]
    // 9
    public Material m_MATERIAL_ACTIVE_ENEMY_MORPHER;
    public static Material[] s_MATERIALS_ACTIVE_ENEMY_MORPHER;
    // 10
    public Material m_MATERIAL_ATTACHED_ENEMY_MORPHER;
    public static Material[] s_MATERIALS_ATTACHED_ENEMY_MORPHER;
    // 11
    public Material m_MATERIAL_CORE_ENEMY_MORPHER;
    public static Material[] s_MATERIALS_CORE_ENEMY_MORPHER;
    [Header("----- DEBUG -----")]
    public int m_minBrightness;

    //private static int cubeNumber = 0;
    private static CubeEntityMaterials s_Instance = null;
    private string m_objectName = "CubeScriptObject";

    // Use this for initialization
    void Start()
    {    
        m_objectName = this.gameObject.name;
        m_minBrightness = (int)(m_MATERIAL_INACTIVE.GetColor("_EmissionColor").maxColorComponent * 100.001f);


        createMaterials();
    }


    void createMaterials()
    {
        createInactiveMaterial();
        createActiveNeutralMaterial();

        createActivePlayerMaterial();
        createAttachedPlayerMaterial();

        createActiveDroneMaterial();
        createAttachedEnemyDroneMaterial();
        createCoreDroneMaterial();

        createActiveEnemyEjectorMaterial();
        createAttachedEnemyEjectorMaterial();
        createCoreEjectorMaterial();


        createAttachedEnemyWormMaterial();
        createCoreWormMaterial();

        createActiveEnemyMorpherMaterial();
        createAttachedEnemyMorpherMaterial();
        createCoreMorpherMaterial();
    }
    public static Material getMaterial(CubeEntityAppearance appearanceScript, float factor)
    {
        if(appearanceScript.m_material == getInstance().m_MATERIAL_ACTIVE_NEUTRAL)
        {
           return CubeEntityMaterials.getActiveNeutralMaterial(factor);
        }
        if (appearanceScript.m_material == getInstance().m_MATERIAL_ACTIVE_PLAYER)
        {
            return CubeEntityMaterials.getActivePlayerMaterial(factor);
        }
        if (appearanceScript.m_material == getInstance().m_MATERIAL_ACTIVE_ENEMY_EJECTOR)
        {
            return CubeEntityMaterials.getActiveEnemyEjectorMaterial(factor);
        }
        if(appearanceScript.m_material == getInstance().m_MATERIAL_ACTIVE_ENEMY_MORPHER)
        {
            return CubeEntityMaterials.getActiveEnemyMorpherMaterial(factor);
        }
        if(appearanceScript.m_material == getInstance().m_MATERIAL_ACTIVE_DRONE)
        {
            return CubeEntityMaterials.getActiveEnemyDroneMaterial(factor);
        }
        return null;
    }

    // Neutral
    void createInactiveMaterial()
    {
        if(m_MATERIAL_INACTIVE != null)
        {
            
        }
    }
    void createActiveNeutralMaterial()
    {
        if (m_MATERIAL_ACTIVE_NEUTRAL != null)
        {
            int brightness = (int)(m_MATERIAL_ACTIVE_NEUTRAL.GetColor("_EmissionColor").maxColorComponent * 100f);
            if (brightness <= 0)
            {
                Debug.Log("Warning: brightness less equal to zero! (" + brightness + ")");
                return;
            }
            int adjustedBrightness = brightness - m_minBrightness;
            adjustedBrightness = Mathf.Max(adjustedBrightness, 0);
            s_MATERIALS_ACTIVE_NEUTRAL = new Material[adjustedBrightness + 1];
            s_MATERIALS_ACTIVE_NEUTRAL[adjustedBrightness] = m_MATERIAL_ACTIVE_NEUTRAL;
            for (int i = adjustedBrightness - 1; i >= 0; i--)
            {
                Material tmpMat = new Material(m_MATERIAL_ACTIVE_NEUTRAL);
                float multiplier = (i + m_minBrightness) / (float)(brightness);
                tmpMat.SetColor("_EmissionColor", tmpMat.GetColor("_EmissionColor") * multiplier);
                s_MATERIALS_ACTIVE_NEUTRAL[i] = tmpMat;
            }
        }
    }
    public static Material getActiveNeutralMaterial(float factor)
    {
        factor = Mathf.Clamp01(factor);
        int index = (int)(factor * (s_MATERIALS_ACTIVE_NEUTRAL.Length - 1));
        return s_MATERIALS_ACTIVE_NEUTRAL[index];
    }

    // Player
    void createActivePlayerMaterial()
    {
        if (m_MATERIAL_ACTIVE_PLAYER != null)
        {
            int brightness = (int)(m_MATERIAL_ACTIVE_PLAYER.GetColor("_EmissionColor").maxColorComponent * 100f);
            if(brightness <= 0)
            {
                Debug.Log("Warning: brightness less equal to zero! (" + brightness + ")");
                return;
            }
            int adjustedBrightness = brightness - m_minBrightness;
            adjustedBrightness = Mathf.Max(adjustedBrightness, 0);
            s_MATERIALS_ACTIVE_PLAYER = new Material[adjustedBrightness + 1];
            s_MATERIALS_ACTIVE_PLAYER[adjustedBrightness] = m_MATERIAL_ACTIVE_PLAYER;
            for(int i = adjustedBrightness - 1; i >= 0; i--)
            {
                Material tmpMat = new Material(m_MATERIAL_ACTIVE_PLAYER);
                float multiplier = (i + m_minBrightness) / (float)(brightness);
                tmpMat.SetColor("_EmissionColor", tmpMat.GetColor("_EmissionColor") * multiplier);
                s_MATERIALS_ACTIVE_PLAYER[i] = tmpMat;
            }
        }
    }
    public static Material getActivePlayerMaterial(float factor)
    {
        factor = Mathf.Clamp01(factor);
        int index = (int)(factor * (s_MATERIALS_ACTIVE_PLAYER.Length - 1));

        return s_MATERIALS_ACTIVE_PLAYER[index];
    }
    void createAttachedPlayerMaterial()
    {

    }

    // Drone
    void createActiveDroneMaterial()
    {
        if (m_MATERIAL_ACTIVE_DRONE != null)
        {
            int brightness = (int)(m_MATERIAL_ACTIVE_DRONE.GetColor("_EmissionColor").maxColorComponent * 100f);
            if (brightness <= 0)
            {
                Debug.Log("Warning: brightness less equal to zero! (" + brightness + ")");
                return;
            }
            int adjustedBrightness = brightness - m_minBrightness;
            adjustedBrightness = Mathf.Max(adjustedBrightness, 0);
            s_MATERIALS_ACTIVE_DRONE = new Material[adjustedBrightness + 1];
            s_MATERIALS_ACTIVE_DRONE[adjustedBrightness] = m_MATERIAL_ACTIVE_DRONE;
            for (int i = adjustedBrightness - 1; i >= 0; i--)
            {
                Material tmpMat = new Material(m_MATERIAL_ACTIVE_DRONE);
                float multiplier = (i + m_minBrightness) / (float)(brightness);
                tmpMat.SetColor("_EmissionColor", tmpMat.GetColor("_EmissionColor") * multiplier);
                s_MATERIALS_ACTIVE_DRONE[i] = tmpMat;
            }
        }
    }
    public static Material getActiveEnemyDroneMaterial(float factor)
    {
        factor = Mathf.Clamp01(factor);
        int index = (int)(factor * (s_MATERIALS_ACTIVE_DRONE.Length - 1));
        return s_MATERIALS_ACTIVE_DRONE[index];
    }
    void createAttachedEnemyDroneMaterial()
    {

    }
    void createCoreDroneMaterial()
    {

    }

    // Ejector
    void createActiveEnemyEjectorMaterial()
    {
        if (m_MATERIAL_ACTIVE_ENEMY_EJECTOR != null)
        {
            int brightness = (int)(m_MATERIAL_ACTIVE_ENEMY_EJECTOR.GetColor("_EmissionColor").maxColorComponent * 100f);
            if (brightness <= 0)
            {
                Debug.Log("Warning: brightness less equal to zero! (" + brightness + ")");
                return;
            }
            int adjustedBrightness = brightness - m_minBrightness;
            adjustedBrightness = Mathf.Max(adjustedBrightness, 0);
            s_MATERIALS_ACTIVE_ENEMY_EJECTOR = new Material[adjustedBrightness + 1];
            s_MATERIALS_ACTIVE_ENEMY_EJECTOR[adjustedBrightness] = m_MATERIAL_ACTIVE_ENEMY_EJECTOR;
            for (int i = adjustedBrightness - 1; i >= 0; i--)
            {
                Material tmpMat = new Material(m_MATERIAL_ACTIVE_ENEMY_EJECTOR);
                float multiplier = (i + m_minBrightness) / (float)(brightness);
                tmpMat.SetColor("_EmissionColor", tmpMat.GetColor("_EmissionColor") * multiplier);
                s_MATERIALS_ACTIVE_ENEMY_EJECTOR[i] = tmpMat;
            }
        }
    }
    public static Material getActiveEnemyEjectorMaterial(float factor)
    {
        factor = Mathf.Clamp01(factor);
        int index = (int)(factor * (s_MATERIALS_ACTIVE_ENEMY_EJECTOR.Length - 1));
        return s_MATERIALS_ACTIVE_ENEMY_EJECTOR[index];
    }
    void createAttachedEnemyEjectorMaterial()
    {

    }
    void createCoreEjectorMaterial()
    {

    }

    // Worm
    void createAttachedEnemyWormMaterial()
    {

    }
    void createCoreWormMaterial()
    {

    }

    // Morpher
    void createActiveEnemyMorpherMaterial()
    {
        if (m_MATERIAL_ACTIVE_ENEMY_MORPHER != null)
        {
            int brightness = (int)(m_MATERIAL_ACTIVE_ENEMY_MORPHER.GetColor("_EmissionColor").maxColorComponent * 100f);
            if (brightness <= 0)
            {
                Debug.Log("Warning: brightness less equal to zero! (" + brightness + ")");
                return;
            }
            int adjustedBrightness = brightness - m_minBrightness;
            adjustedBrightness = Mathf.Max(adjustedBrightness, 0);
            s_MATERIALS_ACTIVE_ENEMY_MORPHER = new Material[adjustedBrightness + 1];
            s_MATERIALS_ACTIVE_ENEMY_MORPHER[adjustedBrightness] = m_MATERIAL_ACTIVE_ENEMY_MORPHER;
            for (int i = adjustedBrightness - 1; i >= 0; i--)
            {
                Material tmpMat = new Material(m_MATERIAL_ACTIVE_ENEMY_MORPHER);
                float multiplier = (i + m_minBrightness) / (float)(brightness);
                tmpMat.SetColor("_EmissionColor", tmpMat.GetColor("_EmissionColor") * multiplier);
                s_MATERIALS_ACTIVE_ENEMY_MORPHER[i] = tmpMat;
            }
        }
    }
    public static Material getActiveEnemyMorpherMaterial(float factor)
    {
        factor = Mathf.Clamp01(factor);
        int index = (int)(factor * (s_MATERIALS_ACTIVE_ENEMY_MORPHER.Length - 1));
        return s_MATERIALS_ACTIVE_ENEMY_MORPHER[index];
    }
    void createAttachedEnemyMorpherMaterial()
    {

    }
    void createCoreMorpherMaterial()
    {

    }



    public static CubeEntityMaterials getInstance()
    {
        if (s_Instance == null)
        {
            s_Instance = FindObjectOfType(typeof(CubeEntityMaterials)) as CubeEntityMaterials;
        }

        if (s_Instance == null)
        {
            Debug.Log("Singleton of CubeEntityMaterials not working properly!");
        }
        return s_Instance;
    }
}
