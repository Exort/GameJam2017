using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour {
    public GameManager GameManager;
    public Wave PositiveWaveTemplate;
    public Wave NegativeWaveTemplate;
    Level currentLevel;
	// Use this for initialization
	void Start () {
		
	}
    public void Init()
    {
        currentLevel = LevelManager.Instance.getNextLevel();
        
    }
    // Update is called once per frame
    void Update () {

        if (currentLevel != null)
        {
            List<ObjectInstance> objs = currentLevel.GetObjectsToSpawn(Time.deltaTime);
            foreach (ObjectInstance inst in objs)
            {
                if (inst.GetType() == typeof(PositiveWave))
                {
                    Wave obj = Instantiate(PositiveWaveTemplate, GameManager.Lanes[inst.LaneIndex].transform,false);

                }
                if (inst.GetType() == typeof(NegativeWave))
                {
                    Wave obj = Instantiate(PositiveWaveTemplate, GameManager.Lanes[inst.LaneIndex].transform,false);

                }
            }
            Debug.Log(objs);
        }
	}
}
