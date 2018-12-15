using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CemBoidRuleBase : EntityCopiableAbstract, IRemoveOnStateChange
{
    [Header("------- Base Settings -------")]
    public bool m_useRule = true;

    [Header("------- Base Debug -------")]
    public CemBoidBase m_baseScript;


    public abstract void setValues(CemBoidRuleBase copyScript);
    public abstract void getInformation(List<GameObject> agents);
    public abstract void getInformation();
    public abstract void applyRule(List<GameObject> agents);
    public abstract void applyRule();
    public abstract void onAddAgent(List<GameObject> agents, GameObject agent);
    public abstract void onRemoveAgent(List<GameObject> agents, GameObject agent);

    public void onRemove()
    {
        Destroy(this);
    }
    
}
