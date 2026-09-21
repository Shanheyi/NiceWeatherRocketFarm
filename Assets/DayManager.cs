using System;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    public int currentDay = 1;
    public int maxDays = 7;
    public int enemiesRemaining = 0;

    public static event Action<int> OnDayChanged;
    public static event Action OnAllEnemiesDefeated;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
    }

    public void StartGame()
    {
        currentDay = 1;
        OnDayChanged?.Invoke(currentDay);
    }

    public void RegisterEnemy()
    {
        enemiesRemaining++;
    }

    public void EnemyDefeated()
    {
        enemiesRemaining--;
        if (enemiesRemaining <= 0)
        {
            enemiesRemaining = 0;
            OnAllEnemiesDefeated?.Invoke();
        }
    }

    public void AdvanceDay()
    {
        currentDay++;

        if (currentDay > maxDays)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameWin();
            }

            return;
        }

        OnDayChanged?.Invoke(currentDay);
    }

    public int GetMaxTierForCurrentDay()
    {
        if (currentDay <= 2)
        {
            return 1;
        }

        if (currentDay <= 4)
        {
            return 2;
        }

        if (currentDay <= 6)
        {
            return 3;
        }

        return 4;
    }

    public bool IsMaxEnemyTierUnlocked(int tier)
    {
        if (tier == 1)
        {
            return currentDay >= 1;
        }

        if (tier == 2)
        {
            return currentDay >= 3;
        }

        if (tier == 3)
        {
            return currentDay >= 5;
        }

        if (tier == 4)
        {
            return currentDay >= 7;
        }

        return false;
    }
}
