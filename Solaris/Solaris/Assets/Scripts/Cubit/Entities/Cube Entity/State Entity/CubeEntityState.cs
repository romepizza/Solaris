    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityState : MonoBehaviour, ICopyValues
{
    [Header("----- SETTINGS -----")]
    public float m_duration;

    // 0 = inactive
    // 1 = active
    // 2 = attached
    // 3 = core
    // 4 = ressource
    // 5 = special
    public int m_state;
    public static int s_STATE_INACTIVE = 0;
    public static int s_STATE_ACTIVE = 1;
    public static int s_STATE_ATTACHED = 2;
    public static int s_STATE_CORE = 3;
    public static int s_STATE_RESSOURCE = 4;
    public static int s_STATE_SPECIAL = 5;

    public static int s_STATE_PARTICLE = 10;

    // 0 = neutral
    // 1 = player
    // 2 = agressive
    // 5 = player ally

    // 20 = enemy1
    public int m_affiliation;
    public static int s_AFFILIATION_NEUTRAL = 0;
    public static int s_AFFILIATION_PLAYER = 1;
    public static int s_AFFILIATION_AGRESSIVE = 2;
    public static int s_AFFILIATION_PLAYER_ALLY = 5;
    public static int s_AFFILIATION_ENEMY_1 = 20;

    // 0 = none
    // 1 = player

    // 5 = player ally:      drone

    // 20 = monster:         ejector
    // 21 = monster:         worm
    // 22 = monster:         morpher
    // 23 = monster:         swarm
    // 30 = monster:         launcher

    // 50 = dynamic:         plasma projectile
    // 52 = dynamic:         anti missle

    public int m_type;
    public static int s_TYPE_NONE = 0;
    public static int s_TYPE_PLAYER = 1;
    public static int s_TYPE_DRONE = 5;
    public static int s_TYPE_EJECTOR = 20;
    public static int s_TYPE_WORM = 21;
    public static int s_TYPE_MORPHER = 22;
    public static int s_TYPE_SWARM = 23;
    public static int s_TYPE_LAUNCHER = 30;
    public static int s_TYPE_PLASMA_PROJECTILE = 50;
    public static int s_TYPE_ANTI_MISSLE = 51;

    [Header("----- DEBUG -----")]
    public float m_durationEndTime;
    public CubeEntitySystem m_entitySystemScript;
    public CubeEntityAttached m_attachedScript; // TODO : not in use
    //public List<CubeEntityEndState> m_endStateScripts;


    // Add Attached Script
    public CubeEntityAttached addAttachedScript()
    {
        m_attachedScript = gameObject.AddComponent<CubeEntityAttached>();
        return m_attachedScript;
    }

    public void removeAttachedScript()
    {
        //((IRemoveOnStateChange)m_attachedScript).onStateChangeRemove();
        //m_attachedScript = null;
    }

    // interfaces
    public void onCopyValues(ICopyValues copiable)
    {
        CubeEntityState stateScript = ((MonoBehaviour)copiable).GetComponent<CubeEntityState>();
        if (stateScript != null)
        {
            m_duration = stateScript.m_duration;
            m_state = stateScript.m_state;
            m_affiliation = stateScript.m_affiliation;
            m_type = stateScript.m_type;

            if (m_duration > 0)
            {

                //CubeEntityEndState endStateScript = gameObject.AddComponent<CubeEntityEndState>();
                //if (endStateScript == null)
                CubeEntityEndState endStateScript = gameObject.AddComponent<CubeEntityEndState>();
                endStateScript.setValuesByPrefab(this, m_entitySystemScript);
            }
            else
            {
                //Destroy(GetComponent<CubeEntityEndState>());
            }


        }
        else
            Debug.Log("no state settings found!");


    }
    /*
    // Setter
    public void setStateByPrefab(GameObject prefab)
    {
        CubeEntityState stateScript = prefab.GetComponent<CubeEntityState>();
        if (stateScript != null)
        {
            m_duration = stateScript.m_duration;
            m_state = stateScript.m_state;
            m_affiliation = stateScript.m_affiliation;
            m_type = stateScript.m_type;

            if (m_duration > 0)
            {
                CubeEntityEndState endStateScript = gameObject.GetComponent<CubeEntityEndState>();
                if (endStateScript == null)
                    endStateScript = gameObject.AddComponent<CubeEntityEndState>();
                endStateScript.setValuesByPrefab(prefab, m_entitySystemScript);
            }
            else
                Destroy(GetComponent<CubeEntityEndState>());
        }
        else
            Debug.Log("no state settings found!");
    }*/
    // copy
    public void setDuration(float duration)
    {
        if (m_duration > 0)
        {
            CubeEntityEndState endStateScript = gameObject.GetComponent<CubeEntityEndState>();
            if (endStateScript == null)
                endStateScript = gameObject.AddComponent<CubeEntityEndState>();
            endStateScript.setDuration(duration, m_entitySystemScript);
        }
        else
        {
            //Destroy(gameObject.GetComponent<CubeEntityState>());
        }
    }
    public bool isInactive()
    {
        checkStateValidity();
        return m_state == s_STATE_INACTIVE && m_affiliation == s_AFFILIATION_NEUTRAL && m_type == s_TYPE_NONE;
    }

    // Getter
    public CubeEntityAttached getAttachedScript()
    {
        return m_attachedScript;
    }

    

    // State checks
    public bool canBeInactive()
    {
        return true;
    }
    public bool canBeActiveNeutral()
    {
        return true;
    }
    public bool canBeActivePlayer()
    {
        checkStateValidity();
        bool valid_0 = m_state == s_STATE_INACTIVE && m_affiliation == s_AFFILIATION_NEUTRAL;

        return valid_0;
    }
    public bool canBeAttachedToPlayer()
    {
        checkStateValidity();
        bool valid_0 = m_state == s_STATE_INACTIVE && m_affiliation == s_AFFILIATION_NEUTRAL;

        return valid_0;
    }
    public bool canBeActiveEnemyEjector()
    {
        checkStateValidity();
        bool valid_0 = m_state == s_STATE_ATTACHED && m_affiliation == s_AFFILIATION_ENEMY_1 && m_type == s_TYPE_EJECTOR;

        return valid_0;
    }
    public bool canBeCoreToEnemyEjector()
    {
        checkStateValidity();
        bool valid_0 = m_state == s_STATE_INACTIVE && m_affiliation == s_AFFILIATION_NEUTRAL;

        return valid_0;
    }
    public bool canBeCoreToEnemyWorm()
    {
        checkStateValidity();
        bool valid_0 = m_state == s_STATE_INACTIVE && m_affiliation == s_AFFILIATION_NEUTRAL;

        return valid_0;
    }

    // general state checks
    public bool canBeCoreGeneral()
    {
        checkStateValidity();
        bool valid_0 = m_state == s_STATE_INACTIVE && m_affiliation == s_AFFILIATION_NEUTRAL;

        return valid_0;
    }
    public bool canBeAttachedToEnemy()
    {
        checkStateValidity();
        bool valid_0 = m_state == s_STATE_INACTIVE && m_affiliation == s_AFFILIATION_NEUTRAL && m_type == s_TYPE_NONE;

        return valid_0;
    }

    // state special checks
    public bool isFoe(GameObject gameObject)
    {
        CubeEntityState script = gameObject.GetComponent<CubeEntityState>();
        if(script == null)
        {
            Debug.Log("Aborted: script was null!");
            return false;
        }

        if (script.m_affiliation == s_AFFILIATION_AGRESSIVE || m_affiliation == s_AFFILIATION_AGRESSIVE)
            return true;

        // player
        if (m_affiliation == s_AFFILIATION_PLAYER || m_affiliation == s_AFFILIATION_PLAYER_ALLY)
        {
            if (script.m_affiliation == s_AFFILIATION_ENEMY_1)
                return true;
        }
        if(m_affiliation == s_AFFILIATION_ENEMY_1)
        {
            if (script.m_affiliation == s_AFFILIATION_PLAYER || script.m_affiliation == s_AFFILIATION_PLAYER_ALLY)
                return true;
        }
        return false;
    }
    public bool isFoe(CubeEntityState script)
    {
        if (script == null)
        {
            Debug.Log("Aborted: script was null!");
            return false;
        }

        if (script.m_affiliation == s_AFFILIATION_AGRESSIVE || m_affiliation == s_AFFILIATION_AGRESSIVE)
            return true;

        // player
        if (m_affiliation == s_AFFILIATION_PLAYER || m_affiliation == s_AFFILIATION_PLAYER_ALLY)
        {
            if (script.m_affiliation == s_AFFILIATION_ENEMY_1)
                return true;
        }
        if (m_affiliation == s_AFFILIATION_ENEMY_1)
        {
            if (script.m_affiliation == s_AFFILIATION_PLAYER || script.m_affiliation == s_AFFILIATION_PLAYER_ALLY)
                return true;
        }
        return false;
    }
    public bool isFoe(int otherAffiliation)
    {
        if (otherAffiliation == s_AFFILIATION_AGRESSIVE || m_affiliation == s_AFFILIATION_AGRESSIVE)
            return true;

        // player
        if (m_affiliation == s_AFFILIATION_PLAYER || m_affiliation == s_AFFILIATION_PLAYER_ALLY)
        {
            if (otherAffiliation == s_AFFILIATION_ENEMY_1)
                return true;
        }
        if (m_affiliation == s_AFFILIATION_ENEMY_1)
        {
            if (otherAffiliation == s_AFFILIATION_PLAYER || otherAffiliation == s_AFFILIATION_PLAYER_ALLY)
                return true;
        }
        return false;
    }
    public bool isFriendly(GameObject gameObject)
    {
        CubeEntityState script = gameObject.GetComponent<CubeEntityState>();
        if (script == null)
        {
            Debug.Log("Aborted: script was null!");
            return false;
        }

        if (script.m_affiliation == s_AFFILIATION_AGRESSIVE || m_affiliation == s_AFFILIATION_AGRESSIVE)
            return false;

        // player
        if (m_affiliation == s_AFFILIATION_PLAYER || m_affiliation == s_AFFILIATION_PLAYER_ALLY)
        {
            if(script.m_affiliation == s_AFFILIATION_PLAYER || script.m_affiliation == s_AFFILIATION_PLAYER_ALLY)
                return true;
        }
        if (m_affiliation == s_AFFILIATION_ENEMY_1)
        {
            if (script.m_affiliation == s_AFFILIATION_ENEMY_1)
                return true;
        }
        return false;
    }
    public bool isFriendly(CubeEntityState script)
    {
        if (script == null)
        {
            Debug.Log("Aborted: script was null!");
            return false;
        }

        if (script.m_affiliation == s_AFFILIATION_AGRESSIVE || m_affiliation == s_AFFILIATION_AGRESSIVE)
            return false;

        // player
        if (m_affiliation == s_AFFILIATION_PLAYER || m_affiliation == s_AFFILIATION_PLAYER_ALLY)
        {
            if (script.m_affiliation == s_AFFILIATION_PLAYER || script.m_affiliation == s_AFFILIATION_PLAYER_ALLY)
                return true;
        }
        if (m_affiliation == s_AFFILIATION_ENEMY_1)
        {
            if (script.m_affiliation == s_AFFILIATION_ENEMY_1)
                return true;
        }
        return false;
    }
    public bool isFriendly(int otherAffiliation)
    {
        if (otherAffiliation == s_AFFILIATION_AGRESSIVE || m_affiliation == s_AFFILIATION_AGRESSIVE)
            return false;

        // player
        if (m_affiliation == s_AFFILIATION_PLAYER || m_affiliation == s_AFFILIATION_PLAYER_ALLY)
        {
            if (otherAffiliation == s_AFFILIATION_PLAYER || otherAffiliation == s_AFFILIATION_PLAYER_ALLY)
                return true;
        }
        if (m_affiliation == s_AFFILIATION_ENEMY_1)
        {
            if (otherAffiliation == s_AFFILIATION_ENEMY_1)
                return true;
        }
        return false;
    }

    // Check Validity
    void checkStateValidity()
    {
        if (m_state == s_STATE_INACTIVE && m_affiliation != s_AFFILIATION_NEUTRAL)
            Debug.Log("(" + gameObject.name + ") Incorrect state: inactive & !neutral");
        if (m_state == s_STATE_ATTACHED && m_affiliation == s_AFFILIATION_NEUTRAL)
            Debug.Log("(" + gameObject.name + ") Incorrect state: attached & neutral");
        if (m_state == s_STATE_CORE && (m_affiliation == s_AFFILIATION_NEUTRAL))
            Debug.Log("(" + gameObject.name + ") Incorrect state: core & neutral");
        //if (m_state == s_STATE_CORE && (m_affiliation == s_AFFILIATION_PLAYER))
            //Debug.Log("(" + gameObject.name + ") Incorrect state: core & player");
    }
}
