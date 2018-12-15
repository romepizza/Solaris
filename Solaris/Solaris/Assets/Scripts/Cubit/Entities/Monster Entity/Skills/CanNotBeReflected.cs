using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanNotBeReflected : MonoBehaviour
{
    public float m_durationEndTime;

    private void Update()
    {
        if (m_durationEndTime < Time.time)
            Destroy(this);
    }
}
