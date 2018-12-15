using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CemBoidRulePredator : CemBoidRuleBase, ICopiable, IPostCopy
{

    [Header("------- Settings -------")]
    //public bool m_useRepellence = true;
    //public List<float> m_predatorPowers;
    public float m_predatorPower;
    public float m_predatorRadius;
    public int m_predatorMaxPartners;
    public int m_predatorMaxPartnerChecks;
    

    [Header("- (Predators) -")]
    public bool m_predatorFillPredatorsOnAdd;
    public int m_predatorMaxPredators;
    public int m_predatorNumberInitialPredators;
    public GameObject[] m_predatorInitialPredators;
    public Material m_predatorMaterial;
    public bool m_predatorsHighlightPredators;
    //public Material m_predatorStandartMaterial;

    [Header("--- (Leader) ---")]
    public float m_predatorAffectLeader;
    public bool m_leaderIsPredator;

    [Header("--- (Adjust Radius) ---")]
    public bool m_useAdjustRadius;
    public int m_predatorMinAdjustmentDifference;
    public float m_predatorMinRadius;
    public float m_predatorAdjustStep;

    [Header("--- (Line of Sight) ---")]
    public bool m_requireLineOfSight;
    public LayerMask m_layerMask;

    [Header("--- (Angle) ---")]
    public bool m_requireAngle;
    public float m_maxAngle;

    [Header("------- Debug -------")]
    public Dictionary<GameObject, Vector3> m_predatorForceVectors;
    public List<GameObject> m_predatorPredators;
    public List<GameObject> m_predatorNotPredators;
    Dictionary<GameObject, float> m_predatorActualRadii;
    public bool m_predatorPlayerIsPredator;
    public bool m_isInitialized;
    public bool m_initialPredatorsReached;

    void Start()
    {
        if (!m_isInitialized)
        {
            initializeStuff();
            manageInitialPredator();
        }
    }
    void initializeStuff()
    {
        m_predatorForceVectors = new Dictionary<GameObject, Vector3>();
        m_predatorActualRadii = new Dictionary<GameObject, float>();
        m_predatorNotPredators = new List<GameObject>();
        m_predatorPredators = new List<GameObject>();

        m_isInitialized = true;
    }

    void Update()
    {
        //getInput();
        //if(!CemBoidBase.s_calculateInBase && !CemBoidBase.s_calculateInFixedUpdate)
            getInformation();
    }
    void FixedUpdate()
    {
        //if (!CemBoidBase.s_calculateInBase)
        {
            //if (CemBoidBase.s_calculateInFixedUpdate)
                //getInformation();
            applyRule();
        }
    }

    public override void getInformation(List<GameObject> agents)
    {
        if (!m_useRule)
            return;

        getPredatorForceVector(agents);
    }
    public override void getInformation()
    {
        if (m_baseScript == null || !m_useRule)
            return;

        List<GameObject> agents = m_baseScript.m_agents;
        if (agents.Count <= 0)
            return;

        getPredatorForceVector(agents);
    }

    void getPredatorForceVector(List<GameObject> agents)
    {
        if (!m_useRule)
            return;


        foreach (GameObject agent in agents)
        {
            if (m_predatorForceVectors.ContainsKey(agent))  // TODO Performance : Delete
                m_predatorForceVectors[agent] = Vector3.zero;
            else
            {
                Debug.Log("Warning!");
            }
        }

        foreach (GameObject predator in m_predatorPredators)
        {
            int nearAgentsCount = 0;
            int nearObjectsCount = 0;

            if (predator == null)
                continue;


            Collider[] colliders = Physics.OverlapSphere(predator.transform.position, m_predatorActualRadii[predator]);
            // adjust radius
            if (m_useAdjustRadius && m_predatorMaxPartners > 0)
            {
                if (colliders.Length - m_predatorMaxPartners > m_predatorMinAdjustmentDifference)
                {
                    m_predatorActualRadii[predator] -= m_predatorAdjustStep;
                }
                else if (colliders.Length - m_predatorMaxPartners < -m_predatorMinAdjustmentDifference)
                {
                    m_predatorActualRadii[predator] += m_predatorAdjustStep * 2f;
                }
                m_predatorActualRadii[predator] = Mathf.Clamp(m_predatorActualRadii[predator], m_predatorMinRadius, m_predatorRadius);
            }
            foreach (Collider collider in colliders)
            {
                GameObject agent = collider.gameObject;
                // max partner checks
                if (nearObjectsCount >= m_predatorMaxPartnerChecks && m_predatorMaxPartnerChecks > 0)
                    break;
                nearObjectsCount++;

                // max partners
                if (nearAgentsCount >= m_predatorMaxPartners && m_predatorMaxPartners > 0)
                    break;

                // line of sight
                if (m_requireLineOfSight && !isLineOfSight(predator, collider))
                    continue;

                // check for angle
                if (m_requireAngle && Utility.getAngle(collider.transform.position - predator.transform.position, predator.transform.forward) > m_maxAngle)
                    continue;

                if (collider.GetComponent<CemBoidAttached>() != null && collider.GetComponent<CemBoidAttached>().m_isAttachedToBases.Contains(m_baseScript))
                {
                    float distanceFactor = Mathf.Clamp01(1f - (Vector3.Distance(agent.transform.position, predator.transform.position) / m_predatorRadius));
                    Vector3 forceVector = (agent.transform.position - predator.gameObject.transform.position).normalized * m_predatorPower * distanceFactor;
                    //directionFactor = Mathf.Clamp01(directionFactor);
                    if (m_predatorForceVectors.ContainsKey(agent)) // TODO Performance : Delete
                        m_predatorForceVectors[agent] += forceVector;
                    else
                        Debug.Log("Warning!");

                    if (agent == m_baseScript.m_leader)
                    {
                        m_predatorForceVectors[agent] *= m_predatorAffectLeader;
                    }
                    nearAgentsCount++;
                }
            }
        }

    }

    public override void applyRule(List<GameObject> agents)
    {
        if (!m_useRule)
            return;

        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Rigidbody>().AddForce(m_predatorForceVectors[agent], ForceMode.Acceleration);
        }
    }
    public override void applyRule()
    {
        if (m_baseScript == null || !m_useRule)
            return;

        List<GameObject> agents = m_baseScript.m_agents;
        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Rigidbody>().AddForce(m_predatorForceVectors[agent], ForceMode.Acceleration);
        }
    }

    // manage agents
    public override void onAddAgent(List<GameObject> agents, GameObject agent)
    {

        //if (!m_predatorForceVectors.ContainsKey(agent))
        {
            m_predatorForceVectors.Add(agent, Vector3.zero);
            m_predatorNotPredators.Add(agent);
            if (m_predatorFillPredatorsOnAdd && !m_initialPredatorsReached)
            {
                addPredator(agent);
                if (m_predatorPredators.Count >= m_predatorNumberInitialPredators)
                {
                    m_initialPredatorsReached = true;
                }
            }
        }
        //else
            //Debug.Log("Warning: Tried to add agent to predator vectors, but it was already in the list!");
    }
    public override void onRemoveAgent(List<GameObject> agents, GameObject agent)
    {
        //if (m_predatorForceVectors.ContainsKey(agent))
        {
            m_predatorForceVectors.Remove(agent);
            if (m_predatorPredators.Contains(agent))
            {
                Debug.Log("Warning"); // TODO Performance : Delete
                removePredator(agent);
            }
            else
                m_predatorNotPredators.Remove(agent);
            
        }
        //else
            //Debug.Log("Warning: Tried to remove agent from predator vectors, but it was not in the list!");
    }

    // manage predators
    public void manageInitialPredator()
    {
        if (m_predatorInitialPredators.Length > m_predatorMaxPredators)
            Debug.Log("Warning: parameter maxPredators is smaller than the amount of initial predators!");

        foreach (GameObject predator in m_predatorInitialPredators)
        {
            addPredator(predator);
        }
    }
    public void setPlayerAsPredator(bool set)
    {
        GameObject player = Constants.getPlayer();
        if (set)
        {
            if (!m_predatorPredators.Contains(player))
                addPredator(player);
            m_predatorPlayerIsPredator = m_predatorPredators.Contains(player);
        }
        else
        {
            if (m_predatorPredators.Contains(player))
                removePredator(player);
            m_predatorPlayerIsPredator = m_predatorPredators.Contains(player);
        }
    }
    public void setNumberOfPredators(int number)
    {
        int tries = 0;

        int p = m_predatorPredators.Contains(Constants.getPlayer()) ? 1 : 0;

        while (m_predatorPredators.Count - p != number && tries < 1000)
        {
            if (m_predatorPredators.Count - p < number && m_predatorNotPredators.Count > 0)
            {
                addRandomPredator();
            }
            else if(m_predatorPredators.Count - p > 0)
            {
                removeRandomPredator();
            }
            tries++;
        }
        if (tries >= 999)
            Debug.Log("Oops!");
    }
    public bool addPredator(GameObject predator)
    {
        if (m_predatorPredators.Count < m_predatorMaxPredators)
        {
            if (predator != null)
            {
                if (!m_predatorPredators.Contains(predator))
                {
                    CemBoidAttached script = null;
                    CemBoidAttached[] attachedScripts = predator.GetComponents<CemBoidAttached>();
                    foreach (CemBoidAttached attachedScript in attachedScripts)
                    {
                        if (attachedScript.m_isAttachedToBases.Contains(m_baseScript))
                        {
                            script = attachedScript;
                        }
                    }
                    m_predatorPredators.Add(predator);
                    m_predatorActualRadii.Add(predator, m_predatorRadius);
                    m_predatorNotPredators.Remove(predator);

                    if (script != null)
                    {
                        if (script.m_predatorBaseScripts.Contains(this))
                            Debug.Log("Warning: Tried to mark attached script of predator, but it was already marked!");
                        else
                            script.m_predatorBaseScripts.Add(this);

                        if (m_predatorsHighlightPredators && predator.GetComponent<CubeEntitySystem>() != null && m_predatorMaterial != null)
                            predator.GetComponent<Renderer>().material = m_predatorMaterial;

                        return true;
                    }
                    else
                        ;// Debug.Log("Warning: Tried to add object to predators, that was already in the list!");
                }
                else
                    ;//Debug.Log("Aborted: predator already was a predator!");
            }
            else
            {
                Debug.Log("Aborted: Tried to add Null object to predators!");
                return false;
            }
        }
        return false;
    }
    public bool addRandomPredator()
    {
        if (m_baseScript == null || !m_useRule)
            return false;
        
        List<GameObject> agents = m_baseScript.m_agents;
        if (agents.Count <= 0)
            return false;
        
        GameObject predator = m_predatorNotPredators[Random.Range(0, m_predatorNotPredators.Count)];

        if (m_predatorPredators.Count < m_predatorMaxPredators)
        {
            if (predator != null && !m_predatorPredators.Contains(predator))
            {
                CemBoidAttached script = null;
                CemBoidAttached[] attachedScripts = predator.GetComponents<CemBoidAttached>();
                foreach (CemBoidAttached attachedScript in attachedScripts)
                {
                    if (attachedScript.m_isAttachedToBases.Contains(m_baseScript))
                    {
                        script = attachedScript;
                    }
                }
                m_predatorPredators.Add(predator);
                m_predatorActualRadii.Add(predator, m_predatorRadius);
                m_predatorNotPredators.Remove(predator);

                if (script != null)
                {
                    if (script.m_predatorBaseScripts.Contains(this))
                        Debug.Log("Warning: Tried to mark attached script of predator, but it was already marked!");
                    else
                        script.m_predatorBaseScripts.Add(this);


                    if (m_predatorsHighlightPredators && predator.GetComponent<CubeEntitySystem>() != null && m_predatorMaterial != null)
                        predator.GetComponent<Renderer>().material = m_predatorMaterial;

                    return true;
                }
                else
                    ;//Debug.Log("Warning: Tried to add object to predators, that was already in the list!");
            }
            else
            {
                Debug.Log("Warning: Tried to add Null object to predators!");
                return false;
            }
        }
        return false;
    }
    public bool removePredator(GameObject predator)
    {
        if (predator != null)
        {
            if (m_predatorPredators.Contains(predator))
            {
                CemBoidAttached script = null;
                CemBoidAttached[] attachedScripts = predator.GetComponents<CemBoidAttached>();
                foreach (CemBoidAttached attachedScript in attachedScripts)
                {
                    if (attachedScript.m_isAttachedToBases.Contains(m_baseScript))
                        script = attachedScript;
                }

                m_predatorPredators.Remove(predator);
                m_predatorActualRadii.Remove(predator);
                m_predatorNotPredators.Add(predator);

                if (script != null)
                {
                    if (!script.m_predatorBaseScripts.Contains(this))
                        Debug.Log("Warning: Tried to remove predator, but it was not in the list!");
                    else
                        script.m_predatorBaseScripts.Remove(this);



                    if (predator.GetComponent<CubeEntitySystem>() != null)
                        predator.GetComponent<Renderer>().material = predator.GetComponent<CubeEntitySystem>().getAppearanceComponent().m_material;

                    return true;
                }
                else
                    ;// Debug.Log("Warning: Tried to remove predator to predators, that did not have an attached script attached!");
            }
            else
                Debug.Log("Warning: Tried to remove object to predators, that was not in the list!");
        }
        else
            Debug.Log("Warning: Tried to remove Null object to predators!");
        return false;
    }
    public bool removeRandomPredator()
    {
        List<GameObject> predators = new List<GameObject>(m_predatorPredators);
        if (predators.Contains(Constants.getPlayer()))
            predators.Remove(Constants.getPlayer());

        if (predators.Count <= 0)
            return false;

        GameObject predator = predators[Random.Range(0, predators.Count)];
        

        if (predator != null)
        {
            if (m_predatorPredators.Contains(predator))
            {
                CemBoidAttached script = null;
                CemBoidAttached[] attachedScripts = predator.GetComponents<CemBoidAttached>();
                foreach (CemBoidAttached attachedScript in attachedScripts)
                {
                    if (attachedScript.m_isAttachedToBases.Contains(m_baseScript))
                        script = attachedScript;
                }

                m_predatorPredators.Remove(predator);
                m_predatorActualRadii.Remove(predator);
                m_predatorNotPredators.Add(predator);

                if (script != null)
                {
                    if (!script.m_predatorBaseScripts.Contains(this))
                        Debug.Log("Warning: Tried to remove predator, but it was not in the list!");
                    else
                        script.m_predatorBaseScripts.Remove(this);

                    


                    if (predator.GetComponent<CubeEntitySystem>() != null)
                        predator.GetComponent<Renderer>().material = predator.GetComponent<CubeEntitySystem>().getAppearanceComponent().m_material;

                    return true;
                }
                else
                    Debug.Log("Warning: Tried to remove predator from predators, that did not have an attached script attached!");
            }
            else
                Debug.Log("Warning: Tried to remove object to predators, that was not in the list!");
        }
        else
            Debug.Log("Warning: Tried to remove Null object to predators!");
        return false;
    }
    public int getNumberPredators()
    {
        int number = m_predatorPredators.Count;
        if (m_predatorPredators.Contains(Constants.getPlayer()))
            number--;
        return number;
    }
    
    // utility
    bool isLineOfSight(GameObject from, Collider to)
    {
        Vector3 direction = to.transform.position - from.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(from.transform.position, direction, out hit, direction.magnitude, m_layerMask) && hit.collider != to)
        {
            return false;
        }
        return true;
    }
    public void resetRadii()
    {
        List<GameObject> agnets = new List<GameObject>(m_predatorActualRadii.Keys);
        foreach (GameObject agent in agnets)
            m_predatorActualRadii[agent] = m_predatorRadius;
    }
    public void setPredatorHighlight(bool highlight)
    {
        foreach(GameObject predator in m_predatorPredators)
        {
            if(predator.GetComponent<CubeEntitySystem>() != null)
            {
                if (highlight)
                    predator.GetComponent<Renderer>().material = m_predatorMaterial;
                else
                    predator.GetComponent<Renderer>().material = predator.GetComponent<CubeEntitySystem>().getAppearanceComponent().m_material;
            }
        }
        m_predatorsHighlightPredators = highlight;
    }

    // input
    /*
    void getInput()
    {
        if(!m_useRule)
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            addRandomPredator();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            removeRandomPredator();
        }
    }
    */
    // copy
    public override void setValues(CemBoidRuleBase copyScript)
    {
        if (copyScript == null)
        {
            Debug.Log("Aborted: CemBoidRulePredator script was null!");
            return;
        }
        if (copyScript.GetType() != this.GetType())
        {
            Debug.Log("Aborted: Copy script wasn't predator script!");
            return;
        }

        CemBoidRulePredator copyScript2 = (CemBoidRulePredator)copyScript;

        m_useRule = copyScript2.m_useRule;

        m_predatorPower = copyScript2.m_predatorPower;
        m_predatorRadius = copyScript2.m_predatorRadius;
        m_predatorMaxPartners = copyScript2.m_predatorMaxPartners;
        m_predatorMaxPartnerChecks = copyScript2.m_predatorMaxPartnerChecks;

        m_predatorMaxPredators = copyScript2.m_predatorMaxPredators;
        m_predatorNumberInitialPredators = copyScript2.m_predatorNumberInitialPredators;
        m_predatorsHighlightPredators = copyScript2.m_predatorsHighlightPredators;

        m_predatorAffectLeader = copyScript2.m_predatorAffectLeader;
        m_leaderIsPredator = copyScript2.m_leaderIsPredator;
        m_predatorFillPredatorsOnAdd = copyScript2.m_predatorFillPredatorsOnAdd;

        m_useAdjustRadius = copyScript2.m_useAdjustRadius;
        m_predatorMinAdjustmentDifference = copyScript2.m_predatorMinAdjustmentDifference;
        m_predatorMinRadius = copyScript2.m_predatorMinRadius;
        m_predatorAdjustStep = copyScript2.m_predatorAdjustStep;

        m_requireLineOfSight = copyScript2.m_requireLineOfSight;
        m_requireAngle = copyScript2.m_requireAngle;
        m_maxAngle = copyScript2.m_maxAngle;
        m_predatorPlayerIsPredator = copyScript2.m_predatorPlayerIsPredator;
        //setPlayerAsPredator(m_predatorPlayerIsPredator);

        //setNumberOfPredators(m_predatorNumberInitialPredators);

        List<GameObject> agnets = new List<GameObject>(m_predatorForceVectors.Keys);
        foreach (GameObject agent in agnets)
            m_predatorForceVectors[agent] = Vector3.zero;
    }
    public void setValuesPredator(CemBoidRuleBase copyScript, int numberPredators)
    {
        if (copyScript == null)
        {
            Debug.Log("Aborted: CemBoidRulePredator script was null!");
            return;
        }
        if (copyScript.GetType() != this.GetType())
        {
            Debug.Log("Aborted: Copy script wasn't predator script!");
            return;
        }

        CemBoidRulePredator copyScript2 = (CemBoidRulePredator)copyScript;

        m_useRule = copyScript2.m_useRule;

        m_predatorPower = copyScript2.m_predatorPower;
        m_predatorRadius = copyScript2.m_predatorRadius;
        m_predatorMaxPartners = copyScript2.m_predatorMaxPartners;
        m_predatorMaxPartnerChecks = copyScript2.m_predatorMaxPartnerChecks;

        m_predatorMaxPredators = copyScript2.m_predatorMaxPredators;
        m_predatorFillPredatorsOnAdd = copyScript2.m_predatorFillPredatorsOnAdd;
        m_predatorNumberInitialPredators = copyScript2.m_predatorNumberInitialPredators;
        m_predatorsHighlightPredators = copyScript2.m_predatorsHighlightPredators;

        m_predatorAffectLeader = copyScript2.m_predatorAffectLeader;
        m_leaderIsPredator = copyScript2.m_leaderIsPredator;

        m_useAdjustRadius = copyScript2.m_useAdjustRadius;
        m_predatorMinAdjustmentDifference = copyScript2.m_predatorMinAdjustmentDifference;
        m_predatorMinRadius = copyScript2.m_predatorMinRadius;
        m_predatorAdjustStep = copyScript2.m_predatorAdjustStep;

        m_requireLineOfSight = copyScript2.m_requireLineOfSight;
        m_requireAngle = copyScript2.m_requireAngle;
        m_maxAngle = copyScript2.m_maxAngle;

        m_predatorPlayerIsPredator = copyScript2.m_predatorPlayerIsPredator;
        //setPlayerAsPredator(m_predatorPlayerIsPredator);

        //setNumberOfPredators(numberPredators);
        setPredatorHighlight(m_predatorsHighlightPredators);

        List<GameObject> agnets = new List<GameObject>(m_predatorForceVectors.Keys);
        foreach (GameObject agent in agnets)
            m_predatorForceVectors[agent] = Vector3.zero;

        agnets = new List<GameObject>(m_predatorActualRadii.Keys);
        foreach (GameObject agent in agnets)
            m_predatorActualRadii[agent] = m_predatorRadius;
    }


    // interfaces
    public void onCopy(ICopiable baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CemBoidRuleBase)baseScript);
    }
    public void onPostCopy()
    {
        m_baseScript = GetComponent<CemBoidBase>();
        if (!m_isInitialized)
            initializeStuff();

        if (m_leaderIsPredator && m_baseScript.m_leader != null)
            addPredator(m_baseScript.m_leader);
    }

    // abstract
    /*
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CemBoidRuleBase)baseScript);
    }
    public override void prepareDestroyScript()
    {
        Destroy(this);
    }
    public override void onPostCopy()
    {
        m_baseScript = GetComponent<CemBoidBase>();
        if (!m_isInitialized)
            initializeStuff();
        
        if (m_leaderIsPredator && m_baseScript.m_leader != null)
            addPredator(m_baseScript.m_leader);
    }
    */
}
