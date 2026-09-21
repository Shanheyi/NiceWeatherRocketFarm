using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public static event Action OnGameStart;

    public GameObject startPanel;
    public GameObject[] persistentUIs;
    public GameObject[] eventUIs;
    public Button playButton;
    public Sprite playNormalSprite;
    public Sprite playPressedSprite;

    private Image playButtonImage;
    private Coroutine hideUIsCoroutine;
    private bool isStarting;

    private void Start()
    {
        Debug.Log("StartMenu Start");
        Time.timeScale = 0f;

        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }

        HideGameplayUIs();
        if (startPanel != null)
        {
            Debug.Log($"StartPanel active: {startPanel.activeInHierarchy}");
        }

        Cursor.visible = true;

        if (playButton != null)
        {
            playButtonImage = playButton.GetComponent<Image>();
            SetPlayButtonSprite(playNormalSprite);

            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnPlayClicked);
        }

        hideUIsCoroutine = StartCoroutine(HideUIsForFrames(5));
    }

    private IEnumerator HideUIsForFrames(int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            HideGameplayUIs();
            yield return null;
        }
        hideUIsCoroutine = null;
    }

    private void HideGameplayUIs()
    {
        SetUIActive(persistentUIs, false);
        SetUIActive(eventUIs, false);
    }

    public void OnPlayClicked()
    {
        if (isStarting) return;
        isStarting = true;
        Debug.Log("Play clicked 1");

        if (hideUIsCoroutine != null)
        {
            StopCoroutine(hideUIsCoroutine);
            hideUIsCoroutine = null;
        }

        if (startPanel != null)
        {
            startPanel.SetActive(false);
            Debug.Log("startPanel hidden");
        }
        else
        {
            Debug.LogError("startPanel is null!");
        }

        SetUIActive(persistentUIs, true);
        Debug.Log($"Showed {persistentUIs?.Length ?? 0} persistent UIs");
        Time.timeScale = 0f;
        Debug.Log($"timeScale = {Time.timeScale}");
        Cursor.visible = false;

        StartCoroutine(StartAfterDialogue());
    }

    private IEnumerator StartAfterDialogue()
    {
        if (DialogueTrigger.Instance != null)
        {
            DialogueTrigger.Instance.TriggerIntro();
            Time.timeScale = 0f;
        }

        while (DialogueSystem.Instance != null && DialogueSystem.Instance.IsActive)
        {
            yield return null;
        }

        Time.timeScale = 1f;

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.StartGame();
            Debug.Log("TimeManager started");
        }

        if (DayManager.Instance != null)
        {
            DayManager.Instance.StartGame();
            Debug.Log("DayManager started");
        }

        OnGameStart?.Invoke();
    }

    private void SetUIActive(GameObject[] uiObjects, bool isActive)
    {
        if (uiObjects == null)
        {
            return;
        }

        foreach (GameObject ui in uiObjects)
        {
            if (ui != null)
            {
                ui.SetActive(isActive);
            }
        }
    }

    private void SetPlayButtonSprite(Sprite sprite)
    {
        if (playButtonImage != null && sprite != null)
        {
            playButtonImage.sprite = sprite;
        }
    }
}
