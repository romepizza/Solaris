using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralUIStuff : MonoBehaviour
{
    public GameObject[] enablePanel;
    public GameObject[] disablePanel;
    public GameObject[] togglePanel;

    public GameObject logInformationObject;

    public void onClickChangePanel()
    {
        foreach (GameObject panel in enablePanel)
        {
            if (panel != null)
                panel.SetActive(true);
        }
        foreach (GameObject panel in disablePanel)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }

    public void onClickTogglePanel()
    {
        foreach (GameObject panel in togglePanel)
        {
            if (panel != null)
                panel.SetActive(!panel.activeSelf);
        }
    }

    public void loadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void resetLevel(int sceneIndex)
    {
        GameObject.Find("GeneralScriptObject").GetComponent<Options>().isFreeze = false;
        Time.timeScale = 1;
        Cursor.visible = true;
        SceneManager.LoadScene(sceneIndex);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
