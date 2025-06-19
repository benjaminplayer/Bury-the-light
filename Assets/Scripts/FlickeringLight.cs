using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]

public class FlickeringLight : MonoBehaviour
{
    private Light2D light;
    [SerializeField, Range(0f, 3f)] private float minIntensity = 0.5f;
    [SerializeField, Range(0f, 3f)] private float maxIntensity = 1.2f;
    [SerializeField, Range(0, 2f)] private float minFlickerTime = 0f;
    [SerializeField, Range(0f, 2f)] private float maxFlickerTime = .1f;

    private void Awake()
    {
        if (light == null)
        { 
            light = GetComponent<Light2D>();
        }

        ValidateIntesnityBounds();

        StartCoroutine(Flicker());

    }

    private void ValidateIntesnityBounds()
    {
        if (!(minIntensity > maxIntensity)) return;
        Debug.LogWarning("Min intenity is greater than max intansity! Swapping value");
        (minIntensity, maxIntensity) = (maxIntensity, minIntensity);
    }



    // linearno interpolira med flickers
    private IEnumerator Flicker()
    {
        while (true)
        { 
            float elapsed = 0, t;
            float start = light.intensity;
            float duration = Random.Range(minFlickerTime, maxFlickerTime);
            float endIntesity = Random.Range(minIntensity, maxIntensity);
            float easedT;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                t = (elapsed / duration);
                easedT = Mathf.SmoothStep(0, 1, t);
                light.intensity = Mathf.Lerp(start, endIntesity, easedT);

               yield return null;
            }
            light.intensity = endIntesity;
            
            yield return null;
        }


    }

}
