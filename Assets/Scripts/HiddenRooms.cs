using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenRooms : MonoBehaviour
{

    private bool isFirstEnter = true;

    #region Tilemaps
    [SerializeField]
    private Tilemap hiddenWall;
    #endregion

    [SerializeField]
    private Color tilemapColor;

    [SerializeField]
    private bool hasCollectable;
    [SerializeField]
    private GameObject collectable;

    private void Awake()
    {
        if(collectable != null)
            collectable.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            if(hasCollectable)
                collectable.SetActive(true);

            if (isFirstEnter)
            {
                isFirstEnter = false;
                //add counter for secrets
            }

            if (collision.tag.Equals("Player"))
            {
                StartCoroutine(FadeWall(1f, 0, 0.1f));
            }        
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        { 
            StartCoroutine(FadeWall(0, 1f, 0.1f));
            if (hasCollectable)
                collectable.SetActive(false);
        }
    }

    #region WallFade
    IEnumerator FadeWall(float startOppacity, float endOppacity, float duration)
    {
        float elapsed = 0f;
        tilemapColor = hiddenWall.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startOppacity, endOppacity, elapsed / duration);
            tilemapColor.a = newAlpha;
            hiddenWall.color = tilemapColor;

            yield return null;
        }

        tilemapColor.a = endOppacity;
        hiddenWall.color = tilemapColor;
    }
    #endregion

}
