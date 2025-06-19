using System.Collections;
using TMPro;
using UnityEngine;
using System.IO;
using System;

public class Dialouge : MonoBehaviour
{

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialougeText;

    public ArrayList dialouge = new ArrayList();
    private ArrayList waitIdx = new ArrayList();
    private int index;

    public GameObject button;
    public int[] waitIndexes = {6, 9};

    [SerializeField]
    private GameObject player;

    public static bool skipDialouge = false;

    private Animator playerAnimator;
    public float wordSpeed;

    [SerializeField]
    private GameObject HealthBar;
    
    [SerializeField]
    private string fileName;

    private void OnEnable()
    {
        //_path = Application.dataPath + "/Dialogue/"+fileName;
        TextAsset textAsset = Resources.Load<TextAsset>("Dialogues/"+fileName);
        string content = textAsset.text;

        ConvertToArrayList(content);

        if (!skipDialouge)
        {
            HealthBar.SetActive(false);
            dialoguePanel.SetActive(true);
            playerAnimator = player.GetComponent<Animator>();
            fillWaitIndexes(waitIndexes);
            StartCoroutine(Typing());
        }
        else
        { 
            dialoguePanel.SetActive(false);
        }
    }

    private void ConvertToArrayList(string data)
    {
        string[] parts = data.Split(new[] { "\r\n" }, StringSplitOptions.None); // splitta string na new line characters \n -> new line (10) \r -> carriage return (13)

        foreach (string s in parts)
        {
            dialouge.Add(s);
        }

    }

    public void NextLine()
    {
        Debug.Log("Thy next line shall be read!");
        button.SetActive(false);

        if (index < dialouge.Count - 1)
        {
            index++;
            dialougeText.text = "";
            if (waitIdx.Contains(index))
            {
                Debug.Log("Here");
                StartCoroutine(Wait(3));
            }
            else
                StartCoroutine(Typing());
        }
        else{
            Clear();
            player.GetComponent<CharacterController>().enabled = true;
            HealthBar.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    private void Clear() 
    { 
        dialougeText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
    }

    IEnumerator Typing()
    {
        if (player != null) 
        {
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            playerAnimator.SetBool("isMoving", false);
            playerAnimator.SetBool("isRunning", false);
            playerAnimator.SetBool("isGrounded",true);
            player.GetComponent<CharacterController>().enabled = false;
        }
        foreach (char letter in (dialouge.ToArray(typeof(string)) as string[])[index].ToCharArray())
        {
            dialougeText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        button.SetActive(true);
        Debug.Log("Activate thy button");
    }

    IEnumerator Wait(float duration) 
    { 
        yield return new WaitForSeconds(duration);
        StartCoroutine(Typing());
    }

    private void fillWaitIndexes(int[] indexes)
    {
        foreach (int index in indexes) 
        {
            waitIdx.Add(index);
        }

    }

}
