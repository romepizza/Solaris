using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityAttached : EntityCopiableAbstract, ICopiable, IPostCopy, IRemoveOnStateChange, IPrepareRemoveOnStateChange
{
    public GameObject m_attachedToGameObject;
    public int m_affiliation;
    public int m_monster;
    public AttachSystemBase m_attachSystemScript;

    public void setValuesByScript(GameObject prefab, AttachSystemBase attachSystemScript)
    {
        CubeEntityAttached script = prefab.GetComponent<CubeEntityAttached>();
        m_affiliation = script.m_affiliation;
        m_monster = script.m_monster;
        m_attachSystemScript = attachSystemScript;
    }

    public void setValuesByObject(GameObject gameObject, AttachSystemBase attachSystemScript)
    {
        m_attachSystemScript = attachSystemScript;
        if (gameObject == Constants.getPlayer())
        {
            m_attachedToGameObject = gameObject;
            m_affiliation = CubeEntityState.s_AFFILIATION_PLAYER;
            m_monster = CubeEntityState.s_TYPE_NONE;
        }
        else 
        {
            m_attachedToGameObject = gameObject;
            m_affiliation = GetComponent<CubeEntityState>().m_affiliation;
            m_monster = GetComponent<CubeEntityState>().m_type;
        }
        //else
        //    Debug.Log("Warning: Something might have gone wrong here!");
    }

    public void setValuesPlain(CubeEntityAttached baseScript)
    {
        m_attachedToGameObject = baseScript.m_attachedToGameObject;
        m_affiliation = baseScript.m_affiliation;
        m_monster = baseScript.m_monster;
    }

    public void deregisterAttach()
    {
        if (m_attachSystemScript != null)
        {
            m_attachSystemScript.deregisterCube(gameObject);
        }
        else
            ;// Debug.Log("Warning: m_attachSystemScript was null");
    }


    
    public void onCopy(ICopiable copiable)
    {
        setValuesPlain((CubeEntityAttached)copiable);
    }
    public void onStateChangePrepareRemove()
    {
        deregisterAttach();
    }
    public void onRemove()
    {
        //Debug.Log("SAD");
        Destroy(this);
    }
    public void onPostCopy()
    {
        m_attachSystemScript = GetComponent<AttachSystemBase>();
    }
}
