using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.Rendering.Universal;

public class CharacterController : MonoBehaviour
{
    // TODO: Polej jump buffer, cap gravity, popravi jumps, wall jump in hang, set sprites for wall jmp, nrdi slide
    //       smooth transition ko menjas stran movementa
    //       popravi animations -> probs gravity ma inpact na y velocity

    public int health = 100;

    [SerializeField] private TextMeshPro debugTMP;

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
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpTime;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideAccTime;
    [Range(0, 1f)][SerializeField] private float WallJumpLerpAmount = 1f;
    [SerializeField] private float WallJumpHangTime = 0;
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
    public bool IsWallJumping { get; private set; }
    public bool IsFiring { get; private set; } // Lahko settas samo privately ampak je accessable publicly
    public bool IsSliding { get; private set; }
    public bool IsHanging { get; private set; }
    #endregion

    private float _wallJumpStartTime;
    private float _lastWallJumpDir;

    private bool _isJumpCut;

    [SerializeField]
    private bool _isAlive = true;
    public bool canMove = true;
    private Collider2D usableInTrigger;

    

    #region Timers init
    public float LastOnGroundTime { get; private set; }
    public float LastJumpTime { get; private set; }
    public float LastOnRightWallTime { get; private set; }
    public float LastOnLeftWallTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float HangTime { get; private set; }
    #endregion

    #region Checks
    [Header("Ground Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(.49f, .03f);
    [SerializeField] private Transform _rightWallCheck;
    [SerializeField] private Transform _leftWallCheck;
    [SerializeField] private Vector2 _rightWallCheckSize;
    [SerializeField] private Vector2 _leftWallCheckSize;
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

    [SerializeField] private ParticleSystem particleSystem;

    private Rigidbody2D rb;
    Animator animator;
    private Collider2D platformHit;

    private bool _Resetable;

    private void Awake()
    {
        startPos();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        particleSystem.enableEmission = false;
        HangTime = WallJumpHangTime;
    }

    private void Start()
    {
        IsFacingRight = true;
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
        LastOnLeftWallTime -= Time.deltaTime;
        LastOnRightWallTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        #endregion

        #region Input
        moveInput.x = canMove ? Input.GetAxisRaw("Horizontal") : 0;

        if (usableInTrigger != null && Input.GetKeyDown(KeyCode.E))
        {
            Usable us = usableInTrigger.gameObject.GetComponent<Usable>();
            
            if(us != null)
                us.triggerPlatformMovement();

        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            BowAttack();  
        }

        #endregion

        if (moveInput.x != 0)
            CheckFacingDirection(moveInput.x > 0);

        if (Input.GetKeyDown(KeyCode.Space) && canMove)
            OnJump();

        animator.SetFloat("yVelocity", rb.linearVelocityY);
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

            if (platformHit != null && platformHit.CompareTag("MovingPlatform"))
            {
                //player.transform.SetParent(platform);
                transform.SetParent(platformHit.transform);
            }



        }

        // Right wall intersection heck
        if ((Physics2D.OverlapBox(_rightWallCheck.position, _rightWallCheckSize, 0, _groundLayer) && IsFacingRight) || (Physics2D.OverlapBox(_leftWallCheck.position, _leftWallCheckSize, 0, _groundLayer) && !IsFacingRight) && !IsWallJumping)
        {
            LastOnRightWallTime = jumpCoyoteTime; // nastavi coyote time za wall jmp
        }

        // Left wall intersection heck
        if ((Physics2D.OverlapBox(_rightWallCheck.position, _rightWallCheckSize, 0, _groundLayer) && !IsFacingRight) || (Physics2D.OverlapBox(_leftWallCheck.position, _leftWallCheckSize, 0, _groundLayer) && IsFacingRight) && !IsWallJumping)
        {
            LastOnLeftWallTime = jumpCoyoteTime; // nastavi coyote time za wall jmp
        }

        LastOnWallTime = Mathf.Max(LastOnRightWallTime, LastOnLeftWallTime);

        //debugTMP.SetText("CanHang?" + CanHang() + "\nLastOnWallTimeABS" + Mathf.Abs(LastOnWallTime) + "\nlast on right wall time " + LastOnRightWallTime + "\n hangTime=" + HangTime + "\n IsHanging?= " + IsHanging);

        /*if (CanHang() && Mathf.Abs(LastOnWallTime) < HangTime)
        {
            Debug.Log("HangTimeIF");
            HangTime -= Time.deltaTime;
            IsHanging = true;
        }
        else //if (IsHanging && Mathf.Abs(LastOnWallTime) > HangTime)
        {
            IsHanging = false;
        }*/

        #region Jump Checks
        if (IsJumping && rb.linearVelocityY < 0) // ce je ze skocil in je hitrost na y osi = 0 dovoli next skok
        {
            IsJumping = false;
        }

        if (IsWallJumping && Time.time - _wallJumpStartTime > wallJumpTime)
        { 
            IsWallJumping = false;
        }

        if (LastOnGroundTime > 0 && !IsJumping)
        {
            _isJumpCut = false;
        }

        if (CanJump() && LastJumpTime > 0) // LastJumpTime > 0 -> preveri ali je bil jump pressed v ti
        {                                  // coyote time -> character lahko pritisne jump malo izven platforme
            IsJumping = true;
            _isJumpCut = false;
            IsWallJumping = false;
            jump();
        }
        else if (CanWallJump() && LastJumpTime > 0)
        {
            IsWallJumping = true;
            IsJumping = false;
            _wallJumpStartTime = Time.time; // shrani si kdaj se je wall jump zacel
            _lastWallJumpDir = (LastOnRightWallTime > 0) ? -1 : 1; //sharni v katero smer je igralec skocil
            HangTime = WallJumpHangTime;
            //IsHanging = false;
            WallJump(_lastWallJumpDir);
        }
        #endregion

        if (CanSlide() && (LastOnLeftWallTime > 0 && moveInput.x < 0) || (LastOnRightWallTime > 0 && moveInput.x > 0)) // Ce lahko igralec slidea in se premika v zid, dovoli slide
        {
            IsSliding = true;
        }
        else
            IsSliding = false;


        #region GravityManipulation
        if (IsSliding)
        {
            //setGravityScale(0);
        }
        else if (rb.linearVelocityY < 0)
        {
            // povecuje gravitacijo med padanjem
            setGravityScale(gravityScale * fallGravityMult);

            // capa fall speed / omeji hitrost padanja
            rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Max(rb.linearVelocityY, -maxFallSpeed));

        }
        else if ((IsJumping) && Mathf.Abs(rb.linearVelocityY) < 0.1)
        {
            setGravityScale(gravityScale * reduceGravityMult);
        }
        else
        {
            // ponsatavi gravitacijo na osnovno vrednost
            setGravityScale(gravityScale);
        }
        #endregion
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (IsHanging)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (IsWallJumping)
        {
            Run(WallJumpLerpAmount);
        }
        else
        {
            Run(1);
        }


        if (IsSliding)
        { 
            Slide();
        }
        animator.SetBool("IsSliding", IsSliding);

    }

