﻿using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : BaseSingleton<GameManager> , EventListener
{
    public ScrollHorizontal ScrollingBackground;
    public GameOverView GameOverScreenPrefab;
    public GameUI GameUI;
    public SpawnerManager Spawner;
    public List<GameObject> Lanes;
    public PlayerCharacter PlayerPrefab;
    public float StartScrollSpeed = 0.25f;

    public float PlayerOffset = -2.25f;
    public float ScrollIncrement = 0.25f;

    public string PlayerName;

    private int _level = 0;
    private float previousVerticalAxis = 0f;

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
            if(_score >= Spawner.GetTargetScore())
            {
                Spawner.NextLevel();
            }
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
            ScrollingBackground.ScrollSpeed = StartScrollSpeed + ScrollIncrement * value;
        }
    }

    public PlayerCharacter PlayerCharacter { get; private set; }

    public int CurrentLane { get; private set; }

    private FSM fsm = new FSM();

    private enum States
    {
        Title,
        Start,
        Active,
        ChangingLanes,
        GameOver
    };

    void Awake()
    {
        fsm.AddState(StateTitle);
        fsm.AddState(StateStart);
        fsm.AddState(StateActive);
        fsm.AddState(StateChangeLane);
        fsm.AddState(StateGameOver);
    }

    void Start()
    {
        fsm.ChangeState((int)States.Title);
        HighScoreTool.Instance.StartThread();
        EventManager.Instance.Register(this);
    }

    void Reset()
    {
        if (PlayerCharacter != null)
        {
            DestroyImmediate(PlayerCharacter.gameObject);
            PlayerCharacter.EnteredWave -= OnPlayerCharacterEnteredWave;
            PlayerCharacter = null;
        }

        _level = 0;
        _multiplier = 0;
        if (ScrollingBackground != null)
        {
            ScrollingBackground.ScrollSpeed = StartScrollSpeed;
        }
        _score = 0;

        GameUI.Reset();

        CurrentLane = -1;
    }

    void Update()
    {
        fsm.Update(Time.deltaTime);
    }

    void FixedUpdate()
    {
        fsm.FixedUpdate (Time.deltaTime);
    }

    public void OnMessage(EventType tp, object param)
    {
        if (tp == EventType.StartGame)
        {
            fsm.ChangeState((int)States.Start);
        }
        if(tp == EventType.GameOver)
        {
            fsm.ChangeState((int)States.GameOver);
            GameOver();
        }
    }

    private void GameOver()
    {
        //Remove
        Score = 5;

        GameOverView gv = Instantiate(GameOverScreenPrefab);
        gv.Fillout(Score);
    }

    private void ChangeLane(int laneIndex, bool jump = false)
    {
        if(PlayerCharacter.CanChangeLane && laneIndex != CurrentLane)
        {
            PlayerCharacter.ChangeLane (Lanes [laneIndex].transform.position.y, jump);
            PlayerCharacter.ResetWave();
            CurrentLane = laneIndex;
        }
    }

    private void StateTitle(StateMethod method, float deltaTime)
    {
        //do nothing
    }

    private void StateStart(StateMethod method, float deltaTime)
    {
        switch(method)
        {
            case StateMethod.Enter:
                Spawner.enabled = true;
                Spawner.Init();
                break;
            case StateMethod.Update:
                {
                    fsm.ChangeState((int)States.Active);
                    break;
                }
            case StateMethod.Exit:
                Spawner.enabled = false;
                break;
        }
    }

    private void StateActive(StateMethod method, float deltaTime)
    {
        switch(method)
        {
            case StateMethod.Enter:
                {
                    Reset();
                    Spawner.enabled = true;
                    
                    Spawner.Init();
                    Score = 0;
                    Multiplier = 1;
                    Level = 1;

                    PlayerName = "MTL";
                    PlayerCharacter = Instantiate(PlayerPrefab, new Vector3(PlayerOffset, 0f), Quaternion.identity);
                    PlayerCharacter.EnteredWave += OnPlayerCharacterEnteredWave;

                    ChangeLane(2);
                    break;
                }
            case StateMethod.Update:
                {
                    float verticalAxis = Input.GetAxis("Vertical");

                    // Up
                    if (Mathf.Approximately(previousVerticalAxis, 0f) && verticalAxis > 0)
                    {
                        ChangeLane(Mathf.Max(0, CurrentLane - 1));
                    }

                    // Down
                    if (Mathf.Approximately(previousVerticalAxis, 0f) &&  verticalAxis < 0)
                    {
                        ChangeLane(Mathf.Min(Lanes.Count - 1, CurrentLane + 1));
                    }

                    previousVerticalAxis = verticalAxis;
                    break;
                }
            case StateMethod.Exit:
                {
                    break;
                }
        }
    }

    private void OnPlayerCharacterEnteredWave(Wave wave)
    {
        if(wave.Source is PositiveWave)
        {
            Multiplier++;
            Score += wave.Source.PointValue;
        }
        else
        {
            Multiplier = 1;
        }
    }

    private void StateChangeLane(StateMethod method, float deltaTime)
    {
        switch(method)
        {
            case StateMethod.Enter:
                {
                    break;
                }
            case StateMethod.Update:
                {
                    break;
                }
            case StateMethod.Exit:
                {
                    break;
                }
        }
    }

    private void StateGameOver(StateMethod method, float deltaTime)
    {
        switch (method)
        {
            case StateMethod.Enter:
                {
                    break;
                }
            case StateMethod.Update:
                {
                    break;
                }
            case StateMethod.Exit:
                {
                    break;
                }
        }
    }
}
