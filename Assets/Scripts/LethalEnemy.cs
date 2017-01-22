using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LethalEnemy : MonoBehaviour
{
    private Rigidbody2D body;
    public float MoveSpeed;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.velocity = new Vector2(MoveSpeed, 0f);
    }

    void FixedUpdate()
    {
        if (transform.position.x <= -20f)
        {
            DestroyObject(gameObject);
        }
    }
}
