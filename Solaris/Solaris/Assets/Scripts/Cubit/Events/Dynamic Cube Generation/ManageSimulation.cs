using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;
public class ManageSimulation : MonoBehaviour
{
    ////public GameObject m_infoFull;
    ////public GameObject m_infoLight;

    //[Header("------- Settings -------")]
    ////public int m_maxAgents;
    ////public int m_numberInitialAgents;
    //public Vector3 m_boundsSizeLight;
    //public Vector3 m_boundsSizeFull;
    ////public float m_lightBoundsBonus;
    ////public float m_fullBoundsBonus;

    //[Header("--- Check Method ---")]
    ////public CheckType m_checkType;
    ////public bool m_useCheckFullViaGameObject;
    ////public bool m_useJobSystem;

    //[Header("--- Checks Operations ---")]

    //[Header("- From Full To Light -")]
    //public int m_checksPerFrameFull;
    //public float m_checksPerFrameFullRate;
    ////public float m_minDurationStayFull;
    //[Header("- From Light To Full -")]
    //public int m_checksPerFrameLight;
    //public float m_checksPerFrameLightRate;
    //[Header("- From Light To None")]
    ////public int m_checksPerFrameNone;
    ////public float m_checksPerFrameNoneRate;
    ////public float m_noneTime;
    ////public float m_noneDistance;

    //[Header("--- (On State Change) ---")]
    //public bool m_useStructPool;
    //public bool m_usePositionMarker;
    //[Header("- Remove from Full Actions -")]
    //public bool m_removeFullDeactivateGameObject;
    //public bool m_removeFullDeactivateImportant;

    //[Header("- Light to Full -")]
    //public bool m_addFullActivateGameObject;
    //public bool m_addFullActivateImportant;


    ////[Header("--- Line of Sight ---")]
    ////public bool m_requireLos;
    ////public LayerMask m_layerMask;

    ////[Header("--- Light Movement ---")]
    ////public Vector3 m_startSpeedMin;
    ////public Vector3 m_startSpeedMax;

    //[Header("------- Debug -------")]
    //public int m_sizeLight;
    //public int m_sizeFull;
    //public Dictionary<GameObject, FullAgent> m_dict;
    //public List<FullAgent> m_agentsFull;
    //public List<GameObject> m_agentsFullGameObjects;
    //public List<LightAgent> m_agentsLight;
    //public int m_checksFullIndex;
    //public int m_checksLightIndex;
    //public int m_checksNoneIndex;
    //public Plane[] m_cameraPlanes;
    //public bool m_cameraPlanesCalculated;
    //public Bounds m_bounds;

    //public int m_switchesToFullThisFrame;
    //public int m_switchesToLightThisFrame;

    //public Queue<FullAgent> m_fullAgentPool;
    //public Queue<LightAgent> m_lightAgentPool;

    //// job stuff
    ////NativeArray<Vector3> m_outputArray;
    ////TransformAccessArray m_transformAccessArray;
    ////JobGetPositions m_jobGetPositions;
    ////JobHandle m_handleGetPositions;
    ////Vector3[] m_resultGetPositions;
    ////Transform[] m_transformList;

    //LightAgent m_cachedLightAgent;


    ////public int toFull;
    ////public int toLight;


    //private Camera m_camera;
    //// Structs
    ////public struct LightAgent
    ////{
    ////    //public float fadeTime;
    ////    public Vector3 position;
    ////    //public Vector3 boundsSize;
    ////    //public Vector3 velocity;
    ////    //public Collider collider;
    ////    public GameObject positionMarker;
    ////    //public GameObject colliderGameobject;
    ////}
    ////public struct FullAgent
    ////{   
    ////    public GameObject gameObject;
    ////    //public float minDurationEndTime;
    ////    //public bool aboutToChange;
    ////    //public bool rdyToChange;
    ////    //public Collider collider;
    ////    //public Vector3 boundsSize;
    ////    //public SimulationAgent simulationAgentScript;
    ////}

    //// Enums
    ////public enum CheckType
    ////{
    ////    bounds,
    ////    jobs,
    ////    screenCoords
    ////}

    //// Start
    //void Start()
    //{
    //    initializeStuff();
    //    manageInitialAgents();
    //}

    //void initializeStuff()
    //{
    //    m_camera = Constants.getMainCamera().GetComponent<Camera>();

