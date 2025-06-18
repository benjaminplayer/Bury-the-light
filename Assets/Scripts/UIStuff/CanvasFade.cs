using System.Collections;
using UnityEngine;

public class CanvasFade : MonoBehaviour
{
    private static int idx = 0;

    [SerializeField] private CanvasGroup[] _canvasGroups;
    [Range(0, 2f)][SerializeField] private float duration;

    [SerializeField] private CanvasGroup _currentScene;
    [SerializeField] private CanvasGroup _nextScene;
    public void HandleSwitch()
    {
        Debug.Log("Handle Switch called!\n idx : "+idx);

        if (idx + 1 < _canvasGroups.Length)
        {
            _currentScene = _canvasGroups[idx];
            _nextScene = _canvasGroups[idx + 1];

            StartCoroutine(HandleFade());
            
            idx++;
            if (idx + 1 == _canvasGroups.Length) idx = 0;
        }


    }

    private IEnumerator HandleFade()
    {
        yield return StartCoroutine(FadeCanvas(_currentScene, 0, duration));
        _nextScene.alpha = 0;
        _nextScene.gameObject.SetActive(true);
        yield return StartCoroutine(FadeCanvas(_nextScene, 1, duration));
    }

    private IEnumerator FadeCanvas(CanvasGroup _canvasGroup, float endValue, float duration)
    {

        float elapsed = 0;
        float startAlpha = _canvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endValue, t);
            yield return null;
        }

        _canvasGroup.alpha = endValue;

        // Only deactivate if we faded to 0
        if (endValue == 0)
            _canvasGroup.gameObject.SetActive(false);

    }
}
