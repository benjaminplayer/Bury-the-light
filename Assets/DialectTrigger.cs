using UnityEngine;

public class DialectTrigger : MonoBehaviour
{

    [SerializeField] private GameObject Dialogue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Dialogue.SetActive(true);
        }
    }

}