    //    m_bounds = new Bounds();
    //    m_agentsFull = new List<FullAgent>();
    //    m_agentsFullGameObjects = new List<GameObject>();
    //    m_agentsLight = new List<LightAgent>();
    //    m_dict = new Dictionary<GameObject, FullAgent>();

    //    m_fullAgentPool = new Queue<FullAgent>();
    //    m_lightAgentPool = new Queue<LightAgent>();


    //    //m_resultGetPositions = new Vector3[20000];
    //    //m_transformList = new Transform[20000];
    //    //m_outputArray = new NativeArray<Vector3>(20000, Allocator.Persistent);
    //}
    //void manageInitialAgents()
    //{
    //    if (m_useStructPool)
    //    {
    //        createFullAgentStructs(m_numberInitialAgents);
    //        createLightAgentStructs(m_numberInitialAgents);
    //    }

    //    for (int i = 0; i < m_numberInitialAgents; i++)
    //    {
    //        GameObject agent = Constants.getPoolingSystemPasserby().setAgentToActive();
    //        //agent.GetComponent<SimulationAgent>().activate();
    //        addAgentToSimulation(agent);
    //    }
    //}

    //// Update
    //void Update()
    //{
    //    checkAllAgents();

    //    updateTextInfo();
    //}
    //private void FixedUpdate()
    //{
    //    manageLightWeighted();
    //}
    //private void LateUpdate()
    //{
    //    m_cameraPlanesCalculated = false;
    //}

    //void checkAllAgents()
    //{
    //    onPreCheck();

    //    checkLightToNone();

    //    if (m_checkType == CheckType.jobs)
    //    {
    //        checkAllFullJob();
    //        checkAllLightJob();
    //    }
    //    else
    //    {
    //        if (m_useCheckFullViaGameObject)
    //        {
    //            checkFullToLightObject();
    //            checkLightToFullGameObject();
    //        }
    //        else
    //        {
    //            //Debug.Log("Warning! Better not use this!");
    //            checkFullToLight();
    //            checkLightToFull();
    //        }
    //    }
    //}
    //void onPreCheck()
    //{
    //    m_switchesToFullThisFrame = 0;
    //    m_switchesToLightThisFrame = 0;
    //    m_cameraPlanes = GeometryUtility.CalculateFrustumPlanes(m_camera);
    //}

    //void checkFullToLight()
    //{
    //    if (m_agentsFull.Count <= 0)
    //        return;

    //    int checksAbsolute = m_checksPerFrameFull < 0 ? m_agentsFull.Count : Mathf.Clamp(m_checksPerFrameFull, 0, m_agentsFull.Count);
    //    int checksRate = m_checksPerFrameFullRate < 0 ? m_agentsFull.Count : Mathf.Clamp(Mathf.Max((int)(m_checksPerFrameFullRate * m_agentsFull.Count)), (m_checksPerFrameFullRate > 0 ? 1 : 0), m_agentsFull.Count);
    //    int checks = Mathf.Max(checksAbsolute, checksRate);

    //    m_switchesToFullThisFrame = 0;
    //    FullAgent cacheAgent;
    //    Vector3 cacheVector = Vector3.zero;
    //    // this loop causes pretty much all of the per-frame calculation time for the full to light checks.
    //    // trying to keep it as performant as possible.
    //    for (int i = 0; i < checks; i++)
    //    {
    //        m_checksFullIndex %= m_agentsFull.Count;

    //        cacheAgent = m_agentsFull[m_checksFullIndex];
    //        cacheVector = cacheAgent.gameObject.transform.position;     // gameObject.transform.position causes about 70% of calculation time for the per-frame checks!

    //        /*
    //        if (m_minDurationStayFull > 0 && cacheAgent.aboutToChange)
    //        {
    //            if (cacheAgent.minDurationEndTime < Time.time)
    //                cacheAgent.rdyToChange = true;
    //            else
    //                continue;
    //        }*/

    //        // check, if agent should be set to light
    //        if (!checkAgentIsFrustum(cacheVector, m_boundsSizeFull))
    //        {
    //            //if(m_minDurationStayFull <= 0 || cacheAgent.rdyToChange)
    //            addAgentToLight(removeAgentFromFull(cacheAgent, m_checksFullIndex), cacheVector); // TODO : what exactly?
    //            m_switchesToFullThisFrame++;
    //            //else
    //            //{
    //            //    cacheAgent.minDurationEndTime = m_minDurationStayFull + Time.time;
    //            //    cacheAgent.aboutToChange = true;
    //            //}
    //        }

