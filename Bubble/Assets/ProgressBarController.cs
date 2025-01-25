using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways] // Ensures the script runs in both Play Mode and Edit Mode
public class ProgressBarController : MonoBehaviour
{
    [Header("UI Components")]
    public Image progressBar; // Assign your fill Image here (0 to 1)

    [Header("Prefab to Toggle")]
    public GameObject targetPrefab; // Assign the prefab or GameObject to activate/deactivate

    [Header("Progress Settings")]
    [Range(0f, 1f)] // Makes a slider appear in the Inspector
    public float progress = 0f; // Current progress (0 to 1)

    private void Update()
    {
        // Update the fill amount of the progress bar
        if (progressBar != null)
        {
            progressBar.fillAmount = progress;
        }

        // Activate or deactivate the prefab based on progress
        if (targetPrefab != null)
        {
            targetPrefab.SetActive(progress >= 1f);
        }
    }

    private void OnValidate()
    {
        // Ensure changes in the Editor update the progress bar and prefab
        Update();
    }

    public void SetProgress(float progress)
    {
        this.progress = progress;
        Update();
    }
}