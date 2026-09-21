using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DayNightTint : MonoBehaviour
{
    public Color nightColor = new Color(0.01f, 0.01f, 0.06f, 0.82f);
    public Color dayColor = new Color(0f, 0f, 0f, 0f);
    public float transitionDuration = 2f;

    private Image tintImage;
    private Coroutine transitionCoroutine;

    private void Awake()
    {
        tintImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        TimeManager.OnDayStart += HandleDayStart;
        TimeManager.OnNightStart += HandleNightStart;

        if (TimeManager.Instance != null)
        {
            tintImage.color = TimeManager.Instance.IsDay ? dayColor : nightColor;
        }
    }

    private void OnDisable()
    {
        TimeManager.OnDayStart -= HandleDayStart;
        TimeManager.OnNightStart -= HandleNightStart;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }
    }

    private void HandleDayStart()
    {
        StartTransition(dayColor);
    }

    private void HandleNightStart()
    {
        StartTransition(nightColor);
    }

    private void StartTransition(Color targetColor)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(TransitionToColor(targetColor));
    }

    private IEnumerator TransitionToColor(Color targetColor)
    {
        Color startColor = tintImage.color;

        if (transitionDuration <= 0f)
        {
            tintImage.color = targetColor;
            transitionCoroutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / transitionDuration);
            tintImage.color = Color.Lerp(startColor, targetColor, Mathf.SmoothStep(0f, 1f, progress));
            yield return null;
        }

        tintImage.color = targetColor;
        transitionCoroutine = null;
    }
}
