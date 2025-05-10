using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.Rendering.Universal;

public class CharacterController : MonoBehaviour
{
    // TODO: Polej jump buffer, cap gravity, popravi jumps, tweak values, wall jump in hang,
    //       smooth transition ko menjas stran movementa
    //       popravi animations -> probs gravity ma inpact na y velocity

    public int health = 100;

    #region Action Values
    [Header("Action Values")]
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private float acceleration = 1f;
    [SerializeField]
    private float decceleration = 1f;
    [SerializeField]
    private float velPower = .9f;
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    private float airAcceleration;
    [SerializeField]
    private float airDecceleration;


    public float platformVel;
    #endregion

    private Vector3 startingPos = new Vector3(-38.14f, -14.605f, 0.01886861f);

    #region GravityValues
    [Header("Gravity")]
    [SerializeField]
    private float gravityScale;
    [SerializeField]
    private float gravityStrength;
    [SerializeField]
    private float fallGravityMult;
    [SerializeField]
    private float maxFallSpeed = 10f;
    [SerializeField]
    private float reduceGravityMult;
    #endregion

    [SerializeField]
    private Vector2 moveInput;
    

    #region Player Actions
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    #endregion

    private bool _isJumpCut;

    [SerializeField]
    private bool _isAlive = true;
    public bool canMove = true;
    private Collider2D usableInTrigger;

    #region Timers init
    public float LastOnGroundTime { get; private set; }
    public float LastJumpTime { get; private set; }
    #endregion

    #region Checks
    [Header("Ground Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(.49f, .03f);
    [SerializeField] private float _groundCheckDist;
    #endregion

    #region Layers and Tags
    [Header("Layers and tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float jumpCoyoteTime; // Grace period after falling off a platform, where you can still jump
    [Range(0.01f, 0.5f)] public float jumpBufferTime; // Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

    [SerializeField]
    private bool _useStartPos = true;

    private Rigidbody2D rb;
    Animator animator;
    private Collider2D platformHit;

    private bool _Resetable;

    private void Awake()
    {
        startPos();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    private void Update()
    {
        if (!_isAlive)
        {
            GetComponent<CharacterController>().enabled = false;
        }

        

        if (health <= 0)
        {
            handleDeath();
        }

        #region Timers
        LastOnGroundTime -= Time.deltaTime; // preveri kdaj je bil player na zadnje na tleh
        LastJumpTime -= Time.deltaTime;     // preveri kdaj je bil player na zadnje v zraku
        #endregion

        #region Input
        moveInput.x = canMove ? Input.GetAxisRaw("Horizontal") : 0;

        if (usableInTrigger != null && Input.GetKeyDown(KeyCode.E))
        {
            Usable us = usableInTrigger.gameObject.GetComponent<Usable>();
            
            if(us != null)
                us.triggerPlatformMovement();

        }

        #endregion

        if (moveInput.x != 0)
            CheckFacingDirection(moveInput.x < 0);

        if (Input.GetKeyDown(KeyCode.Space) && canMove)
            OnJump();

        animator.SetFloat("yVelocity", rb.linearVelocityY);
        //Debug.Log(rb.linearVelocityY);
        if (!IsJumping)
        {
            platformHit = Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer);
            // Naredi box okoli child objekta -> ce intersecta groubnd -> is on ground -> lahko skoci
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
            {
                LastOnGroundTime = jumpCoyoteTime;
                animator.SetBool("isGrounded", true);
                animator.ResetTrigger("jump");
                rb.linearVelocityY = 0;
            }
            else 
            {
                animator.SetBool("isGrounded", false);
            }

            //Debug.Log(platformHit);

            if (platformHit != null && platformHit.CompareTag("MovingPlatform"))
            {
                //player.transform.SetParent(platform);
                transform.SetParent(platformHit.transform);
            }



        }
        #region Jump Checks
        if (IsJumping && rb.linearVelocityY < 0) // ce je ze skocil in je hitrost na y osi = 0 dovoli next skok
        {
            IsJumping = false;
        }

        if (LastOnGroundTime > 0 && !IsJumping)
        {
            _isJumpCut = false;
        }

        if (CanJump() && LastJumpTime > 0) // LastJumpTime > 0 -> preveri ali je bil jump pressed v ti
        {                                  // coyote time -> character lahko pritisne jump malo izven platforme
            IsJumping = true;
            _isJumpCut = false;
            jump();
        }
        #endregion


        #region GravityManipulation
        if (rb.linearVelocityY < 0)
        {
            // povecuje gravitacijo med padanjem
            setGravityScale(gravityScale * fallGravityMult);

            // capa fall speed / omeji hitrost padanja
            rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Max(rb.linearVelocityY, -maxFallSpeed));

        }
        else if ((IsJumping) && Mathf.Abs(rb.linearVelocityY) < 0.1)
        {
            //Debug.Log(rb.linearVelocityY);
            setGravityScale(gravityScale * reduceGravityMult);
        }
        else
        {
            // ponsatavi gravitacijo na osnovno vrednost
            setGravityScale(gravityScale);
        }
        #endregion
    }

    public void setGravityScale(float newScale)
    {
        rb.gravityScale = newScale;
    }

    public void setGravityStrength(float newStrength)
    {
        gravityStrength = newStrength;
    }

    public void OnJump()
    {
        LastJumpTime = jumpBufferTime;
    }

    public void OnJumpUp()
    {
        if (CanJumpCut())
            _isJumpCut = true;

    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    private bool CanJumpCut()
    {
        return IsJumping && rb.linearVelocityY > 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        #region Movement
        // Dobi desired speed
        float targetSpeed = moveInput.x * moveSpeed;
        // Izracuna difference med desired speed in current speed
        float speedDif = targetSpeed - rb.linearVelocityX;
        // Dobi kako hitro more character ali pospesiti ali upocasniti

        #region Calc Acceleration Rate
        float accelerationRate;

        if (LastOnGroundTime > 0) // izracuna acceleration / deceleration za air in ground movement
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        else
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration * airAcceleration : decceleration * airDecceleration;
        #endregion

        #region Set animations for the character based on its speed

        //Debug.Log(rb.linearVelocityX);

        if ((Mathf.Abs(rb.linearVelocityX) > 0) && Mathf.Abs(rb.linearVelocityX) < .0000005f) //prepreci nepravilno nastavljanje linVelX
        {
            rb.linearVelocityX = 0;
        }

        if (Mathf.Abs(rb.linearVelocityX) == 0)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isRunning", false);
        }
        else if (Mathf.Abs(rb.linearVelocityX) < 5)
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isRunning", false);
        }
        else
            animator.SetBool("isRunning", true);
        #endregion

        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelerationRate, velPower) * Mathf.Sign(speedDif);
        // Doda silo na characterja
        rb.AddForce(movement * Vector2.right); //Vector2.right -> affecta samo x os

        #endregion

    }

    private void jump()
    {
        LastOnGroundTime = 0; // Reset vrednisti
        LastJumpTime = 0;

        #region Preform Jump
        float force = jumpForce;

        if (rb.linearVelocityY < 0)
            force -= rb.linearVelocityY;
        //Debug.Log(force);

        if (platformHit != null && platformHit.CompareTag("MovingPlatform"))
        {
            platformVel = platformHit.GetComponent<FloatingPlatforms>().speed;
            //Debug.Log("Platform vel:" + platformVel);
            transform.SetParent(null);
            force += platformVel;
        }

        #region Aniamtor Toggles
        animator.SetBool("isGrounded", false);
        animator.SetTrigger("jump");
        #endregion
        Debug.Log("Character jump force: " + force);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse); // Doda force na rb v vertikalo
        #endregion
    }

