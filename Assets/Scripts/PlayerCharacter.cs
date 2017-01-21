using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    private float startY;
    private Wave currentWave;

    private Rigidbody2D playerBody;
    private SpriteRenderer spriteRender;

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

    void FixedUpdate ()
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
}

