using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySystemBase : MonoBehaviour
{
    public static string s_createChildName = "OnCopyCreate";
    public List<GameObject> m_childObjects;

    private void Start()
    {
        //m_childObjects = new List<GameObject>();
    }
    public Component copyPasteComponent(ICopiable copiable, bool forceCopy)
    {
        if(!forceCopy && copiable is EntityCopiableAbstract && ((EntityCopiableAbstract)copiable).m_prohibitCopy != 0)
        {
            Debug.Log("Aborted: This should not happen!");
            return null;
        }

        System.Type type = copiable.GetType();
        Component copy = gameObject.AddComponent(type);

        ((MonoBehaviour)copy).enabled = ((MonoBehaviour)copiable).enabled;
        ((ICopiable)copy).onCopy(copiable);

        if(copiable is IPostCopy)
        {
            ((IPostCopy)copy).onPostCopy();
        }

        return copy;
    }

    public void copyPasteComponents(GameObject prefab)
    {
        ICopiable[] copyScripts = prefab.GetComponents<ICopiable>();

        // copy
        foreach (ICopiable copyScript in copyScripts)
        {
            if (copyScript is EntityCopiableAbstract && ((EntityCopiableAbstract)copyScript).m_prohibitCopy != 0)
            {
                //Debug.Log("Aborted: This should not happen (maybe)!");
                continue;
            }

            System.Type type = copyScript.GetType();
            Component copy = gameObject.AddComponent(type);

            ((MonoBehaviour)copy).enabled = ((MonoBehaviour)copyScript).enabled;
            ((ICopiable)copy).onCopy(copyScript);
        }

        ICopyValues[] copyValues = GetComponents<ICopyValues>();
        foreach (ICopyValues script in copyValues)
        {
            foreach (ICopyValues script2 in prefab.GetComponents<ICopyValues>())                        // TODO: Performance
            {
                if (script.GetType().Equals(script2.GetType()))
                {
                    script.onCopyValues(script2);
                }
            }
        }

        // post copy
        IPostCopy[] newScripts = GetComponents<IPostCopy>();
        foreach (IPostCopy script in newScripts)
        {
            script.onPostCopy();
        }

        // children
        //killChildren();
        instantiateChildren(prefab);
    }
    public void killChildren()
    {
        for(int i = m_childObjects.Count - 1; i >= 0; i--)
        {
            
            GameObject child = m_childObjects[i];
            m_childObjects.RemoveAt(i);

            Transform[] transforms = child.GetComponentsInChildren<Transform>();
            foreach (Transform transform in transforms)
            {
                foreach (IStateOnStateChange script in transform.GetComponents<IStateOnStateChange>())
                {
                    script.onStateExit();
                }
                foreach (IPrepareRemoveOnStateChange script in transform.GetComponents<IPrepareRemoveOnStateChange>())
                {
                    script.onStateChangePrepareRemove();
                }
                foreach (IRemoveOnStateChange script in transform.GetComponents<IRemoveOnStateChange>())
                {
                    script.onRemove();
                }
            }

            Destroy(child);
        }
    }
    void instantiateChildren(GameObject prefab)
    {
        foreach(Transform prefabChild in prefab.transform)
        {
            if(prefabChild.name == s_createChildName)
            {
                GameObject newChild = Instantiate(prefabChild.gameObject, transform);
                newChild.name = prefabChild.name;
                if (m_childObjects == null)
                    m_childObjects = new List<GameObject>();
                m_childObjects.Add(newChild);
                
                copyPasteChildren(newChild);
            }
        }
    }

    void copyPasteChildren(GameObject parentGameObject)
    {
        Transform[] transforms = parentGameObject.GetComponentsInChildren<Transform>();
        foreach(Transform transform in transforms)
        {
            foreach (IPostCopy script in transform.GetComponents<IPostCopy>())
            {
                script.onPostCopy();
            }
        }

        /*
        foreach(Transform t in parentGameObject.transform)
        {
            foreach (IPostCopy script in newChild.GetComponents<IPostCopy>())
            {
                script.onPostCopy();
            }
        }*/
    }

    /*
    public EntityCopiableAbstract copyPasteScript(EntityCopiableAbstract copyScript)
    {
        if(copyScript == null)
        {
            Debug.Log("Aborted: copyScript was null!");
        }

        System.Type type = copyScript.GetType();
        Component copy = gameObject.AddComponent(type);

        ((EntityCopiableAbstract)copy).enabled = copyScript.enabled;
        ((EntityCopiableAbstract)copy).pasteScript(copyScript);
        ((EntityCopiableAbstract)copy).assignScripts();

        return (EntityCopiableAbstract)copy;
    }
    */


    // OLD
    /*public void copyPasteComponents(GameObject prefab)
    {
        EntityCopiableAbstract[] copyScripts = prefab.GetComponents<EntityCopiableAbstract>();
        //Debug.Log(prefab.GetComponents<MonsterEntityAbstractBase>().Lengh) ;
        List<EntityCopiableAbstract> newScripts = new List<EntityCopiableAbstract>();

        foreach (EntityCopiableAbstract copyScript in copyScripts)
        {
            System.Type type = copyScript.GetType();
            Component copy = gameObject.AddComponent(type);

            ((EntityCopiableAbstract)copy).enabled = copyScript.enabled;
            ((EntityCopiableAbstract)copy).pasteScript(copyScript);
            newScripts.Add((EntityCopiableAbstract)copy);
        }
        foreach (EntityCopiableAbstract script in newScripts)
        {
            script.assignScripts();
        }
        killChildren();
        instantiateChildren(prefab);
    }
    void killChildren()
    {
        for(int i = m_childObjects.Count - 1; i >= 0; i--)
        {
            
            GameObject child = m_childObjects[i];

            foreach (EntityCopiableAbstract script in child.GetComponents<EntityCopiableAbstract>())
            {
                script.prepareDestroyScript();
            }
            m_childObjects.RemoveAt(i);
            Destroy(child);
        }
    }
    void instantiateChildren(GameObject prefab)
    {
        foreach(Transform prefabChild in prefab.transform)
        {
            //if(prefabChild.name == s_createChildName)
            {
                GameObject newChild = Instantiate(prefabChild.gameObject, transform);
                if (m_childObjects == null)
                    m_childObjects = new List<GameObject>();
                m_childObjects.Add(newChild);
                //foreach(Transform t in newChild.transform)
                {
                    foreach (EntityCopiableAbstract script in newChild.GetComponents<EntityCopiableAbstract>())
                    {
                        script.assignScripts();
                    }
                }
            }
        }
    }
    /*
    public EntityCopiableAbstract copyPasteScript(EntityCopiableAbstract copyScript)
    {
        if(copyScript == null)
        {
            Debug.Log("Aborted: copyScript was null!");
        }

        System.Type type = copyScript.GetType();
        Component copy = gameObject.AddComponent(type);

        ((EntityCopiableAbstract)copy).enabled = copyScript.enabled;
        ((EntityCopiableAbstract)copy).pasteScript(copyScript);
        ((EntityCopiableAbstract)copy).assignScripts();

        return (EntityCopiableAbstract)copy;
    }
    */
}
