using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : BaseSingleton<LevelManager> {

    public int CurrentLevel = 0;
    public Level getNextLevel()
    {
        CurrentLevel++;
        Debug.Log("Now at level " + CurrentLevel);
        Level ret = new Level(CurrentLevel);
        
        return ret;
    }
    
}
