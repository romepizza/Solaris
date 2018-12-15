using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObjectsOnSphere : MonoBehaviour
{
    [Header("------- Actions -------")]
    public bool m_generate;
    public bool m_destroyObjects;

    [Header("------- Settings -------")]
    public int m_numberObjects;
    public float m_sphereRadius;

    [Header("--- Orientation ---")]
    public Vector3 m_customOrientation;

    [Header("--- Constraints ---")]
    public int m_constraintsMaxTries;
    //[Header("- Distance Constraint -")]
    public float m_minDistApartMin;
    public float m_minDistApartMax;
    //public int m_maxDistTries;
    //[Header("- Rotation Constraint -")]
    public float m_minDegreeApartMin;
    public float m_minDegreeApartMax;
    //public int m_maxDegreeTries;

    [Header("--- Objects ---")]
    public Transform m_center;
    public GameObject m_prefab;
    public Transform m_parent;
    public bool m_useSelfAsParent;

    [Header("------- Debug -------")]
    public bool m_isOkay = true;
    public List<GameObject> m_createdObjects;
    public List<Vector3> m_objectPositions;
    public List<Quaternion> m_objectRotations;
    public Transform m_actualParent;

    public struct PositionRotation
    {
        public bool isOkay;
        public Vector3 position;
        public Quaternion rotation;
    }

    private void OnDrawGizmosSelected()
    {
        if(m_generate)
        {
            generate();
            m_generate = false;
        }

        if(m_destroyObjects)
        {
            destroyObjects();
            m_destroyObjects = false;
        }

        showRadius();
    }


    // create
    void generate()
    {
        if (!checkStatus())
            return;

        if (m_useSelfAsParent)
            m_actualParent = transform;
        else
            m_actualParent = m_parent;

        for (int i = 0; i < m_numberObjects; i++)
        {
            PositionRotation data = findData();
            if (!data.isOkay)
                break;

            Vector3 targetPosition = data.position;
            Quaternion targetRotation = data.rotation;

            GameObject o = Instantiate(m_prefab, targetPosition, targetRotation);
            m_createdObjects.Add(o);
            m_objectPositions.Add(targetPosition);
            m_objectRotations.Add(targetRotation);
            o.transform.SetParent(m_actualParent);
        }
    }



    // get stuff
    PositionRotation findData()
    {
        PositionRotation data = new PositionRotation();
        data.isOkay = true;

        int tries = 0;
        bool found = false;
        do
        {
            tries++;
            Vector3 randomVector = Random.insideUnitSphere * 360;
            data.position = findPosition(randomVector);
            data.rotation = findRotation(randomVector);

            if (positionIsOkay(data.position) && rotationIsOkay(data.rotation))
                found = true;
        } while (!found && tries <= m_constraintsMaxTries);

        if (m_constraintsMaxTries > 0 && tries >= m_constraintsMaxTries)
            data.isOkay = false;

        return data;
    }
    Vector3 findPosition(Vector3 randomVector)
    {
        Quaternion randomQuat = Quaternion.Euler(randomVector);
        Vector3 targetPosition = m_center.position + randomQuat * (transform.up * m_sphereRadius);

        return targetPosition;
    }
    Quaternion findRotation(Vector3 randomVector)
    {
        Quaternion randomQuat = Quaternion.Euler(randomVector);
        Quaternion targetRotation = randomQuat * Quaternion.Euler(m_customOrientation);

        return targetRotation;
    }

    // constraints
    bool positionIsOkay(Vector3 position)
    {
        if (m_minDistApartMin <= 0 && m_minDistApartMax <= 0)
            return true;

        bool isOkay = true;
        for(int i = 0; i < m_objectPositions.Count; i++)
        {
            float dist = (position - m_objectPositions[i]).magnitude;
            if(dist < Random.Range(m_minDistApartMin, m_minDistApartMax))
            {
                isOkay = false;
                break;
            }
        }

        return isOkay;
    }
    bool rotationIsOkay(Quaternion rotation)
    {
        if (m_minDegreeApartMin <= 0 && m_minDegreeApartMax <= 0)
            return true;

        Vector3 euler = rotation.eulerAngles;
        bool isOkay = true;
        for (int i = 0; i < m_objectRotations.Count; i++)
        {
            float dist = Vector3.Angle(m_objectRotations[i].eulerAngles, euler);
            if (dist < Random.Range(m_minDegreeApartMin, m_minDegreeApartMax))
            {
                isOkay = false;
                break;
            }
        }

        return isOkay;
    }


    // destroy
    void destroyObjects()
    {
        for(int i = m_createdObjects.Count - 1; i >= 0; i--)
        {
            GameObject o = m_createdObjects[i];
            //m_createdObjects.RemoveAt(i);
            //m_objectPositions.RemoveAt(i);
            //m_objectRotations.RemoveAt(i);
            DestroyImmediate(o);
        }

        m_createdObjects.Clear();
        m_objectPositions.Clear();
        m_objectRotations.Clear();
    }

    // helper stuff
    void showRadius()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(m_center.position, Vector3.up * m_sphereRadius);
    }
    bool checkStatus()
    {
        bool isOkay = true;

        if (m_center == null)
            isOkay = false;

        if (m_prefab == null)
            isOkay = false;

        if (m_parent == null)
            isOkay = false;

        m_isOkay = isOkay;
        return isOkay;
    }











    // Use this for initialization
    void Start ()
    {
        Destroy(this);
	}
}
