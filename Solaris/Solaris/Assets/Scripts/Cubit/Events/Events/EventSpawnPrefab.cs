using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSpawnPrefab : EventAbstract, IEvent
{
    public GameObject m_prefab;
    public SpaceAbstract m_space;
    public Zone m_zone;

    public override int triggerEvent()
    {
        if(m_prefab == null)
        {
            Debug.Log("Aborted: m_prefab was null!");
            return -1;
        }

        Vector3 spawnPosition = Vector3.zero;
        if (m_space != null)
            spawnPosition = m_space.getRandomPoint();
        if (m_zone != null)
            spawnPosition = m_zone.getRandomPointSub();

        GameObject cube = Constants.getMainCge().activateCubeSafe(spawnPosition);
        cube.GetComponent<CubeEntityPrefapSystem>().setToPrefab(m_prefab);

        return 0;
    }
}
