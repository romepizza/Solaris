using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject visualBody;
    public GameObject visualWingLeft;
    public GameObject visualWingRight;
    public GameObject gyrometer;

    [Header("----- SETTINGS -----")]
    public bool lerpRotation;
    public float rotateBodyBy;
    [Header("--- (X/Z) ---")]
    public float maxRotationXAxis;
    public float maxRotationZAxis;
    public float rotationSpeedXZ;
    [Header("--- (Y) ---")]
    public bool rotateYWings;
    public float maxRotationY;
    public float rotationSpeedY;

    [Header("--- (Boost) ---")]
    public float maxRotationXAxisBoost;
    public float maxRotationZAxisBoost;
    public float rotationSpeedXZBoost;
    public float rotationSpeedYBoost;

    [Header("--- (Penalty) ---")]
    public float maxRotationXAxisPenalty;
    public float maxRotationZAxisPenalty;
    public float rotationSpeedXZPenalty;
    public float rotationSpeedYPenalty;


    [Header("----- DEBUG -----")]
    [Header("--- (Actual Stuff) ---")]
    public float maxRotationXAxisActual;
    public float maxRotationZAxisActual;
    public float rotationSpeedXZActual;
    public float rotationSpeedYActual;
    public float rotationActualXAxis;
    public float rotationActualZAxis;

    [Header("--- (Rotation) ---")]
    public float wantRotationXAxis;
    public float wantRotationYAxis;
    public float wantRotationZAxis;
    public float yRotationDifference;

    [Header("--- (Buttons) ---")]
    public bool isPressingButtonForward;
    public bool isPressingButtonBackward;
    public bool isPressingButtonLeft;
    public bool isPressingButtonRight;
    public bool isPressingAnyButtonXZ;

    [Header("--- (Misc) ---")]
    public bool isBoosting;

    [Header("--- (Quaternions) ---")]
    public Quaternion defaultRotationBody;
    public Quaternion defaultRotationWingLeft;
    public Quaternion defaultRotationWingRight;
    public Quaternion defaultRotationGyrometer;

    public Quaternion wantQuaternionGameObject;
    public Quaternion wantQuaternionBody;
    public Quaternion wantQuaternionWingLeft;
    public Quaternion wantQuaternionWingRight;
    public Quaternion wantQuaternionGyrometer;

    public Quaternion lerpedQuaternionGameObject;
    public Quaternion lerpedQuaternionBody;
    public Quaternion lerpedQuaternionWingLeft;
    public Quaternion lerpedQuaternionWingRight;
    public Quaternion lerpedQuaternionGyrometer;

    public Quaternion finalQuaternionGameObject;
    public Quaternion finalQuaternionBody;
    public Quaternion finalQuaternionWingLeft;
    public Quaternion finalQuaternionWingRight;
    public Quaternion finalQuaternionGyrometer;


    void Start ()
    {
        defaultRotationBody = visualBody.transform.rotation;
        defaultRotationWingLeft = visualWingLeft.transform.rotation;
        defaultRotationWingRight = visualWingRight.transform.rotation;
        defaultRotationGyrometer = gyrometer.transform.rotation;
    }
	
	void FixedUpdate ()
    {
        updateInformation();
        getInput();
        calculateWantRotation();
        calculateLerpedRotation();
        applyRotation();
	}

    void updateInformation()
    {
        maxRotationXAxisActual = maxRotationXAxis;
        maxRotationZAxisActual = maxRotationZAxis;
        rotationSpeedXZActual = rotationSpeedXZ;
        rotationSpeedYActual = rotationSpeedY;
    }

    void getInput()
    {
        isPressingButtonForward = false;
        isPressingButtonBackward = false;
        isPressingButtonLeft = false;
        isPressingButtonRight = false;
        isPressingAnyButtonXZ = false;
        

        if (Input.GetKey("w") || Input.GetAxis("LeftStickVertical") < 0)
        { 
            isPressingButtonForward = true;
        }
        else if (Input.GetKey("s") || Input.GetAxis("LeftStickVertical") > 0)
        {
            isPressingButtonBackward = true;
        }
        if (Input.GetKey("a") || Input.GetAxis("LeftStickHorizontal") < 0)
        {
            isPressingButtonLeft = true;
        }
        if (Input.GetKey("d") || Input.GetAxis("LeftStickHorizontal") > 0)
        {
            isPressingButtonRight = true;
        }

        isPressingAnyButtonXZ = isPressingButtonBackward || isPressingButtonForward || isPressingButtonLeft || isPressingButtonRight;
    }

    void calculateWantRotation()
    {
        wantQuaternionBody = defaultRotationBody;
        wantQuaternionWingLeft = defaultRotationWingLeft;
        wantQuaternionWingRight = defaultRotationWingRight;
        wantQuaternionGyrometer = defaultRotationGyrometer;

        wantRotationXAxis = 0;
        wantRotationYAxis = playerCamera.transform.rotation.eulerAngles.y;
        wantRotationZAxis = 0;

        if (isPressingButtonForward)
            wantRotationXAxis += maxRotationXAxisActual;
        if (isPressingButtonBackward)
            wantRotationXAxis -= maxRotationXAxisActual;
        if (isPressingButtonRight)
            wantRotationZAxis -= maxRotationZAxisActual;
        if (isPressingButtonLeft)
            wantRotationZAxis += maxRotationZAxisActual;

        if (rotateYWings)
        {
            yRotationDifference = wantRotationYAxis - visualBody.transform.rotation.eulerAngles.y;
            if (yRotationDifference > 180)
                yRotationDifference -= 360;
            if (yRotationDifference < -180)
                yRotationDifference += 360;

            yRotationDifference = Mathf.Clamp(yRotationDifference, -maxRotationY, maxRotationY);
        }
        else
            yRotationDifference = 0;

        wantQuaternionGameObject = Quaternion.Euler(0, wantRotationYAxis, 0);
        wantQuaternionBody = Quaternion.Euler(defaultRotationBody.eulerAngles.x + wantRotationXAxis * rotateBodyBy, wantRotationYAxis, defaultRotationBody.eulerAngles.z + wantRotationZAxis * rotateBodyBy);
        wantQuaternionWingLeft = Quaternion.Euler(defaultRotationWingLeft.eulerAngles.x + wantRotationXAxis + yRotationDifference, wantRotationYAxis, defaultRotationWingLeft.eulerAngles.z + wantRotationZAxis);
        wantQuaternionWingRight = Quaternion.Euler(defaultRotationWingRight.eulerAngles.x - wantRotationXAxis + yRotationDifference, wantRotationYAxis + 180f, defaultRotationWingRight.eulerAngles.z - wantRotationZAxis);
        wantQuaternionGyrometer = Quaternion.Euler(defaultRotationGyrometer.eulerAngles.x + wantRotationXAxis, wantRotationYAxis, defaultRotationGyrometer.eulerAngles.z + wantRotationZAxis);
    }

    void calculateLerpedRotation()
    {
        lerpedQuaternionGameObject = Quaternion.Lerp(transform.rotation, wantQuaternionGameObject, rotationSpeedYActual * Time.deltaTime);
        lerpedQuaternionBody = Quaternion.Lerp(visualBody.transform.rotation, wantQuaternionBody, rotationSpeedXZActual * Time.deltaTime);
        lerpedQuaternionWingLeft = Quaternion.Lerp(visualWingLeft.transform.rotation, wantQuaternionWingLeft, rotationSpeedXZActual * Time.deltaTime);
        lerpedQuaternionWingRight = Quaternion.Lerp(visualWingRight.transform.rotation, wantQuaternionWingRight, rotationSpeedXZActual * Time.deltaTime);
        lerpedQuaternionGyrometer = Quaternion.Lerp(gyrometer.transform.rotation, wantQuaternionGyrometer, rotationSpeedXZActual * Time.deltaTime);

        rotationActualXAxis = lerpedQuaternionGyrometer.eulerAngles.x;
        if (rotationActualXAxis > 180)
            rotationActualXAxis -= 360;
        rotationActualZAxis = lerpedQuaternionGyrometer.eulerAngles.z;
        if (rotationActualZAxis > 180)
            rotationActualZAxis -= 360;
    }

    void applyRotation()
    {
        if (lerpRotation)
        {
            finalQuaternionGameObject = lerpedQuaternionGameObject;
            finalQuaternionBody = lerpedQuaternionBody;
            finalQuaternionWingLeft = lerpedQuaternionWingLeft;
            finalQuaternionWingRight = lerpedQuaternionWingRight;
            finalQuaternionGyrometer = lerpedQuaternionGyrometer;
        }
        else
        {
            finalQuaternionGameObject = wantQuaternionGameObject;
            finalQuaternionBody = wantQuaternionBody;
            finalQuaternionWingLeft = wantQuaternionWingLeft;
            finalQuaternionWingRight = wantQuaternionWingRight;
            finalQuaternionGyrometer = wantQuaternionGyrometer;
        }

        transform.rotation = finalQuaternionGameObject;
        visualBody.transform.rotation = finalQuaternionBody;
        visualWingLeft.transform.rotation = finalQuaternionWingLeft;
        visualWingRight.transform.rotation = finalQuaternionWingRight;
        gyrometer.transform.rotation = finalQuaternionGyrometer;
    }
}