    private void Run(float lerpAmount)
    {
        // Dobi desired speed
        float targetSpeed = moveInput.x * moveSpeed; // Izracuna difference med desired speed in current speed
        targetSpeed = Mathf.Lerp(rb.linearVelocityX, targetSpeed, lerpAmount); // doda linearno interpolacijo med trenutno in zeleno hitrostijo

        //debugTMP.SetText("Current speed: " + rb.linearVelocityX + "\nTarget speed: " + targetSpeed +"\n lerp amount: "+lerpAmount +" | is WallJumping? "+IsWallJumping);

        #region Calc Acceleration Rate
        float accelerationRate;

        if (LastOnGroundTime > 0) // izracuna acceleration / deceleration za air in ground movement
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        else
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration * airAcceleration : decceleration * airDecceleration;
        #endregion

        #region Set animations for the character based on its speed

        if ((Mathf.Abs(rb.linearVelocityX) > 0) && Mathf.Abs(rb.linearVelocityX) < .0000005f) //prepreci nepravilno nastavljanje linVelX
        {
            rb.linearVelocityX = 0;
        }

        if (Mathf.Abs(rb.linearVelocityX) == 0)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isRunning", false);
            particleSystem.enableEmission = false;
        }
        else if (Mathf.Abs(rb.linearVelocityX) < 5)
        {
            particleSystem.enableEmission = true;
            animator.SetBool("isMoving", true);
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isRunning", true);
        }
        #endregion

