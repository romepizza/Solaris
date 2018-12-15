using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntitySkillTeleport : EntityCopiableAbstract
{

    public bool m_useSkill = true;

    [Header("------- Input -------")]
    public KeyCode keyCode0;
    public KeyCode keyCode1;
    public KeyCode keyCode2;
    public KeyCode doubleTapKeyCode;

    [Header("----- SETTINGS -----")]

    public float m_cooldown;

    [Header("--- Ressources) ---")]
    public float[] m_ressourceCosts;
    public float[] m_ressourcesMinNeeded;
    public AnimationCurve m_ressourceCurve;

    [Header("--- (Effect) ---")]
    public float m_teleportDelay;
    public float m_teleportDistance;
    public float m_colliderSize;
    public float m_backUpDistance;
    public float m_angleForOriginalDirection;
    [Header("--- (Camera Lerp) ---")]
    public float m_cameraLerpDelay;
    public float m_cameraLerpTime;
    public float m_cameraLerpStartT;
    public AnimationCurve m_cameraLerpAnimationCurve;

    [Header("- Particle Effects -")]
    public GameObject[] m_onTeleportParticleEffects;
    public float[] m_onTeleportParticleEffectCooldowns;

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
    public PlayerEntityCameraRotation m_cameraScript;
    public CubeEntityRessourceManager m_ressourceManager;

    public float m_teleportRdyTime;
    public bool m_rdyToTeleport;

    public bool m_isInput;
    public bool m_isCooldown;
    public bool m_isActive;
    public bool m_activeThisFrame;

    public float m_cameraLerpDone;
    public float m_cameraLerpCurrentT;
    public bool m_isCameraLerping;

    public float[] m_onTeleportParticleCooldownsRdy;
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

        if (m_cameraScript == null)
            m_cameraScript = Constants.getPlayer().GetComponent<PlayerEntityCameraRotation>();
        if (m_cameraScript == null)
            Debug.Log("Warning: cameraScript is null!");

        if (m_ressourceManager == null)
            m_ressourceManager = (CubeEntityRessourceManager)Utility.getComponentInParents<CubeEntityRessourceManager>(transform);
        if (m_ressourceManager == null)
            Debug.Log("Warning: ressourceManager is null!");

        checkParticleEffects();
    }

    void checkParticleEffects()
    {
        if (m_onTeleportParticleEffects == null && m_onTeleportParticleEffectCooldowns != null)
            Debug.Log("Warning!");
        if (m_onTeleportParticleEffects != null && m_onTeleportParticleEffectCooldowns == null)
            Debug.Log("Warning!");

        if (m_onTeleportParticleEffects.Length != m_onTeleportParticleEffectCooldowns.Length)
            Debug.Log("Warning");

        m_onTeleportParticleCooldownsRdy = new float[m_onTeleportParticleEffects.Length];
        for (int i = 0; i < m_onTeleportParticleCooldownsRdy.Length; i++)
            m_onTeleportParticleCooldownsRdy[i] = float.MaxValue;

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

        //particle
        manageParticleEffects();

        // teleport
        manageDelayTeleport();
        if (m_rdyToTeleport)
        {
            getRessources();
            if (m_ressourceFactor > 0)
            {
                getDirectionInput();
                getForceVector();
                performTeleport();
                initializeCameraLerp();
            }
            m_isActive = false;
            m_rdyToTeleport = false;
        }

        manageCameraLerp();

        m_activeThisFrame = false;
    }

    // cooldown and delay
    void manageCooldown()
    {
        if (!m_isActive && !m_isCooldown && m_isInput && m_cooldownRdy <= Time.time)
        {
            m_cooldownRdy = m_cooldown + Time.time;
            m_teleportRdyTime = m_teleportDelay + Time.time;
            m_isCooldown = true;
            m_isActive = true;
            m_activeThisFrame = true;
        }
        else
        {
            m_isCooldown = false;
        }
    }
    bool manageDelayTeleport()
    {
        if (m_isActive && m_teleportRdyTime <= Time.time)
        {
            m_rdyToTeleport = true;
            return true;
        }
        else
        {
            m_rdyToTeleport = false;
            return false;
        }
    }

    // input and direction
    void getRessources()
    {
        bool hasEnoughRessources = m_ressourceManager.hasEnoughRessurces(m_ressourcesMinNeeded);
        if(hasEnoughRessources)
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

    // perform teleport
    void performTeleport()
    {
        if (m_teleportDistance == 0)
            return;

        Vector3 teleportDestination = Vector3.zero;

        float finalDistance = m_teleportDistance * m_ressourceFactor;
        if (finalDistance == 0)
            return;

        bool isOkay = false;
        do
        {
            teleportDestination = transform.position + m_finalDirection.normalized * finalDistance;
            Collider[] colliders = Physics.OverlapSphere(teleportDestination, m_colliderSize);

            if (Physics.OverlapSphere(teleportDestination, m_colliderSize).Length > 0)
            {
                if (finalDistance > m_colliderSize)
                {
                    finalDistance -= m_backUpDistance;
                }
                else
                {
                    break;
                }
            }
            else
            {
                isOkay = true;
            }
        } while (!isOkay);

        if (isOkay)
        {
            m_player.transform.position = teleportDestination;
        }
    }

    // particle system
    void manageParticleEffects()
    {

        // if activated this frame, set all rdy timers
        if (m_activeThisFrame)
        {
            for (int i = 0; i < m_onTeleportParticleEffects.Length; i++)
            {
                m_onTeleportParticleCooldownsRdy[i] = m_onTeleportParticleEffectCooldowns[i] + Time.time;
            }
        }

        // check teleport rdy
        for (int i = 0; i < m_onTeleportParticleEffects.Length; i++)
        {
            if (m_onTeleportParticleCooldownsRdy[i] <= Time.time)
            {
                if (m_onTeleportParticleEffects[i] == null)
                {
                    Debug.Log("Warning: effect was null!");
                    continue;
                }

                GameObject effect = null; // m_onTeleportParticleEffects[i];
                if (m_onTeleportParticleEffects[i].GetComponent<EntityIsParticleEffect>().m_stayOnGameObject)
                    effect = m_particleSystem.createParticleEffect(m_onTeleportParticleEffects[i]);
                else
                {
                    effect = CubeEntityParticleSystem.createParticleEffet(m_onTeleportParticleEffects[i], transform.position);// + m_rb.velocity * m_onTeleportParticleEffectCooldowns[i]);
                    effect.transform.rotation = Quaternion.LookRotation(-m_finalDirection);
                }
                m_onTeleportParticleCooldownsRdy[i] = float.MaxValue;
            }
        }
    }

    // camera lerp
    void initializeCameraLerp()
    {
        m_isCameraLerping = true;
        m_cameraLerpDone = m_cameraLerpTime + Time.time;
        m_cameraLerpCurrentT = 0;
    }
    void manageCameraLerp()
    {
        if (!m_isCameraLerping)
            return;

        if(m_cameraLerpDone <= Time.time)
        {
            m_cameraScript.m_t = m_cameraScript.m_defaultT;
            m_isCameraLerping = false;
        }

        float finalT = 0;
        m_cameraLerpCurrentT += Time.deltaTime / m_cameraLerpTime;

        float curveValue = m_cameraLerpAnimationCurve.Evaluate(m_cameraLerpCurrentT);

        finalT = m_cameraLerpStartT + curveValue * (1 - m_cameraLerpStartT);// - (1 - m_cameraScript.m_defaultT);

        //Debug.Log("0: " + m_cameraLerpStartT);
        //Debug.Log("1: " + curveValue);
        //Debug.Log("2: " + curveValue * (1 - m_cameraLerpStartT));
        //Debug.Log("3: " + (1 - m_cameraScript.m_defaultT));

        m_cameraScript.m_t = finalT;
    }

    void getInput()
    {
        m_isInput = gameObject.layer != 9 || Input.GetKeyDown(keyCode0) || Input.GetKeyDown(keyCode1) || Input.GetKeyDown(keyCode2);
    }
}
