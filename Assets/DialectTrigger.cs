using UnityEngine;

public class DialectTrigger : MonoBehaviour
{

    [SerializeField] private GameObject Dialogue;
    public bool scriptTrigger;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (scriptTrigger) return;
        if (collision.CompareTag("Player"))
        {
            Dialogue.SetActive(true);
        }
    }

    public void TriggerDialogue(bool b)
    { 
        Dialogue.SetActive(b);
    }

}
