using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
    public GameObject player;
    public Canvas menu;
    public GameObject gameFinishedPanel;
    public Text changedText1;
    public Text changedText2;
    public Text showGrabbedText;
    public Text playerHpText;
    public GameObject fpsText;
    public GameObject goal;
    public GameObject Crosshair;
    public GameObject cubeSystem;

    // Use this for initialization
    [Header("----- SETTINGS -----")]
    public int[] lightCountNumbers;
    public bool showMouse;

    public bool m_allowSpawnEjector = true;
    public bool m_allowSpawnWorm = true;
    

    [Header("----- DEBUG -----")]
    public int colorNumberState;
    public bool isFreeze;
    public bool isShowingFps;
    public bool isShowingCrosshair;
    public bool isShowGrabbed;
    public bool isShowPlayerHp;
    public bool gameIsFinished;


    //private SkillGather skillGatherScript;
    void Start ()
    {
        //goal = GameObject.Find("Goal");
        //skillGatherScript = GameObject.Find("PlayerDrone").GetComponent<SkillGather>();
        player = GameObject.Find("PlayerDrone");

    }

    public void endGame()
    {
        gameFinishedPanel.SetActive(true);
        Time.timeScale = 0.0f;
        if(showMouse)
            Cursor.visible = true;
        gameIsFinished = true;
    }


    // ----- Change States -----
    public void changeGameState()
    {
        isFreeze = !isFreeze;

        if (!isFreeze)
            Time.timeScale = 1;
        else
            Time.timeScale = 0.0f;
        if(showMouse)
            Cursor.visible = isFreeze;
        menu.gameObject.SetActive(isFreeze);
    }



    // Update is called once per frame
    void Update ()
    {
        // ----- INPUTS -----

        // (de-)activate aim help
        if(Input.GetKeyDown(KeyCode.Y))
        {
            //changeAimHelp();
        }
        
        // Show Player Hp
        if(Input.GetKeyDown(KeyCode.F1))
        {
            //changeShowPlayerHp();
        }

        // Show Grabbed Cubes
        if (Input.GetKeyDown(KeyCode.F4))
        {
            //changeShowGrabbed();
        }

        // Monster Worm Spawn
        if (Input.GetKeyDown(KeyCode.G) && m_allowSpawnWorm)
        {
            //cubeSystem.GetComponent<MonsterManager>().createMonsterChase(1);
        }

        // Monster Throw Spawn
        if ((Input.GetKeyDown(KeyCode.T) || Input.GetButtonDown("LBumper")) && m_allowSpawnEjector)
        {
            //cubeSystem.GetComponent<MonsterManager>().createMonsterThrow(1);
        }
        // Esc
        if(Input.GetKeyDown(KeyCode.Escape) && !gameIsFinished)
        {
            changeGameState();
        }

        // FPS
        if(Input.GetKeyDown(KeyCode.F2))
        {
            //changeShowFps();
        }

        // Crosshair
        if(Input.GetKeyDown(KeyCode.F3))
        {
            //changeCrosshair();
        }

        // activate / deactivate Goal
        if (false && Input.GetKeyDown(KeyCode.F1))
        {
            goal.SetActive(!goal.activeSelf);
            string showText = "";
            if (goal.activeSelf)
                showText = "Disc activated";
            else
                showText = "Disc deactivated";
            changedText2.GetComponent<ShowTimedText>().showText(showText, 5.0f);
        }


        // Enable/Disable Post-Processing
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            Camera.main.GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>().enabled = !Camera.main.GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>().enabled;
            changedText2.GetComponent<ShowTimedText>().showText("Post-Processing set to " + Camera.main.GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>().enabled, 5.0f);
        }

        // Number of Lights
        if (Input.GetKeyDown(KeyCode.O))
        {
            colorNumberState++;
            colorNumberState = Mathf.Clamp(colorNumberState, 0, lightCountNumbers.Length - 1);
            QualitySettings.pixelLightCount = lightCountNumbers[colorNumberState];

            string showText = "maximum shown lights increased to " + lightCountNumbers[colorNumberState];
            changedText2.GetComponent<ShowTimedText>().showText(showText, 5.0f);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            colorNumberState--;
            colorNumberState = Mathf.Clamp(colorNumberState, 0, lightCountNumbers.Length - 1);
            QualitySettings.pixelLightCount = lightCountNumbers[colorNumberState];
            string showText = "maximum shown lights decreased to " + lightCountNumbers[colorNumberState];
            changedText2.GetComponent<ShowTimedText>().showText(showText, 5.0f);
        }

        // Light Intensity
        //if(Inpu)
    }

    public void returnMenuSettingsNoLog(int sceneIndex)
    {
        isFreeze = false;
        Time.timeScale = 1;
        if(showMouse)
            Cursor.visible = true;
        SceneManager.LoadScene(sceneIndex);
    }

    public void returnMenuSettingsLog(int sceneIndex)
    {
        isFreeze = false;
        Time.timeScale = 1;
        if(showMouse)
            Cursor.visible = true;
        SceneManager.LoadScene(sceneIndex);
    }
}
