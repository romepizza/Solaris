using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeEntityRessourceManager : MonoBehaviour, ICopiable, IRemoveOnStateChange
{
    public static string[] s_ressourceNames;
    public static string s_ressourceName0 = "Offensive";
    public static string s_ressourceName1 = "Defensive";
    public static string s_ressourceName2 = "Utility";
    public static bool s_staticsInitialized;

    public GameObject m_displayOffensive;
    public GameObject m_displayDefensive;
    public GameObject m_displayUtility;

    [Header("------- Settings -------")]

    [Header("--- Ressources ---")]
    public float[] m_ressourceMaxValues;
    public float[] m_ressourceStartValues;
    public float[] m_ressourceRegenerations;

    [Header("------- Debug -------")]
    public bool m_isInitialized;
    public Dictionary<string, int> m_dictionary;

    public float[] m_ressourcesCurrentValues;

	// Use this for initialization
	void Start ()
    {
        if (!s_staticsInitialized)
            initializeStatics();

        if (!m_isInitialized && gameObject.layer == 9) // player layer playerlayer
            initializeStuff();
	}
    void initializeStatics()
    {
        s_ressourceNames = new string[3];
        s_ressourceNames[0] = s_ressourceName0;
        s_ressourceNames[1] = s_ressourceName1;
        s_ressourceNames[2] = s_ressourceName2;

        s_staticsInitialized = true;
    }
    void initializeStuff()
    {
        validateLengths();

        for (int i = 0; i < m_ressourceMaxValues.Length; i++)
        {
            m_ressourcesCurrentValues[i] = m_ressourceStartValues[i];
        }

        m_isInitialized = true;
    }
    void validateLengths()
    {
        if (m_ressourceMaxValues == null)
            Debug.Log("Warning");
        if (m_ressourceStartValues == null)
            Debug.Log("Warning");
        if (m_ressourceRegenerations == null)
            Debug.Log("Warning");

        int lenght = m_ressourceMaxValues.Length;

        if (m_ressourceStartValues.Length != lenght)
            Debug.Log("Warning");
        if (m_ressourceRegenerations.Length != lenght)
            Debug.Log("Warning");

        m_dictionary = new Dictionary<string, int>();
        m_dictionary.Add(s_ressourceNames[0], 0);
        m_dictionary.Add(s_ressourceNames[1], 1);
        m_dictionary.Add(s_ressourceNames[2], 2);

        m_ressourcesCurrentValues = new float[3];
    }
	
	// Update is called once per frame
	void Update ()
    {
        regenerateRessources();
    }
    void regenerateRessources()
    {
        //for(int i = 0; i < m_ressourceRegenerations.Length; i++)
        //{
        //    m_ressourcesCurrentValues[i] = Mathf.Clamp(m_ressourcesCurrentValues[i] + m_ressourceRegenerations[i] * Time.deltaTime, 0, m_ressourceMaxValues[i]);
        //}
        addRessources(Utility.multiplyFloatArray(m_ressourceRegenerations, Time.deltaTime));
    }

    // getter
    public bool hasEnoughRessurces(float[] costs)
    {
        if (costs.Length != m_ressourcesCurrentValues.Length)
            Debug.Log("Warning: lengths weren't the same!");

        bool hasEnough = true;

        for (int i = 0; i < m_ressourcesCurrentValues.Length; i++)
        {
            if (m_ressourcesCurrentValues[i] < -costs[i])
            {
                hasEnough = false;
                break;
            }
        }

        return hasEnough;
    }
    public float[] addRessources(float[] addRessources)
    {
        float[] changeValue = new float[m_ressourcesCurrentValues.Length];

        for(int i = 0; i < m_ressourcesCurrentValues.Length; i++)
        {
            float oldValue = m_ressourcesCurrentValues[i];
            float newValue = setRessourceValue(m_ressourcesCurrentValues[i] + addRessources[i], i);
            changeValue[i] = newValue - oldValue;
        }

        return changeValue;
    }
    public float setRessourceValue(float changeToRessource, int index)
    {
        float newValue = Mathf.Clamp(changeToRessource, 0, m_ressourceMaxValues[index]);
        m_ressourcesCurrentValues[index] = newValue;
        updateDisplays();
        return newValue;
    }

    // Display
    void updateDisplays()
    {
        if (gameObject.layer != Constants.s_playerLayer)
            return;

        if(m_displayOffensive != null)
        {
            Text text = m_displayOffensive.GetComponent<Text>();
            if (text != null)
                text.text = "" + (int)m_ressourcesCurrentValues[0];
        }
        if (m_displayDefensive != null)
        {
            Text text = m_displayDefensive.GetComponent<Text>();
            if (text != null)
                text.text = "" + (int)m_ressourcesCurrentValues[1];
        }
        if (m_displayUtility != null)
        {
            Text text = m_displayUtility.GetComponent<Text>();
            if (text != null)
                text.text = "" + (int)m_ressourcesCurrentValues[2];
        }
    }

    public void onCopy(ICopiable copiable)
    {
        CubeEntityRessourceManager copy = (CubeEntityRessourceManager)copiable;

        m_ressourceMaxValues = new float[copy.m_ressourceMaxValues.Length];
        for (int i = 0; i < copy.m_ressourceMaxValues.Length; i++)
        {
            m_ressourceMaxValues[i] = copy.m_ressourceMaxValues[i];
        }
        m_ressourceStartValues = new float[copy.m_ressourceStartValues.Length];
        for (int i = 0; i < copy.m_ressourceStartValues.Length; i++)
        {
            m_ressourceStartValues[i] = copy.m_ressourceStartValues[i];
        }
        m_ressourceRegenerations = new float[copy.m_ressourceRegenerations.Length];
        for (int i = 0; i < copy.m_ressourceRegenerations.Length; i++)
        {
            m_ressourceRegenerations[i] = copy.m_ressourceRegenerations[i];
        }

        if (!m_isInitialized)
            initializeStuff();
        
    }
    public void onRemove()
    {
        Destroy(this);
    }
}
