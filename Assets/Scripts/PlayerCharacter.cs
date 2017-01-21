using UnityEngine;
using Assets.Scripts;
using System;

public class PlayerCharacter : MonoBehaviour
{
    private enum States
    {
        Standing,
        ChangingLane
    };

    public float LaneChangeSpeed = 5f;
    public PlayerWaveDetector WaveDetectorPrefab;

    private Rigidbody2D playerBody;
    private PlayerWaveDetector waveDetector;

    private float _targetY;
    private float _animationTime;

    private FSM fsm = new FSM();

    public Action<Wave> EnteredWave;

    public void ResetWave()
    {
        waveDetector.ResetWave();
    }

    void Start ()
    {
        playerBody = GetComponent<Rigidbody2D>();

        waveDetector = Instantiate(WaveDetectorPrefab, transform.position, Quaternion.identity);
        waveDetector.PlayerBody = playerBody;
        waveDetector.EnteredWave += onEnteredWave;
    }

    void Awake()
    {
        fsm.AddState (StateStanding);
        fsm.AddState (StateChangingLane);

        fsm.ChangeState ((int)States.Standing);
    }

    void Update()
    {
        fsm.Update (Time.deltaTime);
    }

    void FixedUpdate()
    {
        fsm.FixedUpdate(Time.deltaTime);

        waveDetector.PositionX = playerBody.position.x;
    }

    public bool CanChangeLane
    {
        get
        {
            return fsm.CurrentState == (int)States.Standing;
        }
    }

    public void ChangeLane(float targetY, bool jump)
    {
        Debug.Assert (CanChangeLane);

        _targetY = targetY;
        fsm.ChangeState ((int)States.ChangingLane);
    }

    void StateStanding(StateMethod method, float deltaTime)
    {
    }

    void StateChangingLane(StateMethod method, float deltaTime)
    {
        switch(method)
        {
        case StateMethod.Enter:
            _animationTime = 0;
            waveDetector.enabled = false;
            break;
        case StateMethod.Update:
            LerpToTargetY (deltaTime, LaneChangeSpeed);
            break;
        case StateMethod.Exit:
            waveDetector.enabled = true;
            break;
        }
    }

    void LerpToTargetY(float deltaTime, float speed)
    {
        Vector3 targetPosition = playerBody.position;
        targetPosition.y = _targetY;

        if(speed > 0)
        {
            _animationTime += deltaTime;
            playerBody.position = Vector3.Lerp (playerBody.position, targetPosition, _animationTime * speed);
        }
        else
        {
            playerBody.position = targetPosition;
            waveDetector.Position = targetPosition;
        }

        if(Mathf.Abs(playerBody.position.y - targetPosition.y) < Mathf.Epsilon)
        {
            waveDetector.Position = targetPosition;

            fsm.ChangeState ((int)States.Standing);
        }
    }

    private void onEnteredWave(Wave wave)
    {
        var handler = EnteredWave;
        if (handler != null)
        {
            handler.Invoke(wave);
        }
    }
}

