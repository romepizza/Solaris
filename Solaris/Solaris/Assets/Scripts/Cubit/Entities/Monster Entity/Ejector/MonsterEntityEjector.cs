using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntityEjector : MonsterEntityAbstractBase, IRemoveOnStateChange, IPrepareRemoveOnStateChange, ICopiable, IStateOnStateChange
{
    [Header("----- SETTINGS -----")]
    [Header("----- DEBUG -----")]
    [Header("--- (Scripts) ---")]
    public bool placeHolder;

    public void onCopy(ICopiable copiable)
    {

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
