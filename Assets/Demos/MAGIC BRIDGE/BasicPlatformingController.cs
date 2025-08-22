using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPlatformingController : MonoBehaviour
{
    public float Speed;
    float MoveInput;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * 500);
        }
    }

    private void FixedUpdate()
    {
        MoveInput = Input.GetAxisRaw("Horizontal") * Speed;
        
        GetComponent<Rigidbody2D>().velocity = new Vector2(MoveInput, GetComponent<Rigidbody2D>().velocity.y);
    }
}
