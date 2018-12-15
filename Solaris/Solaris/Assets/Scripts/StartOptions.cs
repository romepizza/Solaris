using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartOptions : MonoBehaviour
{

    public int m_targetFrameRate = 999;

    public bool isInMenu;
    public bool isInGame;
    public bool showMouse;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = m_targetFrameRate;
    }

    void Start()
    {
        if (!showMouse)
            Cursor.visible = false;
    }
}
