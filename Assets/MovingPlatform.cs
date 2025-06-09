using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform pointA;
    [Tooltip("This is the point, which platform initialy moves to")]
    public Transform pointB;
    public Transform CenterPos;
    public float moveSpeed = 2f;

    private Vector3 nextPos;

    CharacterController characterController;
    Rigidbody2D rb;
    Vector3 moveDir;

    private void Awake()
    {
        characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        nextPos = pointB.position;
        CalcDir();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);


        if (Vector2.Distance(transform.position, pointA.position) < 0.05f)
            {
            nextPos = pointB.position;
            CalcDir();
        }
        
        if (Vector2.Distance(transform.position, pointB.position) < 0.05f)
        {
            nextPos = pointA.position;
            CalcDir();
        }

    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDir * moveSpeed;
    }

    private void CalcDir()
    {
        moveDir = (nextPos - transform.position).normalized;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //collision.gameObject.transform.parent = transform;
            characterController.isOnPlatform = true;
            characterController.platRB = rb;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //collision.gameObject.transform.parent = null;
            characterController.isOnPlatform = false;
        }
    }

}