    //        m_checksFullIndex++;
    //    }
    //}
    //void checkFullToLightObject()
    //{
    //    if (m_agentsFullGameObjects.Count <= 0)
    //        return;

    //    int checksAbsolute = m_checksPerFrameFull < 0 ? m_agentsFullGameObjects.Count : Mathf.Clamp(m_checksPerFrameFull, 0, m_agentsFullGameObjects.Count);
    //    int checksRate = m_checksPerFrameFullRate < 0 ? m_agentsFullGameObjects.Count : Mathf.Clamp(Mathf.Max((int)(m_checksPerFrameFullRate * m_agentsFullGameObjects.Count)), (m_checksPerFrameFullRate > 0 ? 1 : 0), m_agentsFullGameObjects.Count);
    //    int checks = Mathf.Max(checksAbsolute, checksRate);
    //    if (checks == 0)
    //        return;



    //    //getTransformSlice(m_agentsFullGameObjects, m_checksFullIndex, checks);
    //    m_switchesToFullThisFrame = 0;
    //    GameObject cacheAgent;
    //    Vector3 cacheVector = Vector3.zero;
    //    for (int i = 0; i < checks; i++)
    //    {
    //        m_checksFullIndex %= m_agentsFullGameObjects.Count;

    //        cacheAgent = m_agentsFullGameObjects[m_checksFullIndex];
    //        cacheVector = cacheAgent.transform.position;                // Performance: agent.transform.position causes 70% of time
    //        // check, if agent should be set to light
    //        if (!checkAgentIsFrustum(cacheVector, m_boundsSizeFull))
    //        {
    //            removeAgentFromFull(m_checksFullIndex);
    //            addAgentToLight(cacheAgent, cacheVector);
    //            m_switchesToFullThisFrame++;
    //        }

    //        m_checksFullIndex++;
    //    }
    //}
    //void checkLightToFull()
    //{
    //    if (m_agentsLight.Count <= 0)
    //        return;

    //    int checksAbsolute = m_checksPerFrameLight < 0 ? m_agentsLight.Count : Mathf.Clamp(m_checksPerFrameLight, 0, m_agentsLight.Count);
    //    int checksRate = m_checksPerFrameLightRate < 0 ? m_agentsLight.Count : Mathf.Clamp(Mathf.Max(1, (int)(m_checksPerFrameLightRate * m_agentsLight.Count)), (m_checksPerFrameLightRate > 0 ? 1 : 0), m_agentsLight.Count);
    //    int checks = Mathf.Max(checksAbsolute, checksRate);

    //    m_switchesToLightThisFrame = 0;
    //    LightAgent cacheAgent;
    //    for (int i = 0; i < checks; i++)
    //    {
    //        m_checksLightIndex %= m_agentsLight.Count;
    //        cacheAgent = m_agentsLight[m_checksLightIndex];

    //        // check, if agent should be set to full
    //        if (checkAgentIsFrustum(cacheAgent.position, m_boundsSizeLight))
    //        {
    //            removeAgentFromLight(m_checksLightIndex);
    //            addAgentToFull(Constants.getPoolingSystemPasserby().setAgentToActive(cacheAgent.position));
    //            m_switchesToLightThisFrame++;
    //        }

    //        m_checksLightIndex++;
    //    }
    //}
    //void checkLightToFullGameObject()
    //{
    //    if (m_agentsLight.Count <= 0)
    //        return;

    //    int checksAbsolute = m_checksPerFrameLight < 0 ? m_agentsLight.Count : Mathf.Clamp(m_checksPerFrameLight, 0, m_agentsLight.Count);
    //    int checksRate = m_checksPerFrameLightRate < 0 ? m_agentsLight.Count : Mathf.Clamp(Mathf.Max(1, (int)(m_checksPerFrameLightRate * m_agentsLight.Count)), (m_checksPerFrameLightRate > 0 ? 1 : 0), m_agentsLight.Count);
    //    int checks = Mathf.Max(checksAbsolute, checksRate);

    //    m_switchesToLightThisFrame = 0;
    //    LightAgent cacheAgent;
    //    for (int i = 0; i < checks; i++)
    //    {
    //        m_checksLightIndex %= m_agentsLight.Count;
    //        cacheAgent = m_agentsLight[m_checksLightIndex];

