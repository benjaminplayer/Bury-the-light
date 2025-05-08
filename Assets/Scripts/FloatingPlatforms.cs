using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
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

    private Vector3 lastpos;
    public Vector3 velocity { get; private set; }
    public float speed => velocity.magnitude;


    private void Awake()
    {
        lastpos =transform.position;

        if (horizontalMovement && !moveOnTouch && isContinous)
        {
            StartCoroutine(hoverBetweenValuesHorizontally(platform, horizontalChange, horizontalSpeed));
        }
        else if (verticalMovement && !moveOnTouch && isContinous)
            StartCoroutine(hoverBetweenValuesVertically(platform, verticalChange, verticalSpeed));

    }

    private void Update()
    {
        velocity = (transform.position - lastpos) / Time.deltaTime;
        lastpos = transform.position;

        Debug.Log("Platform speeed" + speed);
    }

    #region BasicMovements
    IEnumerator movePlatformVertically(Transform platform, float dy, float duration)
    {
        canMove = false;
        Vector3 startpos = transform.position;
        Vector3 endpos = new Vector3 (platform.position.x, platform.position.y + dy + platform.position.z);

        float elapsed = 0f;
        Vector3 previousPos = platform.position;

        while (elapsed < duration)
        { 
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            platform.position = Vector3.Lerp(startpos, endpos, t);

            previousPos = platform.position;
            yield return null;
        }

        IsMoving = false;

    }

    IEnumerator movePlatformVertically(Transform platform, Vector3 endpos, float duration)
    {
        canMove = false;
        Vector3 startpos = transform.position;

        float elapsed = 0f;
        Vector3 previousPos = platform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            platform.position = Vector3.Lerp(startpos, endpos, t);

            previousPos = platform.position;
            yield return null;
        }

        IsMoving = false;

    }

    IEnumerator movePlatformHorizontally(Transform platform, float dx, float duration) 
    {
        canMove = false;
        Vector2 startpos = transform.position;
        Vector2 endpos = new Vector2(platform.position.x + dx, platform.position.y);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            platform.position = Vector2.Lerp(startpos, endpos, t);

            yield return null;
        }
        IsMoving = false;


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
        Vector2 endpos = new Vector2(platform.position.x +dx, platform.position.y);

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

    #region TriggerBasedMovements
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            //TODO: Naredi da lahko skocis med premikom platforma
            player = collision.gameObject;
            Debug.Log(player);
            //Nastavi playerja kot child od platform -> se premika z njim -> ne clippa skozi tla
            player.transform.SetParent(platform);

            if ((horizontalMovement && !IsMoving) && canMove)
            {
                IsMoving = true;


                StartCoroutine(movePlatformHorizontally(platform, horizontalChange, duration));

            }
            else if ((verticalMovement && !IsMoving) && canMove)
            {
                IsMoving = true;
                cr = collision.GetComponent<CharacterController>();
                cr.isOnMovingPlatform = true;
                StartCoroutine(movePlatformVertically(platform, verticalChange, duration));
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player")) 
        { 
            player.transform.SetParent(null); // Ko gre iz platforma, ga odstrani kot childa

            cr = collision.GetComponent<CharacterController>();
            cr.isOnMovingPlatform = false;
            cr.platformVel = 0f;
        }
    }

    #endregion

    #region Collision Triggers
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!moveOnTouch) return;
        if(collision.collider.CompareTag("Player"))
        {
            StartCoroutine(movePlatformVertically(platform, new Vector3(transform.position.x, verticalChange, transform.position.z), duration)); // test dis

        }
    }
    #endregion

}
