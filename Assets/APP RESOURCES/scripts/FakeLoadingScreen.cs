using UnityEngine;
using UnityEngine.UI; // To access the Slider
using UnityEngine.SceneManagement; // To manage scenes

public class FakeLoadingScreen : MonoBehaviour
{
    public Slider loadingSlider; // Reference to the slider
    public string sceneToLoad = "NextScene"; // The scene to load after the loading is complete

    private void Start()
    {
        // Make sure the slider starts at 0
        loadingSlider.value = 0;
        StartCoroutine(FakeLoadingProcess());
    }

    private System.Collections.IEnumerator FakeLoadingProcess()
    {
        // Fake loading process
        float targetValue = 1f; // Fully loaded
        while (loadingSlider.value < targetValue)
        {
            loadingSlider.value += 0.01f; // Increase slider value by 1% every frame
            yield return new WaitForSeconds(0.02f); // Wait for a short period to simulate loading time
        }

        // After the slider is filled, load the next scene
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        // Load the scene by its name
        SceneManager.LoadScene(sceneToLoad);
    }
}
