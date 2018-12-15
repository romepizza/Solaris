using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityCollisionEffect : EntityCopiableAbstract, IRemoveOnStateChange
{
    [Header("----- SETTINGS -----")]
    public int m_collisionDelayFrames;
    public GameObject m_explosionEffect;

    [Header("----- DEBUG -----")]
    public GameObject m_player;
    public int m_currentDelayFrames;
    public CubeEntityCollision.collisionRelationship m_collisionRelationship;

    private CubeEntityCollision m_collisionScript;


	
	// Update is called once per frame
	void FixedUpdate ()
    {
        evaluateCollisionEffect();
    }

    void evaluateCollisionEffect()
    {
        if (m_currentDelayFrames >= m_collisionDelayFrames)
        {
            // --- Player ---
            if (m_collisionRelationship == CubeEntityCollision.collisionRelationship.activeEnemyVsPlayer)
            {
                loseLife(m_player, 1);
                //CubeEntitySoundInstance soundInstance = SoundManager.addSoundPlayerCubeHit(transform.position);
                //soundInstance.m_audioSource.volume *= 0.2f;
            }
            if (m_collisionRelationship == CubeEntityCollision.collisionRelationship.attachedEnemyVsPlayer)
            {
                loseLife(m_player, 2);
            }
            if (m_collisionRelationship == CubeEntityCollision.collisionRelationship.coreVsPlayer)
            {
                loseLife(m_player, 3);
            }

            // --- Inactive ---
            if (m_collisionRelationship == CubeEntityCollision.collisionRelationship.inactiveVsPlayer)
            {
                launchCubePlayer();
            }

            // --- Active Neutral ---
            if (m_collisionRelationship == CubeEntityCollision.collisionRelationship.activeNeutralVsPlayer)
            {
                launchCubePlayer();
            }
            // --- Active Player ---
            if (m_collisionRelationship == CubeEntityCollision.collisionRelationship.activePlayerVsPlayer)
            {
                launchCubePlayer();
            }
            if (m_collisionRelationship == CubeEntityCollision.collisionRelationship.activePlayerVsEnemy)
            {
                setCubeToActiveNeutral();
            }

            // --- Active Enemy ---
            if (m_collisionRelationship == CubeEntityCollision.collisionRelationship.activeEnemyVsPlayer)
            {
                setCubeToActiveNeutral();
            }
            if (m_collisionRelationship == CubeEntityCollision.collisionRelationship.activeEnemyVsActivePlayer)
            {
                setCubeToActiveNeutral();
            }

            // --- Attached player ---
            if (m_collisionRelationship == CubeEntityCollision.collisionRelationship.attachedPlayerVsActiveEnemy)
            {
                setCubeToActiveNeutral();
            }

            // --- Attached Enemy ---
            if (m_collisionRelationship == CubeEntityCollision.collisionRelationship.attachedEnemyVsPlayer)
            { 
                setCubeToActiveNeutral();
            }
            if (m_collisionRelationship == CubeEntityCollision.collisionRelationship.attachedEnemyVsActivePlayer)
            {
                loseLife(this.gameObject, 1);
                //SoundManager.addSoundPlayerCubeHit(transform.position);
                //if(Random.Range(0f, 1f) < 0.3f)
                  //changeMaxCubes(-1);
                //setCubeToActiveNeutral();

                //Constants.getPlayer().GetComponent<PlayerEntityAttachSystem>().addToGrab(this.gameObject);
            }

            // --- Core ---
            if (m_collisionRelationship == CubeEntityCollision.collisionRelationship.coreVsActivePlayer)
            {
                //CubeEntitySoundInstance instance = SoundManager.addSoundPlayerCubeHit(transform.position);
                //instance.m_audioSource.pitch -= 1f;
                loseLife(this.gameObject, 3);
            }

            // Explosion
            if(m_explosionEffect != null)
            {
                GameObject explosion = Instantiate(m_explosionEffect);
                explosion.transform.position = transform.position;
            }

            Destroy(this);
            //m_collisionScript.removeCollisionEffectScript();
        }
        else if (m_currentDelayFrames < m_collisionDelayFrames)
            m_currentDelayFrames++;
    }


    void setCubeToActiveNeutral()
    {
        //if (GetComponent<CubeEntityAttached>() != null)
            //GetComponent<CubeEntityAttached>().deregisterAttach();
        m_collisionScript.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activeNeutralPrefab);
    }

    void loseLife(GameObject victim, int damage)
    {
        if (victim == m_player)
        {
            m_player.GetComponent<PlayerEntityLifeSystem>().loseHp(damage);
        }
        else if (victim.GetComponent<CubeEntitySystem>() != null)
        {
            if (victim.GetComponent<CubeEntitySystem>().getStateComponent().m_state == CubeEntityState.s_STATE_ATTACHED || victim.GetComponent<CubeEntitySystem>().getStateComponent().m_state == CubeEntityState.s_STATE_CORE)
            {
                if (victim.GetComponent<CubeEntitySystem>().getStateComponent().getAttachedScript() != null)
                {
                    if (victim.GetComponent<CubeEntitySystem>().getStateComponent().getAttachedScript().m_attachedToGameObject != null)
                    {
                        if (victim.GetComponent<CubeEntitySystem>().getStateComponent().getAttachedScript().m_attachedToGameObject.GetComponent<MonsterEntityBase>() != null)
                        {
                            victim.GetComponent<CubeEntitySystem>().getStateComponent().getAttachedScript().m_attachedToGameObject.GetComponent<MonsterEntityBase>().loseHp(damage, victim);
                        }
                        else
                            Debug.Log("Warning: attached Cube's core isn't actually a core!");
                    }
                    else
                        Debug.Log("Warning: attached Cube doesn't know what cubeCore it is attached to!");
                }
                else
                    Debug.Log("Warning: attached Cube doesn't have an attachedScript attached to it!");
            }
        }
    }
    void changeMaxCubes(int cubeNumber)
    {
        if (GetComponent<CubeEntityAttached>() != null && GetComponent<CubeEntityAttached>().m_attachedToGameObject != null && GetComponent<CubeEntityAttached>().m_attachedToGameObject.GetComponent<MonsterEntityBase>() != null)
        {
            GetComponent<CubeEntityAttached>().m_attachedToGameObject.GetComponent<MonsterEntityBase>().changeMaxCubes(cubeNumber);
        }
    }
    void launchCubePlayer()
    {
        //m_collisionScript.m_entitySystemScript.getMovementComponent().addAccelerationComponent(transform.position + Camera.main.transform.forward, 0.5f, 1000f, 50f);
        m_collisionScript.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activePlayerPrefab); ;
    }


    public void setValues(CubeEntityCollision collisionScript, CubeEntityCollision.collisionRelationship collisionRelationship, int delayFrames, GameObject explosionEffect)
    {
        m_player = Constants.getPlayer();
        m_collisionDelayFrames = delayFrames;
        m_collisionScript = collisionScript;
        m_collisionRelationship = collisionRelationship;
        m_explosionEffect = explosionEffect;
    }

    public void onRemove()
    {
        Destroy(this);
    }
}
