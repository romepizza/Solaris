using UnityEngine;

public class PlayerEntitySkillDash : EntityCopiableAbstract
{
    public bool m_useSkill = true;

    [Header("------- Input -------")]
    public KeyCode keyCode0;
    public KeyCode keyCode1;
    public KeyCode keyCode2;

    [Header("----- SETTINGS -----")]

    public float m_cooldown;

    [Header("--- (Effect) ---")]
    public float m_dashDelay;
    public float m_forcePower;
    public float m_angleForOriginalDirection;

    [Header("--- Ressources) ---")]
    public float[] m_ressourceCosts;
    public float[] m_ressourcesMinNeeded;
    public AnimationCurve m_ressourceCurve;

    [Header("- Particle Effects -")]
    public GameObject[] m_onDashParticleEffects;
    public float[] m_onDashParticleEffectCooldowns;

    [Header("------- Debug -------")]
    public bool m_isPressingButtonUp;
    public bool m_isPressingButtonDown;
    public bool m_isPressingButtonForward;
    public bool m_isPressingButtonBackward;
    public bool m_isPressingButtonLeft;
    public bool m_isPressingButtonRight;
    public bool m_isPressingAnyButtonXZ;
    public bool m_isPressingAnyButton;

    public Vector3 m_finalDirection;
    public Vector3 m_tmpDirection;
    public GameObject m_player;

    public float m_cooldownRdy;

    public Rigidbody m_rb;
    public CubeEntityParticleSystem m_particleSystem;
    public CubeEntityRessourceManager m_ressourceManager;

    public float m_dashRdyTime;
    public bool m_rdyToDash;

    public bool m_isInput;
    public bool m_isCooldown;
    public bool m_isActive;
    public bool m_activeThisFrame;

    public float[] m_onDashParticleCooldownsRdy;
    public float[] m_ressourcesUsed;
    public float[] m_ressourceFactors;
    public float m_ressourceFactor;

    private void Start()
    {
        initializeStuff();
    }

    void initializeStuff()
    {
        m_rb = (Rigidbody)Utility.getComponentInParents<Rigidbody>(transform);
        if (m_rb == null)
            Debug.Log("Warning: Rigidbody is null!");

        m_particleSystem = (CubeEntityParticleSystem)Utility.getComponentInParents<CubeEntityParticleSystem>(transform);
        if (m_particleSystem == null)
            Debug.Log("Warning: ParticleSystem is null!");

        if (m_player == null)
            m_player = Constants.getPlayer();

        if (m_player == null)
            Debug.Log("Warning: player is null!");

        if (m_ressourceManager == null)
            m_ressourceManager = (CubeEntityRessourceManager)Utility.getComponentInParents<CubeEntityRessourceManager>(transform);
        if (m_ressourceManager == null)
            Debug.Log("Warning: ressourceManager is null!");

        checkParticleEffects();
    }

    void checkParticleEffects()
    {
        if (m_onDashParticleEffects == null && m_onDashParticleEffectCooldowns != null)
            Debug.Log("Warning!");
        if (m_onDashParticleEffects != null && m_onDashParticleEffectCooldowns == null)
            Debug.Log("Warning!");

        if (m_onDashParticleEffects.Length != m_onDashParticleEffectCooldowns.Length)
            Debug.Log("Warning");

        m_onDashParticleCooldownsRdy = new float[m_onDashParticleEffects.Length];
        for (int i = 0; i < m_onDashParticleCooldownsRdy.Length; i++)
            m_onDashParticleCooldownsRdy[i] = float.MaxValue;

        if (m_ressourcesMinNeeded.Length != m_ressourceCosts.Length)
            Debug.Log("Warning");
    }

    private void Update()
    {
        manageSkill();
    }

    void manageSkill()
    {
        getInput();
        manageCooldown();

        // dash
        manageDelayDash();
        if (m_rdyToDash)
        {
            getRessources();
            if (m_ressourceFactor > 0)
            {
                getDirectionInput();
                getForceVector();
                performDash();
            }
            m_isActive = false;
            m_rdyToDash = false;
        }

        //particle
        manageParticleEffects();

        m_activeThisFrame = false;
    }

    void getRessources()
    {
        bool hasEnoughRessources = m_ressourceManager.hasEnoughRessurces(m_ressourcesMinNeeded);
        if (hasEnoughRessources)
        {
            m_ressourcesUsed = m_ressourceManager.addRessources(m_ressourceCosts);
            m_ressourceFactors = new float[m_ressourcesUsed.Length];
            for (int i = 0; i < m_ressourceFactors.Length; i++)
                m_ressourceFactors[i] = m_ressourceCosts[i] < 0 ? (m_ressourcesUsed[i] / m_ressourceCosts[i]) : 1f;
        }
        else
        {
            //m_ressourcesUsed = m_ressourceManager.addRessources(m_ressourceCosts);
            m_ressourceFactors = new float[m_ressourcesUsed.Length];
            for (int i = 0; i < m_ressourceFactors.Length; i++)
                m_ressourceFactors[i] = 0;
        }

        m_ressourceFactor = m_ressourceCurve.Evaluate(Mathf.Clamp01(Mathf.Min(m_ressourceFactors)));
    }
    void manageCooldown()
    {
        if (!m_isActive && !m_isCooldown && m_isInput && m_cooldownRdy <= Time.time)
        {
            m_cooldownRdy = m_cooldown + Time.time;
            m_dashRdyTime = m_dashDelay + Time.time;
            m_isCooldown = true;
            m_isActive = true;
            m_activeThisFrame = true;
        }
        else
        {
            m_isCooldown = false;
        }
    }

