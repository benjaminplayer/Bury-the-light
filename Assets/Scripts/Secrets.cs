using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Secrets : MonoBehaviour
{

    #region Tilemaps
    [SerializeField]
    private Tilemap hiddenWall;
    [SerializeField]
    private Tilemap secretPlatform;
    #endregion

    [SerializeField]
    private Color tilemapColor;

    [SerializeField]
    private const float platformEndY = 2.059999f - 12.64f;
    [SerializeField]
    private GameData GameData;

    private bool isFirstEnter = false;
    private void Awake()
    {
        tilemapColor = hiddenWall.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (isFirstEnter) 
        {
            isFirstEnter = false;
            GameData.secretsFound++;
        }

        if (collision.tag.Equals("Player"))
        {
            StartCoroutine(FadeWall(1f ,0 , 0.1f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            Debug.Log("platfrom y: "+secretPlatform.transform.position.y);
            StartCoroutine(movePlatform(secretPlatform.transform,secretPlatform.transform.position.y, platformEndY, .2f));
            StartCoroutine(FadeWall(0, 1f, 0.1f));

        }
    }

    #region WallFade
    IEnumerator FadeWall(float startOppacity, float endOppacity, float duration)
    {
        float elapsed = 0f;
        tilemapColor = hiddenWall.color;

        while (elapsed < duration)
        { 
            elapsed+= Time.deltaTime;
            float newAlpha = Mathf.Lerp(startOppacity, endOppacity, elapsed / duration);
            tilemapColor.a = newAlpha;
            hiddenWall.color = tilemapColor;

            yield return null;
        }

        tilemapColor.a = endOppacity;
        hiddenWall.color = tilemapColor;
    }
    #endregion

    #region MovePlatform
    IEnumerator movePlatform(Transform platform, float start, float end, float duration) 
    {
        float elapsed = 0f;
        Vector3 startPos = platform.position;
        Vector3 target = new Vector3(startPos.x, end, startPos.z);

        while (elapsed < duration)
        { 
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            platform.position = Vector3.Lerp(startPos, target, t);

            yield return null; // pocaka na naslednji frame
        }

        platform.position = target;

    }
    #endregion
}
