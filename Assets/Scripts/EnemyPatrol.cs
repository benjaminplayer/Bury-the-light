using System.Collections;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    private GameObject pointA, pointB;
    private Rigidbody2D rb;
    private Transform currentPoint;

    [SerializeField]
    private float speed;
    [SerializeField] private bool HorizontalMovement;
    [SerializeField] private bool VerticalMovement;
    [SerializeField] private bool _WaitBeforeMoving;
    private bool _CanMove = true;

    [SerializeField] private bool followTarget;

    [Header("Follow AI values")]
    [SerializeField] private Transform target;
    [SerializeField] private float maxDetectionDistance;
    [SerializeField] private float followSpeed;
    private float distance;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (!followTarget)
            currentPoint = pointB.transform;
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

            return;
        }

        Vector2 point = currentPoint.position - transform.position;
        if (HorizontalMovement)
        {
            if (currentPoint == pointB.transform) // preveri ali je trenutna tocka tocka B(Desna tocka)
            {
                rb.linearVelocity = new Vector2(speed, 0); // ce je nastavi hitrost tako, da se premika v desni smeri
            }
            else
            {
                rb.linearVelocity = new Vector2(-speed, 0); // ce ni obrne smer premikanja
            }

            if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform) //pogleda ali je enemy prisel do pointB (Desna tocka)
            {
                currentPoint = pointA.transform; // ce je nastavi da je naslednja tocka A
                //transform.localScale = new Vector3(-1, 1, 1);
                transform.localScale = new Vector3(- this.transform.localScale.x, 1, 1);
            }

            if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform) //pogleda ali je enemy prisel do pointA (Leva tocka)
            {
                currentPoint = pointB.transform; // ce je nastavi da je naslednja tocka
                //transform.localScale = new Vector3(1, 1, 1);
                transform.localScale = new Vector3(-this.transform.localScale.x, 1, 1);
            }

        }
        else if (VerticalMovement)
        {
            if (_CanMove)
            { 
                if (currentPoint == pointB.transform)
                    rb.linearVelocity = new Vector2(0, speed);
                else if(currentPoint != pointB.transform)
                    rb.linearVelocity = new Vector2(0, -speed);
                if (Vector2.Distance(transform.position, currentPoint.position) < .5f && currentPoint == pointB.transform)
                {
                    if (_WaitBeforeMoving)
                    {
                        _CanMove = false;
                        StartCoroutine(WaitBeforeMoving(pointA.transform));
                    }
                    else
                        currentPoint = pointA.transform;
                }
                if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
                {
                    if (_WaitBeforeMoving)
                    {
                        _CanMove = false;
                        StartCoroutine(WaitBeforeMoving(pointB.transform));
                    }
                    else
                        currentPoint = pointB.transform;
                }            
            }else
                rb.linearVelocity = Vector2.zero;
        }

    }

    private IEnumerator WaitBeforeMoving(Transform newPoint)
    {
        yield return new WaitForSeconds(2);
        currentPoint = newPoint;
        _CanMove = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireCube(this.transform.position, _detectionSize);
    }

}
