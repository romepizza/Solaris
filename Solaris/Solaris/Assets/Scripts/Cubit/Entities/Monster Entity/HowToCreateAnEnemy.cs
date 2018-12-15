using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToCreateAnEnemy : MonoBehaviour {


    /*
     * 1. Singletons:
     *  CubeEntityPrefabs:
     *      - in CubeEntityPrefabs declare new prefab variables
     *      - Create new needed Prefabs and assign them in the singletons prefab script
     *  CubeEntityMaterials:
     *      - in CubeEntityMaterials declare new material variables
     *      - declare new Methods "create***Materials" and "get***Materials"
     *      - createMaterials: call create***Materials
     *      - getMaterial: call get***Materials
     * 
     * 2. CubeEntityState:
     *      - add s_MONSTER_*** static variable
     * 
     * 3. CubeEntityPrefapSystem:
     *      - create setToPrefab methods
     * 
     * 4. CubeEntitySystem:
     *      - create setToPrefab methods
     *      - setDynamicly methods: add new stuff if needed
     *      
     * 5. CgeMonsterManager
     *      - if(Input.GetKeyUp(KeyCode.*))
     *            create***()
     * 
     * 
     *      
     */
}
