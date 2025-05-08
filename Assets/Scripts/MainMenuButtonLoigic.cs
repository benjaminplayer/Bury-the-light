using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuButtonLoigic : MonoBehaviour
{

    [SerializeField] private float duration;
    [SerializeField] private GameObject toggleBtn;
    public void LoadGame()
    {
        SceneManager.LoadScene("Scene1");
    }

    public void ToggleIntroDialouge()
    {
        if (toggleBtn.GetComponent<Toggle>().isOn)
        {
            Dialouge.skipDialouge = true;
        }
        else
        {
            Dialouge.skipDialouge = false;
        }
    }

    public void ExitGame()
    { 
        Application.Quit();
    }

    /*private IEnumerator FadeTrasition()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
        }

    }*/


}
