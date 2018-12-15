using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntityBase : EntityCopiableAbstract, ICopiable, IPostCopy, IRemoveOnStateChange, IPrepareRemoveOnStateChange
{
    [Header("----- SETTINGS -----")]
    public bool m_faceTarget;
    public bool m_isMovable;
    public float m_aimColliderRadius;

    [Header("--- (Hp) ---")]
    public int m_maxHp;
    public int m_minHp;
    //public GameObject m_deathExplosion;
    

    [Header("--- (Cubes) ---")]
    public int m_maxCubes;

    [Header("----- DEBUG -----")]
    public int m_currentHp;
    public int m_currentMaxCubes;
    public List<LevelEntityMonsterManager> m_registeredInManager;
    public SphereCollider m_collider;

    public float m_size;
    //public GameObject m_target;

    public Rigidbody m_rb;
	void Start ()
    {
        m_rb = GetComponent<Rigidbody>();
        if (m_rb == null)
            Debug.Log("Warning: No rigidbody detected!");

        initializeValues();
    }

    // Update is called once per frame
    
    void Update ()
    {
        if (m_faceTarget)
            faceTarget();
	}
    
    void faceTarget()
    {
        if (GetComponent<CubeEntityTargetManager>().m_target != null)
            transform.rotation = Quaternion.LookRotation(GetComponent<CubeEntityTargetManager>().m_target.transform.position - transform.position);
    }

    // Setter
    /*
    public void createMonster(GameObject prefab)
    {
        setValues(prefab);
        //GetComponent<CubeEntitySystem>().copyPasteComponents(prefab);
        applyCollider();
        m_size = Mathf.Max(Mathf.Max(5f, m_maxHp / 10f));
        //setSize();
    }
    */

    
    /*
    public void setValues(GameObject prefab)
    {
        // Base Script
        MonsterEntityBase scriptBase = prefab.GetComponent<MonsterEntityBase>();
        if (scriptBase != null)
        {
            m_isMovable = scriptBase.m_isMovable;
            m_minHp = scriptBase.m_minHp;
            m_maxHp = scriptBase.m_maxHp;
            m_maxCubes = scriptBase.m_maxCubes;
            m_aimColliderRadius = scriptBase.m_aimColliderRadius;

            m_deathExplosion = scriptBase.m_deathExplosion;
        }
        else
            Debug.Log("Warning: Tried to Copy MonsterEntityBase script from prefab, that didn't have it attached!");
        initializeValues();

        // other scripts
        EntityCopiableAbstract[] copyScripts = prefab.GetComponents<EntityCopiableAbstract>();

        foreach(EntityCopiableAbstract copyScript in copyScripts)
        {

            System.Type type = copyScript.GetType();
            Component copy = gameObject.AddComponent(type);

            ((EntityCopiableAbstract)copy).enabled = copyScript.enabled;
            ((EntityCopiableAbstract)copy).pasteScript(copyScript);
        }
        foreach(EntityCopiableAbstract script in gameObject.GetComponents<EntityCopiableAbstract>())
        {
            script.assignScripts();
        }


        m_attachSystemScript = GetComponent<MonsterEntityAttachSystem>();
        m_attachSystemScriptNew = GetComponent<AttachSystemBase>();
        m_attachedToScript = GetComponent<CubeEntityAttached>();

        m_ejectorScript = GetComponent<MonsterEntityEjector>();
        m_wormScript = GetComponent<MonsterEntityWorm>();

        m_grabScript = GetComponent<MonsterEntitySkillGrab>();
        m_ejectScript = GetComponent<MonsterEntitySkillEject>();


        GetComponent<CubeEntityState>().m_attachedScript = m_attachedToScript;
        m_attachedToScript.m_attachedToGameObject = gameObject;
    }
    */
    // Hp
    public void loseHp(int lifeLoss, GameObject cube)
    {
        m_currentHp -= lifeLoss;
        //m_currentMaxCubes = Mathf.Min(m_currentHp, m_maxCubes);

        //if (cube != null)
           // m_attachSystemScript.deregisterCube(cube);

        manageHp();
    }

    public void changeMaxCubes(int cubeNumber)
    {
        m_currentMaxCubes += cubeNumber;
        m_currentMaxCubes = Mathf.Max(0, m_currentMaxCubes);
    }

    void manageHp()
    {
        //setSize();
        if(m_currentHp < m_minHp)
        {
            die();
        }
    }

    // Die
    public void die()
    {
        //SoundManager.addSoundEjectorDeath(transform.position);

        foreach (LevelEntityMonsterManager script in m_registeredInManager)
        {
            script.deregisterEnemy(this.gameObject);
        }
        //activateDeathEffect();
        
        //GetComponent<CubeEntityState>().removeAttachedScript(); List<EntityCopiableAbstract> list = new List<EntityCopiableAbstract>(GetComponents<EntityCopiableAbstract>());
        //int i = 0;
        //foreach (EntityCopiableAbstract script in list)
        //{
        //    //Debug.Log("i_" + i + " :" + gameObject.GetComponents<Collider>());
        //    i++;
        //    script.prepareDestroyScript();
        //}
        //Destroy(this);

        gameObject.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
    }
    public void activateDeathEffect()
    {
        //MonsterEntityDeathEffect[] deathEffects = gameObject.GetComponents<MonsterEntityDeathEffect>();
    }

    // Collider
    void applyCollider()
    {
        m_collider = gameObject.AddComponent<SphereCollider>();
        m_collider.radius = m_aimColliderRadius;
        m_collider.isTrigger = true;
    }
    void removeCollider()
    {
        Destroy(m_collider);
        //m_collider = null;
    }

    // Intern
    public void initializeValues()
    {
        m_currentHp = m_maxHp;
        m_currentMaxCubes = Mathf.Min(m_maxCubes);
        m_registeredInManager = new List<LevelEntityMonsterManager>();
    }
    // target
    //public GameObject getTarget()
    //{
        
    //}
    
    // copy
    void setValues(MonsterEntityBase baseScript)
    {
        m_faceTarget = baseScript.m_faceTarget;
        m_isMovable = baseScript.m_isMovable;
        m_minHp = baseScript.m_minHp;
        m_maxHp = baseScript.m_maxHp;
        m_maxCubes = baseScript.m_maxCubes;
        m_aimColliderRadius = baseScript.m_aimColliderRadius;

        //m_deathExplosion = baseScript.m_deathExplosion;
    }

    // interface
    public void onStateChangePrepareRemove()
    {
        removeCollider();
    }
    public void onRemove()
    {
        //if (gameObject != Constants.getPlayer())
            Destroy(this);
    }
    // abstract
    public void onCopy(ICopiable copiable)
    {
        setValues((MonsterEntityBase)copiable);
    }
    public void onPostCopy()
    {
        CubeEntityAttached attachedToScript = GetComponent<CubeEntityAttached>();
        GetComponent<CubeEntityState>().m_attachedScript = attachedToScript;
        attachedToScript.m_attachedToGameObject = gameObject;

        
        //Debug.Log(m_target);
        applyCollider();
    }
}
