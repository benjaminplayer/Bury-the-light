using System.Collections;
using TMPro;
using UnityEngine;
using System.IO;
using System;

public class Dialouge : MonoBehaviour
{

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialougeText;
    //public string[] dialouge;
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
    private string _path;

    private void Awake()
    {
        _path = Application.dataPath + "/Dialogue/"+fileName;
        if (!skipDialouge)
        {
            HealthBar.SetActive(false);
            dialoguePanel.SetActive(true);
            playerAnimator = player.GetComponent<Animator>();
            fillWaitIndexes(waitIndexes);
            readFromFile();
            StartCoroutine(Typing());
        }
        else
        { 
            dialoguePanel.SetActive(false);
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

    private void readFromFile()
    {
        Debug.Log(Application.dataPath + "/Dialogue/IntroDialogue.txt");
        Debug.Log("Path aquired:" + _path);
        //StreamReader sr = new StreamReader(Application.dataPath + "/Dialogue/IntroDialogue.txt");
        StreamReader sr = new StreamReader(_path);
        try
        {
            string line;

            while ((line = sr.ReadLine()) != null)
            { 
                dialouge.Add(line); 
            }

            sr.Close();
            sr.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void fillWaitIndexes(int[] indexes)
    {
        foreach (int index in indexes) 
        {
            waitIdx.Add(index);
        }

    }

}
