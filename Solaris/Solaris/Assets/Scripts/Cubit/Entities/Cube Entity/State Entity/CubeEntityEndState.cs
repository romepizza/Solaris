using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityEndState : EntityCopiableAbstract, IRemoveOnStateChange
{
    [Header("----- SETTINGS -----")]
    public float m_duration;

    [Header("----- DEBUG -----")]
    public float m_durationEndTime;

    private CubeEntitySystem m_cubeSystemScript;
	
	// Update is called once per frame
	void Update ()
    {
		if(m_durationEndTime < Time.time)
        {
            //if (GetComponent<MonsterEntityBase>() != null)
            //GetComponent<MonsterEntityBase>().die();
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
        }
	}

    public void setValuesByPrefab(CubeEntityState stateScript, CubeEntitySystem systemScript)
    {
        // CubeEntityState stateScript = prefab.GetComponent<CubeEntityState>();

        if (stateScript != null)
        {
            m_duration = stateScript.m_duration;
            m_durationEndTime = stateScript.m_duration + Time.time;
            m_cubeSystemScript = systemScript;
        }
        else
            Debug.Log("Warning: stateScript was null!");
    }

    public void setDuration(float duration, CubeEntitySystem systemScript)
    {
        m_duration = duration;
        m_durationEndTime = duration + Time.time;
        m_cubeSystemScript = systemScript;
    }

    public void onRemove()
    {
        Destroy(this);
    }

    /*
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        
    }

    // abstract
    public override void prepareDestroyScript()
    {
        Destroy(this);
    }
    public override void onPostCopy()
    {
        
    }*/
}
