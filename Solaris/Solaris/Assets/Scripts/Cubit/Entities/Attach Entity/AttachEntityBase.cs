using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttachEntityBase : EntityCopiableAbstract
{
	
    public abstract bool addAgent(GameObject agent);
    public abstract void removeAgent(GameObject agent);
    public abstract void setValuesByPrefab(GameObject prefab);
}
