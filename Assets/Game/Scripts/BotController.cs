using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour {


    #region Variables

    #region Rays
    [Header("Rays")]
    [SerializeField]
    private RaycastHit2D RaycastForward, RaycastLeft, RaycastRight;
    private int distance = 20;
    #endregion

    #region Rotation And Attack
    [Header("Rotation And Attack")]

    #region Rotation

    private float degreesPerSec = 180f; //speed of the rotation
    #endregion

    #region Dash/Attack_Values
    private int health = 3;
    private Rigidbody2D rb;
    private float dashForce = 15f;
    private bool dash = false;
    public bool isDashing = false;
    private bool isCooldownFinished = true;
    private float timer;
    #endregion
    #endregion

    #region Neural_Inputs
    [Header("Neural Inputs")]
    [SerializeField]
    private float _raycastForwardDistance;
    [SerializeField]
    private float _raycastLeftDistance;
    [SerializeField]
    private float _raycastRightDistance;
    #endregion

    #region Neural_Outputs
    [Header("Neural Outputs")]
    [SerializeField]
    private float _rotationButton;
    [SerializeField]
    private bool _dashButton;
    #endregion

    #region For_Neural_Network
    private bool _hasBeenDestroyed = false;
    private bool _hasRecievedDamage = false;
    private bool _hasDealtDamage = false;
    private bool _hasDestroyedOpponent = false;
    #endregion

    #region Properties

    #region Neural_Inputs
    public float RaycastForwardDistance { get { return this._raycastForwardDistance; } }
    public float RaycastLeftDistance { get { return this._raycastLeftDistance; } }
    public float RaycastRightDistance { get { return this._raycastRightDistance; } }
    #endregion

    #region Neural_Outputs
    public int RotationButton { set { this._rotationButton = value; } }
    public bool DashButton { set { this._dashButton = value; } }
    #endregion

    #region For_Neural_Network
    public bool HasBeenDestroyed { get { return this._hasBeenDestroyed; } }
    public bool HasRecievedDamage { get { return this._hasRecievedDamage; } }
    public bool DealtDamage { get { return this._hasDealtDamage; } }
    public bool DestroyedOpponent { get { return this._hasDestroyedOpponent; } }
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

        #region Bot_Controls(Outputs of the Neural network)
        if (_dashButton && isCooldownFinished)
        {
            dash = true;
        }

        RotateBot(_rotationButton);
        #endregion

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
        BotController BotCtrl = collision.gameObject.GetComponent<BotController>(); //Cashe the gameObj 
        if (isDashing && BotCtrl.gameObject.CompareTag("Bot") && !BotCtrl.isDashing)
        {

            BotCtrl.health--;
            if (BotCtrl.health <= 0)
            {
                _hasDestroyedOpponent = true;
                Destroy(BotCtrl.gameObject); //Destroy object
            }
            else
            {
                _hasDealtDamage = true;
            }
            
        }
        else if (!isDashing && BotCtrl.gameObject.CompareTag("Bot") && !BotCtrl.GetComponent<BotController>().isDashing)
        {
            rb.velocity = Vector2.zero;
            Vector2 direction = -(collision.contacts[0].point - (Vector2)transform.position);
            rb.AddForce(direction * 5f, ForceMode2D.Impulse);
        }
        else
        {
            rb.velocity = Vector2.zero;
            Vector2 direction = -(collision.contacts[0].point - (Vector2)transform.position);
            rb.AddForce(direction * 5f, ForceMode2D.Impulse);
        }
    }

}
