using UnityEngine;

public class GroundWisp : MonoBehaviour
{
    [SerializeField] private CameraController camCon;
    [SerializeField] private Animator animator;
    private Vector2 startPos;

    private void Awake()
    {
        camCon = Camera.main.GetComponent<CameraController>();
        startPos = camCon.entryPos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if(animator == null) //dobi playerjev animator component, samo ce ta ne obstaja, da se prepreci nepomembne inicializacije
                animator = collision.gameObject.GetComponent<Animator>();

            startPos = camCon.entryPos; // posodobitev koordinat zacetka levela
            collision.collider.gameObject.transform.position = startPos; // ce je igralec zadet, ga vrne na zacetek levela
            animator.SetTrigger("isHurt"); // predvaja animacijo
        }
    }

}
