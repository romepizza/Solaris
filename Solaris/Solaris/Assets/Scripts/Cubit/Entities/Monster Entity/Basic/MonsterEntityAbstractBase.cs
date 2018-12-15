using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntityAbstractBase : EntityCopiableAbstract, IStateOnStateChange
{
    /*
    // abstract
    public override void onCopy(EntityCopiableAbstract baseScript)
    {

    }
    public override void prepareDestroyScript()
    {
        Destroy(this);
    }
    public override void onPostCopy()
    {

    }
    */

    public void onStateEnter()
    {
        GetComponent<CubeEntityTargetManager>().updateTarget();
    }
    public void onStateExit()
    {

    }
}
