using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour, IStateOnStateChange
{
    public float m_speedOfSound;
    public static float s_speedOfSound;

    public GameObject m_sourceBackground;
    public static AudioSource s_sourceBackground;
    public GameObject m_sourceEjectorDeath;
    public static AudioSource s_sourceEjectorDeath;
    public GameObject m_sourcePlayerCubeHit;
    public static AudioSource s_sourcePlayerCubeHit;
    public GameObject m_sourceActivePlayerSound;
    public static AudioSource s_sourceActivePlayerSound;
    public GameObject m_sourceActiveEnemyEjectorSound;
    public static AudioSource s_sourceActiveEnemyEjectorSound;
    public GameObject m_sourceCoreEjectorSound;
    public static AudioSource s_sourceCoreEjectorSound;



    // Use this for initialization
    void Start ()
    {
        s_speedOfSound = m_speedOfSound;
        
        s_sourceEjectorDeath = m_sourceEjectorDeath.GetComponent<AudioSource>();
        s_sourcePlayerCubeHit = m_sourcePlayerCubeHit.GetComponent<AudioSource>();
        s_sourceActivePlayerSound = m_sourceActivePlayerSound.GetComponent<AudioSource>();
        s_sourceActiveEnemyEjectorSound = m_sourceActiveEnemyEjectorSound.GetComponent<AudioSource>();
        s_sourceCoreEjectorSound = m_sourceCoreEjectorSound.GetComponent<AudioSource>();
        s_sourceBackground = m_sourceBackground != null ? m_sourceBackground.GetComponent<AudioSource>() : null;


        SoundManager.addSoundBackground(Vector3.zero);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}



    // ------------ Add functions ----------------


    // Background
    public static CubeEntitySoundInstance addSoundBackground(GameObject gameObject)
    {
        if (gameObject != null)
        {
            float delay = 0;

            CubeEntitySoundInstance instance = gameObject.AddComponent<CubeEntitySoundInstance>();
            instance.addSoundToObject(s_sourceBackground, -1f, false, delay);
            return instance;
        }
        else
            Debug.Log("Aborted: Tried to access an object, that is null!");

        return null;
    }
    public static CubeEntitySoundInstance addSoundBackground(Vector3 position)
    {
        if (s_sourceBackground == null)
            return null;
        float delay = 0;

        GameObject placeHolderObject = new GameObject("SoundInstance");
        placeHolderObject.transform.position = position;
        CubeEntitySoundInstance instance = placeHolderObject.AddComponent<CubeEntitySoundInstance>();
        instance.addSoundToObject(s_sourceBackground, -1f, true, delay);
        return instance;
    }

    // ejector death
    public static CubeEntitySoundInstance addSoundEjectorDeath(GameObject gameObject)
    {
        if (gameObject != null)
        {
            float delay = getDelay(gameObject.transform.position);

            CubeEntitySoundInstance instance = gameObject.AddComponent<CubeEntitySoundInstance>();
            instance.addSoundToObject(s_sourceEjectorDeath, -1f, false, delay);
            return instance;
        }
        else
            Debug.Log("Aborted: Tried to access an object, that is null!");

        return null;
    }
    public static CubeEntitySoundInstance addSoundEjectorDeath(Vector3 position)
    {
        float delay = getDelay(position);

        GameObject placeHolderObject = new GameObject("SoundInstance");
        placeHolderObject.transform.position = position;
        CubeEntitySoundInstance instance = placeHolderObject.AddComponent<CubeEntitySoundInstance>();
        instance.addSoundToObject(s_sourceEjectorDeath, -1f, true, delay);
        return instance;
    }

    // player cube hit
    public static CubeEntitySoundInstance addSoundPlayerCubeHit(GameObject gameObject)
    {
        if (gameObject != null)
        {
            float delay = getDelay(gameObject.transform.position);

            CubeEntitySoundInstance instance = gameObject.AddComponent<CubeEntitySoundInstance>();
            instance.addSoundToObject(s_sourcePlayerCubeHit, -1f, false, delay);
            return instance;
        }
        else
            Debug.Log("Aborted: Tried to access an object, that is null!");

        return null;
    }
    public static CubeEntitySoundInstance addSoundPlayerCubeHit(Vector3 position)
    {
        //float delay = SoundManager.getDelay(position);

        GameObject placeHolderObject = new GameObject("SoundInstance");
        placeHolderObject.transform.position = position;
        CubeEntitySoundInstance instance = placeHolderObject.AddComponent<CubeEntitySoundInstance>();
        instance.addSoundToObject(s_sourcePlayerCubeHit, 2f, true, 0);
        return instance;
    }

    // Active enemy sound
    public static CubeEntitySoundInstance addSoundActivePlayerSound(GameObject gameObject)
    {
        if (gameObject != null)
        {
            float delay = 0f;

            CubeEntitySoundInstance instance = gameObject.AddComponent<CubeEntitySoundInstance>();
            instance.addSoundToObject(s_sourceActivePlayerSound, -1f, false, delay);
            return instance;
        }
        else
            Debug.Log("Aborted: Tried to access an object, that is null!");

        return null;
    }
    public static CubeEntitySoundInstance addSoundActivePlayerSound(Vector3 position)
    {
        float delay = 0f;

        GameObject placeHolderObject = new GameObject("SoundInstance");
        placeHolderObject.transform.position = position;
        CubeEntitySoundInstance instance = placeHolderObject.AddComponent<CubeEntitySoundInstance>();
        instance.addSoundToObject(s_sourceActivePlayerSound, -1f, true, delay);
        return instance;
    }
    // Active enemy sound
    public static CubeEntitySoundInstance addSoundActiveEnemyEjectorSound(GameObject gameObject)
    {
        if (gameObject != null)
        {
            float delay = 0f;

            CubeEntitySoundInstance instance = gameObject.AddComponent<CubeEntitySoundInstance>();
            instance.addSoundToObject(s_sourceActiveEnemyEjectorSound, -1f, false, delay);
            return instance;
        }
        else
            Debug.Log("Aborted: Tried to access an object, that is null!");

        return null;
    }
    public static CubeEntitySoundInstance addSoundActiveEnemyEjectorSound(Vector3 position)
    {
        float delay = 0f;

        GameObject placeHolderObject = new GameObject("SoundInstance");
        placeHolderObject.transform.position = position;
        CubeEntitySoundInstance instance = placeHolderObject.AddComponent<CubeEntitySoundInstance>();
        instance.addSoundToObject(s_sourceActiveEnemyEjectorSound, -1f, true, delay);
        return instance;
    }

    // Core ejector sound
    public static CubeEntitySoundInstance addSoundCoreEjectorSound(GameObject gameObject)
    {
        if (gameObject != null)
        {
            float delay = 0f;

            CubeEntitySoundInstance instance = gameObject.AddComponent<CubeEntitySoundInstance>();
            instance.addSoundToObject(s_sourceCoreEjectorSound, -1f, false, delay);
            return instance;
        }
        else
            Debug.Log("Aborted: Tried to access an object, that is null!");

        return null;
    }
    public static CubeEntitySoundInstance addSoundCoreEjectorSound(Vector3 position)
    {
        float delay = 0f;

        GameObject placeHolderObject = new GameObject("SoundInstance");
        placeHolderObject.transform.position = position;
        CubeEntitySoundInstance instance = placeHolderObject.AddComponent<CubeEntitySoundInstance>();
        instance.addSoundToObject(s_sourceCoreEjectorSound, -1f, true, delay);
        return instance;
    }


    public static void clearSounds(GameObject gameObject)
    {
        CubeEntitySoundInstance[] sounds = gameObject.GetComponents<CubeEntitySoundInstance>();
        for (int i = 0; i < sounds.Length; i++)
            sounds[i].removeSound();
    }

    // Intern
    public static float getDelay(Vector3 position)
    {
        float delay = 0;
        if (s_speedOfSound > 0)
            delay = Vector3.Distance(Camera.main.transform.position, position) / s_speedOfSound;
        else
            Debug.Log("Warning: speed of sound wasn't set properly! (" + s_speedOfSound + ")");

        return delay;
    }

    // interfaces
    public void onStateEnter()
    {

    }
    public void onStateExit()
    {
        clearSounds(gameObject);
    }
}
