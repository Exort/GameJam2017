﻿using System.Collections;
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
        currentLevel = new Level(1);

        
    }
    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown(KeyCode.U))
        {
            currentLevel.NextLevel();
        }
        if (currentLevel != null)
        {
            List<ObjectInstance> objs = currentLevel.GetObjectsToSpawn(Time.deltaTime);
           
            foreach (ObjectInstance inst in objs)
            {
                if (inst is PositiveWave)
                {
                    Wave obj = Instantiate(PositiveWaveTemplate, GameManager.Lanes[inst.LaneIndex].transform,false);
                    obj.Source = inst;

                }
                if (inst is NegativeWave)
                {
                    Wave obj = Instantiate(PositiveWaveTemplate, GameManager.Lanes[inst.LaneIndex].transform,false);
                    obj.MoveSpeed = obj.MoveSpeed * -1;
                    Vector3 t = obj.transform.position;
                    t.x *= -1;
                    obj.transform.position = t;
                    obj.Source = inst;
                }
                if (inst is FireWall)
                {
                    Wave obj = Instantiate(PositiveWaveTemplate, GameManager.Lanes[inst.LaneIndex].transform, false);
                    obj.MoveSpeed = obj.MoveSpeed * -1;
                    Vector3 t = obj.transform.position;
                    t.x *= -1;
                    obj.transform.position = t;
                    obj.Source = inst;
                }
            }
           
        }
	}
}
