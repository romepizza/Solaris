using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntitySystem : MonoBehaviour
{
    [Header("----- SETTINGS -----")]

    [Header("----- DEBUG -----")]
    public CubeEntityAppearance m_appearanceComponent;
    public CubeEntityCharge m_chargeComponent;
    //public CubeEntityCollision m_collisionComponent;
    public CubeEntityMovement m_movementComponent;
    public CubeEntityState m_stateComponent;
    public CubeEntityTransform m_transformComponent;
    public CubeEntityPrefapSystem m_prefapSystemComponent;


    void Start ()
    {
        addComponentsAtStart();
    }
    



    public void addComponentsAtStart()
    {
        if(GetComponent<CubeEntityAppearance>() == null)
            gameObject.AddComponent<CubeEntityAppearance>();
        m_appearanceComponent = GetComponent<CubeEntityAppearance>();
        //GetComponent<CubeEntityAppearance>().m_entitySystemComponent = this;

        //if(GetComponent<CubeEntityCharge>() == null)
            //gameObject.AddComponent<CubeEntityCharge>();
        //m_cubeEntityChargeComponent = GetComponent<CubeEntityCharge>();
        //GetComponent<CubeEntityCharge>() = this;

        //if(GetComponent<CubeEntityCollision>() == null)
            //gameObject.AddComponent<CubeEntityCollision>();
        //m_collisionComponent = GetComponent<CubeEntityCollision>();
        //GetComponent<CubeEntityCollision>().m_entitySystemScript = this;

        if(GetComponent<CubeEntityMovement>() == null)
           gameObject.AddComponent<CubeEntityMovement>();
        m_movementComponent = GetComponent<CubeEntityMovement>();
        //GetComponent<CubeEntityMovement>().m_entitySystemScript = this;

        if(GetComponent<CubeEntityState>() == null)
            gameObject.AddComponent<CubeEntityState>();
        m_stateComponent = GetComponent<CubeEntityState>();
        GetComponent<CubeEntityState>().m_entitySystemScript = this;

        if(GetComponent<CubeEntityTransform>() == null)
            gameObject.AddComponent<CubeEntityTransform>();
        m_transformComponent = gameObject.GetComponent<CubeEntityTransform>();
        GetComponent<CubeEntityTransform>().m_entitySystemScript = this;

        if(GetComponent<CubeEntityPrefapSystem>() == null)
            gameObject.AddComponent<CubeEntityPrefapSystem>();
        m_prefapSystemComponent = GetComponent<CubeEntityPrefapSystem>();
        GetComponent<CubeEntityPrefapSystem>().m_entitySystemScript = this;
    }

    /*
    // --- Prefabs ---
    // neutral
    public void setToInactive()
    {
        getPrefapSystem().setToInactive();
    }
    public void setToActiveNeutral()
    {
        getPrefapSystem().setToActiveNeutral();
    }
    // player
    public void setToActivePlayer()
    {
        getPrefapSystem().setToActivePlayer();
    }
    public void setToAttachedPlayer(Vector3 targetPoint, float duration, float power, float maxSpeed)
    {
        getMovementComponent().removeComponents(typeof(CubeEntityMovementAbstract));
        getPrefapSystem().setToAttachedPlayer();
    }
    public void setToAttachedPlayer()
    {
        getMovementComponent().removeComponents(typeof(CubeEntityMovementAbstract));
        getPrefapSystem().setToAttachedPlayer();
    }
    // ejector
    public void setToActiveEnemyEjector()
    {
        getPrefapSystem().setToActiveEnemyEjector();
    }
    public void setToAttachedEnemyEjector(Vector3 targetPoint, float duration, float power, float maxSpeed)
    {
        getPrefapSystem().setToAttachedEnemyEjector();
        getMovementComponent().removeComponents(typeof(CubeEntityMovementAbstract));
        getMovementComponent().addFollowPointComponent(targetPoint, duration, power, maxSpeed);
    }
    public void setToAttachedEnemyEjector()
    {
        getMovementComponent().removeComponents(typeof(CubeEntityMovementAbstract));
        getPrefapSystem().setToAttachedEnemyEjector();
    }
    public void setToCoreEjector()
    {
        getMovementComponent().removeComponents(typeof(CubeEntityMovementAbstract));
        getPrefapSystem().setToCoreEjector();
    }
    // worm
    public void setToAttachedEnemyWorm()
    {
        getMovementComponent().removeComponents(typeof(CubeEntityMovementAbstract));
        getPrefapSystem().setToAttachedEnemyWorm();
    }
    public void setToCoreWorm()
    {
        getPrefapSystem().setToCoreWorm();
        getMovementComponent().removeComponents(typeof(CubeEntityMovementAbstract));
    }
    // morpher
    public void setToActiveEnemyMorpher()
    {
        getPrefapSystem().setToActiveEnemyMorpher();
    }
    public void setToAttachedEnemyMorpher()
    {
        getMovementComponent().removeComponents(typeof(CubeEntityMovementAbstract));
        getPrefapSystem().setToAttachedEnemyMorpher();
    }
    public void setToCoreMorpher()
    {
        getMovementComponent().removeComponents(typeof(CubeEntityMovementAbstract));
        getPrefapSystem().setToCoreMorpher();
    }
    */
    // Dynamically
    public void setAttachedDynamicly(CubeEntityState stateScript) // TODO : This is utterly shit
    {
        // player
        if (stateScript.GetComponent<CubeEntityState>().m_type == CubeEntityState.s_TYPE_PLAYER)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_attachedPlayerPrefab);

        // drone
        if (stateScript.GetComponent<CubeEntityState>().m_type == CubeEntityState.s_TYPE_DRONE)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_attachedDronePrefab);

        // ejector
        if (stateScript.GetComponent<CubeEntityState>().m_type == CubeEntityState.s_TYPE_EJECTOR)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_attachedEnemyEjector);

        // worm
        if (stateScript.GetComponent<CubeEntityState>().m_type == CubeEntityState.s_TYPE_WORM)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_attachedEnemyWorm);

        // Morpher
        if (stateScript.GetComponent<CubeEntityState>().m_type == CubeEntityState.s_TYPE_MORPHER)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_attachedEnemyMorpher);

        // swarm
        if (stateScript.GetComponent<CubeEntityState>().m_type == CubeEntityState.s_TYPE_SWARM)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_attachedEnemySwarm);
    }
    public void setActiveDynamicly(CubeEntityState stateScript)
    {
        // player
        if (stateScript.GetComponent<CubeEntityState>().m_type == CubeEntityState.s_TYPE_PLAYER)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activePlayerPrefab);

        // drone
        if (stateScript.GetComponent<CubeEntityState>().m_type == CubeEntityState.s_TYPE_DRONE)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activeDronePrefab);

        // ejector
        if (stateScript.GetComponent<CubeEntityState>().m_type == CubeEntityState.s_TYPE_EJECTOR)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activeEnemyEjector);

        // worm
        if (stateScript.GetComponent<CubeEntityState>().m_type == CubeEntityState.s_TYPE_WORM)
        {
            if(Random.Range(0f, 1f) > 0.5f)
                GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activeEnemyEjector);
            else
                GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activeEnemyMorpher);
        }

        // Morpher
        if (stateScript.GetComponent<CubeEntityState>().m_type == CubeEntityState.s_TYPE_MORPHER)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activeEnemyMorpher);
    }
    public void setActiveDynamicly(int type)
    {
        // player
        if (type == CubeEntityState.s_TYPE_PLAYER)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activePlayerPrefab);

        // drone
        if (type == CubeEntityState.s_TYPE_DRONE)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activeDronePrefab);

        // ejector
        if (type == CubeEntityState.s_TYPE_EJECTOR)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activeEnemyEjector);

        // worm
        if (type == CubeEntityState.s_TYPE_WORM)
        {
            if (Random.Range(0f, 1f) > 0.5f)
                GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activeEnemyEjector);
            else
                GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activeEnemyMorpher);

        }

        // Morpher
        if (type == CubeEntityState.s_TYPE_MORPHER)
            GetComponent<CubeEntityPrefapSystem>().setToPrefab(CubeEntityPrefabs.getInstance().s_activeEnemyMorpher);
    }


    // Setter
    /*
    public bool setAppearanceComponent(GameObject appearanceSettings)
    {
        if (m_appearanceComponent == null)
        {
            if (appearanceSettings.GetComponent<CubeEntityAppearance>() != null)
            {
                gameObject.AddComponent<CubeEntityAppearance>();
                gameObject.GetComponent<CubeEntityAppearance>().m_entitySystemComponent = this;
            }
            m_appearanceComponent = gameObject.GetComponent<CubeEntityAppearance>();
            gameObject.GetComponent<CubeEntityAppearance>().setAppearanceByScript(appearanceSettings);

            return true;
        }
        else
        {
            if (GetComponent<CubeEntityAppearance>() == null)
            {
                Destroy(getAppearanceComponent());
            }
            else
            {
                getAppearanceComponent().setAppearanceByScript(appearanceSettings);
            }
            return false;
        }
    }
    public bool setChargeComponent(GameObject chargeSettings)
    {
        if (m_chargeComponent == null)
        {
            if (chargeSettings.GetComponent<CubeEntityCharge>() == null)
            { 
                gameObject.AddComponent<CubeEntityCharge>();
                m_chargeComponent = gameObject.GetComponent<CubeEntityCharge>();
            }
            // Copy charge settings here

            return true;
        }
        else
        {
            if (chargeSettings.GetComponent<CubeEntityCharge>() == null)
            {

            }
            else
            {
                Destroy(getChargeComponent());
            }
            return false;
        }
    }
    public bool setCollisionComponent(GameObject collisionSettings)
    {
        if (m_collisionComponent == null)
        {
            if (collisionSettings.GetComponent<CubeEntityCollision>() == null)
            {
                gameObject.AddComponent<CubeEntityCollision>();
                gameObject.GetComponent<CubeEntityCollision>().m_entitySystemScript = this;
            }
            m_collisionComponent = gameObject.GetComponent<CubeEntityCollision>();
            // Copy collision settings here

            return true;
        }
        else
        {
            if (collisionSettings.GetComponent<CubeEntityCollision>() == null)
            {

            }
            else
            {
                Destroy(getCollisionComponent());
            }
            return false;
        }
    }
    public bool setMovementComponent(GameObject movementSettings)
    {
        if (m_movementComponent == null)
        {
            if (movementSettings.GetComponent<CubeEntityMovement>() == null)
            {
                gameObject.AddComponent<CubeEntityMovement>();
                m_movementComponent = gameObject.GetComponent<CubeEntityMovement>();
            }
            // Copy movement settings here

            return true;
        }
        else
        {
            if (movementSettings.GetComponent<CubeEntityMovement>() == null)
            {

            }
            else
            {
                Destroy(getMovementComponent());
            }
            return false;
        }
    }
    public bool setStateComponent(GameObject stateSettings)
    {
        if (m_stateComponent == null)
        {
            if (stateSettings.GetComponent<CubeEntityState>() == null)
            {
                gameObject.AddComponent<CubeEntityState>();
                m_stateComponent = gameObject.GetComponent<CubeEntityState>();
            }
            // Copy state settings here

            return true;
        }
        else
        {
            if (stateSettings.GetComponent<CubeEntityState>() == null)
            {

            }
            else
            {
                Destroy(getStateComponent());
            }
            return false;
        }
    }
    public bool setTransformComponent(GameObject transformSettings)
    {
        if (m_transformComponent == null)
        {
            if (transformSettings.GetComponent<CubeEntityTransform>() == null)
            {
                gameObject.AddComponent<CubeEntityTransform>();
                m_transformComponent = gameObject.GetComponent<CubeEntityTransform>();
            }
            // Copy transform settings here

            return true;
        }
        else
        {
            if (transformSettings.GetComponent<CubeEntityTransform>() == null)
            {

            }
            else
            {
                Destroy(getTransformComponent());
            }
            return false;
        }
    }
    public bool setPrefapSystemComponent()
    {
        return false;
    }
    */

    // Getter
    public CubeEntityAppearance getAppearanceComponent()
    {
        return m_appearanceComponent;
    }
    public CubeEntityCharge getChargeComponent()
    {
        return m_chargeComponent;
    }
    public CubeEntityCollision getCollisionComponent()
    {
        return null;
    }
    public CubeEntityMovement getMovementComponent()
    {
        return m_movementComponent;
    }
    public CubeEntityState getStateComponent()
    {
        return m_stateComponent;
    }
    public CubeEntityTransform getTransformComponent()
    {
        return m_transformComponent;
    }
    public CubeEntityPrefapSystem getPrefapSystem()
    {
        return m_prefapSystemComponent;
    }
}
