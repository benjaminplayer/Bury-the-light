using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class FallLogic : MonoBehaviour
{
    [SerializeField] private GameObject platform;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 endCameraPos;
    [SerializeField] private GameObject player;
    [SerializeField] private PlayableDirector pd;


    #region SFX
    [SerializeField] private AudioClip woodCreak;
    [SerializeField] private AudioClip woodBreak;
    #endregion

    // 38.66
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            endCameraPos = new Vector3(player.transform.position.x, player.transform.position.y, -.3f);
            Debug.Log("Collision with the fall start");

            //Nastavi da player ne more vec na start
            transform.position = new Vector2(39.16f, transform.position.y);
            transform.GetComponent<BoxCollider2D>().isTrigger = false;


            StartCoroutine(TriggerFall());
            StartCoroutine(zoomCamera(2,5,true));
        }
    }

    private void Update()
    {
        if (player != null)
            endCameraPos = new Vector3(player.transform.position.x, player.transform.position.y, -.3f);
    }

    IEnumerator TriggerFall()
    {
        SFXManager.Instance.PlaySFXClip(woodCreak, transform, 1f);
        yield return new WaitForSeconds(woodCreak.length);
        SFXManager.Instance.PlaySFXClip(woodBreak,transform,1f);
        pd.Play();
        Destroy(platform.GetComponent<BoxCollider2D>());
        StartCoroutine(zoomCamera(.2f, 7, false));
    }


    private IEnumerator zoomCamera(float duration, float targetSize, bool zoomOnPlayer)
    {
        float startSize = Camera.main.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            Camera.main.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            if(zoomOnPlayer)
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, endCameraPos, t);
            yield return null;
        }

        // Just to be sure it's exactly at the target
        Camera.main.orthographicSize = targetSize;
        if(zoomOnPlayer)
            Camera.main.transform.GetComponent<CameraController>().SetFollowCam(true);
    }

}
