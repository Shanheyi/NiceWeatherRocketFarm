using System.Collections;
using TMPro;
using UnityEngine;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;

    public GameObject toastPanel;
    public TextMeshProUGUI toastText;
    public float displayDuration = 1.5f;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    public void Show(string message)
    {
        if (toastPanel == null || toastText == null)
        {
            return;
        }

        toastText.text = message;
        toastPanel.SetActive(true);

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(displayDuration);
        toastPanel.SetActive(false);
        currentCoroutine = null;
    }
}
