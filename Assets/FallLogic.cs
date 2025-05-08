using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FallLogic : MonoBehaviour
{
    [SerializeField] private Transform platform;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 endCameraPos;
    [SerializeField] private GameObject player;
    // 38.66
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            endCameraPos = new Vector3(player.transform.position.x, player.transform.position.y, -.3f);
            Debug.Log("Collision with the fall start");
            transform.position = new Vector2(39.16f, transform.position.y);
            transform.GetComponent<BoxCollider2D>().isTrigger = false;
            StartCoroutine(TriggerFall(3));
            StartCoroutine(zoomCamera(2,5,true));
        }
    }

    private void Update()
    {
        if (player != null)
            endCameraPos = new Vector3(player.transform.position.x, player.transform.position.y, -.3f);
    }

    IEnumerator TriggerFall(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartCoroutine(zoomCamera(.2f, 7, false));
        StartCoroutine(DropPlatform());
        StartCoroutine(FixOffset());
    }

    private IEnumerator DropPlatform()
    {

        float elapsedTime = 0f;
        Vector2 moveDirection = new Vector2(0f, -1f).normalized; // down
        while (elapsedTime < 2f)
        { 
            elapsedTime += Time.deltaTime;
            platform.transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
            yield return null;
        }

        moveDirection = new Vector2(1f, -1f); //down right;

        while (true)
        {
            platform.transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
            yield return null;
        }

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

    private IEnumerator FixOffset() 
    { 
        yield return new WaitForSeconds(1);
        Camera.main.transform.GetComponent<CameraController>().SetOffsetY(-2);
        Camera.main.transform.GetComponent<CameraController>().FixOffset(1);
    }

}
