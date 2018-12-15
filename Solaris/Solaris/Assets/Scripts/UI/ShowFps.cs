using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowFps : MonoBehaviour
{
    public GameObject m_textObject;
    [Header("------- Settings -------")]
    public int m_considerLastFramesNumber;
    public float m_refreshTextTime;
    public string m_prefixText;

    [Header("------- Debug -------")]
    public float m_finalFps;
    public Queue<float> m_lastUpdateTimes;

    private Text m_fpsText;
    private bool m_showText;
    private int m_sizeActual;
    private float m_refreshTextRdyTime;

	// Use this for initialization
	void Start ()
    {
        if (m_textObject == null)
            m_fpsText = GetComponent<Text>();
        else
            m_fpsText = m_textObject.GetComponent<Text>();


        if (m_fpsText == null)
        {
            m_showText = false;
            Debug.Log("Aborted: text object or text component is null!");
            return;
        }

        m_showText = true;


        m_lastUpdateTimes = new Queue<float>();
        m_sizeActual = m_considerLastFramesNumber;
        if (m_sizeActual < 1)
        {
            Debug.Log("Warning: The parameter m_considerLastFramesNumber of the class ShowFps should be above 0!" +
                        " It has been set to the default value of 1.");
            m_sizeActual = 1;
        }
    }

	
	// Update is called once per frame
	void Update ()
    {
        manageFps();
	}

    void manageFps()
    {
        if (!m_showText)
            return;

        calculateFps();
        displayFps();
    }

    void calculateFps()
    {
        float fps = 0;
        if (m_finalFps > 0)
            fps = 1 / m_finalFps;

        if(m_lastUpdateTimes.Count < m_sizeActual)
        {
            if(m_lastUpdateTimes.Count > 1)
                fps *= m_lastUpdateTimes.Count;

            m_lastUpdateTimes.Enqueue(Time.deltaTime);
            fps += Time.deltaTime;
            fps /= m_lastUpdateTimes.Count;
        }
        else
        {
            float oldValue = m_lastUpdateTimes.Dequeue();
            fps -= oldValue / (m_lastUpdateTimes.Count + 1);
            m_lastUpdateTimes.Enqueue(Time.deltaTime);
            fps += Time.deltaTime / m_lastUpdateTimes.Count;
        }

        if(fps > 0)
            m_finalFps = 1 / fps;
    }

    void displayFps()
    {
        if (m_refreshTextRdyTime > Time.time)
            return;

        m_fpsText.text = m_prefixText + (int)m_finalFps;
        m_refreshTextRdyTime = m_refreshTextTime + Time.time;
    }
}
