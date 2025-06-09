using System.Collections;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class EnemyPatrol : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Point references")]
    [Tooltip("The player will firstly move towards point B")]
    [SerializeField] private GameObject pointA; 
    [SerializeField] private GameObject pointB;
    private Rigidbody2D rb;
    private Transform currentPoint;

    [Header("Movement properties")]
    [SerializeField] private bool HorizontalMovement;
    [SerializeField] private bool VerticalMovement;
    [SerializeField] private float speed;

    [Header("Wait properties")]
    [SerializeField] private bool _WaitBeforeMoving;
    [SerializeField][Range(0.1f, 10f)] private float waitTime;

    [Header("Follow AI values")]
    [SerializeField] private bool followTarget;
    [SerializeField] private Transform target;
    [SerializeField] private float maxDetectionDistance;
    [SerializeField] private float followSpeed;
    private float distance;

    private Animator animator;

    private void Awake()
    {
        try
        {
            animator = GetComponent<Animator>();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Could Not have fetched animator\n"+ e.StackTrace);
        }
    }

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (!followTarget)
            currentPoint = pointB.transform;

        if (!followTarget)
            StartCoroutine(Movement());

    }

    // Update is called once per frame
    void Update()
    {
        if (followTarget)
        {
            distance = Vector2.Distance(transform.position, target.position);
            Vector2 direction = target.position - transform.position;
            direction.Normalize();

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


            if (distance < maxDetectionDistance)
            {
                transform.position = Vector2.MoveTowards(this.transform.position, target.position, followSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(Vector3.forward * angle);
            }

        }

    }

    private IEnumerator Movement()
    {
        while (true)
        {
            Vector2 moveDir = Vector2.zero;

            if (HorizontalMovement)
            {
                if(animator !=null)
                    animator.Play("test");
                if (currentPoint == pointB.transform)
                    moveDir = Vector2.right;
                else
                    moveDir = Vector2.left;
            }
            else if (HorizontalMovement)
            { 
                if(currentPoint == pointB.transform)
                    moveDir = Vector2.up;
                else
                    moveDir = Vector2.down;
            }


                rb.linearVelocity = moveDir * speed;

            while (Vector2.Distance(transform.position, currentPoint.position) > .5f)
            {
                yield return null;
            }

            rb.linearVelocity = Vector2.zero;

            if (_WaitBeforeMoving)
            {
                if(animator != null)
                    animator.Play("idle");
                yield return new WaitForSeconds(waitTime);
            }

            if (HorizontalMovement)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y);
            }

            currentPoint = (currentPoint == pointB.transform) ? pointA.transform : pointB.transform;

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireCube(this.transform.position, _detectionSize);
    }

}
