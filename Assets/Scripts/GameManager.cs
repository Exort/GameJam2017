using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : BaseSingleton<GameManager> , EventListener
{
    public GameOverView GameOverScreenPrefab;
    public GameUI GameUI;
    public SpawnerManager SpawnerManager;
    public List<GameObject> Lanes;
    public PlayerCharacter PlayerPrefab;
    public float PlayerOffset = -2.25f;

    public string PlayerName;

    private int _level = 0;
    public int Level
    {
        get 
        {
            return _level;
        }
        set 
        {
            _level = value;
            GameUI.LevelText.text = "Level " + _level.ToString();
        }
    }

    private long _score = 0;
    public long Score
    {
        get 
        {
            return _score;
        }
        set 
        {
            _score = value;
            GameUI.ScoreText.text = _score.ToString();
        }
    }

    private int _multiplier = 0;
    public int Multiplier
    {
        get 
        {
            return _multiplier;
        }
        set 
        {
            _multiplier = value;
            GameUI.MultiplierText.text = "X" + _multiplier.ToString();
        }
    }

    public PlayerCharacter PlayerCharacter { get; private set; }

    public int CurrentLane { get; private set; }

    void Start()
    {
        Reset ();
        HighScoreTool.Instance.StartThread();
        EventManager.Instance.Register(this);
    }
    public void OnMessage(EventType tp, object param)
    {
        if(tp == EventType.StartGame)
        {
            StartGame();
        }
    }
    private void StartGame()
    {
        Reset ();

        Score = 0;
        Multiplier = 1;
        Level = 1;

        PlayerName = "MTL";
        PlayerCharacter = Instantiate (PlayerPrefab);
        PlayerCharacter.transform.localPosition = new Vector3(PlayerOffset, 0, 0);
        ChangeLane (2);
    }

    private void GameOver()
    {
        //Remove
        Score = 5;
   

        GameOverView gv = Instantiate(GameOverScreenPrefab);
        gv.Fillout(Score);
    }
    
    private void ChangeLane(int laneIndex)
    {
        if(laneIndex != CurrentLane)
        {
            var localPosition = PlayerCharacter.transform.localPosition;
            PlayerCharacter.transform.SetParent (Lanes [laneIndex].transform, false);
            PlayerCharacter.transform.localPosition = localPosition;

            CurrentLane = laneIndex;
        }
    }

    private void Reset()
    {
        if(PlayerCharacter != null)
        {
            DestroyImmediate (PlayerCharacter.gameObject);
            PlayerCharacter = null;
        }

        _level = 0;
        _multiplier = 0;
        _score = 0;

        GameUI.Reset ();

        CurrentLane = -1;
    }

    private void Update()
    {
        //todo use input manager
      
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameOver();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeLane (Mathf.Max (0, CurrentLane - 1));
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeLane (Mathf.Min (Lanes.Count - 1, CurrentLane + 1));
        }
    }
}
