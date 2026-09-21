using TMPro;
using UnityEngine;

public class DayDisplay : MonoBehaviour
{
    public TextMeshProUGUI dayText;

    private void OnEnable()
    {
        DayManager.OnDayChanged += OnDayChangedHandler;
    }

    private void OnDisable()
    {
        DayManager.OnDayChanged -= OnDayChangedHandler;
    }

    private void Start()
    {
        if (DayManager.Instance != null)
        {
            OnDayChangedHandler(DayManager.Instance.currentDay);
        }
    }

    private void OnDayChangedHandler(int day)
    {
        dayText.text = $"第 {day} 天";
    }
}
