using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityTransform : MonoBehaviour, ICopyValues
{
    [Header("----- SETTINGS -----")]
    //public Transform m_transform;
    //public BoxCollider m_boxCollider;
    //public SphereCollider m_sphereCollider;
    [Header("----- DEBUG -----")]
    List<Collider> m_colliders;
    public CubeEntitySystem m_entitySystemScript;

    // interface
    public void onCopyValues(ICopyValues baseScript)
    {
        GameObject o = ((MonoBehaviour)baseScript).gameObject;
        Mesh mesh = /*Instantiate(*/o.GetComponent<MeshFilter>().sharedMesh/*)*/;
        GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.localScale = o.transform.localScale;

        if (m_colliders == null)
            m_colliders = new List<Collider>();

        for (int i = m_colliders.Count - 1; i >= 0; i--)
        {
            Destroy(m_colliders[i]);
        }



        BoxCollider[] boxColliders = o.GetComponents<BoxCollider>();
        foreach (BoxCollider collider in boxColliders)
        {
            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.size = collider.size;
            col.center = collider.center;
            col.material = collider.sharedMaterial;
            col.enabled = collider.enabled;
            m_colliders.Add(col);
        }
        SphereCollider[] shpereColliders = o.GetComponents<SphereCollider>();
        foreach (SphereCollider collider in shpereColliders)
        {
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.radius = collider.radius;
            col.center = collider.center;
            col.material = collider.sharedMaterial;
            col.enabled = collider.enabled;
            m_colliders.Add(col);
        }
        CapsuleCollider[] capsuleColliders = o.GetComponents<CapsuleCollider>();
        foreach (CapsuleCollider collider in capsuleColliders)
        {
            CapsuleCollider col = gameObject.AddComponent<CapsuleCollider>();
            col.radius = collider.radius;
            col.direction = collider.direction;
            col.center = collider.center;
            col.height = collider.height;
            col.material = collider.sharedMaterial;
            col.enabled = collider.enabled;
            m_colliders.Add(col);
        }
        MeshCollider[] meshColliders = o.GetComponents<MeshCollider>();
        foreach (MeshCollider collider in meshColliders)
        {
            MeshCollider col = gameObject.AddComponent<MeshCollider>();
            col.convex = collider.convex;
            col.material = collider.sharedMaterial;
            col.enabled = collider.enabled;
            m_colliders.Add(col);
        }

        Rigidbody rb = o.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Rigidbody myRb = GetComponent<Rigidbody>();
            if (myRb != null)
            {
                myRb.mass = rb.mass;
                myRb.drag = rb.drag;
                myRb.angularDrag = rb.angularDrag;
                myRb.isKinematic = rb.isKinematic;
                myRb.interpolation = rb.interpolation;
                myRb.collisionDetectionMode = rb.collisionDetectionMode;
            }
        }

    }
}


/*
    public void setTransform(GameObject transformObject)
    {
        Mesh mesh = /*Instantiate(transformObject.GetComponent<MeshFilter>().sharedMesh/*);
        GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.localScale = transformObject.transform.localScale;
        Collider[] colliders = GetComponents<Collider>();
        if (colliders.Length > 0)
        {
            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                //Debug.Log("i: " + i);
                Destroy(colliders[i]);
            }
        }


        
        BoxCollider[] boxColliders = transformObject.GetComponents<BoxCollider>();
        foreach (BoxCollider collider in boxColliders)
        {
            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.size = collider.size;
            col.center = collider.center;
            col.material = collider.sharedMaterial;
            col.enabled = collider.enabled;
        }
        SphereCollider[] shpereColliders = transformObject.GetComponents<SphereCollider>();
        foreach (SphereCollider collider in shpereColliders)
        {
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.radius = collider.radius;
            col.center = collider.center;
            col.material = collider.sharedMaterial;
        }
        CapsuleCollider[] capsuleColliders = transformObject.GetComponents<CapsuleCollider>();
        foreach (CapsuleCollider collider in capsuleColliders)
        {
            CapsuleCollider col = gameObject.AddComponent<CapsuleCollider>();
            col.radius = collider.radius;
            col.direction = collider.direction;
            col.center = collider.center;
            col.height = collider.height;
            col.material = collider.sharedMaterial;
        }

        Rigidbody rb = transformObject.GetComponent<Rigidbody>();
        if(rb != null)
        {
            Rigidbody myRb = GetComponent<Rigidbody>();
            if(myRb != null)
            {
                myRb.mass = rb.mass;
                myRb.drag = rb.drag;
                myRb.angularDrag = rb.angularDrag;
                myRb.isKinematic = rb.isKinematic;
                myRb.interpolation = rb.interpolation;
                myRb.collisionDetectionMode = rb.collisionDetectionMode;
            }
        }
    }
}
        */
