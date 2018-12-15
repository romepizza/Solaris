using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEntityDrone : MonsterEntityAbstractBase, IRemoveOnStateChange, IPrepareRemoveOnStateChange
{
    public MonsterEntitySkillDrone m_droneSkillScript;

	// Update is called once per frame
	void Update ()
    {
		if(GetComponent<CubeEntityTargetManager>().getTarget() == null || GetComponent<CubeEntityTargetManager>().getTarget().GetComponent<MonsterEntityBase>() == null)
        {
            GetComponent<CubeEntityTargetManager>().updateTarget();
        }
	}


    public void onStateChangePrepareRemove()
    {
        if (m_droneSkillScript != null)
            m_droneSkillScript.destroyDrone(gameObject);
        Constants.getMainCge().GetComponent<CgeMonsterManager>().deregisterEnemy(this);
    }
    public void onRemove()
    {
        Destroy(this);
    }
}
