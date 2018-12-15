using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntityAttachSystem : AttachSystemBase, IPrepareRemoveOnStateChange
{
    public GameObject m_showGrabbedNumberObject;
    [Header("----- SETTINGS -----")]
    public float m_duration = -1;
    

    [Header("--- (Catch System) ---")]
    public Vector3 m_catchOffset;

    /*
    [Header("--- (Smooth Arrival) ---")]
    public bool m_useSmoothArrival;
    public int m_lastCatchPositionsCount;
    */

        
    [Header("--- (Cube Movement) ---")]
    /*
    public float m_cubeMovementPower;
    public float m_maxSpeed;
    */
    [Header("----- DEBUG -----")]
    //public List<GameObject> m_cubeList;
    public AttachEntityBase m_attachEntity;
    //public List<CubeEntityMovementFollowPoint> m_grabbedCubesFollowPointScripts;

    //[Header("--- (Catch System) ---")]
   // public Vector3 m_catchPoint;

    /*
    [Header("--- (Smooth Arrival) ---")]
    public Vector3 m_lastCatchPointsDirection;
    public List<Vector3> m_lastCatchPointsDirections;
    public Vector3 m_lastCatchPosition;
    public int m_lastCatchPositionsIndex;
    */

    private void Start()
    {
        AttachEntityBase[] attachEntities = GetComponents<AttachEntityBase>();
        if(attachEntities.Length != 1)
        {
            Debug.Log("Aborted: More or less than one attachEntity found on player!");
            return;
        }

        m_attachEntity = attachEntities[0];
    }

    // Update is called once per frame
    void Update ()
    {
        //manageCatchSystem();
    }
    
    // Grab System
    public bool addToGrab(GameObject cubeAdd)
    {
        if (m_cubeList.Count < m_maxCubesGrabbed)
        {
            CubeEntityAttached attachedScript = cubeAdd.GetComponent<CubeEntitySystem>().getStateComponent().addAttachedScript();
            attachedScript.setValuesByObject(this.gameObject, this);
            
            cubeAdd.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_attachedPlayerPrefab);
            //CubeEntityMovementFollowPoint script = cubeAdd.GetComponent<CubeEntitySystem>().getMovementComponent().getSingleFollowPointScript();
            m_cubeList.Add(cubeAdd);
            m_attachEntity.addAgent(cubeAdd);
            //m_grabbedCubesFollowPointScripts.Add(script);
            updateGrabbedText();
            return true;
        }
        else
            return false;
    }
    public override void deregisterCube(GameObject cubeRemove)
    {
        //if (!m_cubeList.Contains(cubeRemove))
        {
            //Debug.Log("Warning: Tried to remove cube from grab, that wasn't grabbed!");
            //return;
        }

        //cubeRemove.GetComponent<CubeEntityState>().removeAttachedScript();
        m_cubeList.Remove(cubeRemove);
        m_attachEntity.removeAgent(cubeRemove);
        updateGrabbedText();
    }

    // Catch System
    public void manageCatchSystem()
    {
        if (m_cubeList.Count > 0)
        {
            //m_catchPoint = transform.position + /*Camera.main.transform.rotation **/ m_catchOffset;
            moveGrabbedCubesSinglePoint();
        }
    }
    public void moveGrabbedCubesSinglePoint()
    {
        /*
        foreach (CubeEntityMovementFollowPoint followPointScript in m_grabbedCubesFollowPointScripts)
        {
            followPointScript.m_targetPoint = m_catchPoint;
        }
        */
    }

    /*
    public void manageSmoothArrival()
    {
        if (m_useSmoothArrival && m_lastCatchPositionsCount > 0)
        {
            Vector3 newDirection = m_catchPoint - m_lastCatchPosition;
            if (m_lastCatchPointsDirections.Count < m_lastCatchPositionsCount)
                m_lastCatchPointsDirections.Add(newDirection);
            else
            {
                m_lastCatchPointsDirections[m_lastCatchPositionsIndex] = newDirection;
            }

            Vector3 total = Vector3.zero;
            foreach (Vector3 lastDirection in m_lastCatchPointsDirections)
            {
                total += lastDirection;
            }
            m_lastCatchPointsDirection = total / m_lastCatchPointsDirections.Count;

            m_lastCatchPosition = m_catchPoint;
            m_lastCatchPositionsIndex = (m_lastCatchPositionsIndex + 1) % m_lastCatchPositionsCount;

            foreach (CubeEntityMovementFollowPoint script in m_grabbedCubesFollowPointScripts)
            {
                script.m_targetPointMoveDirection = m_lastCatchPointsDirection;
            }
            //Debug.DrawRay(m_catchPoint, m_lastCatchPointsDirection * 10, Color.cyan);
        }
    }
    */
    // utility
    void updateGrabbedText()
    {
        if (m_showGrabbedNumberObject == null || m_showGrabbedNumberObject.GetComponent<Text>() == null)
            return;

        if (m_cubeList.Count > 0)
            m_showGrabbedNumberObject.GetComponent<Text>().text = m_cubeList.Count.ToString();
        else
            m_showGrabbedNumberObject.GetComponent<Text>().text = "";
    }

    // interfaces
    public void onStateChangePrepareRemove()
    {
        Debug.Log("Oops?");
        List<GameObject> list = new List<GameObject>(m_cubeList);
        foreach (GameObject agent in list)
            deregisterCube(agent);
    }


    /*
    // abstract
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        Debug.Log("Warning: This Method should not be called!");
    }
    

    public override void prepareDestroyScript()
    {
        List<GameObject> list = new List<GameObject>(m_cubeList);
        foreach (GameObject agent in list)
            deregisterCube(agent);
    }
    public override void onPostCopy()
    {
        Debug.Log("Warning: This Method should not be called!");
    }
    */
}
