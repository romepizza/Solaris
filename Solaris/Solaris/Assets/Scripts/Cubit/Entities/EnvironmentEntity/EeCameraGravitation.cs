using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EeCameraGravitation : MonoBehaviour
{
    [Header("------- Settings -------")]
    public GameObject m_centerObject;

    [Header("------- Debug -------")]
    public Transform m_centerTransform;
    public List<GameObject> m_affectedObjects;
    public PlayerEntityCameraRotation m_playerCameraScript;

	// Use this for initialization
	void Start ()
    {
        initializeStuff();
	}
    void initializeStuff()
    {
        m_affectedObjects = new List<GameObject>();
        if (m_centerObject == null)
        {
            m_centerObject = gameObject;
        }
        m_centerTransform = m_centerObject.transform;
        m_playerCameraScript = m_centerObject.GetComponent<PlayerEntityCameraRotation>();
    }

    // Update is called once per frame
    void Update ()
    {
        manageInput();
        manageAffected();
        //transform.rotation = Quaternion.LookRotation(Constants.getPlayer().transform.position - transform.position);
	}

    void manageAffected()
    {
        if (m_affectedObjects.Count <= 0)
            return;

        foreach(GameObject agent in m_affectedObjects)
        {
            calculateNewGyrometerQuaternion(agent);
        }
    }

    void calculateNewGyrometerQuaternion(GameObject agent)
    {
        Vector3 direction = Constants.getPlayer().transform.position - transform.position;
        //Debug.DrawRay(transform.position, direction, Color.green);

        Quaternion wantQuaternion = Quaternion.LookRotation(-direction, agent.transform.forward);
        wantQuaternion = wantQuaternion * Quaternion.Euler(-90f, 0, 0);
        /*
        float angle = 90; Vector3.Angle(-direction, agent.transform.forward);
        if (angle > 90.2)
            agent.transform.Rotate(agent.transform.right, 1f);
        else if (angle < 89.8)
            agent.transform.Rotate(agent.transform.right, -1f * Mathf.Clamp01((90 / angle + 10)));
        */    
        //Debug.Log(angle);

        //wantQuaternion = wantQuaternion * Quaternion.LookRotation(wantQuaternion * Vector3.forward, direction);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, wantQuaternion, 135f * Time.deltaTime);
        //Debug.DrawRay(agent.transform.position, direction, Color.yellow);
        //Debug.DrawRay(agent.transform.position, Constants.getPlayer().transform.forward * 5f, Color.blue);
        //Debug.DrawRay(agent.transform.position, Constants.getPlayer().transform.right * 5f, Color.red);
        //Debug.DrawRay(agent.transform.position, Constants.getPlayer().transform.up * 5f, Color.green);
    }


    // manage affected objects
    public void addObjectToAffected(GameObject o)
    {
        if(m_affectedObjects.Contains(o))
        {
            Debug.Log("Warning: list already contains object!");
        }

        m_affectedObjects.Add(o);
    }
    public void removeObjectFromAffected(GameObject o)
    {
        if(!m_affectedObjects.Contains(o))
        {
            Debug.Log("Warning: list already contains object!");
        }

        o.transform.rotation = Quaternion.Euler(0, 0, 0);

        m_affectedObjects.Remove(o);
    }
    public List<GameObject> removeAllObjectsFromAffected()
    {
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject o in m_affectedObjects)
            list.Add(o);

        for(int i = m_affectedObjects.Count - 1; i >= 0; i--)
        {
            m_affectedObjects.Remove(m_affectedObjects[0]);
        }

        return list;
    }

    // input
    void manageInput()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (m_affectedObjects.Contains(Constants.getPlayerGyrometer()))
                removeObjectFromAffected(Constants.getPlayerGyrometer());
            else
                addObjectToAffected(Constants.getPlayerGyrometer());
        }
    }
}
