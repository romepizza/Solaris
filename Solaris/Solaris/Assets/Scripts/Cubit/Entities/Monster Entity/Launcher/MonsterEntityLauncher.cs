using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntityLauncher : MonsterEntityAbstractBase, IRemoveOnStateChange, IPrepareRemoveOnStateChange, ICopiable, IStateOnStateChange
{
    [Header("----- SETTINGS -----")]
    public float m_rotationSpeed;
    public Transform m_lookCenter;
    [Header("----- DEBUG -----")]
    public float m_currentRotation;
    [Header("--- (Scripts) ---")]
    public bool placeHolder;
    private void Update()
    {
        m_currentRotation += m_rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(m_lookCenter.position) * Quaternion.AngleAxis(m_currentRotation, Vector3.forward);
    }

    void setValues(MonsterEntityLauncher copyScript)
    {
        m_rotationSpeed = copyScript.m_rotationSpeed;
        m_lookCenter = Constants.getPlanet().transform;
    }

    public void onCopy(ICopiable copiable)
    {
        setValues((MonsterEntityLauncher)copiable);
    }

    public void onStateChangePrepareRemove()
    {

    }
    public void onRemove()
    {
        Destroy(this);
    }
}
