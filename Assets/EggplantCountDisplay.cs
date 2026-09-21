using TMPro;
using UnityEngine;

public class EggplantCountDisplay : MonoBehaviour
{
    public TextMeshProUGUI countText;

    private void Awake()
    {
        if (countText == null)
        {
            countText = GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnEnable()
    {
        GameManager.OnEggplantSeedChanged += OnCountChanged;
    }

    private void OnDisable()
    {
        GameManager.OnEggplantSeedChanged -= OnCountChanged;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            OnCountChanged(GameManager.Instance.eggplantSeedCount);
        }
    }

    private void OnCountChanged(int count)
    {
        if (countText == null)
        {
            return;
        }

        if (count <= 0)
        {
            countText.text = "";
        }
        else
        {
            countText.text = count.ToString();
        }
    }
}
