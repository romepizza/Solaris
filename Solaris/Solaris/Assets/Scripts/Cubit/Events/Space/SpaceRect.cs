using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceRect : SpaceAbstract, ISpace
{
    [Header("------- Settings -------")]
    public Vector3 m_sizeDiagonal;

    [Header("------- Debug -------")]
    bool b;

    public override Vector3 getRandomPoint()
    {
        return m_position + Utility.getRandomVector(-0.5f * m_sizeDiagonal, 0.5f * m_sizeDiagonal) + (m_addGameObjectPosition ? transform.position : Vector3.zero);
    }
}
