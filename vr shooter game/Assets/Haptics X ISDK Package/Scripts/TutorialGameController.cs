using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TutorialGameController : MonoBehaviour
{
    private int destroyedCount = 0; // Number of destroyed objects
    [SerializeField] private List<GameObject> tutorialEnemies;
    [SerializeField] private GameObject mainGameCanvas;
    [SerializeField] private GameObject grabTheBowCanvas;
    [SerializeField] private GameObject grabTheArrowCanvas;
    [SerializeField] private GameObject activateShieldCanvas;

    public Image shootAgainImage;

    private bool isBowGrabbed;
    private bool isArrowGrabbed;
    private bool isShieldActivated;

    private void Awake()
    {
        foreach (GameObject go in tutorialEnemies)
        {
            go.SetActive(false);
        }
    }

    private void Start()
    {
        // Ensure the shield is initially disabled
        mainGameCanvas.SetActive(false);
        activateShieldCanvas.SetActive(false);
    }

    public void OnBowGrabbed()
    {
        if (!isBowGrabbed)
        {
            isBowGrabbed = true;
            grabTheBowCanvas.SetActive(false);
            activateShieldCanvas.SetActive(true);
            CheckTutorialProgress();
        }
    }

    public void OnArrowGrabbed()
    {
        if (!isArrowGrabbed)
        {
            isArrowGrabbed = true;
            grabTheArrowCanvas.SetActive(false);
            CheckTutorialProgress();
        }
    }

    public void OnSheildActivted()
    {
        if (!isShieldActivated)
        {
            isShieldActivated = true;
            activateShieldCanvas.SetActive(false);
            CheckTutorialProgress();
        }
    }

    private void CheckTutorialProgress()
    {
        if (!isBowGrabbed || !isArrowGrabbed || !isShieldActivated)
            return;
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        foreach (GameObject go in tutorialEnemies)
        {
            go.SetActive(true);
            EnemyHealth enemyHealth = go.GetComponent<EnemyHealth>();
            enemyHealth.OnKilled.AddListener(NotifyObjectDestroyed);
        }
    }

    // This method is called whenever one of the tracked objects is destroyed
    private void NotifyObjectDestroyed()
    {
        destroyedCount++;

        // Check if all objects are destroyed
        if (destroyedCount >= tutorialEnemies.Count)
        {
            ShowMainGameCanvas();
        }
    }

    private void ShowMainGameCanvas()
    {
        // Start the shield opening coroutine
        mainGameCanvas.SetActive(true); // Enable the shield object
        StartCoroutine(ShowUI(0f, mainGameCanvas.transform.localScale.x, 2));
    }

    private IEnumerator ShowUI(float startScale, float endScale, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; // Calculate interpolation factor (0 to 1)
            float currentScale = Mathf.Lerp(startScale, endScale, t); // Lerp between start and end scale
            mainGameCanvas.transform.localScale = new Vector3(currentScale, currentScale, currentScale); // Scale uniformly

            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }

        // Ensure the final scale is set
        mainGameCanvas.transform.localScale = new Vector3(endScale, endScale, endScale);
    }

    // Load the next scene
    public void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
