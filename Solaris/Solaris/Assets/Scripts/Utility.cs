using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static Component getComponentInParents<T>(Transform startTransform) where T : Component
    {
        Transform currentTransform = startTransform;
        //System.Type t = type;
        Component component = currentTransform.GetComponent(typeof(T));
        //Debug.Log(component.GetType().ToString());
        while(currentTransform != null && component == null)
        {
            currentTransform = currentTransform.parent;
            if (currentTransform == null)
                continue;

            component = currentTransform.GetComponent(typeof(T));
        }

        return component;
    }
    
    public static GameObject getEarliestCoreObjectInParents(Transform startTransform)
    {
        if(startTransform == null)
        {
            Debug.Log("Aborted: startTransform was null!");
            return null;
        }

        Transform currentTransform = startTransform;
        //System.Type t = type;
        MonsterEntityAbstractBase component = currentTransform.GetComponent<MonsterEntityAbstractBase>();
        //Debug.Log(component.GetType().ToString());
        while (currentTransform != null && component == null)
        {
            if (currentTransform.parent == null)
                break;

            currentTransform = currentTransform.parent;
            if(currentTransform.GetComponent<MonsterEntityAbstractBase>() != null)
                component = currentTransform.GetComponent<MonsterEntityAbstractBase>();
        }

        if(component == null)
        {
            Debug.Log("Aborted: component was null!");
            return null;
        }

        if (currentTransform == null)
        {
            Debug.Log("Aborted: currentTransform was null!");
            return null;
        }

        if (currentTransform.gameObject == null)
        {
            Debug.Log("Aborted: currentTransform.gameObject was null!");
            return null;
        }

        return currentTransform.gameObject;
    }

    public static float getAngle(Vector3 v1, Vector3 v2)
    {
        float dot = Vector3.Dot(v1.normalized, v2.normalized);
        float cos = Mathf.Acos(dot) * 180 / Mathf.PI;
        return cos;
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

    public static float[] multiplyFloatArray(float[] array, float factor)
    {
        float[] newArray = new float[array.Length];

        for(int i = 0; i < array.Length; i++)
        {
            newArray[i] = array[i] * factor;
        }

        return newArray;
    }
    
    public static Vector3 getRandomVector(Vector3 minDimensions, Vector3 maxDimensions)
    {
        return new Vector3(Random.RandomRange(minDimensions.x, maxDimensions.x), Random.RandomRange(minDimensions.y, maxDimensions.y), Random.RandomRange(minDimensions.z, maxDimensions.z));
    }
}