    private void CheckFacingDirection(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            changeDirection();
    }

    private void changeDirection()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        IsFacingRight = !IsFacingRight;
    }

    public void OnHit()
    {
        animator.Play("Fall");
        canMove = false;
        rb.linearVelocity= new Vector2(0, rb.linearVelocityY);
        transform.position = Camera.main.GetComponent<CameraController>().entryPos; // dobi zacetno pozicijo levela in igralca postavi nanjo
        StartCoroutine(RecoverFromHit()); // pocaka, da se animacija izvede do konca, preden na novo dovoli premike

    }

    private void handleDeath()
    {
        animator.SetTrigger("isHurt");
        _isAlive = false;

    }

    private void resetMovementAfterHit()
    {
        Debug.Log("Reset");
        animator.ResetTrigger("isHurt");
        canMove = true;
    }

    private void startPos() 
    { 
        if(_useStartPos)
            transform.position = startingPos;
    }

    private IEnumerator RecoverFromHit() 
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Fall")) // pocaka, da je player v fall state
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) // ce je normalized time 1 pomeni da je animacija koncala
            yield return null;
        resetMovementAfterHit();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.collider);

        if (collision.collider.CompareTag("Enemy"))
        {
            Debug.Log("Hit");
            OnHit();
        }

        if (collision.collider.CompareTag("Spikes"))
        {
            OnHit();
        }



    }

    //Fix dis
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("blockMovement") && IsJumping)
        {
            Debug.Log("collision with rock wall");
            rb.linearVelocity = new Vector2(0f, rb.linearVelocityY);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ResetCamMovement"))
        {
            CameraController cc = Camera.main.GetComponent<CameraController>();
            cc.SetFollowCam(false);
            StartCoroutine(cc.moveCamera(Camera.main.transform.position, new Vector3(44.55f, -35f, Camera.main.transform.position.z), 1f));
            StartCoroutine(cc.ZoomCamera(10f, 1f));
        }

        if (collision.CompareTag("GeneralCamReset") && _Resetable) 
        {
            CameraController cc = Camera.main.GetComponent<CameraController>();
            cc.SetFollowCamX(false);
            StartCoroutine(cc.moveCamera(Camera.main.transform.position, new Vector3(222.3279f, -15f, Camera.main.transform.position.z), 1f));
            StartCoroutine(cc.ZoomCamera(10f, 1f));
            CameraController.IsEndOfLevel = false;

            _Resetable = false;
        }

        if (collision.CompareTag("RemoveCamMovement"))
        {
            CameraController.IsEndOfLevel = true;
        }

        if (collision.CompareTag("TriggerFollowCam"))
        {
            CameraController.IsEndOfLevel = true;
            CameraController cc = Camera.main.GetComponent<CameraController>();
            cc.SetOffsetY(2f);
            StartCoroutine(cc.ZoomCamera(3.67f, 0.2f));
            cc.SetFollowCamX(true);
            _Resetable = true;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7) // Premakni v OnTriggerStay2D
        {
            Debug.Log("Staying In Collision");
            usableInTrigger = collision;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision == usableInTrigger)
            usableInTrigger = null;
    }


}
