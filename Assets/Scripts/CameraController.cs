using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    //TODO: Naredi lepsi prehod med Scenes, fix the lvls

    #region Camera Values
    private Camera cam;
    private readonly float tilesCount = 36; //koliko tiles je vidnih na sirino mape
    private float newSize = 0, ascpet = 0, moveOffset, cameraWidth, cameraHeight;
    #endregion

    [SerializeField][Range(0f, 1f)][Header("Camera Lerp Duration")] private float duration;

    #region Collider references
    [Header("Collider references")]
    [SerializeField] private BoxCollider2D LeftCollider;
    [SerializeField] private BoxCollider2D RightCollider;
    [SerializeField] private Collider2D BottomCollider;
    [SerializeField] private Collider2D TopCollider;
    #endregion
    
    [Header("Other values")]
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

    #region Camera Move Check values
    private bool isMoving = false;
    private Collider2D _lastColliderTouched = null;
    private GameObject moveCheckColldier = null;
    private Collider2D playerCollider = null;
    #endregion

    Vector2 boxSize = Vector2.zero;
    Vector2 boxCenter = Vector2.zero;

    [SerializeField] private AudioClip clip;

    public static bool IsEndOfLevel = false;

    private void Awake()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "CavernLevel":
                SFXManager.Instance.PlaySFXClip(clip,this.transform, .5f, true);
                break;
            default:
                break;
        }
        


        cam = this.GetComponent<Camera>();

        camOffset = new Vector3(camOffsetX, camOffsetY);
        useStartPos(_useStartPos);

        //nastavi scale kamere glede na resolution mobilne naprave
        ascpet = (float)Screen.width / Screen.height;
        newSize = (tilesCount / ascpet) / 2f;

        //nastavi move offset za premik
        cameraHeight = cam.orthographicSize * 2f;
        cam.orthographicSize = newSize;
        cameraWidth = cameraHeight * Screen.width / Screen.height;
        moveOffset = cameraWidth;

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
            if (playerCollider == null) playerCollider = collision;
            Debug.Log("Collided with player!");

            if (!CheckCameraMove(collision)) return;

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

        Debug.Log("Updated last touching collider: " + _lastColliderTouched);

    }

    private bool CheckCameraMove(Collider2D collision)
    {
        if (collision.IsTouching(RightCollider))
        {
            boxCenter = RightCollider.bounds.center + new Vector3(.25f, 0);
            boxSize = RightCollider.bounds.size;
        }
        else
        { 
            boxCenter = LeftCollider.bounds.center - new Vector3(.25f, 0);
            boxSize = LeftCollider.bounds.size;
        }
        
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize,0f, LayerMask.GetMask("Player"));

        return hit == null; // returna ali je player v boxu in nulla vrednost
    }

    private void moveCameraRight()
    {
        Vector3 endPos = new Vector3(this.transform.position.x + moveOffset, this.transform.position.y, this.transform.position.z);
        _lastColliderTouched = LeftCollider;
        StartCoroutine(moveCamera(this.transform.position, endPos, duration));
    }

    private void moveCameraLeft()
    {
        Vector3 endPos = new Vector3(this.transform.position.x - moveOffset, this.transform.position.y, this.transform.position.z);
        _lastColliderTouched = RightCollider;
        StartCoroutine(moveCamera(this.transform.position, endPos, duration));
    }

    private void MoveCameraUp() 
    {
        Vector3 endPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + cameraHeight, Camera.main.transform.position.z);
        StartCoroutine(moveCamera(Camera.main.transform.position, endPos, duration));
    }

    public IEnumerator moveCamera(Vector3 startPos, Vector3 endPos, float duration)
    {
        isMoving = true;
        float elapsed = 0;
        float t = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            t = (elapsed / duration);
            this.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        isMoving = false;

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

    public void SetOffsetY(float camOffsetY) 
    {
        this.camOffsetY = camOffsetY;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

}

