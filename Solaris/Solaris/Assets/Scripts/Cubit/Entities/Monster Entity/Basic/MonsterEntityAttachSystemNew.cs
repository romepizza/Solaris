using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntityAttachSystemNew : AttachSystemBase, ICopiable, IPostCopy, IRemoveOnStateChange, IPrepareRemoveOnStateChange
{
    [Header("----- SETTINGS -----")]
    //public float m_duration = -1;
    public GameObject m_prefab;
    
    [Header("----- DEBUG -----")]
    //public List<GameObject> m_cubeList;
    //private MonsterEntityBase m_monsterBaseScript;
    public AttachEntityBase m_attachEntity;
    public bool m_isInitialized;

    //public int m_freePositions;

    /*
    [Header("--- (Smooth Arrival) ---")]
    public Vector3 m_lastCatchPointsDirection;
    public List<Vector3> m_lastCatchPointsDirections;
    public Vector3 m_lastCatchPosition;
    public int m_lastCatchPositionsIndex;
    */

    // Use this for initialization
    void Start ()
    {
        if(!m_isInitialized)
            initializeStuff();	
	}
    void initializeStuff()
    {
        AttachEntityBase[] attachEntities = GetComponents<AttachEntityBase>();
        if (attachEntities.Length != 1)
        {
            //Debug.Log("Aborted: More or less than one attachEntity found on player!");
            return;
        }

        m_attachEntity = attachEntities[0];
        m_cubeList = new List<GameObject>();
        m_isInitialized = true;
    }
	

    public bool registerToGrab(GameObject cubeAdd)
    {
        if (m_cubeList.Count < m_maxCubesGrabbed)
        {
            if(m_cubeList.Contains(cubeAdd))
            {
                Debug.Log("Aborted: Tried to add cube from attach system that was already in the list!");
                return false;
            }
            //cubeAdd.GetComponent<CubeEntitySystem>().setAttachedDynamicly(GetComponent<CubeEntityState>());
            if (m_prefab != null)
                cubeAdd.GetComponent<CubeEntityPrefapSystem>().setToPrefab(m_prefab);

            CubeEntityAttached attachedScript = cubeAdd.GetComponent<CubeEntityAttached>();// AddComponent<CubeEntityAttached>();
            if(attachedScript == null)
                attachedScript = cubeAdd.AddComponent<CubeEntityAttached>();
            attachedScript.setValuesByObject(gameObject, this);


            m_cubeList.Add(cubeAdd);
            m_attachEntity.addAgent(cubeAdd);

            return true;
        }

        return false;
    }
    public override void deregisterCube(GameObject cubeRemove)
    {
        // if cubeRemove is the core
        if (cubeRemove == gameObject)
        {
            //cubeRemove.GetComponent<CubeEntityState>().removeAttachedScript();
            return;
        }
        if (m_cubeList == null)
            return;

        //if(!m_cubeList.Contains(cubeRemove))    // TODO Performance : delete line
        {
            //Debug.Log("Aborted: Tried to remove cube from attach system that was not in the list!");            
            //return;
        }

        CubeEntityAttached attachScript = cubeRemove.GetComponent<CubeEntityAttached>();
        if (attachScript == null)
            Debug.Log("Warning: attachScript was null!");
        else
            attachScript.onRemove();

        m_attachEntity.removeAgent(cubeRemove);
        m_cubeList.Remove(cubeRemove);
    }
    //public void deregisterAllCubes()
    //{
    //    List<GameObject> list = new List<GameObject>(m_cubeList);

    //    foreach (GameObject cubeRemove in list)
    //        deregisterCube(cubeRemove);
    //}
    
    public void setValues(MonsterEntityAttachSystemNew script)
    {
        m_maxCubesGrabbed = script.m_maxCubesGrabbed;
        m_currentMaxCubesGrabbed = m_maxCubesGrabbed;
        m_setToInactiveWhenNoMaxNumber = script.m_setToInactiveWhenNoMaxNumber;
        m_movementAffectsCubesFactor = script.m_movementAffectsCubesFactor;
        m_prefab = script.m_prefab;
    }

    // interfaces
    public void onCopy(ICopiable copiable)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((MonsterEntityAttachSystemNew)copiable);
    }
    public void onPostCopy()
    {
        AttachEntityBase[] attachEntities = GetComponents<AttachEntityBase>();
        
        if (attachEntities.Length != 1)
        {
            Debug.Log("Aborted: More or less than one attachEntity found on player!");
            return;
        }

        m_attachEntity = attachEntities[0];
        MonsterEntityAbstractBase b = (MonsterEntityAbstractBase)(Utility.getComponentInParents<MonsterEntityAbstractBase>(transform));
        if (b == null)
            m_core = gameObject;
        else
            m_core = b.gameObject;
        //m_monsterBaseScript = GetComponent<MonsterEntityBase>();
    }
    public void onRemove()
    {
        Destroy(this);
    }
    public void onStateChangePrepareRemove()
    {
        //m_attachEntity.prepareDestroyScript();
        //deregisterAllCubes();
        
        for (int i = m_cubeList.Count - 1; i >= 0; i--)
        {
            m_cubeList[i].GetComponent<CubeEntityAttached>().onRemove(); // TODO : maybe not call this method
            Destroy(m_cubeList[i].GetComponent<CubeEntityAttached>());
            // m_cubeList[i].GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
        }
    }
    /*
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((MonsterEntityAttachSystemNew)baseScript);
    }
    public void prepareDestroyScript()
    {
        m_attachEntity.prepareDestroyScript();
        for(int i = m_cubeList.Count - 1; i >= 0; i--)//GameObject cube in m_cubeList)
        {
            m_cubeList[i].GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
        }
        deregisterAllCubes();
        Destroy(this);
    }

    public override void onPostCopy()
    {
        AttachEntityBase[] attachEntities = GetComponents<AttachEntityBase>();
        
        if (attachEntities.Length != 1)
        {
            Debug.Log("Aborted: More or less than one attachEntity found on player!");
            return;
        }


        m_attachEntity = attachEntities[0];
        m_monsterBaseScript = GetComponent<MonsterEntityBase>();
    }
    */
}
