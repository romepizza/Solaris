using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CgeInstance : MonoBehaviour
{
    public List<GameObject> m_activeCubes;
    public CGE m_cge;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void activateCube(Vector3 targetPosition)
    {
        if (m_cge.m_inactiveCubes.Count > 0)
        {
            /*
            GameObject cube = null;
            do
            {
                cube = m_inactiveCubes.Dequeue();
                if(cube.GetComponent<CubeEntityState>().isInactive())
                {
                    m_activeCubes.Add(cube);
                    break;
                }

            } while (m_inactiveCubes.Count > 0);
            */
            GameObject cube = m_cge.m_inactiveCubes.Dequeue();
            if (!cube.GetComponent<CubeEntityState>().isInactive())
            {
                m_activeCubes.Add(cube);
                if (m_cge.m_inactiveCubes.Count > 0)
                {
                    cube = m_cge.m_inactiveCubes.Dequeue();
                    if (!cube.GetComponent<CubeEntityState>().isInactive())
                        cube = null;
                }
            }

            if (cube != null)
            {
                cube.transform.position = targetPosition;
                //cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
                //cube.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                cube.SetActive(true);
                //cube.transform.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 100f, ForceMode.Acceleration);
                //cube.transform.GetComponent<Rigidbody>().velocity = m_player.GetComponent<Rigidbody>().velocity * 0.5f;
                //cube.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;

                m_activeCubes.Add(cube);
            }
        }
    }
}
