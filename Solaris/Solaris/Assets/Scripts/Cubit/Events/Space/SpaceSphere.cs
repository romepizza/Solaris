using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSphere : SpaceAbstract, ISpace
{
    [Header("------- Settings -------")]
    public float m_sizeRadius;

    [Header("------- Debug -------")]
    bool b;

    public override Vector3 getRandomPoint()
    {
        return m_position + Random.insideUnitSphere * m_sizeRadius + (m_addGameObjectPosition ? transform.position : Vector3.zero);
    }
}