    //        // check, if agent should be set to full
    //        if (checkAgentIsFrustum(cacheAgent.position, m_boundsSizeLight))
    //        {
    //            removeAgentFromLight(m_checksLightIndex);
    //            addAgentToFullGameObject(Constants.getPoolingSystemPasserby().setAgentToActive(cacheAgent.position));
    //            m_switchesToLightThisFrame++;
    //        }

    //        m_checksLightIndex++;
    //    }
    //}
    //void checkLightToNone()
    //{
    //    if (m_agentsLight.Count <= 0)
    //        return;

    //    int checksAbsolute = m_checksPerFrameNone < 0 ? m_agentsLight.Count : Mathf.Clamp(m_checksPerFrameNone, 0, m_agentsLight.Count);
    //    int checksRate = m_checksPerFrameNoneRate < 0 ? m_agentsLight.Count : Mathf.Clamp(Mathf.Max(1, (int)(m_checksPerFrameNoneRate * m_agentsLight.Count)), (m_checksPerFrameNoneRate > 0 ? 1 : 0), m_agentsLight.Count);
    //    int checks = Mathf.Max(checksAbsolute, checksRate);

    //    LightAgent cacheAgent;
    //    for (int i = 0; i < checks; i++)
    //    {
    //        m_checksNoneIndex %= m_agentsLight.Count;
    //        cacheAgent = m_agentsLight[m_checksNoneIndex];

    //        // check, if agent should be removed completely from simulation
    //        if (m_noneTime > 0 && cacheAgent.fadeTime < Time.time)
    //        {
    //            removeAgentFromSimulation(cacheAgent, m_checksNoneIndex);
    //            continue;
    //        }
    //        else if (m_noneDistance > 0 && m_noneDistance < (cacheAgent.position - m_camera.transform.position).magnitude)
    //        {
    //            removeAgentFromSimulation(cacheAgent, m_checksNoneIndex);
    //            continue;
    //        }

    //        m_checksNoneIndex++;
    //    }
    //}

    //// manage light weighted
    //void manageLightWeighted()
    //{
    //    //moveLightAgents(); // TODO : uncomment
    //}
    //void moveLightAgents()
    //{
    //    for (int i = 0; i < m_agentsLight.Count; i++)
    //    {
    //        moveLightAgent(m_agentsLight[i]);
    //    }
    //}
    //void moveLightAgent(LightAgent agent)
    //{
    //    Vector3 newPosition = agent.position + agent.velocity * Time.fixedDeltaTime;
    //    agent.position = newPosition;
    //    if (m_usePositionMarker)
    //        agent.positionMarker.transform.position = agent.position;
    //}

    //// add/remove agent to/from simulation
    //public void addAgentToSimulation(GameObject agent)
    //{
    //    SimulationAgent[] scripts = agent.GetComponents<SimulationAgent>(); // TODO : delete
    //    if (scripts.Length == 0)
    //    {

    //    }
    //    else if (scripts.Length > 1)
    //    {
    //        Debug.Log("Warning: Too many scripts attached!");
    //    }
    //    else
    //    {
    //        //SimulationAgent script = agent.AddComponent<SimulationAgent>();
    //        //script.m_boundsSizeFull = m_boundsSizeFull;
    //        //script.m_boundsSizeLight = m_boundsSizeLight;
    //    }



    //    if (m_useCheckFullViaGameObject)
    //        addAgentToFullGameObject(agent);
    //    else
    //        addAgentToFull(agent);

    //    Constants.getGame().onAddSimulation(agent);
    //}
    //public void removeAgentFromSimulation(LightAgent agent, int index)
    //{
    //    if (index >= 0)
    //        removeAgentFromLight(index);
    //    else
    //        removeAgentFromLight(agent);
    //}
    //public void removeAgentFromSimulation(GameObject agent)
    //{
    //    if (m_useCheckFullViaGameObject)
    //    {
    //        FullAgent fullAgent = m_dict[agent];
    //        LightAgent lightAgent = addAgentToLight(removeAgentFromFull(fullAgent, m_agentsFull.IndexOf(fullAgent)), Vector3.zero);
    //        removeAgentFromSimulation(lightAgent, -1);
    //    }
    //    else
    //    {
    //        int index = m_agentsFullGameObjects.IndexOf(agent);
    //        removeAgentFromFull(index);
    //        LightAgent lightAgent = addAgentToLight(agent, Vector3.zero);
    //        removeAgentFromSimulation(lightAgent, -1);
    //    }
    //}
    //public void removeAllAgentsFromSimulation()
    //{
    //    int fullAgents = m_useCheckFullViaGameObject ? m_agentsFullGameObjects.Count : m_agentsFull.Count;
    //    for (int i = fullAgents - 1; i >= 0; i--)
    //    {
    //        if (m_useCheckFullViaGameObject)
    //        {
    //            removeAgentFromFull(i);
    //        }
    //        else
    //        {
    //            removeAgentFromFull(m_agentsFull[i], i);
    //        }
    //    }

