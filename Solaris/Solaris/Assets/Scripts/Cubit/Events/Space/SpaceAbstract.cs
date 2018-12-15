using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpaceAbstract : MonoBehaviour
{
    public bool m_addGameObjectPosition;
    public Vector3 m_position;

    public abstract Vector3 getRandomPoint();
}
