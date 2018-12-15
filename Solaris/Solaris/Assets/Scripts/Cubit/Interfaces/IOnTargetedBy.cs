using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnTargetedBy
{
    void onTargetedByActive(CubeEntityTargetManager targetedBy);
    void onTargetedByCore(CubeEntityTargetManager targetedBy);
}
