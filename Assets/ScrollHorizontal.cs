using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollHorizontal : MonoBehaviour 
{
    public float ScrollSpeed = 1f;
    private Renderer rend;

    void Start() 
    {
        rend = GetComponent<Renderer>();
    }

    void Update() 
    {
        float offset = Time.time * ScrollSpeed;
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}
