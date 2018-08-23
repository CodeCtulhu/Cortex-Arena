using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour {


    #region Variables

    [Header("Rays")]
    [SerializeField]
    private RaycastHit2D RaycastForward, RaycastLeft, RaycastRight;
    private int distance = 20;


    [Header("Rotation And Attack")]
    private float degreesPerSec = 180f; //speed of the rotation

    private Rigidbody2D rb;
    private float dashForce = 15f;
    private bool dash = false;
    public bool isDashing = false;
    private bool isCooldownFinished = true;
    private float timer;

    #region Neural Inputs
    [Header("Neural Inputs")]
    [SerializeField]
    private float _raycastForwardDistance;
    [SerializeField]
    private float _raycastLeftDistance;
    [SerializeField]
    private float _raycastRightDistance;
    #endregion

    #region Neural Outputs
    [Header("Neural Outputs")]
    [SerializeField]
    private float _rotationButton;
    [SerializeField]
    private int _dashButton;
    #endregion

    #region Properties
    
    #region Neural Inputs
    public float RaycastForwardDistance { get { return this._raycastForwardDistance; } }
    public float RaycastLeftDistance { get { return this._raycastLeftDistance; } }
    public float RaycastRightDistance { get { return this._raycastRightDistance; } }
    #endregion

    #region Neural Outputs
    public int RotationButton { set { this._rotationButton = value; } }
    public int DashButton { set { this._dashButton = value; } }
    #endregion

    #endregion

    #endregion


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        #region Raycasts
        Debug.DrawRay(transform.position, transform.right * distance, Color.white);
        Debug.DrawRay(transform.position, transform.up * distance, Color.white);
        Debug.DrawRay(transform.position, -transform.up * distance, Color.white);


        RaycastForward = Physics2D.Raycast(transform.position, transform.right, distance);
        RaycastLeft = Physics2D.Raycast(transform.position, transform.up, distance);
        RaycastRight = Physics2D.Raycast(transform.position, -transform.up, distance);

        _raycastForwardDistance = RaycastForward.distance;
        _raycastLeftDistance = RaycastLeft.distance;
        _raycastLeftDistance = RaycastRight.distance;

        
        #endregion

        //temporary input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            dash = true;
        }


        //temporary input
        _rotationButton = Input.GetAxis("Horizontal");
        RotateBot(_rotationButton);
    }

    
    private void RotateBot(float rotationButton)
    {
        float rotAmount = -degreesPerSec * Time.deltaTime * rotationButton; //We take the direction and the magnitude of rotation
        float currentRot = transform.localRotation.eulerAngles.z; // Then we find current z rotation (because of unity stuff...)
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, currentRot + rotAmount)); //Now we just assign the rotation.
    }

    private void Dash(float dashForce)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(transform.right * dashForce, ForceMode2D.Impulse);
    }


    private void FixedUpdate()
    {
        #region
        if (dash & isCooldownFinished)
        {
            dash = false;
            isDashing = true;
            Dash(dashForce);
            
            timer = 1;
            isCooldownFinished = false;
            
        }
        else if (!isCooldownFinished)
        {
            if (rb.velocity.magnitude <= 3f)
            {
                isDashing = false;
            }
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                isCooldownFinished = true;
                timer = 0;
            }
        }
        #endregion
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var gameObj = collision.gameObject; //Cashe the gameObj 
        if (isDashing && gameObj.CompareTag("Bot") && gameObj.GetComponent<BotController>().isDashing)
        {
            Destroy(gameObj); //Destroy object
        }
    }

}
