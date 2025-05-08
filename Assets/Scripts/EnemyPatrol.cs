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

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        currentPoint = pointB.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 point = currentPoint.position - transform.position;
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
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform) //pogleda ali je enemy prisel do pointA (Leva tocka)
        {
            currentPoint = pointB.transform; // ce je nastavi da je naslednja tocka
            transform.localScale = new Vector3(1, 1,1);
        }

    }
}
