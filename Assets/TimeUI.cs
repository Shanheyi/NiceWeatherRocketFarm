using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    public Image dayNightIcon;
    public Sprite dayIcon;
    public Sprite nightIcon;
    public TextMeshProUGUI countdownText;

    private void Update()
    {
        if (TimeManager.Instance == null)
        {
            return;
        }

        dayNightIcon.sprite = TimeManager.Instance.IsDay ? dayIcon : nightIcon;

        if (TimeManager.Instance.IsDay)
        {
            float remaining = TimeManager.Instance.dayDuration * (1f - TimeManager.Instance.PhaseProgress);
            countdownText.text = Mathf.CeilToInt(remaining).ToString() + "s";
        }
        else
        {
            countdownText.text = "?";
        }
    }
}
