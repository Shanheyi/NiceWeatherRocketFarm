using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int coins = 0;
    public int eggplantSeedCount = 0;
    public List<string> unlockedWeapons = new List<string>();
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI gameWinText;

    public static event Action<int> OnCoinsChanged;
    public static event Action<int> OnEggplantSeedChanged;
    public static event Action<string> OnWeaponUnlocked;

    private bool isGameEnded = false;
    public bool IsGameEnded => isGameEnded;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // 每局从无武器状态开始，覆盖 Inspector 中可能保存的旧列表。
        unlockedWeapons = new List<string>();
        Instance = this;
    }

    private void Start()
    {
        OnCoinsChanged?.Invoke(coins);
        OnEggplantSeedChanged?.Invoke(eggplantSeedCount);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        OnCoinsChanged?.Invoke(coins);
    }

    public bool SpendCoins(int amount)
    {
        if (coins < amount)
        {
            return false;
        }

        coins -= amount;
        OnCoinsChanged?.Invoke(coins);
        return true;
    }

    public void AddEggplantSeed(int amount)
    {
        eggplantSeedCount += amount;
        OnEggplantSeedChanged?.Invoke(eggplantSeedCount);
    }

    public bool SpendEggplantSeed(int amount = 1)
    {
        if (eggplantSeedCount < amount)
        {
            return false;
        }

        eggplantSeedCount -= amount;
        OnEggplantSeedChanged?.Invoke(eggplantSeedCount);
        return true;
    }

    public bool UnlockWeapon(string weaponName, int cost)
    {
        if (unlockedWeapons.Contains(weaponName))
        {
            return false;
        }

        if (!SpendCoins(cost))
        {
            return false;
        }

        unlockedWeapons.Add(weaponName);
        OnWeaponUnlocked?.Invoke(weaponName);
        return true;
    }

    public void UnlockPistolAtNight()
    {
        if (unlockedWeapons.Contains("Pistol")) return;
        unlockedWeapons.Add("Pistol");
        OnWeaponUnlocked?.Invoke("Pistol");
        Debug.Log("手枪已解锁");
    }

    public bool IsWeaponUnlocked(string weaponName)
    {
        return unlockedWeapons.Contains(weaponName);
    }

    public void GameOver()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        if (gameOverPanel != null)
        {
            if (gameOverText != null && DayManager.Instance != null)
            {
                gameOverText.text = $"围栏被攻破\n你撑到了第 {DayManager.Instance.currentDay} 天";
            }

            StartCoroutine(FadeInPanel(gameOverPanel));
        }

        Time.timeScale = 0f;
        Cursor.visible = true;
    }

    public void GameWin()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        if (gameWinPanel != null)
        {
            if (gameWinText != null)
            {
                gameWinText.text = "胜利!\n你成功守住了农场!";
            }

            StartCoroutine(FadeInPanel(gameWinPanel));
        }

        Time.timeScale = 0f;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator FadeInPanel(GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        panel.SetActive(true);

        float elapsed = 0f;
        const float duration = 0.5f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}
