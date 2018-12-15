using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateOnStateChange
{
    void onStateEnter();
    void onStateExit();
}
