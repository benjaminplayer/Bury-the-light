using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class EndCutscene : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private GameObject cam;
    [SerializeField] private CharacterController characterController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            characterController = collision.GetComponent<CharacterController>();
            StartCoroutine(ToggleAnimation());
        }
    }

    private IEnumerator ToggleAnimation()
    {
        cam = Camera.main.gameObject;
        characterController.canMove = false;

        foreach (BoxCollider2D bc in cam.GetComponentsInChildren<BoxCollider2D>())
        {
            if (bc.gameObject.name == "LeftTrigger")
            {
                bc.gameObject.SetActive(false);
            }
        }

        while (!(characterController.LastOnGroundTime > 0))
        {
            Debug.Log("Not on ground");
            yield return null;
        }

        director.Play();

        yield return new WaitForSeconds((float)director.duration);

        GameObject.FindGameObjectsWithTag("LevelLoader")[0].GetComponent<SceneSwitcher>().LoadLevel(3);

    }

}
