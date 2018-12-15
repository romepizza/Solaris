using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntityMorpher : MonsterEntityAbstractBase, IRemoveOnStateChange, IPrepareRemoveOnStateChange, ICopiable
{
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
