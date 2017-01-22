using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollHorizontal : MonoBehaviour 
{
    public float ScrollSpeed = 1f;
    public float MaxSpeed = 4f;

    private float offset;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        offset += Time.deltaTime * Mathf.Clamp(ScrollSpeed, 0f, MaxSpeed) / 2;

        rend.material.mainTextureOffset = new Vector2(offset, 0f);
    }
}
