using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour {

    Level currentLevel;
	// Use this for initialization
	void Start () {
		if(currentLevel == null)
        {
            currentLevel = LevelManager.Instance.getNextLevel();
        }
	}
	
	// Update is called once per frame
	void Update () {
        List<ObjectInstance> objs = currentLevel.GetObjectsToSpawn(Time.deltaTime);

        Debug.Log(objs);
	}
}
