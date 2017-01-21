using System;
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

    public Action<Wave> EnteredWave;

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

            float step = 0f;
            float deltaY = GetDeltaY(currentWave, ref step);

            playerBody.position = new Vector2(playerBody.position.x, startY + deltaY);

            if ((waveSign < 0 && step > 1f) || (waveSign > 0 && step < 0f))
            {
                Debug.Log("Leaving wave");
                playerBody.position = new Vector2(playerBody.position.x, transform.position.y);

                currentWave = null;
            }
        }
    }

    private float GetDeltaY(Wave wave, ref float step)
    {
        float deltaY = 0f;

        float diff = transform.position.x - wave.transform.position.x;

        step = diff / wave.ScreenWaveWidth;

        if (diff >= 0f)
        {
            deltaY = wave.CurrentCurve.Evaluate(step) * wave.ScreenWaveHeight;
        }

        return deltaY;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        var wave = collision.gameObject.GetComponent<Wave>();
        if (wave != null)
        {
            if (wave == currentWave)
            {
                return;
            }

            if (currentWave != null)
            {
                float step = 0f;
                float currentWaveDelta = GetDeltaY(currentWave, ref step);
                float targetWaveDelta = GetDeltaY(wave, ref step);

                if (targetWaveDelta < currentWaveDelta)
                {
                    return;
                }
            }

            currentWave = wave;

            startY = playerBody.position.y;

            Debug.Log("Entering wave");

            var handler = EnteredWave;
            if (handler != null)
            {
                handler.Invoke(currentWave);
            }
        }
    }
}
