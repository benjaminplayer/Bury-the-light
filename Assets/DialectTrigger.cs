using UnityEngine;

public class DialectTrigger : MonoBehaviour
{

    [SerializeField] private GameObject Dialogue;
    public AudioClip audioClip;
    public bool scriptTrigger;
    public bool playSound;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (scriptTrigger) return;
        if (collision.CompareTag("Player"))
        {
            Dialogue.SetActive(true);
            if (playSound)
            {
                SFXManager.Instance.StopAllAudio();
                SFXManager.Instance.PlaySFXClip(audioClip, this.transform, 1f);
            }

        }
    }

    public void TriggerDialogue(bool b)
    { 
        Dialogue.SetActive(b);
        if (playSound)
        {
            SFXManager.Instance.StopAllAudio();
            SFXManager.Instance.PlaySFXClip(audioClip, this.transform, .6f);
        }
    }

}
