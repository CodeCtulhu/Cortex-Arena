using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour {


    #region Variables

    #region Rays
    [Header("View")]
    [SerializeField]
    private int viewAngleChangeSpeed = 1;
    internal float viewRadius = 20;
    [Range(0,360)]
    [SerializeField]
    internal float viewAngle = 60;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [SerializeField]
    private BotController _opponent;
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
    private bool _isEnemyInView;
    [SerializeField]
    private bool _isEnemyDashing;
    [SerializeField]
    private float _viewAngle;


    #endregion

    #region Neural_Outputs
    [Header("Neural Outputs")]
    [SerializeField]
    private float _rotationButton;
    [SerializeField]
    private float _viewAngleChangeButton;
    [SerializeField]
    private bool _dashButton;

    #endregion

    #region For_Neural_Network
    public bool _hasBeenDestroyed = false;
    public bool _hasRecievedDamage = false;
    public bool _hasDealtDamage = false;
    public bool _hasDestroyedOpponent = false;
    #endregion

    #region Properties

    #region Neural_Inputs
    public int RaycastForwardDistance { get { return Convert.ToInt32(this._isEnemyInView); } }
    public int RaycastLeftDistance { get { return Convert.ToInt32(this._isEnemyDashing); } }
    public float RaycastRightDistance { get { return this._viewAngle; } }
    #endregion

    #region Neural_Outputs
    public float RotationButton { set { this._rotationButton = value; } }
    public float ViewAngleChangeButton { set { this._viewAngleChangeButton = value; } }
    public bool DashButton { set { this._dashButton = value; } }
    #endregion

    #region For_Neural_Network
    public bool HasBeenDestroyed { get { return this._hasBeenDestroyed; } set { this._hasBeenDestroyed = value; } }
    public bool HasRecievedDamage { get { return this._hasRecievedDamage; } set { this._hasRecievedDamage = value; } }
    public bool HasDealtDamage { get { return this._hasDealtDamage; } set { this._hasDealtDamage = value; } }
    public bool HasDestroyedOpponent { get { return this._hasDestroyedOpponent; } set { this._hasDestroyedOpponent = value; } }
    public BotController Opponent { get { return this._opponent; } set { this._opponent = value; } }

    #endregion

    #endregion

    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        _isEnemyDashing = _opponent.isDashing;
        _viewAngle = viewAngle;

        #region Bot_Controls(Outputs of the Neural network)
        if (_dashButton && isCooldownFinished)
        {
            dash = true;
        }

        RotateBot(_rotationButton);
        UpdateViewAngle(_viewAngleChangeButton,viewAngleChangeSpeed);
        FindVisibleTargets();
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
        if (BotCtrl.gameObject.CompareTag("Bot"))
        {
            if (isDashing && !BotCtrl.isDashing)
            {

                BotCtrl.health--;
                
                if (BotCtrl.health <= 0)
                {
                    _hasDestroyedOpponent = true;
                    BotCtrl._hasBeenDestroyed = true;
                    Destroy(BotCtrl.gameObject); //Destroy object
                }
                else
                {
                    BotCtrl._hasRecievedDamage = true;
                    _hasDealtDamage = true;
                }

            }
            else if (!isDashing && !BotCtrl.GetComponent<BotController>().isDashing)
            {
                rb.velocity = Vector2.zero;
                Vector2 direction = -(collision.contacts[0].point - (Vector2)transform.position);
                rb.AddForce(direction * 5f, ForceMode2D.Impulse);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            Vector2 direction = -(collision.contacts[0].point - (Vector2)transform.position);
            rb.AddForce(direction * 5f, ForceMode2D.Impulse);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees,bool isAngleGlobal)
    {
        if (!isAngleGlobal)
        {
            angleInDegrees += transform.eulerAngles.z - 90;
        }
        return new Vector3(-Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0 );
    }

    public void FindVisibleTargets()
    {
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position,viewRadius,targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.right,dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position,target.position);
                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget,obstacleMask))
                {
                    Debug.DrawRay(transform.position, dirToTarget * dstToTarget,Color.red);
                    _isEnemyInView = true;
                }
                else
                {
                    _isEnemyInView = false;
                }
            }
        }
    }

    private void UpdateViewAngle(float angleChangeButton,int viewAngleChangeSpeed)
    {
        viewAngle += angleChangeButton * viewAngleChangeSpeed;
        viewAngle = Mathf.Clamp(viewAngle,0,360);
    }
}
