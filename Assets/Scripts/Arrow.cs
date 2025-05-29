using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    [SerializeField] private Sprite _sprite;
    [SerializeField] private float speed = 30f;
    private GameObject spriteGO;
    public void Initialize(Vector3 spawnLoc, bool moveDir)
    {
        spriteGO = new GameObject("arrowSprite");
        _sprite = Resources.Load<Sprite>("Arrow/Arrow");
        SpriteRenderer sr = spriteGO.AddComponent<SpriteRenderer>();

        //parent values
        spriteGO.transform.parent = this.transform; //nastavi da je spite obj child trenutnega obj
        this.transform.position = spawnLoc;
        this.transform.rotation = Quaternion.identity; // no rotation
        this.transform.localScale = Vector3.one;       // no scale changes

        //child values
        spriteGO.transform.localPosition = Vector3.zero;
        spriteGO.transform.rotation = Quaternion.Euler(0f, 0f, -45f);
        spriteGO.transform.localScale = new Vector3(0.5f, .5f, 0.5f);

        //component init
        BoxCollider2D bc = this.AddComponent<BoxCollider2D>();
        Rigidbody2D rb = this.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        bc.isTrigger = true;

        this.tag = "Arrow";
        sr.sprite = _sprite;
        StartCoroutine(MoveArrow(moveDir));
    }

    private IEnumerator MoveArrow(bool moveRight)
    {
        Vector3 direction = moveRight ? Vector3.right : Vector3.left; // nastavi smer glede na to ali se premika levo ali desno

        if (!moveRight) spriteGO.transform.localScale *= -1;

        while (true)
        {
            transform.Translate(direction * speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;

        if (collision.CompareTag("Wall"))
        {
            StopAllCoroutines();                   // Stop the movement
            Debug.Log("Arrow hit the wall — stopped.");
            // Optional: Freeze it in place (disable Rigidbody movement)
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            // Optional: Disable the collider to prevent future triggers
            GetComponent<Collider2D>().enabled = false;
            transform.SetParent(collision.transform);
            return;
        }


        // Otherwise, destroy arrow (e.g., hits enemy)
        StopAllCoroutines();

        if (collision.CompareTag("Enemy"))
        {
            HandleEnemyHit(collision.gameObject);
        }

        Debug.Log("Arrow HIT something else!");
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    private void HandleEnemyHit(GameObject enemy)
    {
        Destroy(enemy.transform.parent.gameObject);
    }

}