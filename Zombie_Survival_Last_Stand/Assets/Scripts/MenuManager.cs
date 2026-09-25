using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    public TextMeshProUGUI highScore;
    public int highScoreValue;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        highScoreValue = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SetCursorForMenu();
        }
    }

    private void SetCursorForMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Start the game
    public void StartGame()
    {
        SceneManager.LoadScene(1);

    }

    // Exit the game
    public void Exit()
    {
        Application.Quit();

        // This message only works in the Unity Editor
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // Restart the current game scene
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Return to the main menu
    public void ReturnHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Update()
    {
        highScore.text = "Top Wave Survived : "+ highScoreValue;
    }

}