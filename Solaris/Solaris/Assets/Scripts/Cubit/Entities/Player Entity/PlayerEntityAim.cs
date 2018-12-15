using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntityAim : MonoBehaviour
{
    public GameObject m_aimSprite;
    public GameObject m_aimingAt;
    [Header("----- SETTINGS -----")]
    public LayerMask m_layerMask;

    public static GameObject aim()
    {
        GameObject cubeCore = null;
        List<GameObject> cubes = new List<GameObject>();

        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward/*, m_layerMask*/);
        foreach (RaycastHit hit in hits)
        {
            GameObject cubeHit = hit.collider.gameObject;
            if (cubeHit.GetComponent<CubeEntityState>() != null && cubeHit.GetComponent<CubeEntityState>().m_state == CubeEntityState.s_STATE_CORE && Constants.getPlayer().transform.InverseTransformPoint(cubeHit.transform.position).z > 0)
            {
                cubes.Add(cubeHit);
            }
        }

        float minDist = float.MaxValue;
        foreach (GameObject cube in cubes)
        {
            //Debug.Log(Camera.main.WorldToScreenPoint(cube.transform.position));

            Vector3 cubePos = Camera.main.WorldToViewportPoint(cube.transform.position);
            cubePos.z = 0;

            Vector3 defaultPos = new Vector3(0.5f, 0.5f, 0);


            float dist = (cubePos - defaultPos).magnitude;
            if (dist < minDist)
            {
                minDist = dist;
                cubeCore = cube;
            }
        }

        //Debug.Log("0: ");
        //Debug.Log((cubeCore != null).ToString());
        return cubeCore;
    }
}
