using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntityDeathEffectActivateCubes : MonsterEntityDeathEffect, ICopiable, IPostCopy, IStateOnStateChange, IRemoveOnStateChange
{
    public GameObject m_spawnPrefab;
    public AudioSource m_deathSound;
    [Header("-------- Settings -------")]
    public int m_cubeTossNumber;
    public bool m_createAditional;
    public bool m_setOtherToInactive;
    //public float m_explosionRadius;
    //public float m_explosionPower;
    //public float m_activeDuration;
    //public float m_maxSpeed;

    [Header("--- (Effect) ---")]
    public float m_tossCooldownActivate;
    public int m_tossesPerBurstActivate;
    public float m_tossCooldownDeactivate;
    public int m_tossesPerBurstDeactivate;
    public List<CubeEntityMovementAbstract> m_movementScripts;
    public List<CubeEntityMovementStartSpeed> m_startSpeedScript;

    [Header("------- Debug -------")]
    public MonsterEntityBase m_baseScript;
    public CubeEntityState m_stateScript;
    public CubeEntityTargetManager m_monsterOrigin;

    /*
    //public override void setValues(GameObject prefab, MonsterEntityBase baseScript)
    //{
    MonsterEntityDeathEffectExplosion script = prefab.GetComponent<MonsterEntityDeathEffectExplosion>();
    m_cubeTossNumber = script.m_cubeTossNumber;
    m_explosionRadius = script.m_explosionRadius;
    m_explosionPower = script.m_explosionPower;
    m_activeDuration = script.m_activeDuration;
    m_maxSpeed = script.m_maxSpeed;
    m_tossesPerFrame = script.m_tossesPerFrame;
    m_movementScript = script.m_movementScript;
    m_baseScript = baseScript;
    //}
    */

    public override void activateDeathEffect(MonsterEntityBase baseScript)
    {
        GameObject child = new GameObject("MonsterEntityDeathEffectExplosionEffect");
        child.transform.position = transform.position;
        MonsterEntityDeathEffectActivateCubesEffect effect = child.AddComponent<MonsterEntityDeathEffectActivateCubesEffect>();
        EntitySystemBase entitySystemBaseScript = effect.gameObject.AddComponent<EntitySystemBase>();
        GameObject spawnPrefab = Instantiate(m_spawnPrefab, child.transform);
        effect.m_spawnPrefab = spawnPrefab;
        effect.m_tossesPerBurstActivate = m_tossesPerBurstActivate;
        effect.m_setOtherToInactive = m_setOtherToInactive;
        effect.m_tossCooldownActivate = m_tossCooldownActivate;
        effect.m_tossCooldownDeactivate = m_tossCooldownDeactivate;
        effect.m_tossesPerBurstDeactivate = m_tossesPerBurstDeactivate;
        effect.m_createAdditional = m_createAditional;
        effect.m_cubeTossesNumber = m_cubeTossNumber;
        effect.m_target = m_monsterOrigin.getTarget();
        effect.m_cubes = new Queue<GameObject>();

        effect.m_movementScript = new List<CubeEntityMovementAbstract>();
        if (m_movementScripts != null)
        {
            foreach (CubeEntityMovementAbstract e in m_movementScripts)
            {
                if (e == null)
                    continue;

                CubeEntityMovementAbstract script = null;

                if (e is ICopiable)
                    script = (CubeEntityMovementAbstract)(entitySystemBaseScript.copyPasteComponent((ICopiable)e, false));
                else
                    Debug.Log("Should not happen!");

                if (script == null)
                    Debug.Log("Should not happen!");
                else
                    script.pasteScriptButDontActivate(e);


                effect.m_movementScript.Add(script);
            }
        }
        effect.m_startSpeedScript = new List<CubeEntityMovementStartSpeed>();
        if (m_startSpeedScript != null)
        {
            foreach (CubeEntityMovementStartSpeed e in m_startSpeedScript)
            {
                if (e == null)
                    continue;

                CubeEntityMovementStartSpeed script = null;

                if (e is ICopiable)
                    script = (CubeEntityMovementStartSpeed)(entitySystemBaseScript.copyPasteComponent(e, false));
                else
                    Debug.Log("Should not happen!");

                if (script == null)
                    Debug.Log("Should not happen!");


                effect.m_startSpeedScript.Add(script);
            }
        }

        AttachSystemBase attachSystem = m_baseScript.GetComponent<AttachSystemBase>();
        //GameObject[] cubesGrabbedPreviously = new GameObject[attachSystem.m_cubeList.Count];
        for (int i = 0; i < attachSystem.m_cubeList.Count; i++)
            effect.m_cubes.Enqueue(attachSystem.m_cubeList[i]);//cubesGrabbedPreviously[i] = attachSystem.m_cubeList[i];
        //if(m_createAditional)
        //{
        //    for (int i = 0; i < m_cubeTossNumber - effect.m_cubes.Count; i++)
        //    {
        //        effect.m_cubes.Enqueue(Constants.getMainCge().activateCubeUnsafe(transform.position + Random.insideUnitSphere * 20f));
        //    }
        //}

        effect.initializeStuff();

        //List<GameObject> cubesGrabbedPreviously = new List<GameObject>();
        //List<GameObject> cubesSetToInactive = new List<GameObject>();
        //AttachSystemBase attachSystem = m_baseScript.GetComponent<AttachSystemBase>();
        //for (int i = 0; i < attachSystem.m_cubeList.Count; i++)
        //    cubesGrabbedPreviously.Add(attachSystem.m_cubeList[i]);
        //// select cubes to launch
        //int numberLoops = Mathf.Max(cubesGrabbedPreviously.Count, m_cubeTossNumber);
        //for (int i = 0; i < numberLoops; i++)
        //{
        //    GameObject cube = null;
        //    if (i < cubesGrabbedPreviously.Count)
        //    {
        //        cube = cubesGrabbedPreviously[i];
        //        if (cube != null && i < m_cubeTossNumber)
        //        {
        //            effect.m_cubes.Enqueue(cube);
        //            CgeDoNotSetToInactive script = cube.AddComponent<CgeDoNotSetToInactive>();
        //        }
        //        else if (cube != null)
        //        {
        //            cubesSetToInactive.Add(cube);
        //        }
        //    }
        //    else
        //    {
        //        cube = Constants.getMainCge().activateCubeUnsafe(transform.position + Random.insideUnitSphere * 20f);
        //        if (cube == null)
        //        {
        //            Debug.Log("Warning: cube was null!");
        //            continue;
        //        }
        //        effect.m_cubes.Enqueue(cube);
        //        CgeDoNotSetToInactive script = cube.AddComponent<CgeDoNotSetToInactive>();
        //    }
        //}
        //int j = 0;
        //foreach (GameObject cube in cubesGrabbedPreviously)
        //{
        //    if (cube != null && j < m_cubeTossNumber)
        //    {
        //        effect.m_cubes.Enqueue(cube);
        //        CgeDoNotSetToInactive script = cube.AddComponent<CgeDoNotSetToInactive>();
        //    }
        //    else if (cube != null)
        //    {
        //        cubesSetToInactive.Add(cube);
        //    }
        //    j++;
        //}
        //set others to inactive
        //foreach (GameObject cube in cubesSetToInactive)
        //{
        //    cube.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
        //}

        if (m_deathSound != null)
        {
            m_deathSound.Play();
        }

        Destroy(this);
    }

    // copy
    public void setValues(MonsterEntityDeathEffectActivateCubes prefab)
    {
        m_spawnPrefab = prefab.m_spawnPrefab;
        m_cubeTossNumber = prefab.m_cubeTossNumber;
        m_createAditional = prefab.m_createAditional;
        m_setOtherToInactive = prefab.m_setOtherToInactive;
        m_tossesPerBurstActivate = prefab.m_tossesPerBurstActivate;
        m_tossCooldownActivate = prefab.m_tossCooldownActivate;
        m_tossCooldownDeactivate = prefab.m_tossCooldownDeactivate;
        m_tossesPerBurstDeactivate = prefab.m_tossesPerBurstDeactivate;
        //m_explosionRadius = prefab.m_explosionRadius;
        //m_explosionPower = prefab.m_explosionPower;
        //m_activeDuration = prefab.m_activeDuration;
        //m_maxSpeed = prefab.m_maxSpeed;
        m_movementScripts = new List<CubeEntityMovementAbstract>();
        foreach (CubeEntityMovementAbstract script in prefab.m_movementScripts)
            m_movementScripts.Add(script);
        m_startSpeedScript = new List<CubeEntityMovementStartSpeed>();
        foreach (CubeEntityMovementStartSpeed script in prefab.m_startSpeedScript)
            m_startSpeedScript.Add(script);
    }

    // interfaces
    public void onStateEnter()
    {

    }
    public void onStateExit()
    {
        activateDeathEffect(null);
    }
    public void onCopy(ICopiable copiable)
    {
        //setValues((MonsterEntityDeathEffectActivateCubes)copiable);
        //Debug.Log("Warning");
    }
    public void onPostCopy()
    {
        m_baseScript = (MonsterEntityBase)Utility.getComponentInParents<MonsterEntityBase>(transform);// GetComponent<MonsterEntityBase>();
        m_stateScript = (CubeEntityState)Utility.getComponentInParents<CubeEntityState>(transform); //GetComponent<CubeEntityState>();
        m_monsterOrigin = (CubeEntityTargetManager)Utility.getComponentInParents<CubeEntityTargetManager>(transform); //GetComponent<CubeEntityTargetManager>();
    }
    public void onRemove()
    {
        Destroy(this);
    }



    /*
    // abstract
    public override void onCopy(EntityCopiableAbstract baseScript)
    {
        setValues((MonsterEntityDeathEffectActivateCubes)baseScript);
    }
    public override void prepareDestroyScript()
    {
        //activateDeathEffect(null);
        Destroy(this);
    }
    */
}