    //    for (int i = m_agentsLight.Count - 1; i >= 0; i--)
    //    {
    //        removeAgentFromSimulation(m_agentsLight[i], i);
    //    }
    //}

    //// change state of agent
    //public void addAgentToFull(GameObject agent)
    //{
    //    FullAgent fullAgent;
    //    if (m_useStructPool)
    //        fullAgent = getFullAgent();
    //    else
    //        fullAgent = new FullAgent();

    //    fullAgent.gameObject = agent;
    //    //fullAgent.aboutToChange = false;
    //    //fullAgent.rdyToChange = false;
    //    //fullAgent.minDurationEndTime = float.MaxValue;

    //    // perform transition actions add to full
    //    if (m_addFullActivateGameObject)
    //        agent.SetActive(true);
    //    if (m_addFullActivateImportant)
    //        fullAgent.gameObject.GetComponent<SimulationAgent>().activateComponents();

    //    m_dict.Add(agent, fullAgent);
    //    m_agentsFull.Add(fullAgent);

    //    Constants.getGame().onTransitionFull(agent);
    //}
    //public void addAgentToFullGameObject(GameObject agent)
    //{
    //    if (m_addFullActivateGameObject)
    //        agent.SetActive(true);
    //    if (m_addFullActivateImportant)
    //        agent.GetComponent<SimulationAgent>().activateComponents();

    //    m_agentsFullGameObjects.Add(agent);

    //    Constants.getGame().onTransitionFull(agent);
    //}
    //public GameObject removeAgentFromFull(FullAgent fullAgent, int index)
    //{
    //    GameObject agent = fullAgent.gameObject;
    //    // perform transition actions remove from full
    //    if (m_removeFullDeactivateGameObject)
    //        agent.SetActive(false);
    //    if (m_removeFullDeactivateImportant)
    //        agent.GetComponent<SimulationAgent>().deactivateComponents();

    //    if (m_useStructPool)
    //        returnFullAgent(fullAgent);

    //    m_agentsFull.RemoveAt(index);
    //    m_dict.Remove(agent);
    //    return agent;
    //}
    //public GameObject removeAgentFromFull(GameObject agent)
    //{
    //    // perform transition actions remove from full
    //    if (m_removeFullDeactivateGameObject)
    //        agent.SetActive(false);
    //    if (m_removeFullDeactivateImportant)
    //        agent.GetComponent<SimulationAgent>().deactivateComponents();

    //    m_agentsFullGameObjects.Remove(agent);
    //    return agent;
    //}
    //public GameObject removeAgentFromFull(GameObject agent, int index)
    //{
    //    // perform transition actions remove from full
    //    if (m_removeFullDeactivateGameObject)
    //        agent.SetActive(false);
    //    if (m_removeFullDeactivateImportant)
    //        agent.GetComponent<SimulationAgent>().deactivateComponents();

    //    m_agentsFullGameObjects.RemoveAt(index);
    //    return agent;
    //}
    //public GameObject removeAgentFromFull(int index)
    //{
    //    GameObject agent = m_agentsFullGameObjects[index];
    //    // perform transition actions remove from full
    //    if (m_removeFullDeactivateGameObject)
    //        agent.SetActive(false);
    //    if (m_removeFullDeactivateImportant)
    //        agent.GetComponent<SimulationAgent>().deactivateComponents();

    //    m_agentsFullGameObjects.RemoveAt(index);
    //    return agent;
    //}
    //public LightAgent addAgentToLight(GameObject agent, Vector3 position)
    //{
    //    //SimulationAgent script = agent.GetComponent<SimulationAgent>();
    //    if (m_useStructPool)
    //        m_cachedLightAgent = getLightAgent();
    //    else
    //        m_cachedLightAgent = new LightAgent();

