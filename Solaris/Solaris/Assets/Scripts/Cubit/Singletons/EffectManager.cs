using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public GameObject m_smallBluePlasmaExplosion;
    public static GameObject s_smallBluePlasmaExplosion;

    public GameObject m_smallRedFireExplosion;
    public static GameObject s_smallRedFireExplosion;


	// Use this for initialization
	void Start ()
    {
        initializeStuff();
	}

    void initializeStuff()
    {
        s_smallRedFireExplosion = m_smallRedFireExplosion;
        s_smallBluePlasmaExplosion = m_smallBluePlasmaExplosion;
    }

    public static GameObject createSingleEffect(GameObject effectPrefab)
    {
        if(effectPrefab == null)
        {
            Debug.Log("Aborted: effect prefab was null!");
            return null;
        }
        GameObject effect = Instantiate(effectPrefab);
        return effect;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
