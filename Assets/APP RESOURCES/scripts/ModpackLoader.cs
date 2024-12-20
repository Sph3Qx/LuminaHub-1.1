using System.IO;
using System.Diagnostics; // Used for process handling
using UnityEngine;
using UnityEngine.UI;

public class ModpackLoader : MonoBehaviour
{
    [SerializeField] private Button[] buttons; // Assign your buttons via the Unity Inspector
    private string modpackFolderPath;

    void Start()
    {
        // Define the Modpack folder path relative to the build game directory
        modpackFolderPath = Path.Combine(Application.dataPath, "../Modpack");

        // Add click event listeners to all buttons
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClick(button.name));
        }
    }

    void OnButtonClick(string buttonName)
    {
        // Construct the expected folder paths
        string modFolderPath = Path.Combine(modpackFolderPath, buttonName);
        string migotoFolderPath = Path.Combine(modFolderPath, "3dmigoto");
        string exePath = Path.Combine(migotoFolderPath, "3DMigoto Loader.exe");

        // Check if the folder structure exists
        if (Directory.Exists(modpackFolderPath))
        {
            if (Directory.Exists(modFolderPath))
            {
                if (Directory.Exists(migotoFolderPath))
                {
                    if (File.Exists(exePath))
                    {
                        // Execute the .exe file
                        ExecuteExe(exePath);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError($"Executable not found: {exePath}");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError($"3dmigoto folder not found: {migotoFolderPath}");
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"Mod folder not found: {modFolderPath}");
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"Modpack folder not found: {modpackFolderPath}");
        }
    }

    void ExecuteExe(string exePath)
    {
        try
        {
            Process process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(exePath);
            process.StartInfo.UseShellExecute = true;
            process.Start();
            UnityEngine.Debug.Log($"Executed: {exePath}");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Failed to execute .exe: {e.Message}");
        }
    }
}
