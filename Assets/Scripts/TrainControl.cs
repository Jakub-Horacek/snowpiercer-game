using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trainControl : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 velocity = rb.velocity;
        velocity.z = moveVertical * speed;

        rb.velocity = velocity;
    }
}
