using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityCollision : MonoBehaviour
{
    [Header("----- SETTINGS -----")]
    public int m_collisionDelayFrames = 1;

    [Header("--- (Explosions) ---")]
    public GameObject m_explosionCurrent;
    public GameObject m_explosionEjector;
    public GameObject m_explosionPlayerIsHit;
    public GameObject m_explosionEnemyIsHit;

    [Header("----- DEBUG -----")]
    public GameObject m_player;
    public GameObject m_collidedWith;
    public collisionRelationship m_collisionRelationship = collisionRelationship.none;
    public int m_ownState = -1;
    public int m_ownAffiliation = -1;
    public int m_colliderState = -1;
    public int m_colliderAffiliation = -1;
    public bool m_addCollision = false;

    public CubeEntitySystem m_entitySystemScript;

    public enum collisionRelationship
    {
        none,
        inactiveVsPlayer,
        activeNeutralVsPlayer,
        activePlayerVsPlayer, activePlayerVsEnemy, 
        activeEnemyVsPlayer, activeEnemyVsActivePlayer,
        attachedPlayerVsActiveEnemy,
        attachedEnemyVsPlayer, attachedEnemyVsActivePlayer,
        coreVsActivePlayer, coreVsPlayer
    }

	void Start ()
    {
        m_entitySystemScript = GetComponent<CubeEntitySystem>();
        m_player = Constants.getPlayer();
    }
	

    public void getRelationship()
    {
        if (m_entitySystemScript != null && m_entitySystemScript.getStateComponent() != null)
        {
            m_explosionCurrent = null;
            m_ownState = m_entitySystemScript.getStateComponent().m_state;
            m_ownAffiliation = m_entitySystemScript.getStateComponent().m_affiliation;
            //m_addCollision = false;

            // if collision with player
            if (m_collidedWith == m_player)
            {
                if (m_ownState == 0)
                {
                    if (m_ownAffiliation == 0) { m_collisionRelationship = collisionRelationship.inactiveVsPlayer; } else { Debug.Log("States messed up!"); }
                }
                if (m_ownState == 1)
                {
                    if (m_ownAffiliation == 2)
                    {
                        m_collisionRelationship = collisionRelationship.activeEnemyVsPlayer;
                        m_explosionCurrent = m_explosionPlayerIsHit;
                    }
                }
                if (m_ownState == 2)
                {
                    if (m_ownAffiliation == 2)
                    {
                        m_collisionRelationship = collisionRelationship.attachedEnemyVsPlayer;
                        m_explosionCurrent = m_explosionPlayerIsHit;
                    }
                }
                if (m_ownState == 3)
                {
                    if (m_ownAffiliation == 2)
                    {
                        m_collisionRelationship = collisionRelationship.coreVsPlayer;
                        m_explosionCurrent = m_explosionPlayerIsHit;
                    }
                }
            }

            // if collision with other cube
            else if (m_collidedWith.GetComponent<CubeEntitySystem>() != null && m_collidedWith.GetComponent<CubeEntitySystem>().getStateComponent() != null)
            {
                m_colliderState = m_collidedWith.GetComponent<CubeEntityState>().m_state;
                m_colliderAffiliation = m_collidedWith.GetComponent<CubeEntityState>().m_affiliation;

                if(m_ownState == 0)
                {

                }
                else if (m_ownState == 1)
                {
                    //if (m_ownAffiliation == 0) { m_collisionRelationship = collisionRelationship.activeNeutralVsPlayer; }
                    if (m_ownAffiliation == 1 && m_colliderAffiliation == 2)
                    {
                        m_collisionRelationship = collisionRelationship.activePlayerVsEnemy;
                        m_explosionCurrent = m_explosionEnemyIsHit;
                    }
                    if (m_ownAffiliation == 2 && m_colliderAffiliation == 1)
                    {
                        m_collisionRelationship = collisionRelationship.activeEnemyVsActivePlayer;
                        m_explosionCurrent = m_explosionEnemyIsHit;
                    }
                }
                else if (m_ownState == 2)
                {
                    if (m_ownAffiliation == 1 && m_colliderState == 1 && m_colliderAffiliation == 2) { m_collisionRelationship = collisionRelationship.attachedPlayerVsActiveEnemy; }
                    if (m_ownAffiliation == 2 && m_colliderState == 1 && m_colliderAffiliation == 1) { m_collisionRelationship = collisionRelationship.attachedEnemyVsActivePlayer; }
                }
                else if (m_ownState == 3)
                {
                    if (m_colliderState == 1 && m_colliderAffiliation == 1) { m_collisionRelationship = collisionRelationship.coreVsActivePlayer; }
                }
                else
                    Debug.Log("Error: No valid state found!");
            }
            else
                m_collisionRelationship = collisionRelationship.none;
        }
        else
            Debug.Log("Error: Tried to evaluate collision relationship, but unable to find own state component!");
    }
    
    void resetScript()
    {
        m_collidedWith = null;
        m_collisionRelationship = collisionRelationship.none;
        m_ownState = -1;
        m_ownAffiliation = -1;
        m_colliderState = -1;
        m_colliderAffiliation = -1;
    }

    void addCollisionEffectScript()
    {
        CubeEntityCollisionEffect collisionEffectScript = gameObject.AddComponent<CubeEntityCollisionEffect>();
        collisionEffectScript.setValues(this, m_collisionRelationship, m_collisionDelayFrames, m_explosionCurrent);
        resetScript();
    }

    public void removeCollisionEffectScripts()
    {
        CubeEntityCollisionEffect[] effects = gameObject.GetComponents<CubeEntityCollisionEffect>();
        for (int i = 0; i < effects.Length; i++)
            Destroy(effects[i]);
        resetScript();
    }

    void OnCollisionEnter(Collision col)
    {
        //if (gameObject.GetComponent<CubeEntityCollisionEffect>() == null)
        {
            m_collidedWith = col.gameObject;
            getRelationship();
            if(!(m_collisionRelationship == collisionRelationship.none))
                addCollisionEffectScript();
        }
    }
}
