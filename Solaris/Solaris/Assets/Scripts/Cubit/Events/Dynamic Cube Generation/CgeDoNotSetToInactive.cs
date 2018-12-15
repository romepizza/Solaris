using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CgeDoNotSetToInactive : EntityCopiableAbstract
{
    public float m_duration;

    public void onStateChangeRemove()
    {
        Destroy(this);
    }
}
