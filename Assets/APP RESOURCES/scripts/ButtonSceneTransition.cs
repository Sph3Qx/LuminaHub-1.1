using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonSceneTransition : MonoBehaviour
{
    public Button targetButton;        // The button to be clicked
    public string sceneToLoad;         // The name of the scene to load
    public Image fadeImage;            // UI Image used to fade out/in
    public float fadeDuration = 1f;    // Duration for the fade effect

    private bool isFading = false;
    private float fadeTimer = 0f;

    void Start()
    {
        // Attach the button click event listener
        if (targetButton != null)
        {
            targetButton.onClick.AddListener(OnButtonClick);
        }

        // Initially set the fadeImage to be fully transparent
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);
        }
    }

    // This method is called when the button is clicked
    void OnButtonClick()
    {
        // Start the fade out effect
        if (fadeImage != null)
        {
            isFading = true;
            fadeTimer = 0f;
        }
    }

    void Update()
    {
        // If fading is active, handle the fade effect
        if (isFading)
        {
            fadeTimer += Time.deltaTime;

            // Update the alpha value of the fadeImage (fade out)
            if (fadeImage != null)
            {
                float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
            }

            // When fade out is complete, load the scene
            if (fadeTimer >= fadeDuration)
            {
                // Load the scene smoothly after fade out
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }

    // This method is called after the scene is loaded to fade back in
    public void OnSceneLoaded()
    {
        // Reset fade image to fully transparent
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);
        }

        // Fade in after scene has loaded
        fadeTimer = 0f;
        isFading = true;
    }
}
