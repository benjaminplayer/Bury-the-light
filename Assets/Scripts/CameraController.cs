using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    //TODO: Naredi lepsi prehod med Scenes, fix the lvls

    [SerializeField]
    private Transform camera;
    //[SerializeField]
    private float moveOffset;

    //Colliders
    [SerializeField]
    private GameObject LeftTrigger;
    [SerializeField]
    private GameObject RightTrigger;
    [SerializeField]
    [Range(0f, 1f)]
    private float duration;


    private BoxCollider2D LeftCollider;
    private BoxCollider2D RightCollider;
    [SerializeField] private BoxCollider2D BottomCollider;
    [SerializeField] private BoxCollider2D TopCollider;
    public Vector2 entryPos;
    [SerializeField] private bool _useStartPos = true;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private GameObject player;

    #region FollowCamAttributes

    public bool FollowPlayer;
    private bool FollowPlayerX;
    public float camOffsetX = 0;
    public float camOffsetY = 0;
    public Vector3 camOffset;
    
    #endregion
    private float cameraWidth;

    public static bool IsEndOfLevel = false;
    private float cameraHeight;

    private void Awake()
    {
        camOffset = new Vector3(camOffsetX, camOffsetY);
        useStartPos(_useStartPos);
        LeftCollider = LeftTrigger.GetComponent<BoxCollider2D>();
        RightCollider = RightTrigger.GetComponent<BoxCollider2D>();

        Camera cam = camera.GetComponentInParent<Camera>();
        cameraHeight = cam.orthographicSize * 2f;

        cameraWidth = cameraHeight * Screen.width / Screen.height;

        moveOffset = cameraWidth;

        //Debug.Log(cameraWidth);

    }

    private void LateUpdate()
    {
        if (FollowPlayer)
        {
            Vector2 playerPos = player.transform.position;
            Camera.main.transform.position = new Vector3(playerPos.x + camOffsetX, playerPos.y + camOffsetY, -0.3f) + camOffset;

        }

        if (FollowPlayerX)
        { 
            Vector2 pos = player.transform.position;
            transform.position = new Vector3(pos.x, transform.position.y, -.3f); 
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.IsTouching(LeftCollider))
            {
                if (IsEndOfLevel)
                { 
                    IsEndOfLevel = false;
                }

                Debug.Log("Collision Left");
                moveCameraLeft();
                entryPos = collision.gameObject.transform.position + (Vector3.left * 2);
            }
            else if (collision.IsTouching(RightCollider))
            {
                if (!IsEndOfLevel)
                {
                    entryPos = collision.gameObject.transform.position + (Vector3.right * 2);
                    moveCameraRight();
                }
                else if (IsEndOfLevel)
                {
                    Debug.Log("End?");
                    SwitchScenes();
                    IsEndOfLevel = false;
                }
            }

            if (collision.IsTouching(BottomCollider))
            {
                collision.gameObject.transform.position = entryPos;
            }

            if (TopCollider != null && collision.IsTouching(TopCollider))
            {
                MoveCameraUp();
            }

        }
    }

    private void moveCameraRight()
    {
        Vector3 endPos = new Vector3(camera.position.x + moveOffset, camera.position.y, camera.position.z);
        StartCoroutine(moveCamera(camera.position, endPos, duration));
    }

    private void moveCameraLeft()
    {
        Vector3 endPos = new Vector3(camera.position.x - moveOffset, camera.position.y, camera.position.z);
        StartCoroutine(moveCamera(camera.position, endPos, duration));
    }

    private void MoveCameraUp() 
    {
        Vector3 endPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + cameraHeight, Camera.main.transform.position.z);
        StartCoroutine(moveCamera(Camera.main.transform.position, endPos, duration));
    }

    public IEnumerator moveCamera(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0;
        float t = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            t = (elapsed / duration);
            camera.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
    }

    private void SwitchScenes()
    {
        SceneManager.LoadScene("CavernLevel");
    }
    private void useStartPos(bool useStartPos) 
    {
        if (useStartPos)
        {
            if (startPos.Equals(new Vector3(0,0,0)))
                Camera.main.transform.position = new Vector3(-29.77f, -9.538046f, -9.981132f);
            else
                transform.position = startPos;
        }
    }

    public void SetFollowCam(bool b)
    {
        if (FollowPlayerX && b)
            FollowPlayerX = false;
        FollowPlayer = b;

    }

    public void SetFollowCamX(bool b) 
    {
        if (FollowPlayer && b)
            FollowPlayer = false;
        transform.position = new Vector3(transform.position.x, -19.21f, -.3f);
        FollowPlayerX = b;
    }

    public void SetOffsets(float camOffsetX, float camOffsetY)
    { 
        this.camOffsetX = camOffsetX;
        this.camOffsetY = camOffsetY;
    }

    public void SetOffsetX(float camOffsetX)
    {
        this.camOffsetX = camOffsetX;
    }

    public void SetOffsetY(float camOffsetY) 
    {
        this.camOffsetY = camOffsetY;
    }

    public IEnumerator LerpOffset(float duration)
    {
        Vector3 startOffset = Vector3.zero;
        Vector3 targetOffset = new Vector3(camOffsetX, camOffsetY, 0f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            camOffset = Vector3.Lerp(startOffset, targetOffset, t);
            yield return null;
        }
    }

    public IEnumerator LerpOffset(float duration, float offsetX, float offsetY)
    {
        Vector3 startOffset = Vector3.zero;
        Vector3 targetOffset = new Vector3(offsetX, offsetY, 0f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            camOffset = Vector3.Lerp(startOffset, targetOffset, t);
            yield return null;
        }
    }

    public void FixOffset(float duration)
    {
        StartCoroutine(LerpOffset(duration));
    }

    public IEnumerator ZoomCamera(float newSize, float duration) 
    {
        float startSize = Camera.main.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            Camera.main.orthographicSize = Mathf.Lerp(startSize, newSize, t);
            yield return null;
        }

        // Just to be sure it's exactly at the target
        Camera.main.orthographicSize = newSize;
    }

}

