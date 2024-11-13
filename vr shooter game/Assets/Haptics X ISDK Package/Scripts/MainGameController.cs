using Oculus.Haptics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameController : MonoBehaviour
{
    private EnemySpawner[] enemySpawners;

    [SerializeField] private GameObject countdownCanvas;
    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private HapticClip gameStartHaptics;
    [SerializeField] private AudioClip gameStartAudio;
    [SerializeField] private AudioSource gameStartAudioSource;

    private TextMeshProUGUI[] textMeshProUGUIs;
    private HapticClipPlayer mainGameStartHapticsPlayer;

    private void Start()
    {
        endGameCanvas.SetActive(false);
        // Get all EnemySpawner components in the s cene

        mainGameStartHapticsPlayer = new HapticClipPlayer(gameStartHaptics);
        enemySpawners = FindObjectsOfType<EnemySpawner>();

        textMeshProUGUIs = countdownCanvas.GetComponentsInChildren<TextMeshProUGUI>();
        playerHealth.onDeath.AddListener(EndGame);

        // Start the countdown coroutine
        StartCoroutine(GameStartCountdown(4)); // 4 seconds countdown
    }

    private IEnumerator GameStartCountdown(int countdownTime)
    {
        mainGameStartHapticsPlayer.Play(Controller.Both);
        gameStartAudioSource.PlayOneShot(gameStartAudio);

        // Countdown logic
        while (countdownTime > 0)
        {
            foreach (var text in textMeshProUGUIs)
            {
                text.text = countdownTime.ToString();
            }
            yield return new WaitForSeconds(1f); // Wait for 1 second
            countdownTime--;
        }

        // Countdown finished, clear the text and start the game
        countdownCanvas.SetActive(false);
        StartGame();
    }

    private void StartGame()
    {
        foreach (EnemySpawner spawner in enemySpawners)
        {
            spawner.StartSpawningEnemies(); // Make sure this method exists in your EnemySpawner script
        }

        Debug.Log("Game Started!");
    }

    private void EndGame()
    {
        foreach (EnemySpawner spawner in enemySpawners)
        {
            spawner.StopSpawningEnemies(); // Make sure this method exists in your EnemySpawner script
        }
        endGameCanvas.SetActive(true);
        StartCoroutine(ShowUI(0, endGameCanvas.transform.localScale.x, 2));
    }

    private IEnumerator ShowUI(float startScale, float endScale, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; // Calculate interpolation factor (0 to 1)
            float currentScale = Mathf.Lerp(startScale, endScale, t); // Lerp between start and end scale
            endGameCanvas.transform.localScale = new Vector3(currentScale, currentScale, currentScale); // Scale uniformly

            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }

        // Ensure the final scale is set
        endGameCanvas.transform.localScale = new Vector3(endScale, endScale, endScale);
    }

    public void Retry()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
