using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Fence : MonoBehaviour
{
    public static Fence Instance;

    public int maxHealth = 100;
    public int currentHealth;
    public int maxHealthUpgradeCost = 100;
    public int maxHealthUpgradeAmount = 50;
    public int healCost = 20;
    public int healAmount = 30;
    public float flashDuration = 0.1f;
    public Tilemap fenceTilemap;

    public static event Action<int, int> OnHealthChanged;

    public bool IsFullHealth => currentHealth >= maxHealth;

    private Coroutine flashCoroutine;
    private Color originalTilemapColor;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (fenceTilemap != null)
        {
            originalTilemapColor = fenceTilemap.color;
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.fenceHit);
        }

        PlayFlash();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void UpgradeMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void PlayFlash()
    {
        if (fenceTilemap == null)
        {
            return;
        }

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        fenceTilemap.color = new Color(1f, 0.2f, 0.2f, 1f);
        yield return new WaitForSeconds(flashDuration);
        fenceTilemap.color = originalTilemapColor;
        flashCoroutine = null;
    }
}
