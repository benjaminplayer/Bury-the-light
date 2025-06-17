using System;
using System.Collections;
using UnityEngine;

public class FloatingPlatforms : MonoBehaviour
{
    [SerializeField]
    private Transform platform;

    [SerializeField]
    private float verticalChange = 1f;
    [SerializeField]
    private float horizontalChange = 1f;
    [SerializeField]
    [Range(0.0f, 2.0f)]
    private float verticalSpeed;
    [SerializeField]
    [Range(0.0f, 2.0f)]
    private float horizontalSpeed;
    [SerializeField]
    [Range(.1f, 20f)]
    private float duration;

    public bool horizontalMovement;
    public bool verticalMovement;
    
    [SerializeField]
    private bool isContinous;

    [SerializeField]
    private bool moveOnTouch;

    [SerializeField]
    private bool IsMoving = false;

    [SerializeField]
    private bool canMove = true;
    [SerializeField] private bool _dontReturn;


    #region Falling Plat logic
    [SerializeField] private bool fallAfterTouch = false;
    [SerializeField] private float _waitTime = 0f;
    private bool startedFallCountdown = false;
    private Coroutine fallingCoroutine;
    #endregion

    private Vector3 _startPos;
    private Vector3 lastpos;
    public Vector3 velocity { get; private set; }
    public float speed => velocity.magnitude;
    public Vector2 platVel;
    public float horizontalSpeedBoost = 1.1f;

    private readonly float platCheckSizeY = 0.2f;
    private Vector2 initialCheckSize = Vector2.zero;

    private void Awake()
    {

        //Debug.Log("Start pos: " + _startPos);

        lastpos = transform.position;

        if (horizontalMovement && !moveOnTouch && isContinous)
        {
            StartCoroutine(hoverBetweenValuesHorizontally(platform, horizontalChange, horizontalSpeed));
        }
        else if (verticalMovement && !moveOnTouch && isContinous)
            StartCoroutine(hoverBetweenValuesVertically(platform, verticalChange, verticalSpeed));

        /*if (horizontalMovement && !moveOnTouch && !isContinous)
            StartCoroutine(Movement());*/

    }

    private void Update()
    {
        if (verticalMovement)
        { 
            velocity = (transform.position - lastpos) / Time.deltaTime;
            lastpos = transform.position;
        }
    }

    private IEnumerator MovePlatform(Rigidbody2D rb, Vector3 endPos)
    {
        Vector3 startPos = rb.position;
        Vector3 lastPos = startPos;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Vector3 newPos = Vector3.Lerp(startPos, endPos, t);

            platVel = (newPos - lastPos) / Time.deltaTime;
            lastPos = newPos;

            // Move platform using physics-aware method
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();

        }
        platVel = Vector3.zero;
    }

    #region Indefinite Movement
    IEnumerator hoverBetweenValuesVertically(Transform platform, float dy, float speed)
    {
        Vector2 startpos = platform.position;
        Vector2 endpos = new Vector2(platform.position.x, platform.position.y + dy);

        float elapsedTime = 0f;

        while (true)
        {
            if (startpos.y == endpos.y)
            {
                startpos = endpos;
            }

            elapsedTime += Time.deltaTime * speed; // Keeps time consistent per frame
            float t = Mathf.PingPong(elapsedTime * speed, 1);
            platform.position = Vector2.Lerp(startpos, endpos, t);

            yield return null;
        }

    }

    IEnumerator hoverBetweenValuesHorizontally(Transform platform, float dx, float speed)
    {
        Vector2 startpos = platform.position;
        Vector2 endpos = new Vector2(platform.position.x + dx, platform.position.y);

        float elapsedTime = 0f;

        while (true)
        {
            if (startpos.x == endpos.x)
            {
                startpos = endpos;
            }

            elapsedTime += Time.deltaTime * speed; // Keeps time consistent per frame
            float t = Mathf.PingPong(elapsedTime * speed, 1);
            platform.position = Vector2.Lerp(startpos, endpos, t);

            yield return null;
        }

    }

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!moveOnTouch) return;
        if (collision.collider.CompareTag("Player") && !IsMoving)
        {

            if (fallAfterTouch && !startedFallCountdown)
            {
                startedFallCountdown = true;
                fallingCoroutine = StartCoroutine(FallAfterSeconds());
            }
            else if(fallAfterTouch && fallingCoroutine !=null)
            {
                StopCoroutine(fallingCoroutine);
                fallingCoroutine = StartCoroutine(FallAfterSeconds());
            }

            var cc = collision.gameObject.GetComponent<CharacterController>();
            initialCheckSize = cc._groundCheckSize;
            cc._groundCheckSize = new Vector2(initialCheckSize.x, platCheckSizeY);

            StartCoroutine(Movement()); // test dis
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!moveOnTouch) return;
        if (collision.collider.CompareTag("Player"))
        {
            var cc = collision.gameObject.GetComponent<CharacterController>();
            cc._groundCheckSize = initialCheckSize;
            Debug.Log("Initiac cc: " + initialCheckSize);
        }
    }

    private IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(MovePlatform(this.GetComponent<Rigidbody2D>(), _startPos));
        _startPos = transform.position;
    }


    public IEnumerator Movement() // trigger movement in pole vrni platform na originalen position
    {
        _startPos = transform.position;
        IsMoving = true;

        Vector3 change = (horizontalMovement) ? (transform.position + new Vector3(horizontalChange, 0)) : (transform.position + new Vector3(0, verticalChange));
        yield return StartCoroutine(MovePlatform(this.GetComponent<Rigidbody2D>(), change));
        if(!_dontReturn)
            yield return ResetPosition();
        
        IsMoving = false;
    }

    private IEnumerator FallAfterSeconds()
    {
        Debug.Log("Tgis name "+this.name);
        yield return new WaitForSeconds(_waitTime);
        duration = .5f;
        yield return StartCoroutine(MovePlatform(this.GetComponent<Rigidbody2D>(), this.transform.position - new Vector3(0,40)));
        startedFallCountdown = false;
        yield return null;
    }

    #region SetterMethods
    public void SetMovementDir(string dir)
    {
        if (dir.ToLower() == "horizontal")
        {
            verticalMovement = false;
           horizontalMovement = true;
        }
        else if (dir.ToLower() == "vertical")
        {
            horizontalMovement = false;
            verticalMovement = true;
        }
    }

    public void SetHorizontalChange(float change)
    { 
        this.horizontalChange = change;
    }

    public void SetDuration(float duration)
    {
        this.duration = duration;
    }

    public float GetDuration()
    { 
        return this.duration;
    }

    public string GetArgs()
    {
        return "Veritcal change" + this.verticalChange +" horizontal chage: "+this.horizontalChange+", currentPos: "+transform.position;
    }
    #endregion
}