    bool manageDelayDash()
    {
        if(m_isActive && m_dashRdyTime <= Time.time)
        {
            m_rdyToDash = true;
            return true;
        }
        else
        {
            m_rdyToDash = false;
            return false;
        }
    }

    void getDirectionInput()
    {
        m_isPressingButtonUp = false;
        m_isPressingButtonDown = false;
        m_isPressingButtonForward = false;
        m_isPressingButtonBackward = false;
        m_isPressingButtonLeft = false;
        m_isPressingButtonRight = false;
        m_isPressingAnyButtonXZ = false;
        m_isPressingAnyButton = false;


        if (Input.GetKey(KeyCode.W) || Input.GetAxis("LeftStickVertical") < 0)
        {
            m_isPressingButtonForward = true;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetAxis("LeftStickVertical") > 0)
        {
            m_isPressingButtonBackward = true;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetAxis("LeftStickHorizontal") < 0)
        {
            m_isPressingButtonLeft = true;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetAxis("LeftStickHorizontal") > 0)
        {
            m_isPressingButtonRight = true;
        }
        if (Input.GetKey(KeyCode.Space) || Input.GetAxis("RTrigger") < -0.3f)
        {
            m_isPressingButtonUp = true;
        }
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift) || Input.GetAxis("LTrigger") > 0)
        {
            m_isPressingButtonDown = true;
        }


        m_isPressingAnyButtonXZ = m_isPressingButtonBackward || m_isPressingButtonForward || m_isPressingButtonLeft || m_isPressingButtonRight;
        m_isPressingAnyButton = m_isPressingAnyButtonXZ || m_isPressingButtonDown || m_isPressingButtonUp;
    }

    void getForceVector()
    {
        m_tmpDirection = Vector3.zero;
        Vector3 forwardDirection = Vector3.zero;

        if (m_isPressingButtonUp)
        {
            m_tmpDirection += Vector3.up;
        }
        if (m_isPressingButtonDown)
        {
            m_tmpDirection += -Vector3.up;
        }
        if (m_isPressingButtonForward)
        {
            forwardDirection += Vector3.forward;
        }
        if (m_isPressingButtonBackward)
        {
            m_tmpDirection += -Vector3.forward;
        }
        if (m_isPressingButtonLeft)
        {
            m_tmpDirection += -Vector3.right;
        }
        if (m_isPressingButtonRight)
        {
            m_tmpDirection += Vector3.right;
        }

        if (m_tmpDirection == Vector3.zero)
            m_tmpDirection = Vector3.forward;

        

        if (m_tmpDirection == Vector3.zero)
            m_finalDirection = Constants.getMainCamera().transform.rotation * forwardDirection;
        else
        {
            m_tmpDirection = Constants.getMainCamera().transform.rotation * (m_tmpDirection + forwardDirection);
            Vector3 currentDirection = m_rb.velocity;

            if (Utility.getAngle(m_tmpDirection, currentDirection) < m_angleForOriginalDirection)
            {
                m_finalDirection = currentDirection;
            }
            else
            {
                m_finalDirection = m_tmpDirection;
            }
        }
    }

    void performDash()
    {
        if (m_rb != null)
        {
            m_rb.velocity += m_finalDirection.normalized * m_forcePower * m_ressourceFactor;
        }
        else
            Debug.Log("Aborted: Rigidbody was null!");
    }

    void manageParticleEffects()
    {
        // if activated this frame, set all rdy timers
        if (m_activeThisFrame)
        {
            for (int i = 0; i < m_onDashParticleEffects.Length; i++)
            {
                m_onDashParticleCooldownsRdy[i] = m_onDashParticleEffectCooldowns[i] + Time.time;
            }
        }

        // check dash rdy
        for (int i = 0; i < m_onDashParticleEffects.Length; i++)
        {
            if (m_onDashParticleCooldownsRdy[i] <= Time.time)
            {
                if (m_onDashParticleEffects[i] == null)
                {
                    Debug.Log("Warning: effect was null!");
                    continue;
                }

                GameObject effect = null;
                if (m_onDashParticleEffects[i].GetComponent<EntityIsParticleEffect>().m_stayOnGameObject)
                    effect = m_particleSystem.createParticleEffect(m_onDashParticleEffects[i]);
                else
                {
                    effect = CubeEntityParticleSystem.createParticleEffet(m_onDashParticleEffects[i], transform.position);
                    effect.transform.rotation = Quaternion.LookRotation(-m_finalDirection);
                }
                m_onDashParticleCooldownsRdy[i] = float.MaxValue;
            }
        }
    }

    void getInput()
    {
        m_isInput = gameObject.layer != 9 || Input.GetKeyDown(keyCode0) || Input.GetKeyDown(keyCode1) || Input.GetKeyDown(keyCode2);
    }
}
