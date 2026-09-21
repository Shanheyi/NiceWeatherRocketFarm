using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public UnityEngine.UI.Image iconImage;
    public float floatSpeed = 1f;
    public float lifetime = 1.2f;
    public float fadeStartTime = 0.8f;

    public void Setup(string text)
    {
        textComponent.text = text;
        StartCoroutine(FloatRoutine());
    }

    private IEnumerator FloatRoutine()
    {
        float elapsed = 0f;
        Color originalColor = textComponent.color;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            if (elapsed >= fadeStartTime)
            {
                float fadeDuration = lifetime - fadeStartTime;
                float progress = fadeDuration > 0f
                    ? Mathf.Clamp01((elapsed - fadeStartTime) / fadeDuration)
                    : 1f;
                Color color = originalColor;
                color.a = Mathf.Lerp(1f, 0f, progress);
                textComponent.color = color;

                if (iconImage != null)
                {
                    Color iconColor = iconImage.color;
                    iconColor.a = color.a;
                    iconImage.color = iconColor;
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