    //    m_cachedLightAgent.position = position;
    //    m_cachedLightAgent.velocity = new Vector3(Random.Range(m_startSpeedMin.x, m_startSpeedMax.x), Random.Range(m_startSpeedMin.y, m_startSpeedMax.y), Random.Range(m_startSpeedMin.z, m_startSpeedMax.z));
    //    m_cachedLightAgent.fadeTime = m_noneTime + Time.time;

    //    //if(m_usePositionMarker)
    //    //lightAgent.positionMarker = Constants.getPoolingSystemPositionMarker().setAgentToActive(lightAgent.position);

    //    m_agentsLight.Add(m_cachedLightAgent);

    //    Constants.getPoolingSystemPasserby().setAgentToInactive(agent);

    //    return m_cachedLightAgent;
    //}
    //public void addAgentToLight(GameObject agent)
    //{
    //    //SimulationAgent script = agent.GetComponent<SimulationAgent>();
    //    LightAgent lightAgent;
    //    if (m_useStructPool)
    //        lightAgent = getLightAgent();
    //    else
    //        lightAgent = new LightAgent();

    //    lightAgent.position = agent.transform.position;
    //    lightAgent.velocity = new Vector3(Random.Range(m_startSpeedMin.x, m_startSpeedMax.x), Random.Range(m_startSpeedMin.y, m_startSpeedMax.y), Random.Range(m_startSpeedMin.z, m_startSpeedMax.z));
    //    lightAgent.fadeTime = m_noneTime + Time.time;

    //    //if(m_usePositionMarker)
    //    //lightAgent.positionMarker = Constants.getPoolingSystemPositionMarker().setAgentToActive(lightAgent.position);

    //    m_agentsLight.Add(lightAgent);

    //    Constants.getPoolingSystemPasserby().setAgentToInactive(agent);
    //}
    //public void removeAgentFromLight(int index)
    //{
    //    if (m_usePositionMarker)
    //        Constants.getPoolingSystemPositionMarker().setAgentToInactive(m_agentsLight[index].positionMarker);
    //    //if (m_useStructPool)
    //    returnLightAgent(m_agentsLight[index]);
    //    m_agentsLight.RemoveAt(index);
    //}
    //public void removeAgentFromLight(LightAgent lightAgent)
    //{
    //    int index = m_agentsLight.IndexOf(lightAgent);
    //    if (m_usePositionMarker)
    //        Constants.getPoolingSystemPositionMarker().setAgentToInactive(m_agentsLight[index].positionMarker);
    //    //if (m_useStructPool)
    //    returnLightAgent(m_agentsLight[index]);
    //    m_agentsLight.RemoveAt(index);
    //}


    //// check agents
    //bool checkFullToLight(FullAgent agent)
    //{
    //    if (checkAgentIsFrustum(agent.gameObject.transform.position, m_boundsSizeFull))
    //    {
    //        return false;
    //    }
    //    return true;
    //}
    //bool checkLightToFull(LightAgent agent)
    //{
    //    if (checkAgentIsFrustum(agent.position, m_boundsSizeLight))
    //    {
    //        return true;
    //    }
    //    return false;
    //}
    //bool checkAgentIsFrustum(Vector3 boundsCenter, Vector3 boundsSize)
    //{
    //    if (m_checkType == CheckType.bounds)
    //    {
    //        m_bounds.center = boundsCenter;
    //        m_bounds.size = boundsSize;
    //        if (GeometryUtility.TestPlanesAABB(m_cameraPlanes, m_bounds))
    //        {
    //            return true;
    //        }
    //    }
    //    //if(m_checkType == checkType.screenCoords)
    //    //{
    //    //    Vector3 screenPosition = m_camera.WorldToViewportPoint(boundsCenter);
    //    //    if (screenPosition.x > 1.1f || screenPosition.x < -0.1f || screenPosition.y > 1.1f || screenPosition.y < -0.1f)
    //    //        return false;
    //    //    return true;
    //    //}
    //    return false;
    //}
    ///*
    //public Plane[] getCameraPlanes()
    //{
    //    if (m_cameraPlanesCalculated)
    //        return m_cameraPlanes;
        
    //    m_cameraPlanes = GeometryUtility.CalculateFrustumPlanes(m_camera);
    //    m_cameraPlanesCalculated = true;
    //    return m_cameraPlanes;
    //}
    //*/


