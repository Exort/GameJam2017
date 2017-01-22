using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWaveDetector : MonoBehaviour
{
    private Wave currentWave;

    private Rigidbody2D myRigidBody;
    private SpriteRenderer spriteRenderer;

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
            transform.position = value;
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

    public float WaveSign
    {
        get
        {
            if (currentWave != null)
            {
                return Mathf.Sign(currentWave.MoveSpeed);
            }

            return 0f;
        }
    }

    public bool IsShadowVisible
    {
        get
        {
            return spriteRenderer.enabled;
        }
        set
        {
            spriteRenderer.enabled = value;
        }
    }

    public Action<Wave> EnteredWave;
    public Action<GameObject> EnteredObject;

    void Start ()
    {
        currentWave = null;
        myRigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

            float startY = transform.position.y;

            playerBody.position = new Vector2(playerBody.position.x, startY + deltaY);

            if ((waveSign < 0 && step > 1f) || (waveSign > 0 && step < 0f))
            {
             //   Debug.Log("Leaving wave");

                playerBody.position = new Vector2(playerBody.position.x, transform.position.y);
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
            deltaY = wave.WaveCurve.Evaluate(step) * wave.ScreenWaveHeight;
        }

        return deltaY;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var wave = collision.gameObject.GetComponent<Wave>();
        if (wave == null)
        {
            var handler = EnteredObject;
            if (handler != null)
            {
                handler.Invoke(collision.gameObject);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        var wave = collision.gameObject.GetComponent<Wave>();
        if (wave != null && wave != currentWave)
        {
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

            //Debug.Log("Entering wave");

            var handler = EnteredWave;
            if (handler != null)
            {
                handler.Invoke(currentWave);
            }
        }
    }
}
