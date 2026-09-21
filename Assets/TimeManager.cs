using System;
using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public float dayDuration = 60f;
    public float nightDuration = 40f;
    public bool isRunning = false;

    public enum Phase { Day, Night }

    public Phase CurrentPhase { get; private set; }

    public static event Action OnDayStart;
    public static event Action OnNightStart;

    public bool IsDay => CurrentPhase == Phase.Day;
    public bool IsNight => CurrentPhase == Phase.Night;

    public float PhaseProgress
    {
        get
        {
            if (CurrentPhase != Phase.Day)
            {
                return -1f;
            }

            return dayDuration <= 0f ? 1f : Mathf.Clamp01((Time.time - phaseStartTime) / dayDuration);
        }
    }

    private float phaseStartTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
    }

    public void StartGame()
    {
        if (isRunning)
        {
            return;
        }

        isRunning = true;
        StartCoroutine(CycleRoutine());
    }

    private IEnumerator CycleRoutine()
    {
        while (true)
        {
            CurrentPhase = Phase.Day;
            phaseStartTime = Time.time;
            OnDayStart?.Invoke();
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBGM(AudioManager.Instance.dayBGM);
            }

            yield return new WaitForSeconds(dayDuration);

            CurrentPhase = Phase.Night;
            phaseStartTime = Time.time;
            OnNightStart?.Invoke();
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBGM(AudioManager.Instance.nightBGM);
            }

            yield return new WaitForSeconds(nightDuration);

            while (true)
            {
                bool spawnDone = EnemySpawner.Instance == null || EnemySpawner.Instance.isSpawningComplete;
                bool allDead = DayManager.Instance.enemiesRemaining <= 0;

                if (spawnDone && allDead)
                {
                    break;
                }

                yield return null;
            }

            DayManager.Instance.AdvanceDay();
        }
    }
}
