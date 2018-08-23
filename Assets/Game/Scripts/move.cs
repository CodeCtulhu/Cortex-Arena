using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    Rigidbody2D rb;
    bool dash;
    [SerializeField]
    float dashForce = 5f;
    [SerializeField]
    float rotationSpeed = 5f;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            dash = true;
        }
        transform.Rotate(new Vector3(0, 0, -Input.GetAxisRaw("Horizontal") * rotationSpeed));
    }
    private void FixedUpdate()
    {
        if (dash)
        {
            Dash(dashForce);
            dash = false;
        }
    }

    private void Dash(float dashForce)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(transform.right * dashForce, ForceMode2D.Impulse);

    }
}