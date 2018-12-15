using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttachSystemBase : EntityCopiableAbstract
{
    [Header("------- Base Settings -------")]
    public int m_maxCubesGrabbed;
    public List<GameObject> m_cubeList;
    public float m_movementAffectsCubesFactor;
    public int m_setToInactiveWhenNoMaxNumber;
    public abstract void deregisterCube(GameObject cube);

    [Header("------- Base Debug -------")]
    public int m_currentMaxCubesGrabbed;
    public GameObject m_core;


    public void reduceMaxGrabbed(int number)
    {
        m_currentMaxCubesGrabbed -= number;
        if (m_setToInactiveWhenNoMaxNumber >= 0 && m_currentMaxCubesGrabbed <= m_setToInactiveWhenNoMaxNumber)
        {
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
        }
    }
}
