using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour, ICopiable
{
    [Header("------- Settings -------")]
    public EventAbstract m_triggerEvent;
    //public bool m_destroyAfterwards;
    public bool m_deactivateAfterwards;
    public LayerMask m_layerMask;

    [Header("------- Debug -------")]
    public bool m_isDeactivated;

    public void OnTriggerEnter(Collider collider)
    {
        if (!Utility.IsInLayerMask(collider.gameObject.layer, m_layerMask))
            return;
        if (m_triggerEvent == null)
            return;

        if (m_deactivateAfterwards && !m_isDeactivated)
        {
            if (m_triggerEvent.triggerEvent() == 0)
                m_isDeactivated = true;
        }

        if (!m_deactivateAfterwards)
            m_triggerEvent.triggerEvent();
    }

    void setValues(TriggerEvent copy)
    {
        m_deactivateAfterwards = copy.m_deactivateAfterwards;
        m_layerMask = copy.m_layerMask;
    }

    // interface
    public void onCopy(ICopiable copiable)
    {
        setValues((TriggerEvent)copiable);
    }
}
