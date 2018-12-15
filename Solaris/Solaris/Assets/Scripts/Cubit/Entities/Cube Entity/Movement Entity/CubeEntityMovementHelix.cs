using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityMovementHelix //: CubeEntityMovementAbstract
{
    /*
    [Header("------- SETTINGS -------")]


    

    [Header("------- DEBUG -------")]
    public bool m_isInitialized;
    public Vector3 m_targetDirection;
    public Vector3 m_targetPoint;
    // Use this for initialization
    void Start () {
		
	}
    public void initializeStuff()
    {
        
    }


    // Update is called once per frame
    void Update () {
		
	}

    // copy
    void setValues(CubeEntityMovementHoming copyScript)
    {
        m_target = copyScript.m_target;
        m_useThis = copyScript.m_useThis;


    }

    // abstract
    public override void prepareDestroyScript()
    {
        //Debug.Log(m_movementScript);
        //if (m_movementScript.m_movementComponents.Contains(this))
        //m_movementScript.m_movementComponents.Remove(this);
        Destroy(this);
    }
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CubeEntityMovementHoming)baseScript);
    }
    public override void onPostCopy()
    {
        //m_movementScript = GetComponent<CubeEntityMovement>();
    }
    public override void pasteScript(EntityCopiableAbstract baseScript, GameObject target, Vector3 targetPosition)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CubeEntityMovementHoming)baseScript);
        m_target = target;
        m_targetPoint = targetPosition;
    }

    public override bool updateDestroy()
    {
        throw new System.NotImplementedException();
    }*/
}
