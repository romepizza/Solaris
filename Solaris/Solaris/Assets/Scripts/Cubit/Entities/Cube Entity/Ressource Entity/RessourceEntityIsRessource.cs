using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceEntityIsRessource : EntityCopiableAbstract, ICopiable
{
    [Header("------- Settings -------")]

    [Header("--- (Gather Value) ---")]
    public float[] m_gatherValues;

    [Header("--- (Gather Criteria) ---")]
    public float m_gatherCheckIntervall;
    public float m_gatherRadius;

    [Header("--- (Cancel Criteria) ---")]
    public float m_removeCheckIntervall;
    public float m_removeDistance;

    [Header("--- (Gather Act) ---")]
    public float m_gatherDoneDistance;
    public float m_gatherMovementSpeed;
    public float m_curveTime;
    public AnimationCurve m_curve;

    [Header("--- (Particle Effects) ---")]
    [Header("- On Gather Start Effect -")]
    public float m_onGatherStartDeactivatePermaEffectTime;
    public GameObject[] m_onGatherStartParticleEffects;
    [Header("- On Gather Done Effect -")]
    public GameObject[] m_onGatherDoneParticleEffects;
    [Header("- On Not Gathered Remove Effect -")]
    public GameObject[] m_onNotGatheredRemoveParticleEffects;

    [Header("------- Debug -------")]
    public GameObject m_player;
    public float m_checkRemoveRdy;
    public float m_checkGatherRdy;

    public bool m_isBeingGathered;
    public float m_currentCuveTime;

    public float m_activateLoopingEffectsTime;
    public bool m_loopingEffectIsActive;

    public bool m_gaveRessources;

    public Rigidbody m_rb;

    void initializeStuff()
    {
        m_player = Constants.getPlayer();
        if (m_onGatherStartParticleEffects == null)
            m_onGatherStartParticleEffects = new GameObject[0];
        if (m_onGatherDoneParticleEffects == null)
            m_onGatherDoneParticleEffects = new GameObject[0];

        if (m_rb == null)
            m_rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        checkStartGather();
        manageGather();
        //manageEffectTimer();
        checkGatherDone();
        checkRemove();
	}

    // check gather
    void checkStartGather()
    {
        if (m_isBeingGathered)
            return;

        if (m_checkGatherRdy > Time.time)
            return;

        float distance = Vector3.Distance(transform.position, m_player.transform.position);
        if(distance < m_gatherRadius)
        {
            initializeGather();
        }

        m_checkGatherRdy = Time.time + m_gatherCheckIntervall;
    }

    // manage gather
    void initializeGather()
    {
        m_isBeingGathered = true;
        m_currentCuveTime = 0;

        deactivateLoopingEffects();

        foreach (GameObject effect in m_onGatherStartParticleEffects)
        {
            GetComponent<CubeEntityParticleSystem>().createParticleEffect(effect);
        }
    }

    void manageGather()
    {
        if (!m_isBeingGathered)
            return;

        float speed = m_gatherMovementSpeed;
        if (m_currentCuveTime < m_curveTime)
        {
            float t = m_currentCuveTime / m_curveTime;
            speed *= m_curve.Evaluate(t);
        }

        Vector3 direction = m_player.transform.position - transform.position;
        m_currentCuveTime += Time.deltaTime;
        m_rb.velocity = direction.normalized * speed;
    }
    void checkGatherDone()
    {
        if (!m_isBeingGathered || m_gaveRessources)
            return;

        float distance = Vector3.Distance(transform.position, m_player.transform.position);
        if (distance < m_gatherDoneDistance)
            gatherRessource();
    }

    // actual gather
    void gatherRessource()
    {
        m_gaveRessources = true;
        m_player.GetComponent<CubeEntityRessourceManager>().addRessources(m_gatherValues);
        foreach(GameObject effect in m_onGatherDoneParticleEffects)
        {
            CubeEntityParticleSystem.createParticleEffet(effect, transform.position);
            //GetComponent<CubeEntityParticleSystem>().createParticleEffect(effect);
        }
        removeRessource();
    }

    // remove
    void checkRemove()
    {
        if (m_isBeingGathered)
            return;

        if (m_checkRemoveRdy > Time.time)
            return;

        float distance = Vector3.Distance(transform.position, m_player.transform.position);
        if (distance > m_removeDistance)
        {
            createOnNotGatheredRemoveEffects();
            removeRessource();
        }

        m_checkRemoveRdy = Time.time + m_removeCheckIntervall;
    }
    void removeRessource()
    {
        GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
    }

    // effects
    void manageEffectTimer()
    {
        //if (m_loopingEffectIsActive)
        //    return;

        //if (m_onGatherStartDeactivatePermaEffectTime <= 0)
        //    return;


        //if (m_activateLoopingEffectsTime <= Time.time)
            // activateLoopingEffects();
    }
    void deactivateLoopingEffects()
    {
        if (m_onGatherStartDeactivatePermaEffectTime <= 0)
            return;

        m_activateLoopingEffectsTime = Time.time + m_onGatherStartDeactivatePermaEffectTime;

        List<GameObject> effects = GetComponent<CubeEntityParticleSystem>().m_loopingParticleEffects;
        foreach (GameObject effect in effects)
        {
            effect.GetComponent<EntityIsParticleEffect>().deactivateRenderer(m_onGatherStartDeactivatePermaEffectTime, false);
            //ParticleSystemRenderer[] particleSystemRenderer = effect.GetComponentsInChildren<ParticleSystemRenderer>();
            //foreach(ParticleSystemRenderer renderer in particleSystemRenderer)
            //{
            //    Debug.Log("0");
            //    renderer.enabled = false;
            //}
        }
        m_loopingEffectIsActive = false;
    }
    void createOnNotGatheredRemoveEffects()
    {
        foreach (GameObject effect in m_onNotGatheredRemoveParticleEffects)
        {
            GameObject e = CubeEntityParticleSystem.createParticleEffet(effect, transform.position);
        }
    }
    //void activateLoopingEffects()
    //{
    //    if (m_onGatherStartDeactivatePermaEffectTime <= 0)
    //        return;

    //    Debug.Log("1: " + Time.time);
    //    List<GameObject> effects = GetComponent<CubeEntityParticleSystem>().m_loopingParticleEffects;
    //    foreach (GameObject effect in effects)
    //    {
    //        effect.GetComponent<EntityIsParticleEffect>().activateRenderer();
    //        //ParticleSystemRenderer[] particleSystemRenderer = effect.GetComponentsInChildren<ParticleSystemRenderer>();
    //        //foreach (ParticleSystemRenderer renderer in particleSystemRenderer)
    //        //{
    //        //    Debug.Log("1");
    //        //    renderer.enabled = true;
    //        //}
    //    }

    //    m_loopingEffectIsActive = true;
    //}

    public void onCopy(ICopiable copiable)
    {
        initializeStuff();
        RessourceEntityIsRessource copy = (RessourceEntityIsRessource)copiable;

        m_gatherValues = new float[copy.m_gatherValues.Length];
        for (int i = 0; i < copy.m_gatherValues.Length; i++)
            m_gatherValues[i] = copy.m_gatherValues[i];

        m_onGatherStartParticleEffects = new GameObject[copy.m_onGatherStartParticleEffects.Length];
        for (int i = 0; i < copy.m_onGatherStartParticleEffects.Length; i++)
            m_onGatherStartParticleEffects[i] = copy.m_onGatherStartParticleEffects[i];
        m_onGatherDoneParticleEffects = new GameObject[copy.m_onGatherDoneParticleEffects.Length];
        for (int i = 0; i < copy.m_onGatherDoneParticleEffects.Length; i++)
            m_onGatherDoneParticleEffects[i] = copy.m_onGatherDoneParticleEffects[i];
        m_onNotGatheredRemoveParticleEffects = new GameObject[copy.m_onNotGatheredRemoveParticleEffects.Length];
        for (int i = 0; i < copy.m_onNotGatheredRemoveParticleEffects.Length; i++)
            m_onNotGatheredRemoveParticleEffects[i] = copy.m_onNotGatheredRemoveParticleEffects[i];
        m_onGatherStartDeactivatePermaEffectTime = copy.m_onGatherStartDeactivatePermaEffectTime;
        m_gatherCheckIntervall = copy.m_gatherCheckIntervall;
        m_gatherRadius = copy.m_gatherRadius;

        m_removeCheckIntervall = copy.m_removeCheckIntervall;
        m_removeDistance = copy.m_removeDistance;

        m_gatherDoneDistance = copy.m_gatherDoneDistance;
        m_gatherMovementSpeed = copy.m_gatherMovementSpeed;

        m_curveTime = copy.m_curveTime;
        m_curve = copy.m_curve;


        m_loopingEffectIsActive = true;
    }
}
