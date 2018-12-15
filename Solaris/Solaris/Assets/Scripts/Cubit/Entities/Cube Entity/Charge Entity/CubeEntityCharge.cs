using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityCharge : MonoBehaviour, ICopyValues
{
    [Header("------- Settings -------")]
    public float m_chargeResistance;
    public float m_maxCharge;
    public float m_minCharge;

    [Header("--- Charge Power ---")]
    public float m_chargePower;

    [Header("--- Out Of Charge Effect ---")]
    public bool m_outOfChargeSetToInactive;
    public bool m_outOfChargeSetToActiveNeutral;
    public int m_outOfChargeLoseLife;
    public int m_outOfChargeReduceMaxGrabbed;
    public float m_refillChargeWhenOut;


    [Header("------- Debug -------")]
    public float m_currentCharge;
    public CubeEntityState m_thisStateScript;
    public CubeEntityCharge m_colliderChargeScript;
    public int m_ownState;
    public int m_ownAffiliation;
    public int m_colliderState;
    public int m_colliderAffiliation;

    public float m_factor = 1;
    // init
    public void initializeScript()
    {
        m_currentCharge = m_maxCharge;
        m_thisStateScript = GetComponent<CubeEntityState>();
        m_ownState = m_thisStateScript.m_state;
        m_ownAffiliation = m_thisStateScript.m_affiliation;
    }

    // charge evaluation
    /*
    public bool evaluateChargeDissolve(GameObject otherObject)
    {
        if(otherObject.GetComponent<CubeEntityCharge>() != null)
        {
            return evaluateChargeDissolve(otherObject.GetComponent<CubeEntityCharge>());
        }
        return false;
    }*/
    public bool evaluateChargeDissolve(CubeEntityCharge otherChargeScript)
    {
        Debug.Log("Caution: Code currently not in use!");
        if (otherChargeScript != null)
        {
            addCharge(-otherChargeScript.m_chargePower);
            if (isOutOfCharge())
                return true;
            else
            {
                createChargeLostEffects();
            }
        }
        return false;
    }
    public void evaluateDischarge(float chargeAdd, int otherAffiliation, int otherState, bool checkOutOfcharge)
    {
        if (m_ownAffiliation != otherAffiliation)
        {
            if (otherState != CubeEntityState.s_STATE_INACTIVE)                   // TODO
            {
                addCharge(chargeAdd);
                if (checkOutOfcharge && isOutOfCharge())
                    performOutOfCharge();
                else if (chargeAdd != 0)
                    createChargeLostEffects();
            }
        }
            
    }

    // out of charge
    public bool isOutOfCharge()
    {
        if (m_currentCharge < m_minCharge)
        {
            return true;
            //performOutOfCharge();
        }
        return false;
    }
    public void performOutOfCharge()
    {
        checkLogic();

        if (m_outOfChargeReduceMaxGrabbed > 0)
            reduceMaxGrabbed(m_outOfChargeReduceMaxGrabbed);

        if(m_outOfChargeLoseLife != 0)
        {
            loseLife(m_outOfChargeLoseLife);
        }

        if (m_outOfChargeSetToInactive)
            setToInactive();

        if (m_outOfChargeSetToActiveNeutral)
            setToActiveNeutral();

        if (m_refillChargeWhenOut > 0)
            setCharge(m_maxCharge);
    }
    void checkLogic()
    {
        if(m_outOfChargeSetToActiveNeutral)
        {
            if(m_refillChargeWhenOut != 0)
            {
                Debug.Log("Caution!");
            }
            if(m_outOfChargeSetToInactive)
            {
                Debug.Log("Caution!");
            }
        }
        if(m_outOfChargeSetToInactive)
        {
            if (m_refillChargeWhenOut != 0)
            {
                Debug.Log("Caution!");
            }
        }

        if (m_outOfChargeSetToActiveNeutral && m_refillChargeWhenOut != 0)
            Debug.Log("Caution!");
    }
    public void createChargeLostEffects()
    {
        foreach(GameObject effect in GetComponent<CubeEntityParticleSystem>().m_OnChargeLost)
        {
            GetComponent<CubeEntityParticleSystem>().createParticleEffect(effect);
        }
    }

    // manage charge
    public void addCharge(float change)
    {
        setCharge(m_currentCharge + change * Mathf.Clamp01(1 - m_chargeResistance));
    }
    public void setCharge(float value)
    {
        m_currentCharge = value;
        m_currentCharge = Mathf.Min(m_maxCharge, m_currentCharge);
    }

    // collision
    public void evaluateCollision(Collider collider/*, CubeEntityState m_stateScript*/)
    {
        

        m_colliderChargeScript = collider.GetComponent<CubeEntityCharge>();
        m_colliderState = collider.GetComponent<CubeEntityState>().m_state;
        m_colliderAffiliation = collider.GetComponent<CubeEntityState>().m_affiliation;


        evaluateDischarge(-m_colliderChargeScript.m_chargePower, m_colliderAffiliation, m_colliderState, false);
        m_colliderChargeScript.evaluateDischarge(-m_chargePower, m_ownAffiliation, m_ownState, false);

        if (isOutOfCharge())
            performOutOfCharge();
        if (m_colliderChargeScript.isOutOfCharge())
            m_colliderChargeScript.performOutOfCharge();

        return;
        /*
        // Affiliation Neutral
        if (m_ownAffiliation == CubeEntityState.s_AFFILIATION_NEUTRAL)
        {

        }
        // Affiliation Player
        else if (m_ownAffiliation == CubeEntityState.s_AFFILIATION_PLAYER)
        {
            // Player Active
            if (m_ownState == CubeEntityState.s_STATE_ACTIVE)
            {
                if (m_colliderAffiliation == CubeEntityState.s_AFFILIATION_ENEMY_1)
                {
                    if (m_colliderState == CubeEntityState.s_STATE_ACTIVE)
                    {
                        if (evaluateChargeDissolve(collider.GetComponent<CubeEntityCharge>()))
                        {
                            //setToActiveNeutral(gameObject);
                            // explosion
                            //GameObject explosion = EffectManager.createSingleEffect(EffectManager.s_smallRedFireExplosion);
                            //explosion.transform.position = collider.gameObject.transform.position;
                        }
                        if (collider.GetComponent<CubeEntityCharge>().evaluateChargeDissolve(this))
                            ;//setToActiveNeutral(collider.gameObject);
                    }
                    else if (m_colliderState == CubeEntityState.s_STATE_ATTACHED)
                    {
                        if (evaluateChargeDissolve(collider.GetComponent<CubeEntityCharge>()))
                            ;// setToActiveNeutral(gameObject);
                        if (collider.GetComponent<CubeEntityCharge>().evaluateChargeDissolve(this))
                            ;// loseLife(collider.gameObject, 1);

                        // explosion
                        //GameObject explosion = EffectManager.createSingleEffect(EffectManager.s_smallBluePlasmaExplosion);
                        //explosion.transform.position = collider.gameObject.transform.position;
                    }
                    else if (m_colliderState == CubeEntityState.s_STATE_CORE)
                    {
                        if (collider.GetComponent<CubeEntityCharge>().evaluateChargeDissolve(this))
                            ;// loseLife(collider.gameObject, 3);
                        if (evaluateChargeDissolve(collider.GetComponent<CubeEntityCharge>()))
                            ;// setToActiveNeutral(gameObject);

                        // explosion
                        //GameObject explosion = EffectManager.createSingleEffect(EffectManager.s_smallRedFireExplosion); ;
                        //explosion.transform.position = collider.gameObject.transform.position;
                    }
                }
            }

            // Player Core
            if (m_ownState == CubeEntityState.s_STATE_CORE)
            {
                if (m_colliderAffiliation == CubeEntityState.s_AFFILIATION_ENEMY_1)
                {
                    collider.GetComponent<CubeEntityCharge>().evaluateChargeDissolve(this);
                    //loseLife(gameObject, 1);

                    if (collider.GetComponent<CubeEntityState>().m_state == CubeEntityState.s_STATE_ACTIVE)
                        ;// setToActiveNeutral(collider.gameObject);

                    // explosion
                    //GameObject explosion = EffectManager.createSingleEffect(EffectManager.s_smallRedFireExplosion); ;
                    //explosion.transform.position = collider.gameObject.transform.position;
                }
            }
        }
        // Affiliation Player Ally
        else if (m_ownAffiliation == CubeEntityState.s_AFFILIATION_PLAYER_ALLY)
        {
            // Player Ally Active
            if (m_ownState == CubeEntityState.s_STATE_ACTIVE)
            {
                if (m_colliderAffiliation == CubeEntityState.s_AFFILIATION_ENEMY_1)
                {
                    if (m_colliderState == CubeEntityState.s_STATE_ACTIVE)
                    {
                        if (evaluateChargeDissolve(collider.GetComponent<CubeEntityCharge>()))
                            ;// setToActiveNeutral(gameObject);
                        if (collider.GetComponent<CubeEntityCharge>().evaluateChargeDissolve(this))
                            ;// setToActiveNeutral(collider.gameObject);
                    }
                    else if (m_colliderState == CubeEntityState.s_STATE_ATTACHED)
                    {
                        if (collider.GetComponent<CubeEntityCharge>().evaluateChargeDissolve(this))
                            ;//loseLife(collider.gameObject, 1);
                        if (evaluateChargeDissolve(collider.GetComponent<CubeEntityCharge>()))
                            ;//setToActiveNeutral(gameObject);

                        // explosion
                        //GameObject explosion = EffectManager.createSingleEffect(EffectManager.s_smallRedFireExplosion);
                        //explosion.transform.position = collider.gameObject.transform.position;
                    }
                    else if (m_colliderState == CubeEntityState.s_STATE_CORE)
                    {
                        //loseLife(collider.gameObject, 3);
                        if (evaluateChargeDissolve(collider.GetComponent<CubeEntityCharge>()))
                            ;// setToActiveNeutral(gameObject);

                        // explosion
                        //GameObject explosion = EffectManager.createSingleEffect(EffectManager.s_smallRedFireExplosion);
                        //explosion.transform.position = collider.gameObject.transform.position;
                    }
                }
            }

            // Player Ally Core
            if (m_ownState == CubeEntityState.s_STATE_CORE)
            {
                if (m_colliderAffiliation == CubeEntityState.s_AFFILIATION_ENEMY_1)
                {
                    //loseLife(gameObject, 1);

                    if (collider.GetComponent<CubeEntityCharge>().evaluateChargeDissolve(this) && collider.GetComponent<CubeEntityState>().m_state == CubeEntityState.s_STATE_ACTIVE)
                        ;// setToActiveNeutral(collider.gameObject);

                    // explosion
                    //GameObject explosion = EffectManager.createSingleEffect(EffectManager.s_smallRedFireExplosion);
                    //explosion.transform.position = collider.gameObject.transform.position;
                }
            }
        }
        // Affiliation Enemy_1
        else if (m_ownAffiliation == CubeEntityState.s_AFFILIATION_ENEMY_1)
        {
            // Enemy_1 Active
            if (m_ownState == CubeEntityState.s_STATE_ACTIVE)
            {
                if (m_colliderAffiliation == CubeEntityState.s_AFFILIATION_PLAYER || m_colliderAffiliation == CubeEntityState.s_AFFILIATION_PLAYER_ALLY)
                {
                    if (m_colliderState == CubeEntityState.s_STATE_ACTIVE)
                    {
                        if (evaluateChargeDissolve(collider.GetComponent<CubeEntityCharge>()))
                            ;// setToActiveNeutral(gameObject);
                        if (collider.GetComponent<CubeEntityCharge>().evaluateChargeDissolve(this))
                            ;// setToActiveNeutral(collider.gameObject);
                    }

                    else if (m_colliderState == CubeEntityState.s_STATE_CORE)
                    {
                        //loseLife(gameObject, 3);
                        if (collider.GetComponent<CubeEntityCharge>().evaluateChargeDissolve(this))
                            ;// setToActiveNeutral(collider.gameObject);
                    }
                }
            }

            // Enemy_1 Attached
            if (m_ownState == CubeEntityState.s_STATE_ATTACHED)
            {
                if ((m_colliderAffiliation == CubeEntityState.s_AFFILIATION_PLAYER || m_colliderAffiliation == CubeEntityState.s_AFFILIATION_PLAYER_ALLY) && collider.GetComponent<CubeEntityState>().m_state == CubeEntityState.s_STATE_ACTIVE)
                {
                    if (m_colliderState == CubeEntityState.s_STATE_ACTIVE)
                    {
                        if (evaluateChargeDissolve(collider.GetComponent<CubeEntityCharge>()))
                            ;// loseLife(gameObject, 1);
                        if (collider.GetComponent<CubeEntityCharge>().evaluateChargeDissolve(this))
                            ;// setToActiveNeutral(collider.gameObject);

                        // explosion
                        //GameObject explosion = EffectManager.createSingleEffect(EffectManager.s_smallBluePlasmaExplosion); ;
                        //explosion.transform.position = collider.gameObject.transform.position;
                    }
                }
            }

            // Enemy_1 Core
            if (m_ownState == CubeEntityState.s_STATE_CORE)
            {
                if ((m_colliderAffiliation == CubeEntityState.s_AFFILIATION_PLAYER || m_colliderAffiliation == CubeEntityState.s_AFFILIATION_PLAYER_ALLY) && collider.GetComponent<CubeEntityState>().m_state == CubeEntityState.s_STATE_ACTIVE)
                {
                    //loseLife(gameObject, 1);
                    if (collider.GetComponent<CubeEntityCharge>().evaluateChargeDissolve(this))
                        ;// setToActiveNeutral(collider.gameObject);

                    // explosion
                    //GameObject explosion = EffectManager.createSingleEffect(EffectManager.s_smallRedFireExplosion);
                    //explosion.transform.position = collider.gameObject.transform.position;
                }
            }
        }
        else
        {
            Debug.Log("Warning: Something probably went wrong!");
        }
        */
    }


    // effects
    void loseLife(GameObject victim, int damage)
    {
        //CubeEntitySoundInstance soundInstance = SoundManager.addSoundPlayerCubeHit(victim.transform.position);
        //soundInstance.m_audioSource.volume *= 0.2f;

        if (victim.GetComponent<PlayerEntityLifeSystem>() != null)
        {
            victim.GetComponent<PlayerEntityLifeSystem>().loseHp(damage);
        }
        else if (victim.GetComponent<CubeEntityAttached>() != null)
        {
            if (victim.GetComponent<CubeEntityAttached>().m_attachedToGameObject != null)
            {
                if (victim.GetComponent<CubeEntityAttached>().m_attachedToGameObject.GetComponent<MonsterEntityBase>() != null)
                {
                    victim.GetComponent<CubeEntityAttached>().m_attachedToGameObject.GetComponent<MonsterEntityBase>().loseHp(1, this.gameObject);
                }
            }
        }
    }
    void loseLife(int damage)
    {
        //CubeEntitySoundInstance soundInstance = SoundManager.addSoundPlayerCubeHit(transform.position);
        //soundInstance.m_audioSource.volume *= 0.2f;

        PlayerEntityLifeSystem lifeScript = GetComponent<PlayerEntityLifeSystem>();
        CubeEntityAttached attachedScript = GetComponent<CubeEntityAttached>();

        if (lifeScript != null)
        {
            lifeScript.loseHp(damage);
        }
        else if (attachedScript != null)
        {
            if (attachedScript.m_attachedToGameObject != null)
            {
                MonsterEntityBase monsterScript = attachedScript.m_attachedToGameObject.GetComponent<MonsterEntityBase>();
                if (monsterScript != null)
                {
                    monsterScript.loseHp(damage, this.gameObject);
                }
            }
        }
    }
    void reduceMaxGrabbed(int number)
    {
        CubeEntityAttached attachedScript = GetComponent<CubeEntityAttached>();
        if (attachedScript != null)
        {
            if (attachedScript.m_attachedToGameObject != null)
            {
                AttachSystemBase attachScriptScript = attachedScript.m_attachedToGameObject.GetComponent<AttachSystemBase>();
                if (attachScriptScript != null)
                {
                    attachScriptScript.reduceMaxGrabbed(number);
                }
            }
        }
    }
    void setToActiveNeutral(GameObject o)
    {
        o.GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activeNeutralPrefab);
    }
    void setToActiveNeutral()
    {
        GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activeNeutralPrefab);
    }
    void setToInactive()
    {
        GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_inactivePrefab);
    }
    
    // factor
    public void applyFactor(float factor)
    {
        m_maxCharge *= factor;
        m_minCharge *= factor;
        m_chargePower *= factor;
    }
    public void onCopyValues(ICopyValues copiable)
    {
        m_thisStateScript = GetComponent<CubeEntityState>();
        m_ownState = m_thisStateScript.m_state;
        m_ownAffiliation = m_thisStateScript.m_affiliation;

        GameObject o = ((MonoBehaviour)copiable).gameObject;
        if (o == null || o.GetComponent<CubeEntityCharge>() == null)
        {
            //Debug.Log("Oops!");
            return;
        }
        CubeEntityCharge prefabScript = o.GetComponent<CubeEntityCharge>();

        m_chargeResistance = prefabScript.m_chargeResistance;

        m_maxCharge = prefabScript.m_maxCharge;
        m_minCharge = prefabScript.m_minCharge;
        m_chargePower = prefabScript.m_chargePower;

        m_outOfChargeSetToInactive = prefabScript.m_outOfChargeSetToInactive;
        m_outOfChargeSetToActiveNeutral = prefabScript.m_outOfChargeSetToActiveNeutral;
        m_outOfChargeLoseLife = prefabScript.m_outOfChargeLoseLife;
        m_outOfChargeReduceMaxGrabbed = prefabScript.m_outOfChargeReduceMaxGrabbed;
        m_refillChargeWhenOut = prefabScript.m_refillChargeWhenOut;



        initializeScript();
    }
    /*
    // copy
    public void setValues(GameObject prefab)
    {
        if (prefab == null || prefab.GetComponent<CubeEntityCharge>() == null)
        {
            //Debug.Log("Oops!");
            return;
        }
        CubeEntityCharge prefabScript = prefab.GetComponent<CubeEntityCharge>();

        m_chargeResistance = prefabScript.m_chargeResistance;

        m_maxCharge = prefabScript.m_maxCharge;
        m_minCharge = prefabScript.m_minCharge;
        m_chargePower = prefabScript.m_chargePower;

        m_outOfChargeSetToActiveNeutral = prefabScript.m_outOfChargeSetToActiveNeutral;
        m_outOfChargeLoseLife = prefabScript.m_outOfChargeLoseLife;
        m_refillChargeWhenOut = prefabScript.m_refillChargeWhenOut;

        initializeScript();
    }
    */
}
