using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int banditCount;
        public float spawnDelay = 1f;
    }

    [Header("Wave Settings")]
    public Wave[] waves;
    private int currentWave = 0;

    [Header("Spawn Settings")]
    public GameObject banditPrefab;
    public Transform[] spawnPoints;
    private List<GameObject> activeBandits = new List<GameObject>();

    [Header("UI")]
    public TextMeshProUGUI waveText;
    public float waveStartDelay = 3f;

    private bool spawning = false;
    private bool allWavesCompleted = false;
    private int totalKills = 0;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        if (!spawning && activeBandits.Count > 0)
        {
            activeBandits.RemoveAll(b => b == null);
        }

        if (!spawning && activeBandits.Count == 0 && !allWavesCompleted)
        {
            StartCoroutine(StartNextWave());
        }
    }

    private IEnumerator StartNextWave()
    {
        if (currentWave >= waves.Length)
        {
            allWavesCompleted = true;
            Debug.Log("🎉 All waves completed!");
            if (waveText != null)
                waveText.text = "All Waves Complete!";
            yield break;
        }

        spawning = true;

        Wave wave = waves[currentWave];
        Debug.Log($"Starting Wave {currentWave + 1} with {wave.banditCount} bandits");

        if (waveText != null)
            waveText.text = $"Wave {currentWave + 1}";

        yield return new WaitForSeconds(waveStartDelay);

        for (int i = 0; i < wave.banditCount; i++)
        {
            SpawnBandit();
            yield return new WaitForSeconds(wave.spawnDelay);
        }

        spawning = false;
        currentWave++;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateWave(currentWave + 1);
    }

    private void SpawnBandit()
    {
        if (banditPrefab == null || spawnPoints.Length == 0)
        {
          if (banditPrefab == null)
    Debug.LogError(" Bandit prefab not assigned to WaveManager!");
else if (spawnPoints == null || spawnPoints.Length == 0)
    Debug.LogError(" No spawn points assigned to WaveManager!");

            return;
        }

        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];

        GameObject newBandit = Instantiate(banditPrefab, spawnPoint.position, Quaternion.identity);
        activeBandits.Add(newBandit);

        Bandit bandit = newBandit.GetComponent<Bandit>();
        if (bandit != null)
            bandit.waveManager = this;
    }

    public void RegisterBanditKill()
    {
        totalKills++;
        Debug.Log($"Bandit killed! Total kills: {totalKills}");

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateKills(totalKills);
    }
}