    //// ---- Struct Pools -----
    //// struct pool full
    //void createFullAgentStructs(int number)
    //{
    //    for (int i = 0; i < number; i++)
    //        createFullAgentStruct();
    //}
    //void createFullAgentStruct()
    //{
    //    FullAgent agent = new FullAgent();
    //    m_fullAgentPool.Enqueue(agent);
    //}
    //FullAgent getFullAgent()
    //{
    //    if (m_fullAgentPool.Count <= 0)
    //    {
    //        Debug.Log("Warning: Creating new full agent structs!");
    //        createFullAgentStructs(100);
    //    }
    //    return m_fullAgentPool.Dequeue();
    //}
    //void returnFullAgent(FullAgent agent)
    //{
    //    m_fullAgentPool.Enqueue(agent);
    //}
    //// struct pool full
    //void createLightAgentStructs(int number)
    //{
    //    for (int i = 0; i < number; i++)
    //        createLightAgentStruct();
    //}
    //void createLightAgentStruct()
    //{
    //    LightAgent agent = new LightAgent();
    //    m_lightAgentPool.Enqueue(agent);
    //}
    //LightAgent getLightAgent()
    //{
    //    if (m_lightAgentPool.Count <= 0)
    //    {
    //        Debug.Log("Info: Creating new light agent structs!");
    //        createLightAgentStructs(100);
    //    }
    //    return m_lightAgentPool.Dequeue();
    //}
    //void returnLightAgent(LightAgent agent)
    //{
    //    m_lightAgentPool.Enqueue(agent);
    //}

    //// job system get transform
    //void getPositions(Transform[] inputArray)
    //{
    //    m_transformAccessArray = new TransformAccessArray(inputArray, 16);

    //    m_jobGetPositions = new JobGetPositions();
    //    m_jobGetPositions.m_positions = m_outputArray;

    //    m_handleGetPositions = m_jobGetPositions.Schedule(m_transformAccessArray);
    //    m_handleGetPositions.Complete();

    //    m_outputArray.CopyTo(m_resultGetPositions);
    //}
    //void getTransformSlice(List<GameObject> goList, int startIndex, int numberIndices)
    //{
    //    int s = 0;
    //    while (s < m_transformList.Length)
    //    {
    //        if (m_transformList[s] == null && s >= numberIndices)
    //            break;

    //        int index = (s + startIndex) % goList.Count;
    //        m_transformList[s] = goList[index].transform;

    //        s++;
    //    }
    //    getPositions(m_transformList);
    //}

    //// job system check frustum
    //void checkAllFullJob()
    //{
    //    NativeArray<int> bools = new NativeArray<int>(m_agentsFullGameObjects.Count, Allocator.Temp);
    //    NativeArray<float> maxDistances = new NativeArray<float>(m_agentsFullGameObjects.Count, Allocator.Temp);
    //    NativeArray<Vector3> centers = new NativeArray<Vector3>(m_agentsFullGameObjects.Count, Allocator.Temp);
    //    NativeArray<Plane> planes = new NativeArray<Plane>(6, Allocator.Temp);
    //    for (int i = 0; i < 6; i++)
    //        planes[i] = m_cameraPlanes[i];


    //    for (int i = 0; i < m_agentsFullGameObjects.Count; i++)
    //    {
    //        bools[i] = 0;
    //        maxDistances[i] = m_boundsSizeFull.magnitude;
    //        centers[i] = m_agentsFullGameObjects[i].transform.position;
    //        //dict.Add(i, m_agentsFull[i]);
    //    }

    //    JobFrustumCheck jobFrustumCheck = new JobFrustumCheck();
    //    jobFrustumCheck.maxDistances = maxDistances;
    //    jobFrustumCheck.centers = centers;
    //    jobFrustumCheck.bools = bools;
    //    jobFrustumCheck.planes = planes;

    //    JobHandle handle = jobFrustumCheck.Schedule(m_agentsFullGameObjects.Count, (m_agentsFull.Count / 16));
    //    handle.Complete();

    //    int total = 0;
    //    for (int i = m_agentsFullGameObjects.Count - 1; i >= 0; i--)
    //    {
    //        if (bools[i] == 0)
    //        {
    //            total++;
    //            //removeAgentFromFull(i);
    //            addAgentToLight(removeAgentFromFull(i), centers[i]);
    //        }
    //    }


