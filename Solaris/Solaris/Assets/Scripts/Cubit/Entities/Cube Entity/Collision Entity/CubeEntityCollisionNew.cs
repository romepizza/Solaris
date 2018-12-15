using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityCollisionNew : MonoBehaviour
{
    [Header("----- DEBUG -----")]
    public bool m_collidedThisFrame;
    public int m_ownState = -1;
    public int m_ownAffiliation = -1;
    public int m_colliderState = -1;
    public int m_colliderAffiliation = -1;

    public CubeEntityState m_stateScript;
    
    // Use this for initialization
    void Start ()
    {
        m_stateScript = GetComponent<CubeEntityState>();
    }

    private void LateUpdate()
    {
        m_collidedThisFrame = false;
    }

    void evaluateCollision(Collider collider)
    {
        GetComponent<CubeEntityCharge>().evaluateCollision(collider);

        m_collidedThisFrame = true;
        collider.gameObject.GetComponent<CubeEntityCollisionNew>().m_collidedThisFrame = true;
    }
    
    bool checkAvailability(Collider collider)
    {
        bool b0 = !m_collidedThisFrame;
        bool b1 = collider.GetComponent<CubeEntityCharge>() != null;
        bool b2 = collider.gameObject.GetComponent<CubeEntityState>() != null;
        bool b3 = collider.isTrigger == false;

        return b0 && b1 && b2 && b3;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(checkAvailability(collision.collider))
        {
            evaluateCollision(collision.collider);
        }
    }
}
