using UnityEngine;

[ExecuteInEditMode]
public class Wave : MonoBehaviour
{
    public float HighestPoint { get; set; }
    private static System.Random r = new System.Random();
    public ObjectInstance Source;
    public int WaveWidth;
    public int WaveHeight;
    public AnimationCurve WaveCurve;
    public Color LineColor;
    public bool HasBeenEntered { get; set; }
    public float MoveSpeed;

    public float ScreenWaveWidth
    {
        get
        {
            return ((float)WaveWidth / 100) * transform.localScale.x;
        }
    }

    public float ScreenWaveHeight
    {
        get
        {
            return ((float)WaveHeight / 100) * transform.localScale.y;
        }
    }

    private Texture2D waveTexture;
    private BoxCollider2D boxCollider;
    private Rigidbody2D waveBody;
    public void ApplySource(ObjectInstance theSource)
    {
        Source = theSource;
        if(theSource is NegativeWave)
        {
            Debug.Log("neg");
        }
        MoveSpeed *= Source.Speed;
    }
    void OnEnable()
    {
        HighestPoint = 0;
        HasBeenEntered = false;
        foreach (Keyframe k in WaveCurve.keys)
        {
            HighestPoint = Mathf.Max(HighestPoint, k.value);
        }
        
        var spriteRenderer = GetComponent<SpriteRenderer>();

        boxCollider = GetComponent<BoxCollider2D>();
        waveBody = GetComponent<Rigidbody2D>();

        waveTexture = new Texture2D(WaveWidth, WaveHeight);

        Color[] transparent = new Color[WaveWidth * WaveHeight];
        for (int i = 0; i < transparent.Length; ++i)
        {
            transparent[i] = Color.clear;
        }
        waveTexture.SetPixels(transparent);

        for (int width = 0; width < WaveWidth; ++width)
        {
            float startCurvePoint = WaveCurve.Evaluate((float)width / (float)WaveWidth) * (float)WaveHeight;

            DrawLine(waveTexture, width, (int)startCurvePoint, width, 0, LineColor);
        }
        waveTexture.Apply();

        spriteRenderer.sprite = Sprite.Create(waveTexture, new Rect(0, 0, WaveWidth, WaveHeight), Vector2.zero);

        boxCollider.size = new Vector2(WaveWidth / 100f, WaveHeight / 100f);
        boxCollider.offset = new Vector2(WaveWidth / 100f / 2f, WaveHeight / 100f / 2f);
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            waveBody.velocity = new Vector2(MoveSpeed, 0f);
        }
    }

    private void FixedUpdate()
    {
        float waveSign = Mathf.Sign(MoveSpeed);

        if ((waveSign > 0 && transform.position.x >= 20f) || (waveSign < 0 && transform.position.x <= -20f))
        {
            DestroyObject(gameObject);
        }
    }

    void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color col)
    {
        int dy = (int)(y1 - y0);
        int dx = (int)(x1 - x0);
        int stepx, stepy;

        if (dy < 0) { dy = -dy; stepy = -1; }
        else { stepy = 1; }
        if (dx < 0) { dx = -dx; stepx = -1; }
        else { stepx = 1; }
        dy <<= 1;
        dx <<= 1;

        float fraction = 0;

        tex.SetPixel(x0, y0, col);
        if (dx > dy)
        {
            fraction = dy - (dx >> 1);
            while (Mathf.Abs(x0 - x1) > 1)
            {
                if (fraction >= 0)
                {
                    y0 += stepy;
                    fraction -= dx;
                }
                x0 += stepx;
                fraction += dy;
                tex.SetPixel(x0, y0, col);
            }
        }
        else
        {
            fraction = dx - (dy >> 1);
            while (Mathf.Abs(y0 - y1) > 1)
            {
                if (fraction >= 0)
                {
                    x0 += stepx;
                    fraction -= dy;
                }
                y0 += stepy;
                fraction += dx;
                tex.SetPixel(x0, y0, col);
            }
        }
    }
}