    //    bools.Dispose();
    //    maxDistances.Dispose();
    //    centers.Dispose();
    //    planes.Dispose();
    //}
    //void checkAllLightJob()
    //{
    //    NativeArray<int> bools = new NativeArray<int>(m_agentsLight.Count, Allocator.Temp);
    //    NativeArray<float> maxDistances = new NativeArray<float>(m_agentsLight.Count, Allocator.Temp);
    //    NativeArray<Vector3> centers = new NativeArray<Vector3>(m_agentsLight.Count, Allocator.Temp);
    //    NativeArray<Plane> planes = new NativeArray<Plane>(6, Allocator.Temp);
    //    for (int i = 0; i < 6; i++)
    //        planes[i] = m_cameraPlanes[i];


    //    for (int i = 0; i < m_agentsLight.Count; i++)
    //    {
    //        bools[i] = 0;
    //        maxDistances[i] = m_boundsSizeLight.magnitude;
    //        centers[i] = m_agentsLight[i].position;
    //        //dict.Add(i, m_agentsFull[i]);
    //    }

    //    JobFrustumCheck jobFrustumCheck = new JobFrustumCheck();
    //    jobFrustumCheck.maxDistances = maxDistances;
    //    jobFrustumCheck.centers = centers;
    //    jobFrustumCheck.bools = bools;
    //    jobFrustumCheck.planes = planes;

    //    JobHandle handle = jobFrustumCheck.Schedule(m_agentsLight.Count, (m_agentsFull.Count / 16));
    //    handle.Complete();

    //    for (int i = bools.Length - 1; i >= 0; i--)
    //    {
    //        if (bools[i] == 1)
    //        {
    //            removeAgentFromLight(i);
    //            addAgentToFullGameObject(Constants.getPoolingSystemPasserby().setAgentToActive(centers[i]));
    //        }
    //    }

    //    bools.Dispose();
    //    maxDistances.Dispose();
    //    centers.Dispose();
    //    planes.Dispose();

    //    /*
    //    NativeArray<int> bools = new NativeArray<int>(m_agentsLight.Count, Allocator.Temp);
    //    Dictionary<int, LightAgent> dict = new Dictionary<int, LightAgent>();
    //    NativeArray<Bounds> bounds = new NativeArray<Bounds>(m_agentsLight.Count, Allocator.Temp);
    //    NativeArray<Plane> planes = new NativeArray<Plane>(6, Allocator.Temp);
    //    for (int i = 0; i < 6; i++)
    //        planes[i] = getCameraPlanes()[i];

    //    for (int i = 0; i < m_agentsLight.Count; i++)
    //    {
    //        bools[i] = 0;
    //        Bounds bound = m_agentsLight[i].collider.bounds;
    //        if (bound.extents.magnitude < 0.01f)
    //            bound.extents = m_agentsLight[i].boundsSize;
    //        bounds[i] = bound;
    //        dict.Add(i, m_agentsLight[i]);
    //    }

    //    JobFrustumCheck jobFrustumCheck = new JobFrustumCheck();
    //    jobFrustumCheck.bounds = bounds;
    //    jobFrustumCheck.bools = bools;
    //    jobFrustumCheck.planes = planes;

    //    JobHandle handle = jobFrustumCheck.Schedule(m_agentsLight.Count, 100);// (m_agentsLight.Count / 6));
    //    handle.Complete();

    //    for (int i = 0; i < bools.Length; i++)
    //    {
    //        if (bools[i] == 1)
    //        {
    //            addAgentToFull(Constants.getPoolingSystemPasserby().setAgentToActive(dict[i].position), dict[i]);
    //        }
    //    }

    //    bools.Dispose();
    //    bounds.Dispose();
    //    planes.Dispose();
    //    */
    //}

    //// UI stuff
    //void updateTextInfo()
    //{
    //    m_sizeLight = m_agentsLight.Count;
    //    m_sizeFull = m_useCheckFullViaGameObject ? m_agentsFullGameObjects.Count : m_agentsFull.Count;

    //    if (m_infoFull != null && m_infoFull.GetComponent<Text>())
    //        m_infoFull.GetComponent<Text>().text = "Active: " + m_sizeFull;
    //    if (m_infoLight != null && m_infoLight.GetComponent<Text>())
    //        m_infoLight.GetComponent<Text>().text = "Light:   " + m_sizeLight;
    //}
}
