using TMPro;
using UnityEngine;

public class CoinDisplay : MonoBehaviour
{
    private TextMeshProUGUI coinText;

    private void Awake()
    {
        coinText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        GameManager.OnCoinsChanged += UpdateCoins;
    }

    private void OnDisable()
    {
        GameManager.OnCoinsChanged -= UpdateCoins;
    }

    private void UpdateCoins(int amount)
    {
        coinText.text = $"金币: {amount}";
    }
}
