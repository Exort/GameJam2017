using UnityEngine;
using Assets.Scripts;

public class PlayerCharacter : MonoBehaviour
{
    private enum States
    {
        Standing,
        ChangingLane
    };

    public float LaneChangeSpeed = 5f;

    private float startY;
    private Wave currentWave;

    private Rigidbody2D playerBody;
    private SpriteRenderer spriteRender;

    private float _targetY;
    private float _animationTime;

    private FSM fsm = new FSM();

    public void ResetWave()
    {
        currentWave = null;
    }

    void Start ()
    {
        currentWave = null;
        playerBody = GetComponent<Rigidbody2D>();
        spriteRender = GetComponentInChildren<SpriteRenderer>();
    }

    void UpdateWaveMovement ()
    {
        if (currentWave != null)
        {
            float waveSign = Mathf.Sign(currentWave.MoveSpeed);

            float waveX = currentWave.transform.position.x;
            float diff = transform.position.x - waveX;

            float step = diff / currentWave.ScreenWaveWidth;

            Debug.Log(string.Format("Step={0}, WaveX={1}, PlayerX={2}, {3}", step, waveX, transform.position.x, diff));

            if (diff >= 0f)
            {
                float deltaY = currentWave.WaveCurve.Evaluate(step) * currentWave.ScreenWaveHeight;

                playerBody.position = new Vector2(playerBody.position.x, startY + deltaY);
            }
            else if ((waveSign < 0 && step > 1f)
                     || (waveSign > 0 && step < 0f))
            {
                Debug.Log("Leaving wave");
                playerBody.position = new Vector2(playerBody.position.x, startY);
                currentWave = null;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var wave = collision.gameObject.GetComponent<Wave>();
        if (wave != null)
        {
            if ((currentWave != null && currentWave != wave) || currentWave == null)
            {
                currentWave = wave;

                startY = playerBody.position.y;

                Debug.Log("Entering wave");
            }
        }
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
        switch(method)
        {
        case StateMethod.FixedUpdate:
            UpdateWaveMovement ();
            break;
        }
    }

    void StateChangingLane(StateMethod method, float deltaTime)
    {
        switch(method)
        {
        case StateMethod.Enter:
            _animationTime = 0;
            break;
        case StateMethod.Update:
            LerpToTargetY (deltaTime, LaneChangeSpeed);
            break;
        case StateMethod.Exit:
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
        }
        if(Mathf.Abs(playerBody.position.y - targetPosition.y) < Mathf.Epsilon)
        {
            fsm.ChangeState ((int)States.Standing);
        }
    }
}

