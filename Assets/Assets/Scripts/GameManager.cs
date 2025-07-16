using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score")]
    public float currentScore = 0f;
    public float highScore = 0f;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Enemy Spawning")]
    public GameObject enemyPrefab;
    public Vector3[] spawnPositions;
    public float waveInterval = 30f;
    private int enemiesPerWave = 3;
    private Coroutine waveCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScoreText = GameObject.Find("Current_Score")?.GetComponent<TextMeshProUGUI>();
        highScoreText = GameObject.Find("Highest Score")?.GetComponent<TextMeshProUGUI>();
        currentScore = 0f;
        UpdateScoreUI();

        // Optionally restart wave spawning if needed after scene load
        if (waveCoroutine == null)
        {
            waveCoroutine = StartCoroutine(SpawnWaves());
        }
    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        currentScore = 0f;
        spawnPositions = new Vector3[]
        {
            new Vector3(7, 0, 16),
            new Vector3(0, 0, 0),
            new Vector3(66, 0, -15),
            new Vector3(89, 0, -54),
            new Vector3(82, 0, 37)
        };

        UpdateScoreUI();
        waveCoroutine = StartCoroutine(SpawnWaves());
    }

    private void Update()
    {
        Player_Movement player = GetComponent<Player_Movement>();
        if (player != null)
        {
            if (player.Health <= 0f)
            {
                StopSpawning();
                Debug.Log("Player defeated! Game Over.");
            }
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount;

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", (int)highScore);
        }

        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (currentScoreText != null)
            currentScoreText.text = $"Score: {currentScore}";

        if (highScoreText != null)
            highScoreText.text = $"Highscore: {highScore}";
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(2f);

        // ✅ Initial spawn of 3 enemies only
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPos = spawnPositions[Random.Range(0, spawnPositions.Length)];
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(0.5f); // Space them out
        }

        yield return new WaitForSeconds(waveInterval);

        while (true)
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                Vector3 spawnPos = spawnPositions[Random.Range(0, spawnPositions.Length)];
                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(waveInterval);

            enemiesPerWave = 3; // ✅ Increase by 3 each wave
            waveInterval = Mathf.Max(10f, waveInterval - 2f);
        }
    }


    public void StopSpawning()
    {
        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
            waveCoroutine = null;
        }
    }
}
