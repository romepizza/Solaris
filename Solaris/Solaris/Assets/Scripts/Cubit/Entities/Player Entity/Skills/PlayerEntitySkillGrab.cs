using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntitySkillGrab : MonoBehaviour
{
    [Header("----- SETTINGS -----")]
    public bool m_addDefaultMovement;
    public bool m_addBoidMovement;
    [Header("--- (Skill) ---")]
    public bool m_grabNearestCube = true;
    public float m_grabCooldown;
    public float m_grabRadius;

    [Header("----- DEBUG -----")]
    public bool m_isGrabbing;

    [Header("--- (Timer) ---")]
    public float m_grabFinishTime;

    private PlayerEntityAttachSystem grabSystemScript;
    
	void Start ()
    {
        grabSystemScript = gameObject.GetComponent<PlayerEntityAttachSystem>();
	}

	// Updates
	void Update ()
    {
        getInput();
        manageSkill();
	}

    // Skill specific
    void manageSkill()
    {
        if (m_isGrabbing && m_grabFinishTime < Time.time)
        {
            activateSkill();
        }
    }
    void activateSkill()
    {
        // chose cube to add to attached
        GameObject cubeAdd = null;
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_grabRadius);

        float nearestDist = float.MaxValue;

        for(int i = 0; i < colliders.Length; i++)
        {
            GameObject cubePotential = colliders[i].gameObject;
            if(cubePotential.layer == 8 && cubePotential.GetComponent<CubeEntitySystem>() != null)
            {
                if(cubePotential.GetComponent<CubeEntitySystem>().getStateComponent() != null && cubePotential.GetComponent<CubeEntitySystem>().getStateComponent().canBeAttachedToPlayer())
                {
                    if(m_grabNearestCube)
                    {
                        float dist = Vector3.Distance(transform.position, cubePotential.transform.position);
                        if(dist < nearestDist)
                        {
                            nearestDist = dist;
                            cubeAdd = cubePotential;
                        }
                    }
                    else
                    {
                        cubeAdd = cubePotential;
                        break;
                    }
                }
            }
        }

        // add cube to attached
        if(cubeAdd != null)
        {
            grabSystemScript.addToGrab(cubeAdd);
        }

        m_grabFinishTime = m_grabCooldown + Time.time;
    }

    void getInput()
    {
        if(Input.GetKeyDown(KeyCode.Mouse2))
        {
            m_isGrabbing = !m_isGrabbing;
        }
    }
}
