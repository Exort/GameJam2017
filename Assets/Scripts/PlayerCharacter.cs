﻿using UnityEngine;
using Assets.Scripts;
using System;

public class PlayerCharacter : MonoBehaviour
{
    public float MaxSpeed;
    public float CurrentSpeed;
    public float BaseSpeed;
    public float KillPosition;
    public float BoostVelocity = 8;
    public float WaterVelocity = 5;
    public float LowestSpeed = -5;
    public float MaxPosition = 7;
    public float LaneChangeSpeed = 5f;
    public PlayerWaveDetector WaveDetectorPrefab;
    public AudioClip GoodWaveSound;
    public AudioClip BadWaveSound;

    private enum States
    {
        Standing,
        ChangingLane
    };

    private bool Dead = false;

    private Rigidbody2D playerBody;
    private AudioSource audioSource;
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
        MaxSpeed = 10;
        CurrentSpeed = 4;
        BaseSpeed = 0;
        KillPosition = -10;

        playerBody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

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
        switch (method)
        {
            case StateMethod.Update:
                {
                    CurrentSpeed -= WaterVelocity;

                    CurrentSpeed = Mathf.Clamp(CurrentSpeed, LowestSpeed, MaxSpeed);

                    if (playerBody.position.x >= MaxPosition && CurrentSpeed > 0)
                    {
                        CurrentSpeed = 0;
                    }

                    playerBody.position = new Vector2(playerBody.position.x + CurrentSpeed * Time.deltaTime, playerBody.position.y);
                    break;
                }
            case StateMethod.FixedUpdate:
                {
                    if (playerBody.position.x < KillPosition)
                    {
                        //GAMEOVER
                        if (!Dead)
                        {
                            Dead = true;
                            EventManager.Instance.SendEvent(EventType.GameOver, null);
                        }
                    }
                    break;
                }
        }
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
                waveDetector.IsShadowVisible = false;
            }
            break;
        case StateMethod.Update:
            LerpToTargetY (deltaTime, LaneChangeSpeed);
            break;
        case StateMethod.Exit:
            if (waveDetector != null)
            {
                waveDetector.enabled = true;
                waveDetector.IsShadowVisible = true;
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
            playerBody.position = targetPosition;
            waveDetector.Position = targetPosition;

            fsm.ChangeState ((int)States.Standing);
        }
    }

    private void onEnteredWave(Wave wave)
    {
        var handler = EnteredWave;
        if (handler != null)
        {
            bool DoApply = true;
            if (Mathf.Sign(wave.MoveSpeed) > 0f)
            {
                
                if(!wave.HasBeenEntered)
                {
                    wave.HasBeenEntered = true;
                }
                else
                {
                    DoApply = false;
                }
               
            }
           
            if (DoApply)
            {
                if (Mathf.Sign(wave.MoveSpeed) > 0f)
                {
                    audioSource.PlayOneShot(GoodWaveSound);

                }
                else
                {
                    audioSource.PlayOneShot(BadWaveSound);

                }
                CurrentSpeed += wave.MoveSpeed * 8;
            }
            
            handler.Invoke(wave);
        }
    }
}
