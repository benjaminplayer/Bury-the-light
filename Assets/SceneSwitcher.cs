using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour, IDataPresistance
{
    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime;

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Application.targetFrameRate = 60;
            DataPresistanceManager.Instance.SaveGame();
        }
    }

    public void LoadLevel(int loadIndex)
    {
        StartCoroutine(Load(loadIndex));
    }

    private IEnumerator Load(int levelIndex)
    { 
        // play anim
        transition.SetTrigger("Start"); // fade to black
        //wait
        yield return new WaitForSeconds(transitionTime);
        // load
        SceneManager.LoadScene(levelIndex);
    }

    public void ExitApp()
    { 
        Application.Quit();
    }

    public void SaveData(ref GameData gamedata)
    {
        gamedata.secretsFound = 0;
    }

    public void LoadData(GameData data)
    { 
    
    }

}
