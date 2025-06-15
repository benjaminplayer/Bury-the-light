using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    #region Variable declaration
    [ColorUsage(true,true)]
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashTime = 0.2f;
    [SerializeField] private AnimationCurve _flashSpeedCurve;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Material material;

    private Coroutine _damageFlashCoRou;
    #endregion


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
            material = _spriteRenderer.material;
    }

    private IEnumerator DamageFlasher()
    {

        material.SetColor("_FlashColor", _flashColor); // nastavi flash color enemyja

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        float t = 0;

        // lerpa value od 1 (max flash) do 0
        while (elapsedTime < _flashTime)
        {
            elapsedTime += Time.deltaTime;
            t = elapsedTime / _flashTime;
            currentFlashAmount = Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(elapsedTime), t);

            material.SetFloat("_FlashAmount", currentFlashAmount); // nastavi flashAmount var na materialu

            yield return null;
        }

    }

    public void CallDamageFlash()
    {
        _damageFlashCoRou = StartCoroutine(DamageFlasher());
    }

}
