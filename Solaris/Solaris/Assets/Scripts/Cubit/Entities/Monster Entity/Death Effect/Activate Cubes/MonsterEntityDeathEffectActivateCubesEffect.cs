using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntityDeathEffectActivateCubesEffect : MonoBehaviour
{
    public GameObject m_spawnPrefab;
    [Header("------- Settings -------")]
    public float m_tossCooldownActivate;
    public int m_tossesPerBurstActivate;
    public float m_tossCooldownDeactivate;
    public int m_tossesPerBurstDeactivate;
    public int m_cubeTossesNumber;
    public bool m_setOtherToInactive;
    public bool m_createAdditional;
    //public float m_explosionRadius;
    //public float m_activeDuration;
    //public float m_explosionPower;
    //public float m_maxSpeed;
    public List<CubeEntityMovementAbstract> m_movementScript;
    public List<CubeEntityMovementStartSpeed> m_startSpeedScript;
    //public float m_distance;

    [Header("------- Debug -------")]
    public GameObject m_target;
    public Queue<GameObject> m_cubes;
    public int m_cubesActivated;
    public float m_cooldownRdyTimeActivate;
    public int m_cubesDeactivated;
    public float m_cooldownRdyTimeDeactivate;
    public int m_cubeTossNumberActive;
    public int m_cubeTossNumberDeactive;
    public bool m_isActivating;
    public bool m_isDeactivating;

    public void initializeStuff()
    {
        m_isActivating = true;
        m_isDeactivating = true;

        m_cooldownRdyTimeActivate = m_tossCooldownActivate + Time.time;
        m_cooldownRdyTimeDeactivate = m_tossCooldownDeactivate + Time.time;
        m_cubeTossNumberActive = m_cubeTossesNumber;
        m_cubeTossNumberDeactive = Mathf.Max(0, m_cubes.Count - m_cubeTossNumberActive);
    }
    void Update ()
    {
        activateCubes();
        deactivateCubes();
        if (!m_isActivating && !m_isDeactivating)
            destroyScript();
	}
    void activateCubes()
    {
        if (!m_isActivating || m_cooldownRdyTimeActivate > Time.time)
            return;
        m_cooldownRdyTimeActivate = m_tossCooldownActivate + Time.time;

        if (m_tossesPerBurstActivate <= 0)
        {
            if (m_tossesPerBurstActivate > 0)
                Debug.Log("Warning: This should not be happening!");

            m_isActivating = false;
            return;
        }


        for (int i = 0; i < m_tossesPerBurstActivate; i++)
        {
            if ((m_cubes.Count <= 0 && !m_createAdditional) || !(m_cubesActivated < m_cubeTossNumberActive))
            {
                m_isActivating = false;
                return;
            }

            GameObject cube;
            if(m_createAdditional)
            {
                if (m_cubes.Count > 0)
                    cube = m_cubes.Dequeue();
                else
                    cube = Constants.getMainCge().activateCubeSafe(transform.position + Random.insideUnitSphere * 50f);
            }
            else
                cube = m_cubes.Dequeue();

            if (cube.activeSelf == false)
            {
                Debug.Log("Info: cube was inactive!");
                return;
            }

            if (m_spawnPrefab == null)
            {
                Debug.Log("Warning: m_prefab was null!");
                cube.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
            }
            else
            {
                cube.GetComponent<CubeEntityPrefapSystem>().setToPrefab(m_spawnPrefab);
                Vector3 targetPosition = transform.position + (cube.transform.position - transform.position).normalized * 10000f;

                if (m_startSpeedScript != null)
                {
                    foreach (CubeEntityMovementStartSpeed script in m_startSpeedScript)
                    {
                        if (script == null)
                            continue;

                        Vector3[] customCoordinateSystem = new Vector3[3];
                        customCoordinateSystem[2] = m_target.transform.position - cube.transform.position;
                        customCoordinateSystem[1] = cube.transform.position - transform.position;
                        customCoordinateSystem[0] = Vector3.Cross(customCoordinateSystem[2], customCoordinateSystem[1]);
                        customCoordinateSystem[1] = Vector3.Cross(customCoordinateSystem[0], customCoordinateSystem[2]);



                        script.applyMovement(cube, m_target.transform.position, transform.position, new Quaternion(), customCoordinateSystem, 1);
                    }
                }

                if (m_movementScript != null)
                {
                    foreach (CubeEntityMovementAbstract script in m_movementScript)
                    {
                        if (script == null)
                            continue;
                        cube.GetComponent<CubeEntityMovement>().addMovementComponent(script, m_target, targetPosition);
                    }
                }

                cube.GetComponent<CubeEntityTargetManager>().setTarget(m_target);
                cube.GetComponent<CubeEntityTargetManager>().setOrigin((CubeEntityTargetManager)Utility.getComponentInParents<CubeEntityTargetManager>(transform));
            }
            m_cubesActivated++;
        }
    }
    void deactivateCubes()
    {
        if (!m_isDeactivating || m_cooldownRdyTimeDeactivate > Time.time)
            return;
        m_cooldownRdyTimeDeactivate = m_tossCooldownDeactivate + Time.time;

        if (m_tossesPerBurstDeactivate <= 0)
        {
            if (m_cubeTossNumberDeactive > 0)
                Debug.Log("Warning: This should not be happening!");

            m_isDeactivating = false;
            return;
        }


        for (int i = 0; i < m_tossesPerBurstDeactivate; i++)
        {
            if (m_cubes.Count <= 0 || !(m_cubesDeactivated < m_cubeTossNumberDeactive))
            {
                m_isDeactivating = false;
                return;
            }

            GameObject cube = m_cubes.Dequeue();
            cube.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);

            m_cubesDeactivated++;
        }
    }

    //void tossCubesOld()
    //{
    //    for (int i = 0; i < m_tossesPerBurstActivate; i++)
    //    {
    //        if (m_cubes.Count <= 0 || m_cubesTossed >= m_cubeTossesNumber)
    //        {
    //            destroyScript();
    //            return;
    //        }

    //        GameObject cube = m_cubes.Dequeue();
    //        CgeDoNotSetToInactive[] list = cube.GetComponents<CgeDoNotSetToInactive>();

    //        for (int j = list.Length - 1; j >= 0; j--)
    //        {
    //            Destroy(list[j]);
    //        }

    //        //CubeEntitySystem systemScript = cube.GetComponent<CubeEntitySystem>();
    //        //systemScript.setActiveDynamicly(m_monsterOrigin);
    //        if (m_spawnPrefab == null)
    //        {
    //            Debug.Log("Warning: m_prefab was null!");
    //            cube.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
    //        }
    //        else
    //        {
    //            cube.GetComponent<CubeEntityPrefapSystem>().setToPrefab(m_spawnPrefab);
    //        }
    //        //systemScript.getMovementComponent().removeComponents(typeof(CubeEntityMovementAbstract));
    //        //float distanceFactor = Mathf.Max(0.2f,  (Vector3.Distance(cube.transform.position, transform.position) / m_explosionRadius));

    //        Vector3 targetPosition = transform.position + (cube.transform.position - transform.position).normalized * 1000f;
    //        //Debug.Log(m_movementScript.Count);
    //        if (m_movementScript != null)
    //        {
    //            foreach (CubeEntityMovementAbstract script in m_movementScript)
    //            {
    //                if (script == null)
    //                    continue;
    //                cube.GetComponent<CubeEntityMovement>().addMovementComponent(script, Constants.getPlayer(), targetPosition);
    //            }
    //        }
    //        m_cubesTossed++;
    //    }
    //}

    void destroyScript()
    {
        Destroy(this);
        Destroy(this.gameObject);
    }
}

