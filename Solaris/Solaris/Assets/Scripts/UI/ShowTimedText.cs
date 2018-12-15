using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowTimedText : MonoBehaviour
{
    public float maxDuration;
    public float currentDuration;
    public bool isShowingTimed;
	// Use this for initialization
	void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isShowingTimed)
        {
            if (currentDuration < maxDuration)
                currentDuration += Time.deltaTime;
            else
            {
                GetComponent<Text>().text = "";
                isShowingTimed = false;
            }
        }
	}

    public void showText(string str, float duration)
    {
        GetComponent<Text>().text = str;
        maxDuration = duration;
        currentDuration = 0;
        isShowingTimed = true;
    }
}
