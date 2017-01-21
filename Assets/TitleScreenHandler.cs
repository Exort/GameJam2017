using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenHandler : MonoBehaviour {

    public GameManager theUi;

    public float BlinkTime = 0.4f;
    public Text AnyKeyComponent;

    private float blinkTimer;

	// Use this for initialization
	void Start () {
        blinkTimer = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        if(true || Input.anyKeyDown)
        {
            EventManager.Instance.SendEvent(EventType.StartGame, null);
            DestroyImmediate(this.gameObject);
            return;
        }

        blinkTimer += Time.deltaTime;
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
