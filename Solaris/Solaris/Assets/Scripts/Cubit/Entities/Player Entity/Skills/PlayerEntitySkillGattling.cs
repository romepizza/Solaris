using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntitySkillGattling : EntityCopiableAbstract
{
    public CubeEntityMovementAbstract[] m_movementScripts;
    [Header("----- SETTINGS -----")]
    public float m_cooldown;

    [Header("--- Aim ---")]
    public bool m_aimLock;
    public float m_randomRadius;

    [Header("--- (Potential Cubes) ---")]
    public bool m_fromGrabbedOnly;
    public bool m_fromGrabbedFirst;
    [Header("- (Box) -")]
    public bool m_useBox;
    public Vector3 m_boxSize;
    public Vector3 m_offsetBox;
    [Header("- (Sphere) -")]
    public bool m_useSphere;
    public float m_radiusSphere;
    public Vector3 m_offsetSphere;
    [Header("- (Hemisphere) -")]
    public bool m_useHemisphere;
    public float m_radiusHemisphere;
    public Vector3 m_offsetHemisphere;


    [Header("----- DEBUG -----")]
    public GameObject m_target;
    public bool m_isAutoLock;
    public bool m_isToggle;
    public float m_cooldownFinishTime;
    public Vector3 m_targetPosition;
    public List<GameObject> m_potentialCubes;
    public GameObject m_cubeToShoot;
    public bool m_cubeSelectedFromGrab;

    public PlayerEntityAttachSystem m_attachScript;

	// Use this for initialization
 	void Start ()
    {
        m_attachScript = GetComponent<PlayerEntityAttachSystem>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        manageShot();
	}

    void manageShot()
    {
        if(Input.GetButton("ButtonB"))
        {
            if (!m_isToggle)
            {
                m_isAutoLock = !m_isAutoLock;
                m_isToggle = true;
            }
        }
        else
        {
            m_isToggle = false;
        }

        if(Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.E))
            m_isAutoLock = !m_isAutoLock;



        if ((Input.GetKey(KeyCode.Mouse0) || Input.GetButton("RBumper") || Input.GetButton("ButtonA") || m_isAutoLock) && m_cooldownFinishTime < Time.time)
        {
            m_target = PlayerEntityAim.aim();
            getAimPositionImmovable();
            selectCube();
            getAimPositionMovable();
            shoot();
            m_cooldownFinishTime = m_cooldown + Time.time;
        }
    }

    void getAimPositionImmovable()
    {
        if (gameObject.GetComponent<PlayerEntityAim>() != null)
        {
            m_target = PlayerEntityAim.aim();
            if (m_target != null)
            {
                if (!m_target.GetComponent<MonsterEntityBase>().m_isMovable)
                {
                    if (m_aimLock)
                        m_targetPosition = m_target.transform.position + Random.insideUnitSphere * m_randomRadius;
                    else
                        m_targetPosition = Camera.main.transform.position + Camera.main.transform.forward * Vector3.Distance(Camera.main.transform.position, m_target.transform.position) + Random.insideUnitSphere * m_randomRadius;
                }
                else
                    m_targetPosition = m_target.transform.position + m_target.GetComponent<Rigidbody>().velocity.normalized * m_target.GetComponent<Rigidbody>().velocity.magnitude / Vector3.Distance(transform.position, m_target.transform.position);
            }
            else
                m_targetPosition = transform.position + Camera.main.transform.forward * 500f;
        }
        else
        {
            m_targetPosition = transform.position + Camera.main.transform.forward * 500f;
        }
    }

    void getAimPositionMovable()
    {
        if (gameObject.GetComponent<PlayerEntityAim>() != null && m_cubeToShoot != null)
        {
            if (m_target != null)
            {
                if (m_target.GetComponent<MonsterEntityBase>().m_isMovable)
                {
                    Vector3 enemyFuturePosition = m_target.transform.position + m_target.GetComponent<Rigidbody>().velocity * Vector3.Distance(m_cubeToShoot.transform.position, m_target.transform.position) / m_movementScripts[0].m_maxSpeed;
                    if (m_aimLock)
                        m_targetPosition = enemyFuturePosition;
                    else
                        m_targetPosition = Camera.main.transform.position + Camera.main.transform.forward * Vector3.Distance(Camera.main.transform.position, enemyFuturePosition) + Random.insideUnitSphere * m_randomRadius;
                }
            }
            else
                m_targetPosition = transform.position + Camera.main.transform.forward * 500f;
        }
        else
        {
            m_targetPosition = transform.position + Camera.main.transform.forward * 500f;
        }
    }

    void selectCube()
    {
        GameObject cubeSelected = null;

        // From grabbed Only
        if (m_fromGrabbedOnly)
        {
            m_cubeSelectedFromGrab = false;

            float minDist = float.MaxValue;
            foreach (GameObject cube in m_attachScript.m_cubeList)
            {
                float dist = Vector3.Distance(cube.transform.position, m_targetPosition);
                if (dist < minDist)
                {
                    m_cubeSelectedFromGrab = true;
                    cubeSelected = cube;
                    minDist = dist;
                }
            }
        }
        else
        {
            m_potentialCubes = new List<GameObject>();
            m_cubeSelectedFromGrab = true;
            // from grabbed first
            if (m_fromGrabbedFirst)
            {
                foreach (GameObject agent in m_attachScript.m_cubeList)
                    m_potentialCubes.Add(agent);
            }

            // if no cubes are grabbed
            if (m_potentialCubes.Count == 0)
            {
                m_cubeSelectedFromGrab = false;

                // Box Colliders
                if (m_useBox)
                {
                    Collider[] collidersBox = Physics.OverlapBox(transform.position + Camera.main.transform.rotation * m_offsetBox, m_boxSize);

                    foreach (Collider cubeCollider in collidersBox)
                    {
                        if (cubeCollider.gameObject.GetComponent<CubeEntitySystem>() != null)
                        {
                            GameObject cube = cubeCollider.gameObject;
                            if (cube.GetComponent<CubeEntitySystem>().getStateComponent().isInactive())
                            {
                                m_potentialCubes.Add(cube);
                            }
                        }
                    }
                }
                // Hemisphere Colliders
                if (m_useHemisphere)
                {
                    Collider[] collidersHemisphere = Physics.OverlapSphere(transform.position + Camera.main.transform.rotation * m_offsetHemisphere, m_radiusHemisphere);

                    foreach (Collider cubeCollider in collidersHemisphere)
                    {
                        if (cubeCollider.gameObject.GetComponent<CubeEntitySystem>() != null)
                        {
                            GameObject cube = cubeCollider.gameObject;
                            if (cube.GetComponent<CubeEntitySystem>().getStateComponent().isInactive() && transform.InverseTransformPoint(cube.transform.position).z > 0)
                            {
                                m_potentialCubes.Add(cube);
                            }
                        }
                    }
                }
                // Sphere Colliders
                if (m_useSphere)
                {
                    Collider[] collidersSphere = Physics.OverlapSphere(transform.position + Camera.main.transform.rotation * m_offsetSphere, m_radiusSphere);

                    foreach (Collider cubeCollider in collidersSphere)
                    {
                        if (cubeCollider.gameObject.GetComponent<CubeEntitySystem>() != null)
                        {
                            GameObject cube = cubeCollider.gameObject;
                            if (cube.GetComponent<CubeEntitySystem>().getStateComponent().isInactive())
                            {
                                m_potentialCubes.Add(cube);
                            }
                        }
                    }
                }
            }

            // Choose Cube
            float minDist = float.MaxValue;
            foreach (GameObject cube in m_potentialCubes)
            {
                float dist = Vector3.Distance(cube.transform.position, m_targetPosition);
                if(dist < minDist)
                {
                    cubeSelected = cube;
                    minDist = dist;
                }
            }
        }

        m_cubeToShoot = cubeSelected;
    }

    void shoot()
    {
        if(m_cubeToShoot != null)
        {
            //if(m_cubeSelectedFromGrab)
                //m_attachScript.deregisterCube(m_cubeToShoot);

            

            //m_cubeToShoot.GetComponent<CubeEntitySystem>().setActiveDynamicly(GetComponent<CubeEntityState>());



            m_cubeToShoot.GetComponent<CubeEntitySystem>().getMovementComponent().removeComponents(typeof(CubeEntityMovementAbstract));
            //m_cubeToShoot.GetComponent<CubeEntitySystem>().getMovementComponent().addAccelerationComponent(m_targetPosition, m_duration, m_power, m_maxSpeed);
            m_cubeToShoot.AddComponent<LookInFlightDirection>();

            //m_cubeToShoot.GetComponent<Rigidbody>().velocity = (m_targetPosition - m_cubeToShoot.transform.position).normalized * m_startSpeed;

            if (m_movementScripts.Length < 1)
            {
                Debug.Log("Aborted: m_movementScript was null!");
                return;
            }
            foreach (CubeEntityMovementAbstract script in m_movementScripts)
            {
                if (script == null)
                    continue;
                m_cubeToShoot.GetComponent<CubeEntitySystem>().getMovementComponent().addMovementComponent(script, m_target, m_targetPosition);
            }

            if (m_cubeToShoot.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
            {
                m_cubeToShoot.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere * 3f;
            }

            resetScript();
        }
    }

    void resetScript()
    {
        m_potentialCubes = new List<GameObject>();
    }
}
