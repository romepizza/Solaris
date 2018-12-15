using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public GameObject playerCamera;

    [Header("----- SETTINGS -----")]
    public float distance;
    public float sensitivityXAxisMouse;
    public float sensitivityYAxisMouse;
    public float sensitivityXAxisJoypad;
    public float sensitivityYAxisJoypad;
    public Vector3 lookOffset;
    public Vector3 positionOffset;
    public LayerMask layerMask;
    public float unitsPerScroll;
    public int minCamScroll;
    public int maxCamScroll;
    public Vector3 m_customRotation;

    [Header("----- DEBUG -----")]
    public float currentAngleX;
    public float currentAngleY;
    public float currentScroll;
    public bool isTargetLocked;
    public bool m_toggleJoypad;
    public Vector3 m_lookAtWorldPositionFinal;

    void Start ()
    {
        currentAngleY = 15;
    }

    void FixedUpdate()
    {
        if (!GameObject.Find("GeneralScriptObject").GetComponent<Options>().isFreeze)
        {
            //if (Constants.getBoidSystem() != null && !Constants.getBoidSystem().m_isFreeze)
                //return;

            bool isPressingUp = Input.GetAxis("ButtonUp") > 0.5f;
            bool isPressingDown = Input.GetAxis("ButtonDown") < -0.5f;
            if (isPressingUp || isPressingDown)
            {
                if (!m_toggleJoypad)
                {
                    if (isPressingUp)
                    {
                        sensitivityXAxisJoypad *= 1.1f;
                        sensitivityYAxisJoypad *= 1.1f;
                    }
                    else if (isPressingDown)
                    {
                        sensitivityXAxisJoypad /= 1.1f;
                        sensitivityYAxisJoypad /= 1.1f;
                    }
                    m_toggleJoypad = true;
                }
            }
            else
            {
                m_toggleJoypad = false;
            }

            if (Input.GetAxis("RightStickHorizontal") > 0.2f || Input.GetAxis("RightStickHorizontal") < -0.2f)
                currentAngleX += Input.GetAxis("RightStickHorizontal") * sensitivityXAxisJoypad;
            if (Input.GetAxis("RightStickVertical") > 0.2f || Input.GetAxis("RightStickVertical") < -0.2f)
                currentAngleY += Input.GetAxis("RightStickVertical") * sensitivityYAxisJoypad;

            currentAngleY = Mathf.Clamp(currentAngleY, -80, 80);
        }
    }

    void Update()
    {
        if (!GameObject.Find("GeneralScriptObject").GetComponent<Options>().isFreeze)
        {
            //if (Constants.getBoidSystem() != null && !Constants.getBoidSystem().m_isFreeze)
                //return;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                sensitivityXAxisMouse *= 1.1f;
                sensitivityYAxisMouse *= 1.1f;
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                sensitivityXAxisMouse /= 1.1f;
                sensitivityYAxisMouse /= 1.1f;
            }

            currentAngleX += Input.GetAxis("Mouse X") * sensitivityXAxisMouse;
            currentAngleY -= Input.GetAxis("Mouse Y") * sensitivityYAxisMouse;

            if (Input.mouseScrollDelta != Vector2.zero)
            {
                currentScroll -= Input.mouseScrollDelta[1] * currentScroll * 0.1f;
            }
            currentScroll = Mathf.Clamp(currentScroll, minCamScroll, maxCamScroll);

            currentAngleY = Mathf.Clamp(currentAngleY, -80, 80);
            }
        }
  

   
	
	void LateUpdate ()
    { 
        Quaternion cameraRotation = playerCamera.transform.rotation;
       

        cameraRotation = Quaternion.Euler(currentAngleY, currentAngleX, 0) * Quaternion.Euler(m_customRotation);

        distance = currentScroll * unitsPerScroll;
        Vector3 direction = cameraRotation * new Vector3(0, 0, -distance);
        Quaternion cameraRotationYBodyRotationXZ = Quaternion.Euler(transform.rotation.eulerAngles.x, playerCamera.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        Vector3 positionOffsetWorldActual = positionOffset;
        Vector3 positionOffsetLocalActual = cameraRotationYBodyRotationXZ * positionOffset;
        //Vector3 positionOffsetLocalActual = cameraRotationYBodyRotationXZ * positionOffset;
        Vector3 lookAtOffsetWorldActual = lookOffset + positionOffset * (transform.position - playerCamera.transform.position).magnitude / distance;
        Vector3 lookAtOffsetLocalActual = cameraRotationYBodyRotationXZ * (lookOffset + positionOffset * (transform.position - playerCamera.transform.position).magnitude / distance);

        /*
        RaycastHit hit;
        if (Physics.Raycast(transform.position, positionOffsetLocalActual, out hit, positionOffsetLocalActual.magnitude, layerMask))
        {
            positionOffsetLocalActual = positionOffsetLocalActual.normalized * hit.distance * 0.5f;
            positionOffsetWorldActual = positionOffsetWorldActual.normalized * hit.distance * 0.5f;
        }
        if (Physics.Raycast(transform.position, lookAtOffsetLocalActual, out hit, lookAtOffsetLocalActual.magnitude, layerMask))
        {
            lookAtOffsetLocalActual = lookAtOffsetLocalActual.normalized * hit.distance * 0.5f;
            lookAtOffsetWorldActual = lookAtOffsetWorldActual.normalized * hit.distance * 0.5f;
        }
        */

        Vector3 cameraPositionFinal = transform.position + transform.rotation * positionOffsetWorldActual + direction;
        Vector3 lookAtWorldPositionFinal = transform.position + cameraRotationYBodyRotationXZ * lookAtOffsetWorldActual;
        m_lookAtWorldPositionFinal = lookAtWorldPositionFinal;

        /*
        bool cameraIsHit = false;
        if (Physics.Raycast(transform.position, cameraPositionFinal - transform.position, out hit, distance, layerMask))
        {
            cameraPositionFinal = hit.point - hit.normal.normalized * 0.5f; //(cameraPositionFinal - transform.position).normalized
            Debug.DrawRay(hit.point, hit.normal, Color.black);
            cameraIsHit = true;
        }

        if (!cameraIsHit)
        {
            cameraPositionFinal = transform.position + transform.rotation * positionOffset + direction;
            lookAtWorldPositionFinal = transform.position + cameraRotationYBodyRotationXZ * lookOffset + positionOffset * (transform.position - playerCamera.transform.position).magnitude / distance; ;
        }
        */



        playerCamera.transform.position = cameraPositionFinal;
        playerCamera.transform.LookAt(lookAtWorldPositionFinal);

        //Debug.DrawLine(transform.position, cameraPositionFinal, Color.cyan);
        //Debug.DrawLine(transform.position, lookAtWorldPositionFinal, Color.green);
        //Debug.DrawRay(transform.position, positionOffsetLocalActual, Color.red);
    }
}
