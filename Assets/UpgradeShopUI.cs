using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShopUI : MonoBehaviour
{
    public GameObject panelRoot;
    public Fence targetFence;
    public WeaponData[] allWeapons;
    public int[] weaponUnlockCosts;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI coinsText;
    public Button[] weaponButtons;
    public TextMeshProUGUI[] weaponButtonTexts;
    public Image[] weaponButtonIcons;
    public Button upgradeFenceButton;
    public TextMeshProUGUI upgradeFenceText;
    public Button healFenceButton;
    public TextMeshProUGUI healFenceText;
    public Button upgradeMoveSpeedButton;
    public TextMeshProUGUI upgradeMoveSpeedText;
    public UpgradeBar moveSpeedUpgradeBar;
    public Button upgradeFarmRangeButton;
    public TextMeshProUGUI upgradeFarmRangeText;
    public gosteavego playerController;
    public Button closeButton;
    public KeyCode closeKey = KeyCode.Escape;

    private bool pausedByShop = false;

    private void Start()
    {
        if (weaponButtons != null)
        {
            for (int i = 0; i < weaponButtons.Length; i++)
            {
                if (weaponButtons[i] == null)
                {
                    continue;
                }

                int index = i;
                weaponButtons[i].onClick.AddListener(() => OnWeaponButtonClicked(index));
            }
        }

        if (upgradeFenceButton != null)
        {
            upgradeFenceButton.onClick.AddListener(OnUpgradeFenceClicked);
        }

        if (healFenceButton != null)
        {
            healFenceButton.onClick.AddListener(OnHealFenceClicked);
        }

        if (upgradeMoveSpeedButton != null)
        {
            upgradeMoveSpeedButton.onClick.AddListener(OnUpgradeMoveSpeedClicked);
        }

        if (upgradeFarmRangeButton != null)
        {
            upgradeFarmRangeButton.onClick.AddListener(OnUpgradeFarmRangeClicked);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }
    }

    private void OnEnable()
    {
        GameManager.OnCoinsChanged += UpdateCoins;
        Fence.OnHealthChanged += UpdateFenceHealth;
        RefreshUI();
    }

    private void OnDisable()
    {
        GameManager.OnCoinsChanged -= UpdateCoins;
        Fence.OnHealthChanged -= UpdateFenceHealth;
    }

    public void Toggle()
    {
        if (panelRoot == null)
        {
            return;
        }

        bool shouldShow = !panelRoot.activeSelf;
        panelRoot.SetActive(shouldShow);

        bool gameHasStarted = TimeManager.Instance != null && TimeManager.Instance.isRunning;
        if (gameHasStarted)
        {
            Time.timeScale = shouldShow ? 0f : 1f;
            pausedByShop = shouldShow;
        }

        if (shouldShow)
        {
            RefreshUI();
        }
    }

    private void Update()
    {
        if (panelRoot != null && panelRoot.activeInHierarchy && Input.GetKeyDown(closeKey))
        {
            ClosePanel();
        }
    }

    public void ClosePanel()
    {
        if (closeButton != null)
        {
            PlayPressEffect(closeButton);
        }

        StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0.16f);
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        if (pausedByShop && TimeManager.Instance != null && TimeManager.Instance.isRunning)
        {
            Time.timeScale = 1f;
            pausedByShop = false;
        }
    }

    public void RefreshUI()
    {
        int coins = GameManager.Instance.coins;
        if (coinsText != null)
        {
            coinsText.text = $"金币: {coins}";
        }

        if (titleText != null)
        {
            titleText.text = "升级商店";
        }

        if (weaponButtons != null)
        {
            for (int i = 0; i < weaponButtons.Length; i++)
            {
                if (weaponButtons[i] == null || allWeapons == null || weaponUnlockCosts == null ||
                    i >= allWeapons.Length || i >= weaponUnlockCosts.Length || allWeapons[i] == null)
                {
                    continue;
                }

                WeaponData weapon = allWeapons[i];
                int cost = weaponUnlockCosts[i];
                bool isUnlocked = GameManager.Instance.IsWeaponUnlocked(weapon.weaponName);

                if (weaponButtonTexts != null && i < weaponButtonTexts.Length && weaponButtonTexts[i] != null)
                {
                    weaponButtonTexts[i].text = isUnlocked ? $"{weapon.displayName} - 已解锁" : $"{weapon.displayName} - {cost}金币";
                }

                weaponButtons[i].interactable = !isUnlocked;
                float alpha = isUnlocked ? 0.5f : 1f;
                SetButtonAlpha(weaponButtons[i], alpha);

                if (weaponButtonIcons != null && i < weaponButtonIcons.Length && weaponButtonIcons[i] != null)
                {
                    weaponButtonIcons[i].sprite = weapon.unlockedIcon;
                    SetImageAlpha(weaponButtonIcons[i], alpha);
                }
            }
        }

        if (targetFence != null)
        {
            int upgradeCost = targetFence.maxHealthUpgradeCost;
            if (upgradeFenceText != null)
            {
                upgradeFenceText.text = $"围栏血上限 +{targetFence.maxHealthUpgradeAmount} - {upgradeCost} 金币";
            }

            if (upgradeFenceButton != null)
            {
                upgradeFenceButton.interactable = true;
            }

            int healCost = targetFence.healCost;
            if (healFenceText != null)
            {
                healFenceText.text = $"围栏回血 +{targetFence.healAmount} - {healCost} 金币";
            }

            if (healFenceButton != null)
            {
                healFenceButton.interactable = true;
            }
        }

        if (playerController != null && upgradeMoveSpeedText != null)
        {
            int level = playerController.moveSpeedLevel;
            int maxLevel = playerController.maxMoveSpeedLevel;

            if (level >= maxLevel)
            {
                upgradeMoveSpeedText.text = $"玩家移速 Lv.{level}(满级)";
                if (upgradeMoveSpeedButton != null)
                {
                    upgradeMoveSpeedButton.interactable = false;
                }
            }
            else
            {
                upgradeMoveSpeedText.text = $"玩家移速 Lv.{level} - {playerController.moveSpeedUpgradeCost} 金币";
                if (upgradeMoveSpeedButton != null)
                {
                    upgradeMoveSpeedButton.interactable = true;
                }
            }
        }

        if (moveSpeedUpgradeBar != null && playerController != null)
        {
            moveSpeedUpgradeBar.SetLevel(playerController.moveSpeedLevel);
        }

        if (upgradeFarmRangeText != null && playerController != null)
        {
            int level = playerController.farmRangeLevel;
            int maxLevel = playerController.maxFarmRangeLevel;

            if (level >= maxLevel)
            {
                upgradeFarmRangeText.text = "种植范围 已加成";
                if (upgradeFarmRangeButton != null)
                {
                    upgradeFarmRangeButton.interactable = false;
                }
            }
            else
            {
                upgradeFarmRangeText.text = $"种植范围 Lv.{level} - {playerController.farmRangeUpgradeCost} 金币";
                if (upgradeFarmRangeButton != null)
                {
                    upgradeFarmRangeButton.interactable = true;
                }
            }
        }

    }

    public void OnWeaponButtonClicked(int index)
    {
        if (GameManager.Instance.IsWeaponUnlocked(allWeapons[index].weaponName))
        {
            return;
        }

        if (GameManager.Instance.coins < weaponUnlockCosts[index])
        {
            ToastManager.Instance.Show("金币不足");
            return;
        }

        if (GameManager.Instance.UnlockWeapon(allWeapons[index].weaponName, weaponUnlockCosts[index]))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.buySound);
            }

            RefreshUI();
        }
    }

    public void OnUpgradeFenceClicked()
    {
        PlayPressEffect(upgradeFenceButton);

        if (GameManager.Instance.coins < targetFence.maxHealthUpgradeCost)
        {
            ToastManager.Instance.Show("金币不足");
            return;
        }

        if (GameManager.Instance.SpendCoins(targetFence.maxHealthUpgradeCost))
        {
            targetFence.UpgradeMaxHealth(targetFence.maxHealthUpgradeAmount);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.buySound);
            }

            RefreshUI();
        }
    }

    public void OnHealFenceClicked()
    {
        PlayPressEffect(healFenceButton);

        if (targetFence.IsFullHealth)
        {
            ToastManager.Instance.Show("围栏已满血");
            return;
        }

        if (GameManager.Instance.coins < targetFence.healCost)
        {
            ToastManager.Instance.Show("金币不足");
            return;
        }

        if (GameManager.Instance.SpendCoins(targetFence.healCost))
        {
            targetFence.Heal(targetFence.healAmount);
            RefreshUI();
        }
    }

    public void OnUpgradeMoveSpeedClicked()
    {
        if (upgradeMoveSpeedButton != null)
        {
            PlayPressEffect(upgradeMoveSpeedButton);
        }

        if (playerController == null)
        {
            return;
        }

        if (playerController.moveSpeedLevel >= playerController.maxMoveSpeedLevel)
        {
            if (ToastManager.Instance != null)
            {
                ToastManager.Instance.Show("已满级");
            }

            return;
        }

        if (!playerController.UpgradeMoveSpeed())
        {
            if (ToastManager.Instance != null)
            {
                ToastManager.Instance.Show("金币不足");
            }

            return;
        }

        RefreshUI();
    }

    public void OnUpgradeFarmRangeClicked()
    {
        if (upgradeFarmRangeButton != null)
        {
            PlayPressEffect(upgradeFarmRangeButton);
        }

        if (playerController == null)
        {
            return;
        }

        if (playerController.farmRangeLevel >= playerController.maxFarmRangeLevel)
        {
            if (ToastManager.Instance != null)
            {
                ToastManager.Instance.Show("已加成");
            }

            return;
        }

        if (!playerController.UpgradeFarmRange())
        {
            if (ToastManager.Instance != null)
            {
                ToastManager.Instance.Show("金币不足");
            }

            return;
        }

        RefreshUI();
    }

    private void UpdateCoins(int amount)
    {
        RefreshUI();
    }

    public void UpdateFenceHealth(int currentHealth, int maxHealth)
    {
        if (panelRoot == null || !panelRoot.activeInHierarchy)
        {
            return;
        }

        RefreshUI();
    }

    public void PlayPressEffect(Button btn)
    {
        if (btn == null)
        {
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.uiClick);
        }

        StartCoroutine(PressEffectRoutine(btn));
    }

    private IEnumerator PressEffectRoutine(Button btn)
    {
        Image img = btn.GetComponent<Image>();
        if (img == null)
        {
            yield break;
        }

        Color original = img.color;
        Color pressed = original;
        pressed.a = 0.5f;

        float halfDuration = 0.08f;
        float t = 0f;

        while (t < halfDuration)
        {
            t += Time.unscaledDeltaTime;
            img.color = Color.Lerp(original, pressed, t / halfDuration);
            yield return null;
        }

        t = 0f;
        while (t < halfDuration)
        {
            t += Time.unscaledDeltaTime;
            img.color = Color.Lerp(pressed, original, t / halfDuration);
            yield return null;
        }

        img.color = original;
    }

    private void SetButtonAlpha(Button btn, float alpha)
    {
        if (btn == null)
        {
            return;
        }

        Image buttonImage = btn.GetComponent<Image>();
        if (buttonImage != null)
        {
            SetImageAlpha(buttonImage, alpha);
        }
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
