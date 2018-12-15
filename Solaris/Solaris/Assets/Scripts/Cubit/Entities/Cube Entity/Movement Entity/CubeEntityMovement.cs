using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntityMovement : MonoBehaviour, IRemoveOnStateChange
{
    [Header("----- SETTINGS -----")]
    [Header("----- DEBUG -----")]
    //public CubeEntitySystem m_entitySystemScript;
    public List<CubeEntityMovementAbstract> m_movementComponents;



    // --- Add Components ---
    public CubeEntityMovementAbstract addMovementComponent(CubeEntityMovementAbstract copyScript, GameObject target, Vector3 targetPosition)
    {
        if (copyScript == null)
        {
            Debug.Log("Aborted: copyScript was null!");
        }
        System.Type type = copyScript.GetType();
        Component copy = gameObject.AddComponent(type);

        ((CubeEntityMovementAbstract)copy).enabled = true;
        ((CubeEntityMovementAbstract)copy).pasteScript(copyScript, target, targetPosition);
        if(copy is IPostCopy)
            ((IPostCopy)copy).onPostCopy();
        ((CubeEntityMovementAbstract)copy).m_useThis = true;

        return (CubeEntityMovementAbstract)copy;
    }

    // --- Remove Components ---
    public void removeComponents(System.Type type)
    {
        System.Type actualType = type;
        if (type == null)
        {
            actualType = typeof(CubeEntityMovementAbstract);
        }

        int i = 0;
        Component component;
        List<IRemoveOnStateChange> removeList = new List<IRemoveOnStateChange>();
        do
        {
            component = GetComponent(actualType) as Component;
            if (component == null)
            {
                break;
            }
            CubeEntityMovementAbstract movementComponent = (CubeEntityMovementAbstract)component;
            if (m_movementComponents.Contains(movementComponent))
                m_movementComponents.Remove(movementComponent);
            if(movementComponent is IPrepareRemoveOnStateChange)
                ((IPrepareRemoveOnStateChange)movementComponent).onStateChangePrepareRemove();
            if (movementComponent is IRemoveOnStateChange)
                removeList.Add((IRemoveOnStateChange)movementComponent);
            i++;
        } while (component == null && i < 20);

        if (removeList.Count > 0)
        {
            for (int j = removeList.Count - 1; i >= 0; i--)
            {
                removeList[j].onRemove();
            }
        }

        if (i >= 15)
        {
            Debug.Log("Warning: >15 movementscripts were attached");
        }
    }

    // interfaces
    public void onRemove()
    {
        //removeComponents(typeof(CubeEntityMovementAbstract));
    }
    /*
    public void removeComponent(CubeEntityMovementAbstract removeScript)
    {
        if (m_movementComponents.Contains(removeScript))
            m_movementComponents.Remove(removeScript);
        if(removeScript is IPrepareRemoveOnStateChange)
            ((IPrepareRemoveOnStateChange)removeScript).onStateChangePrepareRemove();
    }
    
    // Add Movement Components
    public CubeEntityMovementAcceleration addAccelerationComponent(Vector3 targetPoint, float duration, float power, float maxSpeed)
    {
        CubeEntityMovementAcceleration tmp = gameObject.AddComponent<CubeEntityMovementAcceleration>();
        //tmp.m_entitySystemScript = m_entitySystemScript;

        tmp.m_targetDirection = targetPoint-transform.position;
        tmp.m_targetPoint = targetPoint;
        tmp.m_duration = duration;
        tmp.m_durationEndTime = duration + Time.time;
        tmp.m_power = power;
        tmp.m_maxSpeed = maxSpeed;
        tmp.m_movementScript = this;

        m_movementComponents.Add(tmp);

        return tmp;
    }
    public CubeEntityMovementFollowPoint addFollowPointComponent(Vector3 targetPoint, float duration, float power, float maxSpeed)
    {
        CubeEntityMovementFollowPoint tmp = gameObject.AddComponent<CubeEntityMovementFollowPoint>();

        tmp.m_targetPoint = targetPoint;
        tmp.m_power = power;
        tmp.m_maxSpeed = maxSpeed;
        tmp.m_duration = duration;
        tmp.m_durationEndTime = duration + Time.time;
        //tmp.m_useSmoothArrival = smoothArrival;
        tmp.m_movementScript = this;

        m_movementComponents.Add(tmp);

        return tmp;
    }
    public CubeEntityMovementHoming addHomingComponent(GameObject target)
    {
        CubeEntityMovementHoming tmp = gameObject.AddComponent<CubeEntityMovementHoming>();

        /*
        tmp.m_accelerationPower = ;
        tmp.m_maxSpeed = ;
        tmp.m_deviationPower = ;
        tmp.m_duration = ;
        tmp.m_maxAngle = ;
        tmp.m_collisionSpeed = ;

    tmp.m_target = target;
        tmp.m_movementScript = this;
        tmp.initializeStuff();

        m_movementComponents.Add(tmp);

        return tmp;
    }
*/
}
