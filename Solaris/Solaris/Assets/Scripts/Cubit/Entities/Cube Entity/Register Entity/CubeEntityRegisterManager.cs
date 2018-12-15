using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityRegisterManager : MonoBehaviour, ICopyValues, IPostCopy, IPrepareRemoveOnStateChange
{
    [Header("------- Settings -------")]
    public bool m_isDefyGlobal;
    public bool m_isDefyLocal;

    [Header("------- Debug --------")]
    public List<EeGravityAlteration> m_registeredGravitationFieldsLocal;
    public List<EeGravityAlteration> m_registeredGravitationFieldsGlobal;
    public List<CubeEntityMovementAbstract> m_defyGravityScriptsGlobal;
    public List<CubeEntityMovementAbstract> m_defyGravityScriptsLocal;
    public bool m_isInitialized;
    public bool m_isDefyGlobalCurrent;
    public bool m_isDefyLocalCurrent;
    // Use this for initialization

    void initializeStuff()
    {
        m_registeredGravitationFieldsLocal = new List<EeGravityAlteration>();
        m_registeredGravitationFieldsGlobal = new List<EeGravityAlteration>();

        m_defyGravityScriptsGlobal = new List<CubeEntityMovementAbstract>();
        m_defyGravityScriptsLocal = new List<CubeEntityMovementAbstract>();
    }

	// Update is called once per frame
	void Update ()
    {
		
	}

    // management
    public bool registerGravitationField(EeGravityAlteration gravityAlteration)
    {
        
        if (gravityAlteration.m_isGlobal)
        {
            if (m_registeredGravitationFieldsGlobal.Contains(gravityAlteration)) // TODO : Peformance? list proberbly isn't big
            {
                Debug.Log("Warning: gravityAlteration already was in the list!");
                return false;
            }
            gravityAlteration.registerAgent(this);
            m_registeredGravitationFieldsGlobal.Add(gravityAlteration);
        }
        else
        {
            if (m_registeredGravitationFieldsLocal.Contains(gravityAlteration)) // TODO : Peformance? list proberbly isn't big
            {
                ;// Debug.Log("Warning: gravityAlteration already was in the list!");
                return false;
            }
            gravityAlteration.registerAgent(this);
            m_registeredGravitationFieldsLocal.Add(gravityAlteration);
        }

        return true;
    }
    public void deregisterGravitationField(EeGravityAlteration gravityAlteration)
    {
        if (gravityAlteration.m_isGlobal)
        {
            if (!m_registeredGravitationFieldsGlobal.Contains(gravityAlteration)) // TODO : Peformance? list proberbly isn't big
            {
                Debug.Log("Warning: gravityAlteration was not in the list!");
                return;
            }
            gravityAlteration.deregisterAgent(this);
            m_registeredGravitationFieldsGlobal.Remove(gravityAlteration);
        }
        else
        {
            if (!m_registeredGravitationFieldsLocal.Contains(gravityAlteration)) // TODO : Peformance? list proberbly isn't big
            {
                Debug.Log("Warning: gravityAlteration was not in the list!");
                return;
            }
            gravityAlteration.deregisterAgent(this);
            m_registeredGravitationFieldsLocal.Remove(gravityAlteration);
        }
    }

    public void deregisterFromAllGravitaionFieldsLocal()
    {
        for (int i = m_registeredGravitationFieldsLocal.Count - 1; i >= 0; i--) // TODO : Performance, use indices
        {
            deregisterGravitationField(m_registeredGravitationFieldsLocal[i]);
        }
    }
    public void deregisterFromAllGravitaionFieldsGlobal()
    {
        for (int i = m_registeredGravitationFieldsGlobal.Count - 1; i >= 0; i--) // TODO : Performance, use indices
        {
            deregisterGravitationField(m_registeredGravitationFieldsGlobal[i]);
        }
    }
    public void deregisterFromAllGravitationFields()
    {
        deregisterFromAllGravitaionFieldsLocal();
        deregisterFromAllGravitaionFieldsGlobal();
    }

    // skip
    public void changeSkipGlobal(bool skip)
    {
        if (skip)
            deregisterFromAllGravitaionFieldsGlobal();
        else
            EeGravityAlteration.registerAgentStatic(this);
        //for(int i = 0; i < m_registeredGravitationFieldsGlobal.Count; i++)
        //{
            //m_registeredGravitationFieldsGlobal[i].registerSkip(this, skip);
        //}
    }
    public void changeSkipLocal( bool skip)
    {
        if (skip)
            deregisterFromAllGravitaionFieldsLocal();
        else
            ;



        //for (int i = 0; i < m_registeredGravitationFieldsLocal.Count; i++)
        //{
            //m_registeredGravitationFieldsLocal[i].registerSkip(this, skip);
            //m_registeredGravitationFieldsLocal[i].
        //}
    }
    public void registerSkipGlobal(CubeEntityMovementAbstract script, bool skip)
    {
        if (skip)
        {
            if (m_defyGravityScriptsGlobal.Contains(script)) // TODO : delete if not problematic
            {
                Debug.Log("Aborted: script was already in the list!");
                return;
            }
            m_isDefyGlobalCurrent = true;
            m_defyGravityScriptsGlobal.Add(script);
            if (m_defyGravityScriptsGlobal.Count == 1)
                changeSkipGlobal(true);
        }
        else
        {
            if (!m_defyGravityScriptsGlobal.Contains(script)) // TODO : delete if not problematic
            {
                Debug.Log("Aborted: script was not in the list!");
                return;
            }
            m_isDefyGlobalCurrent = m_isDefyGlobal;
            m_defyGravityScriptsGlobal.Remove(script);
            if (m_defyGravityScriptsGlobal.Count >= 0)
                changeSkipGlobal(false);
        }
    }
    public void registerSkipLocal(CubeEntityMovementAbstract script, bool skip)
    {
        if (skip)
        {
            if (m_defyGravityScriptsLocal.Contains(script)) // TODO : delete if not problematic
            {
                Debug.Log("Aborted: script was already in the list!");
                return;
            }
            m_isDefyLocalCurrent = true;
            m_defyGravityScriptsLocal.Add(script);
            if (m_defyGravityScriptsLocal.Count == 1)
                changeSkipLocal(true);
        }
        else
        {
            if (!m_defyGravityScriptsLocal.Contains(script)) // TODO : delete if not problematic
            {
                Debug.Log("Aborted: script was not in the list!");
                return;
            }
            m_isDefyLocalCurrent = m_isDefyLocal;
            m_defyGravityScriptsLocal.Remove(script);
            if (m_defyGravityScriptsLocal.Count >= 0)
                changeSkipLocal(false);
        }
    }
   
    void setValues(CubeEntityRegisterManager copyScript)
    {
        m_isDefyGlobal = copyScript.m_isDefyGlobal;
        m_isDefyLocal = copyScript.m_isDefyLocal;

        m_isDefyGlobalCurrent = m_isDefyGlobal;
        m_isDefyLocalCurrent = m_isDefyLocal;
    }

    // interfaces
    public void onCopyValues(ICopyValues copiable)
    {
        if (!m_isInitialized)
            initializeStuff();
        setValues((CubeEntityRegisterManager)copiable); 
        //EeGravityAlteration script = ((MonoBehaviour)copiable).GetComponent<EeGravityAlteration>();
    }
    public void onPostCopy()
    {
        if(GetComponent<CubeEntityState>().m_state == CubeEntityState.s_STATE_ACTIVE)// || GetComponent<CubeEntityState>().m_state == CubeEntityState.s_STATE_ATTACHED)
            EeGravityAlteration.registerAgentStatic(this);
    }
    public void onStateChangePrepareRemove()
    {
        deregisterFromAllGravitationFields();
    }
}
