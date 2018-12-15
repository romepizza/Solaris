using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventChangePrefab : EventAbstract, IEvent
{
    public GameObject m_prefab;
    public GameObject m_objectToChange;
    public bool m_setToInactiveBefore;

    public void Start()
    {
        if(m_objectToChange == null)
            m_objectToChange = ((CubeEntityPrefapSystem)(Utility.getComponentInParents<CubeEntityPrefapSystem>(transform))).gameObject;
    }

    public override int triggerEvent()
    {
        if (m_prefab == null)
        {
            Debug.Log("Aborted: m_prefab was null!");
            return -1;
        }
        if (m_objectToChange == null)
        {
            Debug.Log("Aborted: m_objectToChange was null!");
            return -1;
        }
        CubeEntityPrefapSystem prefabSystem = m_objectToChange.GetComponent<CubeEntityPrefapSystem>();
        if(prefabSystem == null)
        {
            Debug.Log("Aborted: prefabSystem was null!");
            return -1;
        }
        if (m_setToInactiveBefore)
            prefabSystem.setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
        prefabSystem.setToPrefab(m_prefab);
        return 0;
    }
}
