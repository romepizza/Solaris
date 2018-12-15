using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityPlasmaProjectile : MonsterEntityAbstractBase, IRemoveOnStateChange, IPrepareRemoveOnStateChange
{

    [Header("----- SETTINGS -----")]
    [Header("----- DEBUG -----")]
    [Header("--- (Scripts) ---")]
    public bool placeHolder;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject colliderGameObject = collision.gameObject;
        CubeEntityState thisState = GetComponent<CubeEntityState>();
        CubeEntityState colliderState = colliderGameObject.GetComponent<CubeEntityState>();

        if (colliderState != null)
        {
            if (colliderState.m_affiliation != thisState.m_affiliation)
            {
                if (colliderState.m_state == CubeEntityState.s_STATE_ATTACHED || colliderState.m_state == CubeEntityState.s_STATE_CORE)
                {
                    GetComponent<MonsterEntityBase>().die();
                }
            }
        }
    }

    public void onStateChangePrepareRemove()
    {
        Constants.getMainCge().GetComponent<CgeMonsterManager>().deregisterEnemy(this);
    }
    public void onRemove()
    {
        Destroy(this);
    }
}
