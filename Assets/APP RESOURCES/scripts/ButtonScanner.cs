using System.Collections;  // For IEnumerator and coroutines
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using SFB;  // Make sure to include the StandaloneFileBrowser namespace
using System;  // Added for UnauthorizedAccessException

public class ButtonScanner : MonoBehaviour
{
    public Button firstButton;
    public Button scanButton;
    public TextMeshProUGUI statusText;
    public Image fadeImage;  // Image for fade effect

    public string sceneToLoad = "NewSceneName";  // Public field to set the scene name in the Inspector
    public string exeFileName = "GenshinImpact.exe";  // Public field to set the executable name in the Inspector

    private const string gameFoundKey = "GameFound"; // Key for saving game found status

    private bool gameFound = false;  // To store if the game is found

    // Add public color fields to customize the color
    public Color notFoundColor = Color.red;  // Color when game is not found (Red by default)
    public Color foundColor = new Color(0.0f, 1.0f, 0.0f);  // Color when game is found (Green by default)

    void Start()
    {
        CheckAndUpdateGameStatus();

        firstButton.onClick.AddListener(ShowScanButton);
        scanButton.onClick.AddListener(SelectEpicGamesFolderAndScan);
    }

    void CheckAndUpdateGameStatus()
    {
        // Load the game found status from PlayerPrefs
        gameFound = PlayerPrefs.GetInt(gameFoundKey, 0) == 1;

        if (gameFound && File.Exists(PlayerPrefs.GetString("GamePath", "")))
        {
            statusText.text = exeFileName + " was found.";
            statusText.color = foundColor;  // Set the color to 'foundColor' when the game is found
            scanButton.gameObject.SetActive(false);  // Hide scan button if game is already found
        }
        else
        {
            PlayerPrefs.SetInt(gameFoundKey, 0);  // Reset the game found status
            PlayerPrefs.Save();
            scanButton.gameObject.SetActive(true); // Show scan button if game is not found
            statusText.text = "Game not found. Please scan again.";
            statusText.color = notFoundColor;  // Set the color to 'notFoundColor' when the game is not found
        }

        // Create or override the status file in the build folder
        CreateStatusFile();
    }

    void CreateStatusFile()
    {
        string buildFolder = Path.GetDirectoryName(Application.dataPath); // Ensure it writes to the build folder
        string statusFilePath = Path.Combine(buildFolder, "GameStatus.txt");

        try
        {
            string fileContents;

            if (gameFound)
            {
                string gamePath = PlayerPrefs.GetString("GamePath", "Path not found");
                fileContents = $"Game Found\nPath: {gamePath}";
            }
            else
            {
                fileContents = "Game Not Found";
            }

            File.WriteAllText(statusFilePath, fileContents);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write status file: " + e.Message);
        }
    }

    // Show the scan button when the first button is clicked
    void ShowScanButton()
    {
        scanButton.gameObject.SetActive(true);
    }

    // Open folder dialog to select the Epic Games directory and then scan it
    void SelectEpicGamesFolderAndScan()
    {
        // Open folder dialog to select Epic Games directory using StandaloneFileBrowser
        var folderPath = StandaloneFileBrowser.OpenFolderPanel("Select the Epic Games Folder", "", false);

        if (folderPath.Length > 0)
        {
            string epicGamesDirectory = folderPath[0];

            // Display the folder being scanned
            statusText.text = "Scanning: " + epicGamesDirectory;

            // Start scanning the selected Epic Games folder
            ScanEpicGamesForGameExe(epicGamesDirectory);
        }
        else
        {
            statusText.text = "No folder selected.";
        }
    }

    // Scan the selected Epic Games folder and its subfolders for the specified executable file
    void ScanEpicGamesForGameExe(string epicGamesDirectory)
    {
        try
        {
            // Get all subdirectories in the Epic Games folder
            string[] directories = Directory.GetDirectories(epicGamesDirectory, "*", SearchOption.AllDirectories);

            foreach (string dir in directories)
            {
                // Display the current folder being scanned
                statusText.text = "Scanning: " + dir;

                // Check if directory contains the specified exe file
                string executablePath = Path.Combine(dir, exeFileName);

                if (File.Exists(executablePath))
                {
                    statusText.text = "Found Game";
                    statusText.color = foundColor;  // Set the color to 'foundColor' when the game is found
                    gameFound = true; // Set the gameFound flag to true
                    PlayerPrefs.SetInt(gameFoundKey, 1);  // Save the game found status
                    PlayerPrefs.SetString("GamePath", executablePath);  // Save the executable path
                    PlayerPrefs.Save(); // Save the preferences
                    scanButton.gameObject.SetActive(false); // Hide scan button after finding the game

                    // Update the status file
                    CreateStatusFile();

                    return; // Exit once the executable is found
                }
            }

            // If the file is not found
            statusText.text = exeFileName + " not found in selected directory.";
            statusText.color = notFoundColor;  // Set the color to 'notFoundColor' when game is not found
            gameFound = false; // Set the gameFound flag to false

            // Update the status file
            CreateStatusFile();
        }
        catch (UnauthorizedAccessException)
        {
            statusText.text = "Access denied to some directories.";
        }
    }

    // This method is called when the user clicks the button again
    public void OnButtonClick()
    {
        if (gameFound) // Only load new scene if the game is found
        {
            // Start the scene transition with a fade effect
            StartCoroutine(LoadSceneSmoothly());
        }
        else
        {
            statusText.text = "Game not found, please scan again.";
            statusText.color = notFoundColor;  // Ensure color is set correctly when game is not found
        }
    }

    // Coroutine for smooth scene loading with fade effect
    IEnumerator LoadSceneSmoothly()
    {
        // Start fading out
        yield return StartCoroutine(FadeOut());

        // Load the scene asynchronously
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncOperation.allowSceneActivation = false;  // Prevent automatic scene activation

        // Wait until the scene is fully loaded
        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                // Allow scene activation once it's ready
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        // Start fading in once the scene is activated
        yield return StartCoroutine(FadeIn());
    }

    // Fade out the screen using an Image
    IEnumerator FadeOut()
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, Mathf.Lerp(0f, 1f, time));  // Adjust alpha for fade effect
            yield return null;
        }
    }

    // Fade in the screen using an Image
    IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, Mathf.Lerp(1f, 0f, time));  // Adjust alpha for fade effect
            yield return null;
        }
    }
}
