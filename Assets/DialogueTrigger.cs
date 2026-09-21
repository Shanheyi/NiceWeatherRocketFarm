using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public static DialogueTrigger Instance;

    public string[] introLines;
    public string[] nightLines;
    public int maxNightDialogueDay = 2;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void OnEnable()
    {
        TimeManager.OnNightStart += OnNightStartHandler;
    }

    private void OnDisable()
    {
        TimeManager.OnNightStart -= OnNightStartHandler;
    }

    public void TriggerIntro()
    {
        if (introLines != null && introLines.Length > 0 && DialogueSystem.Instance != null)
        {
            DialogueSystem.Instance.ShowDialogue(introLines);
        }
    }

    private void OnNightStartHandler()
    {
        if (DayManager.Instance == null)
        {
            return;
        }

        if (DayManager.Instance.currentDay == 1 && GameManager.Instance != null)
        {
            GameManager.Instance.UnlockPistolAtNight();
        }

        if (DayManager.Instance.currentDay > maxNightDialogueDay)
        {
            return;
        }

        if (nightLines == null || nightLines.Length == 0)
        {
            return;
        }

        DialogueSystem.Instance.ShowDialogue(nightLines);
    }
}
