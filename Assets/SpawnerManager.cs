using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public System.Random r = new System.Random();
    public GameManager GameManager;
    public List<Wave> PositiveWaveTemplates;
    public List<Wave> NegativeWaveTemplates;
    public List<LethalEnemy> WindowsLogoTemplates;

    Level currentLevel;

    // Use this for initialization
    public int GetTargetScore()
    {
        return currentLevel.targetScore;
    }

    void Start()
    {

    }

    public void NextLevel()
    {
        currentLevel.NextLevel();
        GameManager.GameUI.LevelText.text = "Level " + currentLevel.LevelNumber.ToString();
    }

    public void Init()
    {
        currentLevel = new Level(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentLevel != null)
        {
            List<ObjectInstance> objs = currentLevel.GetObjectsToSpawn(Time.deltaTime);

            foreach (ObjectInstance inst in objs)
            {
                if (inst is PositiveWave)
                {
                    int index = r.Next(0, PositiveWaveTemplates.Count);
                    Wave obj = Instantiate(PositiveWaveTemplates[index], GameManager.Lanes[inst.LaneIndex].transform, false);
                    obj.enabled = false;
                    obj.Source = inst;
                    obj.enabled = true;
                }

                if (inst is NegativeWave)
                {
                    int index = r.Next(0, NegativeWaveTemplates.Count);
                    Wave obj = Instantiate(NegativeWaveTemplates[index], GameManager.Lanes[inst.LaneIndex].transform, false);
                    obj.MoveSpeed = obj.MoveSpeed * -1;
                    Vector3 t = obj.transform.position;
                    t.x *= -1;
                    obj.transform.position = t;
                    obj.enabled = false;
                    obj.Source = inst;
                    obj.enabled = true;
                }

                if (inst is FireWall)
                {
                    int index = r.Next(0, NegativeWaveTemplates.Count);
                    Wave obj = Instantiate(NegativeWaveTemplates[index], GameManager.Lanes[inst.LaneIndex].transform, false);
                    obj.MoveSpeed = obj.MoveSpeed * -1;
                    Vector3 t = obj.transform.position;
                    t.x *= -1;
                    obj.transform.position = t;
                    obj.enabled = false;
                    obj.Source = inst;
                    obj.enabled = true;
                }

                if (inst is WindowsLogo)
                {
                    int spawnIndex = r.Next(0, WindowsLogoTemplates.Count);
                    LethalEnemy obj = Instantiate(WindowsLogoTemplates[spawnIndex], GameManager.Lanes[inst.LaneIndex].transform, false);
                }
            }
        }
    }
}
