using UnityEngine;
using Assets.Scripts;
using System;

public class PlayerCharacter : MonoBehaviour
{
    public float PlayerOffset = -7f;
    public float MaxSpeed;
    public float CurrentSpeed;
    public float BaseSpeed;
    public float KillSpeed;
    public float BoostVelocity = 5;
    public float WaterVelocity = 1;
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
        if (waveDetector != null)
        {
            waveDetector.ResetWave();
        }
    }

    void Start ()
    {

        MaxSpeed = 5;
        CurrentSpeed = 0;
        BaseSpeed = 0;
        KillSpeed = -3;

    

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
      

        /*Speed management*/
        //MaxSpeed = 5;
        if(Input.GetKey(KeyCode.Space))
        {
            if(CurrentSpeed < MaxSpeed)
            {
                CurrentSpeed += (Time.deltaTime * BoostVelocity);
                if(CurrentSpeed > MaxSpeed)
                {
                    CurrentSpeed = MaxSpeed;
                }
            }
        }
        CurrentSpeed -= WaterVelocity * Time.deltaTime;


        if(CurrentSpeed< KillSpeed)
        {
            //GAMEOVER
            EventManager.Instance.SendEvent(EventType.GameOver, null);
        }
        
       Vector3 v3= transform.position;
        Debug.Log(v3.x);
        v3.x = PlayerOffset + CurrentSpeed;
        transform.position = v3;


        fsm.Update(Time.deltaTime);
    }

    void FixedUpdate()
    {
        fsm.FixedUpdate(Time.deltaTime);

        if (waveDetector != null)
        {
            waveDetector.PositionX = playerBody.position.x;
        }
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
            if (waveDetector != null)
            {
                waveDetector.enabled = false;
            }
            break;
        case StateMethod.Update:
            LerpToTargetY (deltaTime, LaneChangeSpeed);
            break;
        case StateMethod.Exit:
            if (waveDetector != null)
            {
                waveDetector.enabled = true;
            }
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
