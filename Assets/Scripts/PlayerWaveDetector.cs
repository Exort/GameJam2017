using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWaveDetector : MonoBehaviour
{
    private float startY;
    private Wave currentWave;

    private Rigidbody2D myRigidBody;

    private Rigidbody2D playerBody;
    public Rigidbody2D PlayerBody
    {
        set
        {
            playerBody = value;
        }
    }

    public Vector2 Position
    {
        set
        {
            myRigidBody.position = value;
        }
    }

    public float PositionX
    {
        set
        {
            myRigidBody.position = new Vector2(value, myRigidBody.position.y);
        }
    }

    public float PositionY
    {
        set
        {
            myRigidBody.position = new Vector2(myRigidBody.position.x, value);
        }
    }

    void Start ()
    {
        currentWave = null;
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    public void ResetWave()
    {
        currentWave = null;
    }

    void FixedUpdate()
    {
        if (currentWave != null && playerBody != null)
        {
            float waveSign = Mathf.Sign(currentWave.MoveSpeed);

            float waveX = currentWave.transform.position.x;
            float diff = transform.position.x - waveX;

            float step = diff / currentWave.ScreenWaveWidth;

            //Debug.Log(string.Format("Step={0}, WaveX={1}, PlayerX={2}, {3}", step, waveX, playerBody.position.y, diff));

            if (diff >= 0f)
            {
                float deltaY = currentWave.WaveCurve.Evaluate(step) * currentWave.ScreenWaveHeight;

                playerBody.position = new Vector2(playerBody.position.x, startY + deltaY);
            }

            if ((waveSign < 0 && step > 1f) || (waveSign > 0 && step < 0f))
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
}
