using TMPro;
using UnityEngine;

public class SignLogic : MonoBehaviour
{
    [SerializeField]
    private GameObject chatBubble;
    private TextMeshPro text;
    private void Awake()
    {
        if(chatBubble != null)
            chatBubble.SetActive(false);

        text = GetComponentInChildren<TextMeshPro>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            if (chatBubble.active == false)
                chatBubble.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        { 
            if(chatBubble.active == true)
                chatBubble.SetActive(false);
        }
    }

}
