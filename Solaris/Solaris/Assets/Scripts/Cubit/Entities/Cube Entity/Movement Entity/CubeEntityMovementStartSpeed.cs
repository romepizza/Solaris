using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityMovementStartSpeed : EntityCopiableAbstract, ICopiable, IRemoveOnStateChange
{
    [Header("------- Settings -------")]
    public Vector3 m_inheritVelocityFactor;
    public Vector3 m_relativeToTargetPositionMin;
    public Vector3 m_relativeToTargetPositionMax;
    public Vector3 m_relativeToCustomPositionMin;
    public Vector3 m_relativeToCustomPositionMax;
    public Vector3 m_relativeToOriginPositionMin;
    public Vector3 m_relativeToOriginPositionMax;
    public Vector3 m_relativeToOriginRotationMin;
    public Vector3 m_relativeToOriginRotationMax;
    public Vector3 m_worldVectorMin;
    public Vector3 m_worldVectorMax;

    [Header("------- Debug -------")]
    public float factor = 1;

    public void applyMovement(GameObject cube, Vector3 targetPosition, Vector3 originPosition, Quaternion rotation, Vector3[] customCoordinateSystem, float factor)
    {
        Rigidbody rb = cube.GetComponent<Rigidbody>();
        if(rb == null)
        {
            Debug.Log("Aborted: rigidBody was null!");
            return;
        }

        Vector3 totalVector = Vector3.zero;
        Vector3 directionZ = Vector3.zero;
        Vector3 directionX = Vector3.zero;
        Vector3 directionY = Vector3.zero;

        // relative to target
        if (targetPosition != Vector3.zero && m_relativeToTargetPositionMin != Vector3.zero && m_relativeToTargetPositionMax != Vector3.zero)
        {
            directionZ = (targetPosition - cube.transform.position).normalized;
            directionX = Vector3.Cross(directionZ, Vector3.up).normalized;
            directionY = Vector3.Cross(directionX, directionZ).normalized;
            totalVector += -Random.Range(m_relativeToTargetPositionMin.x, m_relativeToTargetPositionMax.x) * directionX.normalized + Random.Range(m_relativeToTargetPositionMin.y, m_relativeToTargetPositionMax.y) * directionY.normalized + Random.Range(m_relativeToTargetPositionMin.z, m_relativeToTargetPositionMax.z) * directionZ;
        }
        //Debug.DrawRay(cube.transform.position, directionX.normalized * 100f, Color.red, 5);
        //Debug.DrawRay(cube.transform.position, directionY.normalized * 100f, Color.green, 5);
        //Debug.DrawRay(cube.transform.position, directionZ.normalized * 100f, Color.blue, 5);

        //Debug.DrawRay(cube.transform.position, directionX * 100f, Color.red, cd);
        //Debug.DrawRay(cube.transform.position, directionY * 100f, Color.green, cd);
        //Debug.DrawRay(cube.transform.position, directionZ * 100f, Color.blue, cd);

        // relative to customCoorinateSystem
        // if array length is 3: these vector3 are the direction [x, y, z]
        // if array length is 2: these vector3 are the forward and up vectors [z/forward, y/up]
        if (customCoordinateSystem != null && m_relativeToCustomPositionMin != Vector3.zero && m_relativeToCustomPositionMax != Vector3.zero)
        {
            if(customCoordinateSystem.Length == 2)
            {
                // carfull: first Z, then Y, then X, instead of standard order
                directionZ = customCoordinateSystem[0].normalized;
                directionY = customCoordinateSystem[1].normalized;
                directionX = Vector3.Cross(directionX, directionY).normalized;
                totalVector += Random.Range(m_relativeToCustomPositionMin.x, m_relativeToCustomPositionMax.x) * directionX.normalized + Random.Range(m_relativeToCustomPositionMin.y, m_relativeToCustomPositionMax.y) * directionY.normalized + Random.Range(m_relativeToCustomPositionMin.z, m_relativeToCustomPositionMax.z) * directionZ;
            }
            else if(customCoordinateSystem.Length == 3)
            {
                directionZ = customCoordinateSystem[2].normalized;
                directionX = customCoordinateSystem[0].normalized;
                directionY = customCoordinateSystem[1].normalized;
                totalVector += Random.Range(m_relativeToCustomPositionMin.x, m_relativeToCustomPositionMax.x) * directionX.normalized + Random.Range(m_relativeToCustomPositionMin.y, m_relativeToCustomPositionMax.y) * directionY.normalized + Random.Range(m_relativeToCustomPositionMin.z, m_relativeToCustomPositionMax.z) * directionZ;
            }
            else
                Debug.Log("Warning: Something might have gone wrong!");
        }


        // relative to origin position
        if (originPosition != Vector3.zero && m_relativeToOriginPositionMin != Vector3.zero && m_relativeToOriginPositionMax != Vector3.zero)
        {
            directionZ = (originPosition - cube.transform.position).normalized;
            directionX = Vector3.Cross(directionZ, Vector3.up).normalized;
            directionY = Vector3.Cross(directionX, directionZ).normalized;
            totalVector += -Random.Range(m_relativeToOriginPositionMin.x, m_relativeToOriginPositionMax.x) * directionX.normalized + Random.Range(m_relativeToOriginPositionMin.y, m_relativeToOriginPositionMax.y) * directionY.normalized + Random.Range(m_relativeToOriginPositionMin.z, m_relativeToOriginPositionMax.z) * directionZ;
        }
        //Debug.DrawRay(cube.transform.position, directionX * 100f, Color.red, cd);
        //Debug.DrawRay(cube.transform.position, directionY * 100f, Color.green, cd);
        //Debug.DrawRay(cube.transform.position, directionZ * 100f, Color.blue, cd);

        // to origin rotation
        if (rotation.eulerAngles != Vector3.zero && m_relativeToOriginRotationMin != Vector3.zero && m_relativeToOriginRotationMax != Vector3.zero)
        {
            directionZ = rotation * Vector3.forward;
            directionX = rotation * Vector3.right;
            directionY = rotation * Vector3.up;
            totalVector += Random.Range(m_relativeToOriginRotationMin.x, m_relativeToOriginRotationMax.x) * directionX.normalized + Random.Range(m_relativeToOriginRotationMin.y, m_relativeToOriginRotationMax.y) * directionY.normalized + Random.Range(m_relativeToOriginRotationMin.z, m_relativeToOriginRotationMax.z) * directionZ;
        }
        // world
        totalVector += new Vector3(Random.Range(m_worldVectorMin.x, m_worldVectorMax.x), Random.Range(m_worldVectorMin.y, m_worldVectorMax.y), Random.Range(m_worldVectorMin.z, m_worldVectorMax.z));

        totalVector *= factor;


        Rigidbody rbThis = (Rigidbody)Utility.getComponentInParents<Rigidbody>(cube.transform);
        if(rbThis == null)
        {
            Debug.Log("Warning: This rigidBody was null!");
        }
        else
        {
            Vector3 total = rbThis.velocity;
            total.x *= m_inheritVelocityFactor.x;
            total.y *= m_inheritVelocityFactor.y;
            total.z *= m_inheritVelocityFactor.z;
            totalVector += total;// new Vector3(m_inheritVelocityFactor.x * rbThis.velocity.x + m_inheritVelocityFactor.y * rbThis.velocity.y + m_inheritVelocityFactor.z * rbThis.velocity.z;
        }
        //Debug.DrawRay(cube.transform.position, totalVector * 10f, Color.blue, cd);

        rb.velocity = totalVector;
    }

    // interfaces
    public void onCopy(ICopiable copiable)
    {
        CubeEntityMovementStartSpeed script = (CubeEntityMovementStartSpeed)copiable;

        m_inheritVelocityFactor  = script. m_inheritVelocityFactor;
        m_relativeToTargetPositionMin = script. m_relativeToTargetPositionMin;
        m_relativeToTargetPositionMax = script. m_relativeToTargetPositionMax;
        m_relativeToCustomPositionMin = script. m_relativeToCustomPositionMin;
        m_relativeToCustomPositionMax = script. m_relativeToCustomPositionMax;
        m_relativeToOriginPositionMin = script. m_relativeToOriginPositionMin;
        m_relativeToOriginPositionMax = script. m_relativeToOriginPositionMax;
        m_relativeToOriginRotationMin = script. m_relativeToOriginRotationMin;
        m_relativeToOriginRotationMax = script. m_relativeToOriginRotationMax;
        m_worldVectorMin = script. m_worldVectorMin;
        m_worldVectorMax = script.m_worldVectorMax;
    }
    public void onRemove()
    {
        Destroy(this);
    }
}
