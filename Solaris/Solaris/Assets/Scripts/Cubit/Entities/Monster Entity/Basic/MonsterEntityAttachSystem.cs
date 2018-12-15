using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntityAttachSystem : AttachSystemBase
{
    public class GrabbedCube
    {
        public GameObject cube;
        public CubeEntityMovementFollowPoint script;
        public Vector3 positionOffset;
        public Vector3 positionWorld;

        public GrabbedCube(GameObject cube, CubeEntityMovementFollowPoint script, Vector3 positionOffset, Vector3 positionWorld)
        {
            this.cube = cube;
            this.script = script;
            this.positionOffset = positionOffset;
            this.positionWorld = positionWorld;
        }
    }

    [Header("----- SETTINGS -----")]
    public float m_duration = -1;
    //public int m_maxCubesGrabbed;

    [Header("--- (Catch System) ---")]
    public bool m_putNewCubeToNearestPosition;
    public float m_minCatchRadius;
    public float m_maxCatchRadius;
    
    /*
    [Header("--- (Smooth Arrival) ---")]
    public bool m_useSmoothArrival;
    public int m_lastCatchPositionsCount;
    */

    [Header("--- (Cube Movement) ---")]
    //public float m_movementAffectsCubesFactor;
    public float m_cubeMovementPower;
    public float m_maxSpeed;

    [Header("----- DEBUG -----")]
    

    //[Header("--- (Catch System) ---")]
    public GrabbedCube[] m_cubeListStruct;
    public List<int> m_freePositions;
    public List<int> m_occupiedPositions;

    private MonsterEntityBase m_monsterBaseScript;

    /*
    [Header("--- (Smooth Arrival) ---")]
    public Vector3 m_lastCatchPointsDirection;
    public List<Vector3> m_lastCatchPointsDirections;
    public Vector3 m_lastCatchPosition;
    public int m_lastCatchPositionsIndex;
    */

    // Use this for initialization
    void Start()
    {
        m_monsterBaseScript = GetComponent<MonsterEntityBase>();
        initializeCubeList();
    }

    // Update is called once per frame
    void Update()
    {
        manageCatchSystem();
    }

    // Getter
    public List<GameObject> getAttachedCubes()
    {
        List<GameObject> cubeList = new List<GameObject>();
        foreach(GrabbedCube grabbedCube in m_cubeListStruct)
        {
            if(grabbedCube.cube != null)
            {
                cubeList.Add(grabbedCube.cube);
            }
        }
        return cubeList;
    }

    

    // Grab System
    public bool addToGrab(GameObject cubeAdd)
    {
        if (m_freePositions.Count > 0)
        {
            //cubeAdd.GetComponent<CubeEntitySystem>().setToAttachedEnemyEjector(Vector3.zero, m_duration, m_cubeMovementPower, m_maxSpeed);
            CubeEntityMovementFollowPoint script = null;// = cubeAdd.GetComponent<CubeEntitySystem>().getMovementComponent().getSingleFollowPointScript();
            Debug.Log("Aborted: This line of code should not have been reached at all!");
            return false;

            CubeEntityAttached attachedScript = cubeAdd.GetComponent<CubeEntitySystem>().getStateComponent().addAttachedScript();
            attachedScript.setValuesByObject(this.gameObject, this);

            int index = -1;
            if (m_putNewCubeToNearestPosition)
            {
                float minDist = float.MaxValue;
                for(int i = 0; i < m_freePositions.Count; i++)
                {
                    float dist = (m_cubeListStruct[m_freePositions[i]].positionWorld - cubeAdd.transform.position).magnitude;
                    if(dist < minDist)
                    {
                        index = m_freePositions[i];
                        minDist = dist;
                    }
                }
            }
            else
            {
                index = m_freePositions[Random.Range(0, m_freePositions.Count)];
            }

            if (index >= 0)
            {
                m_cubeListStruct[index].cube = cubeAdd;
                m_cubeListStruct[index].script = script;
                m_freePositions.Remove(index);
                m_occupiedPositions.Add(index);
            }
            else
                Debug.Log("Warning: No matching position for cube found!");
            return true;
        }
        else
            return false;
    }
    public override void deregisterCube(GameObject cubeRemove)
    {
        // if cubeRemove is the core
        if (cubeRemove == this.gameObject)
        {
            //cubeRemove.GetComponent<CubeEntityState>().removeAttachedScript();
            return;
        }
        if (m_cubeListStruct == null)
            return;

        for(int i = 0; i < m_cubeListStruct.Length; i++)
        {
            if(m_cubeListStruct[i].cube == cubeRemove)
            {
                //cubeRemove.GetComponent<CubeEntityState>().removeAttachedScript();
                Destroy(m_cubeListStruct[i].script);
                m_cubeListStruct[i].script = null;
                m_cubeListStruct[i].cube = null;
                m_freePositions.Add(i);
                m_occupiedPositions.Remove(i);
                //cubeRemove.GetComponent<CubeEntitySystem>().setToInactive();
                return;
            }
        }
        //Debug.Log("Warning: Tried to remove cube from grab, that wasn't grabbed!");
    }
    public void deregisterAllCubes(bool setToInactive)
    {
        for(int i = m_cubeListStruct.Length - 1; i >= 0; i--)
        {

            GameObject cubeRemove = m_cubeListStruct[i].cube;
            if (cubeRemove == this.gameObject || cubeRemove == null)
                continue;

            //cubeRemove.GetComponent<CubeEntityState>().removeAttachedScript();
            Destroy(m_cubeListStruct[i].script);
            m_cubeListStruct[i].script = null;
            m_cubeListStruct[i].cube = null;
            m_freePositions.Add(i);
            m_occupiedPositions.Remove(i);

            if (setToInactive)
                cubeRemove.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
        }
    }

    // Catch System
    public void manageCatchSystem()
    {
        foreach(GrabbedCube grabbedCube in m_cubeListStruct)
        {
            if(grabbedCube.cube != null)
            {
                grabbedCube.positionWorld = transform.position + grabbedCube.positionOffset;
                //grabbedCube.script.m_targetPoint = grabbedCube.positionWorld;
            }
        }
    }



    // Intern
    public void initializeCubeList()
    {
        m_freePositions = new List<int>();
        m_occupiedPositions = new List<int>();
        m_cubeListStruct = new GrabbedCube[m_maxCubesGrabbed];

        for (int i = 0; i < m_cubeListStruct.Length; i++)
        {
            Vector3 pos = Random.insideUnitSphere;
            pos = pos.normalized * Random.Range(m_minCatchRadius, m_maxCatchRadius);

            m_cubeListStruct[i] = new GrabbedCube(null, null, pos, transform.position + pos);
            m_freePositions.Add(i);
        }
    }

    // Setter
    public void setValuesByScript(MonsterEntityAttachSystem prefab)
    {
        MonsterEntityAttachSystem script = prefab.GetComponent<MonsterEntityAttachSystem>();
        if (script != null)
        {

            m_duration = script.m_duration;
            m_putNewCubeToNearestPosition = script.m_putNewCubeToNearestPosition;
            m_minCatchRadius = script.m_minCatchRadius;
            m_maxCatchRadius = script.m_maxCatchRadius;
            m_movementAffectsCubesFactor = script.m_movementAffectsCubesFactor;
            m_cubeMovementPower = script.m_cubeMovementPower;
            m_maxSpeed = script.m_maxSpeed;
        }
        else
            Debug.Log("Warning: Tried to copy values of MonsterEntityAttachSystem from prefab, that didn't have the script attached!");

        MonsterEntityBase scriptBase = prefab.GetComponent<MonsterEntityBase>();
        if (scriptBase)
        {
            m_maxCubesGrabbed = scriptBase.m_maxCubes;
        }

        initializeCubeList();
    }
    public void setValuesPlain(MonsterEntityAttachSystem prefab)
    {
        if (prefab != null)
        {
            m_duration = prefab.m_duration;
            m_putNewCubeToNearestPosition = prefab.m_putNewCubeToNearestPosition;
            m_minCatchRadius = prefab.m_minCatchRadius;
            m_maxCatchRadius = prefab.m_maxCatchRadius;
            m_movementAffectsCubesFactor = prefab.m_movementAffectsCubesFactor;
            m_cubeMovementPower = prefab.m_cubeMovementPower;
            m_maxSpeed = prefab.m_maxSpeed;
        }
        else
            Debug.Log("Warning: Tried to copy values of MonsterEntityAttachSystem from prefab, that didn't have the script attached!");
    }


    // interfaces
    public void onCopy(ICopiable copiable)
    {
        setValuesPlain((MonsterEntityAttachSystem)copiable);
    }
    public void onPostCopy()
    {
        m_monsterBaseScript = GetComponent<MonsterEntityBase>();
        if (m_monsterBaseScript == null)
            Debug.Log("Warning: m_attachSystemScript was null");
        else
            m_maxCubesGrabbed = m_monsterBaseScript.m_maxCubes;

        initializeCubeList();
    }
    public void onStateChangeRemove()
    {
        Destroy(this);
    }

    public void destroyScript()
    {
        /*
        foreach(grabbedCube grabbedCube in m_cubeList)
        {
            if(grabbedCube.cube != null)
            {
                //grabbedCube.cube.GetComponent<CubeEntitySystem>().setToInactive();
                deregisterCube(grabbedCube.cube);
            }
        }
        */

        for (int i = m_cubeListStruct.Length - 1; i >= 0; i--)
        {

            GameObject cubeRemove = m_cubeListStruct[i].cube;
            if (cubeRemove == this.gameObject || cubeRemove == null)
                continue;

            //cubeRemove.GetComponent<CubeEntityState>().removeAttachedScript();
            //Destroy(m_cubeList[i].script);
        }

        Destroy(this);
    }


    /*
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        setValuesPlain((MonsterEntityAttachSystem)baseScript);
    }
    
    public override void prepareDestroyScript()
    {
        /*
        foreach(grabbedCube grabbedCube in m_cubeList)
        {
            if(grabbedCube.cube != null)
            {
                //grabbedCube.cube.GetComponent<CubeEntitySystem>().setToInactive();
                deregisterCube(grabbedCube.cube);
            }
        }
        

        for (int i = m_cubeListStruct.Length - 1; i >= 0; i--)
        {

            GameObject cubeRemove = m_cubeListStruct[i].cube;
            if (cubeRemove == this.gameObject || cubeRemove == null)
                continue;

            //cubeRemove.GetComponent<CubeEntityState>().removeAttachedScript();
            //Destroy(m_cubeList[i].script);
        }

        Destroy(this);
    }
    public override void onPostCopy()
    {
        m_monsterBaseScript = GetComponent<MonsterEntityBase>();
        if (m_monsterBaseScript == null)
            Debug.Log("Warning: m_attachSystemScript was null");
        else
            m_maxCubesGrabbed = m_monsterBaseScript.m_maxCubes;

        initializeCubeList();
    }
    */
}
