using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class LevelBounds : MonoBehaviour
{
    public CinemachineCamera cineCam;
    [SerializeField]
    private Camera fixedCam, mainCam;


    [SerializeField]
    private bool isStaticLevel = true;
    [SerializeField]
    private bool entered = false;

    public int level = 1;

    //TODO Fix camera stopping

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!entered)
        {
            entered = true;
            updateCamera(collision);
        }
        else
        {
           entered = false;
           enableFollowCam(collision);
        }
    }

    private void LateUpdate()
    {

    }

    private void updateCamera(Collider2D collision) 
    {
        if (collision.tag == "Player" && isStaticLevel)
        {
            cineCam.gameObject.SetActive(false);
            fixedCam.gameObject.SetActive(true);
            mainCam.gameObject.SetActive(false);
        }
    }

    private void enableFollowCam(Collider2D collison) {
        if (collison.tag.Equals("Player") && isStaticLevel) 
        {
            mainCam.gameObject.SetActive(true);
            fixedCam.gameObject.SetActive(false);
            cineCam.gameObject.SetActive(true);
        }
    }

}
