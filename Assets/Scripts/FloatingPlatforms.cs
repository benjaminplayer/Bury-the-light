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

    [SerializeField]
    private bool horizontalMovement;
    [SerializeField]
    private bool verticalMovement;
    [SerializeField]
    private bool isContinous;

    [SerializeField]
    private bool moveOnTouch;

    [SerializeField]
    private bool IsMoving = false;

    [SerializeField]
    private bool canMove = true;

    private CharacterController cr;
    private GameObject player;

    private Vector3 _startPos;
    private Vector3 lastpos;
    public Vector3 velocity { get; private set; }
    public float speed => velocity.magnitude;
    public float horizontalSpeedBoost = 1.1f;

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

    #region BasicMovements
    IEnumerator movePlatformVertically(Transform platform, Vector3 endpos, float duration)
    {
        canMove = false;

        float elapsed = 0f;
        Vector3 startpos = platform.position;
        Vector3 previousPos = platform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            platform.position = Vector3.Lerp(startpos, endpos, t);

            previousPos = platform.position;
            yield return null;
        }

    }

    private IEnumerator MovePlatformHorizontally(Transform platform, Vector3 endpos, float duration)
    {
        canMove = false;
        Vector2 startpos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            platform.position = Vector2.Lerp(startpos, endpos, t);

            yield return null;
        }

    }
    #endregion

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

    #region Collision Triggers
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!moveOnTouch) return;
        if (collision.collider.CompareTag("Player") && !IsMoving)
        {
            Debug.Log(GetArgs());
            StartCoroutine(Movement()); // test dis
        }
    }

    private IEnumerator ResetPosition()
    {
        Debug.Log("Reset position");
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(movePlatformVertically(platform, _startPos, duration));
        _startPos = transform.position;
    }

    #endregion

    public IEnumerator Movement() // trigger movement in pole vrni platform na originalen position
    {
        _startPos = transform.position;
        
        if (verticalMovement)
        {
            IsMoving = true;
            yield return StartCoroutine(movePlatformVertically(platform, transform.position + new Vector3(0, verticalChange), duration));
        }
        else if(horizontalMovement)
            yield return StartCoroutine(MovePlatformHorizontally(platform, transform.position + new Vector3(horizontalChange, 0), duration));
        yield return ResetPosition();
        IsMoving = false;
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
