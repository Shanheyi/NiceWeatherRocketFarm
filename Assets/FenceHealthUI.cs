using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FenceHealthUI : MonoBehaviour
{
    public Image healthBarFill;
    public TextMeshProUGUI healthText;
    public Color normalColor = Color.green;
    public Color lowColor = Color.red;
    public float lowThreshold = 0.3f;
    public Image fenceIcon;
    public Sprite healthyIcon;
    public Sprite damagedIcon;
    public Sprite dangerIcon;
    public Sprite destroyedIcon;
    public float healthyThreshold = 0.7f;
    public float damagedThreshold = 0.3f;
    public float dangerThreshold = 0.01f;
    public Color healthyColor = Color.green;
    public Color damagedColor = Color.yellow;
    public Color dangerColor = Color.red;
    public Color destroyedColor = new Color(0.3f, 0f, 0f, 1f);

    private void Start()
    {
        UpdateHealth(Fence.Instance.currentHealth, Fence.Instance.maxHealth);
    }

    private void OnEnable()
    {
        Fence.OnHealthChanged += UpdateHealth;
    }

    private void OnDisable()
    {
        Fence.OnHealthChanged -= UpdateHealth;
    }

    private void UpdateHealth(int current, int max)
    {
        float ratio = (float)current / max;
        healthBarFill.fillAmount = ratio;

        if (ratio <= 0f)
        {
            healthBarFill.color = destroyedColor;
        }
        else if (ratio < damagedThreshold)
        {
            healthBarFill.color = dangerColor;
        }
        else if (ratio < healthyThreshold)
        {
            healthBarFill.color = damagedColor;
        }
        else
        {
            healthBarFill.color = healthyColor;
        }

        if (fenceIcon != null)
        {
            if (ratio <= 0f)
            {
                fenceIcon.sprite = destroyedIcon;
            }
            else if (ratio < damagedThreshold)
            {
                fenceIcon.sprite = dangerIcon;
            }
            else if (ratio < healthyThreshold)
            {
                fenceIcon.sprite = damagedIcon;
            }
            else
            {
                fenceIcon.sprite = healthyIcon;
            }
        }

        if (healthText != null)
        {
            healthText.text = $"{current}/{max}";
        }
    }
}