        float speedDif = targetSpeed - rb.linearVelocityX; // Dobi kako hitro more character ali pospesiti ali upocasniti

        float movement = speedDif * accelerationRate;
        // Doda silo na characterja

        rb.AddForce(movement * Vector2.right); //Vector2.right -> affecta samo x os

    }

    public void setGravityScale(float newScale)
    {
        rb.gravityScale = newScale;
    }

    public void setGravityStrength(float newStrength)
    {
        gravityStrength = newStrength;
    }

    #region Jump check methods
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

    private bool CanWallJump()
    { 
        return LastJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping || (LastOnRightWallTime > 0 &&_lastWallJumpDir == 1) || (LastOnLeftWallTime > 0 && _lastWallJumpDir == -1));
    }

    /*private bool CanHang()
    {
        return !IsWallJumping && (LastOnRightWallTime > 0 && moveInput.x == 1) || (LastOnLeftWallTime > 0 && moveInput.x == -1);
    }*/

    #endregion

    #region ActionLogic
    private void jump()
    {
        particleSystem.enableEmission = false;
        LastOnGroundTime = 0; // Reset vrednisti
        LastJumpTime = 0;

        #region Preform Jump
        float force = jumpForce;

        if (rb.linearVelocityY < 0)
            force -= rb.linearVelocityY;

        if (platformHit != null && platformHit.CompareTag("MovingPlatform"))
        {
            platformVel = platformHit.GetComponent<FloatingPlatforms>().speed;
            transform.SetParent(null);
            //force += platformVel;
        }

        #region Aniamtor Toggles
        animator.SetBool("isGrounded", false);
        animator.SetTrigger("jump");
        #endregion

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse); // Doda force na rb v vertikalo
        #endregion
    }

    private void WallJump(float dir)
    {
        LastJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnRightWallTime = 0;
        LastOnLeftWallTime = 0;

        Vector2 force = new Vector2(wallJumpForce.x, wallJumpForce.y);
        force.x *= dir; // ker je dir lahko samo 1 ali -1, ga za pomnožimo z silo na x osi -> obrne smer sile glede na usmerjenost igralca

        if (Mathf.Sign(rb.linearVelocityX) != Mathf.Sign(force.x))
            force.x -= rb.linearVelocityX;
        
        if(rb.linearVelocityY < 0) //preveri ali gralec pada in odsteje to hitrost sili, ki jo moramo dodati
            force.y -= rb.linearVelocityY;

        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void Slide()
    {
        float diff = slideSpeed - rb.linearVelocityY;
        float movement = diff * slideAccTime;

        movement = Mathf.Clamp(movement, -Mathf.Abs(diff) * (1/Time.fixedDeltaTime), Mathf.Abs(diff) * (1 / Time.fixedDeltaTime));

        rb.AddForce(movement * Vector2.up);
    }

    private bool CanSlide()
    {
        return LastOnWallTime > 0 && !IsJumping && !IsWallJumping && LastOnGroundTime <= 0;
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

    private void BowAttack()
    {
        if (CanFire())
        {
            IsFiring = true;
            animator.SetTrigger("fireBow");
            StartCoroutine(WaitForEndOfAnim("player_bow_ground"));
            canMove = false;

        }
    }

    private bool CanFire()
    {
        if(IsJumping || IsWallJumping) return false;


        if (IsFiring)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_bow_ground"))
            {
                return false;
            }
            else
            {
                IsFiring = false;
            }
        }

        return true;
    }

    private void SpawnArrow()
    {
        Vector3 arrowSpawnLocation = _rightWallCheck.position;
        GameObject arrowGO = new GameObject("Arrow");
        Arrow arrow = arrowGO.AddComponent<Arrow>();
        arrow.Initialize(arrowSpawnLocation, IsFacingRight);
    }

    private IEnumerator WaitForEndOfAnim(string name)
    {
        yield return null;

        while(rb.linearVelocityX != 0) yield return null;

        while (AnimatorIsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName(name))
        {
            yield return null;        
        }

        canMove = true;
        SpawnArrow();
    }

    bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length >
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    #endregion

    #region Hit Logic

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
    #endregion

    #region Collision Logic
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
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_groundCheckPoint.transform.position, _groundCheckSize);
        Gizmos.DrawWireCube(_rightWallCheck.transform.position, _rightWallCheckSize);
    }

}
