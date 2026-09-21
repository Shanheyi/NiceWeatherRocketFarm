using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    public Transform fenceCenter;
    public Transform fenceTarget;
    public float fenceRadius = 10f;
    public float spawnRadiusMin = 11f;
    public float spawnRadiusMax = 14f;
    public GameObject enemyPrefab;
    public EnemyData[] allEnemyTypes;
    public int baseSpawnCount = 3;
    public int spawnIncrementPerDay = 2;
    public int maxEnemies = 20;
    public float spawnInterval = 1.5f;
    public float nightStartDelay = 3f;
    public float minSpawnInterval = 0.4f;
    public int maxDays = 7;
    public bool isSpawningComplete = false;

    private bool isSpawning = false;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    private Coroutine spawnCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        TimeManager.OnNightStart += OnNightStart;
        TimeManager.OnDayStart += OnDayStart;
    }

    private void OnDisable()
    {
        TimeManager.OnNightStart -= OnNightStart;
        TimeManager.OnDayStart -= OnDayStart;
    }

    private void OnNightStart()
    {
        StartCoroutine(DelayedStartNight());
    }

    private IEnumerator DelayedStartNight()
    {
        yield return new WaitForSeconds(nightStartDelay);
        StartNight();
    }

    private void StartNight()
    {
        if (DayManager.Instance == null || isSpawning)
        {
            return;
        }

        isSpawning = true;
        isSpawningComplete = false;
        aliveEnemies.RemoveAll(enemy => enemy == null);
        int currentDay = DayManager.Instance.currentDay;
        float dayFactor = maxDays > 1
            ? Mathf.Clamp01((float)(currentDay - 1) / (maxDays - 1))
            : 0f;
        float actualInterval = Mathf.Lerp(spawnInterval, minSpawnInterval, dayFactor);
        Debug.Log($"第 {currentDay} 天，生成间隔 = {actualInterval:F2} 秒");

        int count = baseSpawnCount + (currentDay - 1) * spawnIncrementPerDay;
        int maxTier = DayManager.Instance.GetMaxTierForCurrentDay();
        spawnCoroutine = StartCoroutine(SpawnRoutine(count, maxTier, actualInterval));
    }

    private IEnumerator SpawnRoutine(int totalCount, int maxTier, float interval)
    {
        for (int i = 0; i < totalCount; i++)
        {
            if (aliveEnemies.Count >= maxEnemies)
            {
                break;
            }

            List<EnemyData> available = new List<EnemyData>();
            foreach (EnemyData enemyType in allEnemyTypes)
            {
                if (enemyType.spawnTier <= maxTier)
                {
                    available.Add(enemyType);
                }
            }

            if (available.Count == 0)
            {
                isSpawning = false;
                isSpawningComplete = true;
                spawnCoroutine = null;
                yield break;
            }

            EnemyData selected = available[Random.Range(0, available.Count)];
            GameObject enemyObject = Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity);
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.data = selected;
                enemy.fenceTarget = fenceTarget;
            }

            aliveEnemies.Add(enemyObject);
            yield return new WaitForSeconds(interval);
        }

        isSpawning = false;
        isSpawningComplete = true;
        spawnCoroutine = null;
    }

    private void OnDayStart()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
            isSpawning = false;
            isSpawningComplete = true;
        }

        foreach (GameObject enemy in aliveEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        aliveEnemies.Clear();
    }

    private Vector3 GetSpawnPosition()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float r = Random.Range(spawnRadiusMin, spawnRadiusMax);
        return fenceCenter.position + new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0f);
    }
}
