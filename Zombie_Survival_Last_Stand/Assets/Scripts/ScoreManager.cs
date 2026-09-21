using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public ZombieSpawnerController zombieSpawnerController;

    public int currentScore;
    public int highScore;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Load saved high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void Start()
    {
        UpdateScore();
    }

    private void Update()
    {
        UpdateScore();
    }

    public void UpdateScore()
    {
        if (zombieSpawnerController == null)
        {
            return;
        }

        // Get current wave as score
        currentScore = zombieSpawnerController.currentWave - 1;

        // Update high score
        if (currentScore > highScore)
        {
            highScore = currentScore;

            // Save high score
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
    }
}