using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public string postProcessingTag = "PostProcessing";
    public string soundTag = "Sound";
    public string musicTag = "Music";

    // Buttons representing the toggle states
    public Button postProcessingButton;
    public Button soundButton;
    public Button musicButton;

    private bool isPostProcessingToggled;
    private bool isSoundToggled;
    private bool isMusicToggled;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        UpdateAllButtonColors();
    }

    public void OnPostProcessingButtonClicked()
    {
        isPostProcessingToggled = !isPostProcessingToggled;
        SetObjectsWithTagActive(postProcessingTag, isPostProcessingToggled);
        UpdateButtonColor(postProcessingButton, isPostProcessingToggled);
    }

    public void OnSoundButtonClicked()
    {
        isSoundToggled = !isSoundToggled;
        SetObjectsWithTagActive(soundTag, isSoundToggled);
        UpdateButtonColor(soundButton, isSoundToggled);
    }

    public void OnMusicButtonClicked()
    {
        isMusicToggled = !isMusicToggled;
        SetObjectsWithTagActive(musicTag, isMusicToggled);
        UpdateButtonColor(musicButton, isMusicToggled);
    }

    public void OnLoadSceneButtonClicked(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log("Loading scene: " + sceneName);
    }

    private void SetObjectsWithTagActive(string tag, bool isActive)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            obj.SetActive(isActive);
        }
        Debug.Log(tag + (isActive ? " enabled." : " disabled."));
    }

    private void UpdateButtonColor(Button button, bool isActive)
    {
        Color color = isActive ? Color.green : Color.red;
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        cb.highlightedColor = color;
        cb.pressedColor = color;
        button.colors = cb;
    }

    private void UpdateAllButtonColors()
    {
        if (postProcessingButton != null)
            UpdateButtonColor(postProcessingButton, isPostProcessingToggled);
        if (soundButton != null)
            UpdateButtonColor(soundButton, isSoundToggled);
        if (musicButton != null)
            UpdateButtonColor(musicButton, isMusicToggled);
    }
}
