using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenHandler : MonoBehaviour
{
    public float BlinkTime = 0.4f;
    public Text AnyKeyComponent;
    public Image GlowImage;
    private float blinkTimer;

	void Start () {
        blinkTimer = 0f;
	}

	void Update () {
        blinkTimer += Time.deltaTime;

        if (GlowImage != null)
        {
            Color c = GlowImage.color;
            c.a = blinkTimer / BlinkTime;
            if (AnyKeyComponent.enabled)
            {
                c.a = 1 - c.a;
            }
            GlowImage.color = c;
        }
       
        if (blinkTimer >= BlinkTime)
        {
            if (AnyKeyComponent != null)
            {
                AnyKeyComponent.enabled = !AnyKeyComponent.enabled;

                blinkTimer = 0f;
            }
        }
	}
}
