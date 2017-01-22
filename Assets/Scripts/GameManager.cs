using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : BaseSingleton<GameManager>, EventListener
{
    const float GameOverDuration = 2f;

    public ScrollHorizontal UpperPlayfield;
    public ScrollHorizontal ScrollingBackground;
    public GameOverView GameOverScreenPrefab;
    public TitleScreenHandler TitleScreenPrefab;
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
    private float previousTouchAxis = 0f;

    private GameOverView gameOverView;
    private float _gameOverTimer = 0f;
    private TitleScreenHandler titleScreen;

    private VerticalSwipeHelper _verticalSwipeHelper = new VerticalSwipeHelper();

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
            if (_score >= Spawner.GetTargetScore())
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
        Active,
        GameOver,
        HighScore
    };

    void Awake()
    {
        fsm.AddState(StateTitle);
        fsm.AddState(StateActive);
        fsm.AddState(StateGameOver);
        fsm.AddState(StateHighScore);

        EventManager.Instance.Register(this);
    }

    void Start()
    {
        fsm.ChangeState((int)States.Title);

        HighScoreTool.Instance.StartThread();
    }

    void Reset()
    {
        if (PlayerCharacter != null)
        {
            Destroy(PlayerCharacter.gameObject);
            PlayerCharacter.EnteredWave -= OnPlayerCharacterEnteredWave;
            PlayerCharacter.PickPacket -= OnPlayerPickupPacket;
            PlayerCharacter = null;
        }

        _level = 0;
        _multiplier = 0;

        if (ScrollingBackground != null)
        {
            ScrollingBackground.ScrollSpeed = StartScrollSpeed;
        }

        if (UpperPlayfield != null)
        {
            UpperPlayfield.enabled = true;
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
        fsm.FixedUpdate(Time.deltaTime);
    }

    public void OnMessage(EventType eventType, object param)
    {
        switch(eventType)
        {
            case EventType.StartGame:
                fsm.ChangeState((int)States.Active);
                break;
            case EventType.GameOver:
                fsm.ChangeState((int)States.GameOver);
                break;
        }
    }

    private void ChangeLane(int laneIndex, bool jump = false)
    {
        if (PlayerCharacter.CanChangeLane && laneIndex != CurrentLane)
        {
            PlayerCharacter.ChangeLane(Lanes[laneIndex].transform.position.y, jump);
            PlayerCharacter.ResetWave();
            CurrentLane = laneIndex;
        }
    }

    private void OnPlayerPickupPacket(PickupPacket packet)
    {
        Score += packet.PointsToGive;
        Destroy(packet.gameObject);
    }

    private void OnPlayerCharacterEnteredWave(Wave wave)
    {
        if (wave.Source is PositiveWave)
        {
            Multiplier++;
            Score += wave.Source.PointValue;
        }
        else
        {
            Multiplier = 1;
        }
    }

    private void StateTitle(StateMethod method, float deltaTime)
    {
        switch (method)
        {
            case StateMethod.Enter:
                {
                    EventManager.Instance.SendEvent(EventType.PlayMusic, null);
                    Reset();

                    titleScreen = Instantiate(TitleScreenPrefab);
                    break;
                }
            case StateMethod.Update:
                {
                    if (Input.anyKeyDown)
                    {
                        DestroyImmediate(titleScreen.gameObject);
                        titleScreen = null;

                        fsm.ChangeState((int)States.Active);
                        return;
                    }
                    break;
                }
            case StateMethod.Exit:
                {
                    break;
                }
        }
    }

    private bool upPreviouslyKeyDown = false;
    private bool downPreviouslyKeyDown = false;

    private void StateActive(StateMethod method, float deltaTime)
    {
        switch (method)
        {
            case StateMethod.Enter:
                {
                    Spawner.enabled = true;
                    Spawner.Init();

                    Score = 0;
                    Multiplier = 1;
                    Level = 1;

                    PlayerName = "MTL";
                    PlayerCharacter = Instantiate(PlayerPrefab, new Vector3(PlayerOffset, 0f), Quaternion.identity);
                    PlayerCharacter.EnteredWave += OnPlayerCharacterEnteredWave;
                    PlayerCharacter.PickPacket += OnPlayerPickupPacket;

                    ChangeLane(2);

                    _verticalSwipeHelper = new VerticalSwipeHelper ();
                        
                    break;
                 }
            case StateMethod.Update:
                {
                    float verticalAxis = Input.GetAxis("Vertical");
                    var touchAxis = _verticalSwipeHelper.UpdateAxisValue ();

                if(verticalAxis > previousVerticalAxis || touchAxis > previousTouchAxis)
                     {
                    if(previousVerticalAxis < 0 || previousTouchAxis < 0)
                        {
                            downPreviouslyKeyDown = false;
                        }
                        else
                        {
                            if(!upPreviouslyKeyDown)
                            {
                                upPreviouslyKeyDown = true;
                                ChangeLane(Mathf.Max(0, CurrentLane - 1));
                            }
                        }
                    }
                    else
                    {
                    if(verticalAxis < previousVerticalAxis || touchAxis < previousTouchAxis)
                        {
                        if(previousVerticalAxis > 0 || previousTouchAxis > 0)
                            {
                                upPreviouslyKeyDown = false;
                            }
                            else
                            {
                                if(!downPreviouslyKeyDown)
                                {
                                    downPreviouslyKeyDown = true;
                                    ChangeLane(Mathf.Min(Lanes.Count - 1, CurrentLane + 1));
                                }
                            }
                        }
                    }

                previousVerticalAxis = verticalAxis;
                previousTouchAxis = touchAxis;
                    break;
                }
            case StateMethod.Exit:
                {
                    foreach (var lane in Lanes)
                    {
                        var children = lane.GetComponentsInChildren(typeof(MonoBehaviour));

                        foreach (var child in children)
                        {
                            Destroy(child.gameObject);
                        }
                    }

                    if (ScrollingBackground != null)
                    {
                        ScrollingBackground.ScrollSpeed = 0f;
                    }

                    if (UpperPlayfield != null)
                    {
                        UpperPlayfield.enabled = false;
                    }

                    Spawner.enabled = false;
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
                    EventManager.Instance.SendEvent(EventType.StopMusic, null);
                    _gameOverTimer = 0;
                    break;
                }
            case StateMethod.Update:
                {
                    _gameOverTimer += deltaTime;
                    if(_gameOverTimer >= GameOverDuration)
                    {
                        fsm.ChangeState((int)States.HighScore);
                    }
                    break;
                }
            case StateMethod.Exit:
                {
                    break;
                }
        }
    }

    private void StateHighScore(StateMethod method, float deltaTime)
    {
        switch (method) {
        case StateMethod.Enter:
            {
                gameOverView = Instantiate (GameOverScreenPrefab);
                gameOverView.Fillout (Score);
                break;
            }
        case StateMethod.Update:
            {
                if (gameOverView != null && gameOverView.IsDone) {
                    DestroyImmediate (gameOverView.gameObject);
                    gameOverView = null;

                    fsm.ChangeState ((int)States.Title);
                }
                break;
            }
        case StateMethod.Exit:
            {
                break;
            }
        }
    }
}