using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSaveManager : MonoBehaviour
{
    private const string SavedSceneKey = "SavedScene";

    private static SceneSaveManager instance;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Load the saved scene if it exists
        if (PlayerPrefs.HasKey(SavedSceneKey))
        {
            string savedScene = PlayerPrefs.GetString(SavedSceneKey);
            
            // If the saved scene is not the current one, load it
            if (SceneManager.GetActiveScene().name != savedScene)
            {
                SceneManager.LoadScene(savedScene);
            }
        }
    }

    private void OnApplicationQuit()
    {
        // Save the current scene name when the application quits
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString(SavedSceneKey, currentScene);
        PlayerPrefs.Save();
    }
}
