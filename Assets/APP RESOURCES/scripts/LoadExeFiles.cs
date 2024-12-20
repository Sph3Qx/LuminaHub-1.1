using System.Collections;
using System.Diagnostics;  // System.Diagnostics is used for Process
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SFB;  // Make sure to include the StandaloneFileBrowser namespace
using System.IO; // Required for UnauthorizedAccessException

public class LoadExeFiles : MonoBehaviour
{
    public Button selectExe1Button;
    public Button selectExe2Button;
    public Button loadButton;
    public Text exe1PathText;
    public Text exe2PathText;

    private string exe1Path = "";
    private string exe2Path = "";

    private const string exe1Key = "Exe1Path";
    private const string exe2Key = "Exe2Path";

    void Start()
    {
        // Load saved paths
        exe1Path = PlayerPrefs.GetString(exe1Key, "");
        exe2Path = PlayerPrefs.GetString(exe2Key, "");

        if (!string.IsNullOrEmpty(exe1Path))
        {
            exe1PathText.text = "Exe 1: " + exe1Path;
        }

        if (!string.IsNullOrEmpty(exe2Path))
        {
            exe2PathText.text = "Exe 2: " + exe2Path;
        }

        // Set up button listeners
        selectExe1Button.onClick.AddListener(() => SelectExe(1));
        selectExe2Button.onClick.AddListener(() => SelectExe(2));
        loadButton.onClick.AddListener(LoadExes);
    }

    void SelectExe(int exeNumber)
    {
#if UNITY_EDITOR
        string selectedExe = UnityEditor.EditorUtility.OpenFilePanel("Select an EXE file", "", "exe");
#else
        // Open file using StandaloneFileBrowser for the built version
        string selectedExe = OpenFilePicker();
#endif

        if (!string.IsNullOrEmpty(selectedExe))
        {
            if (exeNumber == 1)
            {
                exe1Path = selectedExe;
                exe1PathText.text = "Exe 1: " + exe1Path;
                PlayerPrefs.SetString(exe1Key, exe1Path);
            }
            else if (exeNumber == 2)
            {
                exe2Path = selectedExe;
                exe2PathText.text = "Exe 2: " + exe2Path;
                PlayerPrefs.SetString(exe2Key, exe2Path);
            }

            PlayerPrefs.Save();
        }
        else
        {
            UnityEngine.Debug.LogWarning("No EXE file selected.");
        }
    }

    // Replace this method with a runtime file picker using StandaloneFileBrowser
    string OpenFilePicker()
    {
        // Open a file panel for EXE files using StandaloneFileBrowser
        var paths = StandaloneFileBrowser.OpenFilePanel("Select an EXE file", "", "exe", false);
        
        if (paths.Length > 0)
        {
            return paths[0];  // Return the selected file path
        }
        else
        {
            return ""; // No file selected
        }
    }

    void LoadExes()
    {
        if (!File.Exists(exe1Path) || !File.Exists(exe2Path))
        {
            UnityEngine.Debug.LogError("One or both EXE paths are invalid.");
            return;
        }

        StartCoroutine(LoadExeSequence());
    }

    IEnumerator LoadExeSequence()
    {
        // Start the first EXE
        StartExe(exe1Path);

        // Wait for 3 seconds
        yield return new WaitForSeconds(1);

        // Start the second EXE
        StartExe(exe2Path);
    }

    void StartExe(string exePath)
    {
        string directory = Path.GetDirectoryName(exePath);
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = exePath,
            WorkingDirectory = directory,
        };

        Process.Start(startInfo);
        UnityEngine.Debug.Log("Started: " + exePath);
    }
}
