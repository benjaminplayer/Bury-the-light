using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))] // Pomeni da ne mores dodati scripta na component, unless da rigidbody2d ze obstaja na njem  

public class PlayerController : MonoBehaviour
{
    /*public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float airSpeed = 3f;
    public float airRunSpeed = 7f;*/
    public float jumpImpulse = 10f;
    //public float mult = 1f;
    Vector2 moveInput;
    TouchingDirections touchingDirections;

    #region Acceleration Parameter Initialization

    [Header("Run with force movement applied")]
    public float runMaxSpeed;
    public float runAcceleration;
    [HideInInspector]
    public float accAmount;
    public float runDeceleration;
    [HideInInspector]
    public float runDecelAmount;
    public bool conserveMomentum;

    [Range(0.01f, 1)]
    public float airAcceleration;
    [Range(0.01f, 1)]
    public float airDeceleration;

    #endregion

    #region WallSlide Parameters
    [Header("Wall Slide Parameters")]
    public float slideSpeed;
    public float speedAcceleration;
    #endregion

    private void OnValidate() // OnValidate is called at various stages during the Editor's normal operation, such as loading scenes, building a player, and entering Play Mode
    {
        accAmount = (50 * runAcceleration) / runMaxSpeed;
        runDecelAmount = (50 * runDeceleration) / runMaxSpeed;

        #region
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDeceleration = Mathf.Clamp(runDeceleration, 0.01f, runMaxSpeed);
        #endregion
    }

    /*public float CurrentMoveSpeed {
        get
        {
            if (IsMoving && !touchingDirections.IsOnWall)
            {

                if (touchingDirections.IsGrounded)
                {
                    if (IsRunning)
                    {
                        return runSpeed;
                    }
                    else
                    {
                        return walkSpeed;
                    }
                }
                else
                {
                    if (IsRunning)
                    {
                        return airRunSpeed;
                    }
                    else
                        return airSpeed;
                }

            }
            else
            {
                return 0; // idle
            }
        }
    }
    */
    [SerializeField]
    private bool _isMoving = false;


    public bool IsMoving { get
        {
            return _isMoving;
        } private set
        {
            _isMoving = value;
            animator.SetBool("isMoving", value);
        }
    }

    [SerializeField]
    private bool _isRunning = false;

    private bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        set
        {
            _isRunning = value;
            animator.SetBool("isRunning", value);
        }
    }

    public bool _isFacingRight = true;

    public bool IsFacingRight { get { return _isFacingRight; } private set {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }

            _isFacingRight = value;

        } }

    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // on awake bo referencalo rigidbody component playeja otherwise da error
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    private void FixedUpdate()
    {
        move();
    }


    public void onMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); // vrne ali 1 => premik v desno ali -1 => premik v levo

        IsMoving = moveInput != Vector2.zero;

        setFacingDirection(moveInput);

    }

    private void setFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            // desno
            IsFacingRight = true;


        } else if (moveInput.x < 0 && IsFacingRight)
        {
            // levo
            IsFacingRight = false;
        }
    }

    public void onRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    public void onJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded)
        {
            animator.SetTrigger("jump");
            //rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpImpulse);
            rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        }
    }


    private void move()
    {
        float targetSpeed = moveInput.x * runMaxSpeed;

        #region Calc Acceleration
        float accRate = 0;

        if (touchingDirections.IsGrounded)
            accRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accAmount : runDecelAmount;
        else
            accRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accAmount * airAcceleration : runDecelAmount * airDeceleration;
        //TODO: Nrdi air movement

        #endregion

        #region Momentum Conservation
        if (conserveMomentum && Mathf.Abs(rb.linearVelocityX) > Math.Abs(targetSpeed) && Mathf.Sign(rb.linearVelocityX) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f & touchingDirections.IsGrounded)
            accRate = 0;
        #endregion

        float speedDif = targetSpeed - rb.linearVelocityX;

        float movement = speedDif * accRate;

        if (!touchingDirections.IsOnWall) 
        {
            rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
            
        else if (touchingDirections.IsOnWall) 
        {
            wallSlide();
        }
            

    }

    //TODO: Dodaj da 1s caka preden wallslide-a
    //Fix animator
    //Fix speed buildup while standing close to wall

    private void wallSlide() 
    {

        float speedDif = slideSpeed - rb.linearVelocityY;   
        float movement = slideSpeed * speedAcceleration;


        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1/ Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        Debug.Log(movement);

        rb.AddForce(movement * Vector2.up);
    }

}
