using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class InstanceData
{
    public string name;
    public string folderPath;
    public string version;
}

[System.Serializable]
public class InstanceDataList
{
    public List<InstanceData> instances = new List<InstanceData>();
}

public class UIManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject uiPanel;
    public TMP_InputField inputField;
    public TMP_Dropdown dropdown;
    public Button createButton;
    public Button openUIPanelButton;
    public Button closeInstanceListButton;
    public GameObject objectPrefab;
    public List<RectTransform> spawnPositions;
    public Transform parentObject;

    [Header("File List UI")]
    public TMP_Text fileListText;
    public Canvas fileListCanvas;

    [Header("Instance Info UI")]
    public TMP_Text instanceNameText;
    public TMP_Text instanceVersionText;

    [Header("Instance Launch Button")]
    public Button launchButtonPrefab;

    [Header("Paths and Settings")]
    public string exeFileName = "GenshinImpact.exe";
    public Button openModsButtonPrefab;

    private int spawnIndex = 0;
    private string saveFilePath;
    private InstanceDataList instanceDataList = new InstanceDataList();
    private Dictionary<string, string> instanceFolderMappings = new Dictionary<string, string>();
    private string genshinExePath;

    void Start()
    {
        string buildFolderPath = GetBuildFolderPath();
        string modpackFolderPath = Path.Combine(buildFolderPath, "Modpack");
        if (!Directory.Exists(modpackFolderPath))
        {
            Directory.CreateDirectory(modpackFolderPath);
            UnityEngine.Debug.Log($"Modpack folder created at: {modpackFolderPath}");
        }

        saveFilePath = Path.Combine(modpackFolderPath, "instances.json");

        genshinExePath = GetGenshinPathFromFile();
        if (string.IsNullOrEmpty(genshinExePath) || !File.Exists(genshinExePath))
        {
            UnityEngine.Debug.LogError("Genshin Impact executable path not found. Please check GameStatus.txt.");
            return;
        }

        uiPanel.SetActive(false);
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string> { "v1.0.0" });

        createButton.onClick.AddListener(OnCreateButtonClicked);
        openUIPanelButton.onClick.AddListener(() => uiPanel.SetActive(true));
        closeInstanceListButton.onClick.AddListener(CloseInstanceListUI);

        LoadInstances();
    }

    private string GetGenshinPathFromFile()
    {
        string buildFolderPath = GetBuildFolderPath();
        string gameStatusFilePath = Path.Combine(buildFolderPath, "GameStatus.txt");

        if (File.Exists(gameStatusFilePath))
        {
            string fileContent = File.ReadAllText(gameStatusFilePath);
            Match match = Regex.Match(fileContent, @"Path:\s*(.+GenshinImpact\.exe)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
        }

        UnityEngine.Debug.LogWarning("GameStatus.txt not found or path not valid.");
        return string.Empty;
    }

    private void OnCreateButtonClicked()
    {
        string newInstanceName = inputField.text;

        if (!string.IsNullOrEmpty(newInstanceName) && spawnPositions.Count > spawnIndex)
        {
            string instanceVersion = dropdown.options[dropdown.value].text;
            string folderPath = CreateInstanceFolder(newInstanceName);

            CopyFilesFromDontDelete(folderPath);

            GameObject newInstance = Instantiate(objectPrefab, spawnPositions[spawnIndex].position, Quaternion.identity, parentObject);
            newInstance.name = newInstanceName;

            TMP_Text newText = newInstance.GetComponentInChildren<TMP_Text>();
            if (newText != null) newText.text = newInstanceName;

            Button instanceButton = newInstance.GetComponent<Button>();
            if (instanceButton != null)
            {
                instanceButton.onClick.AddListener(() => ShowInstanceFileList(newInstanceName, folderPath));
                instanceButton.onClick.AddListener(() => UpdateInstanceInfoUI(newInstanceName, instanceVersion));
            }

            CreateLaunchButtonFor3dmigoto(newInstanceName, folderPath);
            CreateOpenModsFolderButton(newInstanceName, folderPath);

            InstanceData newData = new InstanceData { name = newInstanceName, folderPath = folderPath, version = instanceVersion };
            instanceDataList.instances.Add(newData);
            instanceFolderMappings[newInstanceName] = folderPath;
            SaveInstances();

            spawnIndex++;
        }
        else
        {
            UnityEngine.Debug.Log("Please enter a valid name or check spawn positions.");
        }

        uiPanel.SetActive(false);
    }

    private string CreateInstanceFolder(string instanceName)
    {
        string buildFolderPath = GetBuildFolderPath();
        string modpackFolderPath = Path.Combine(buildFolderPath, "Modpack");
        string newInstanceFolderPath = Path.Combine(modpackFolderPath, instanceName);

        if (!Directory.Exists(newInstanceFolderPath))
        {
            Directory.CreateDirectory(newInstanceFolderPath);
            UnityEngine.Debug.Log($"Created new instance folder: {newInstanceFolderPath}");
        }

        return newInstanceFolderPath;
    }

    private void CopyFilesFromDontDelete(string destinationFolder)
    {
        string dontDeleteFolder = Path.Combine(GetBuildFolderPath(), "DontDelete");

        if (Directory.Exists(dontDeleteFolder))
        {
            foreach (var file in Directory.GetFiles(dontDeleteFolder))
            {
                string destFile = Path.Combine(destinationFolder, Path.GetFileName(file));
                if (!File.Exists(destFile))
                {
                    File.Copy(file, destFile);
                }
            }

            foreach (var directory in Directory.GetDirectories(dontDeleteFolder))
            {
                string destDir = Path.Combine(destinationFolder, Path.GetFileName(directory));
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                CopyDirectoryRecursively(directory, destDir);
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("DontDelete folder does not exist.");
        }
    }

    private void CopyDirectoryRecursively(string sourceDir, string targetDir)
    {
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(targetDir, Path.GetFileName(file));
            if (!File.Exists(destFile))
            {
                File.Copy(file, destFile);
            }
        }

        foreach (var directory in Directory.GetDirectories(sourceDir))
        {
            string destSubDir = Path.Combine(targetDir, Path.GetFileName(directory));
            if (!Directory.Exists(destSubDir))
            {
                Directory.CreateDirectory(destSubDir);
            }
            CopyDirectoryRecursively(directory, destSubDir);
        }
    }

    private void CreateLaunchButtonFor3dmigoto(string instanceName, string folderPath)
    {
        string threeDMigotoFolder = Path.Combine(folderPath, "3dmigoto");

        if (File.Exists(Path.Combine(threeDMigotoFolder, "3dmigoto Loader.exe")))
        {
            Button launchButton = Instantiate(launchButtonPrefab, fileListCanvas.transform);
            launchButton.name = $"{instanceName}_LaunchButton";

            TMP_Text newText = launchButton.GetComponentInChildren<TMP_Text>();
            if (newText != null) newText.text = $"Start Modded Game";

            launchButton.onClick.AddListener(() => Launch3DMigoto(threeDMigotoFolder));
        }
        else
        {
            UnityEngine.Debug.LogWarning($"3dmigoto Loader.exe not found in {threeDMigotoFolder}");
        }
    }

    private void CreateOpenModsFolderButton(string instanceName, string folderPath)
    {
        string modsFolderPath = Path.Combine(folderPath, "3dmigoto", "Mods");

        Button openModsButton = Instantiate(openModsButtonPrefab, fileListCanvas.transform);
        openModsButton.name = $"{instanceName}_OpenModsButton";

        TMP_Text newText = openModsButton.GetComponentInChildren<TMP_Text>();
        if (newText != null) newText.text = $"Add Mods";

        openModsButton.onClick.AddListener(() => OpenModsFolder(modsFolderPath));
    }

    private async void Launch3DMigoto(string threeDMigotoFolder)
    {
        string loaderPath = Path.Combine(threeDMigotoFolder, "3dmigoto Loader.exe");

        if (File.Exists(loaderPath))
        {
            Process loaderProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = loaderPath,
                    WorkingDirectory = threeDMigotoFolder,
                    UseShellExecute = true
                }
            };

            loaderProcess.Start();

            await Task.Delay(3000);

            if (File.Exists(genshinExePath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = genshinExePath,
                    UseShellExecute = true
                });
            }
            else
            {
                UnityEngine.Debug.LogError("Genshin Impact executable not found.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"3DMigoto Loader.exe not found in {threeDMigotoFolder}");
        }
    }

    private void ShowInstanceFileList(string instanceName, string folderPath)
    {
        fileListText.text = ""; // Clear existing content

        string modsFolder = Path.Combine(folderPath, "3dmigoto", "Mods");

        if (Directory.Exists(modsFolder))
        {
            // List directories in the Mods folder
            var modDirectories = Directory.GetDirectories(modsFolder)
                                           .Select(Path.GetFileName)
                                           .ToArray();

            if (modDirectories.Length > 0)
            {
                fileListText.text = "Mods:\n" + string.Join("\n", modDirectories);
            }
            else
            {
                fileListText.text = "Mods Folder is empty.";
            }
        }
        else
        {
            fileListText.text = "Mods Folder not found.";
        }

        fileListCanvas.gameObject.SetActive(true); // Show file list UI
    }

    private void OpenModsFolder(string modsFolderPath)
    {
        if (Directory.Exists(modsFolderPath))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = modsFolderPath,
                UseShellExecute = true
            });
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Mods folder not found at: {modsFolderPath}");
        }
    }

    private void CloseInstanceListUI()
    {
        fileListCanvas.gameObject.SetActive(false);
    }

    private void UpdateInstanceInfoUI(string name, string version)
    {
        instanceNameText.text = $"Name: {name}";
        instanceVersionText.text = $"Version: {version}";
    }

    private string GetBuildFolderPath()
    {
        return Application.isEditor
            ? Path.GetDirectoryName(Application.dataPath)
            : Path.GetDirectoryName(Application.dataPath);
    }

    private void SaveInstances()
    {
        string jsonData = JsonUtility.ToJson(instanceDataList, true);
        File.WriteAllText(saveFilePath, jsonData);
    }

    private void LoadInstances()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            instanceDataList = JsonUtility.FromJson<InstanceDataList>(jsonData);

            foreach (InstanceData data in instanceDataList.instances)
            {
                if (spawnIndex < spawnPositions.Count)
                {
                    GameObject newInstance = Instantiate(objectPrefab, spawnPositions[spawnIndex].position, Quaternion.identity, parentObject);
                    newInstance.name = data.name;

                    TMP_Text newText = newInstance.GetComponentInChildren<TMP_Text>();
                    if (newText != null) newText.text = data.name;

                    Button instanceButton = newInstance.GetComponent<Button>();
                    if (instanceButton != null)
                    {
                        instanceButton.onClick.AddListener(() => ShowInstanceFileList(data.name, data.folderPath));
                        instanceButton.onClick.AddListener(() => UpdateInstanceInfoUI(data.name, data.version));
                    }

                    CreateLaunchButtonFor3dmigoto(data.name, data.folderPath);
                    CreateOpenModsFolderButton(data.name, data.folderPath);

                    instanceFolderMappings[data.name] = data.folderPath;
                    spawnIndex++;
                }
                else
                {
                    UnityEngine.Debug.LogWarning("No more spawn positions available for saved instances.");
                }
            }
        }
        else
        {
            UnityEngine.Debug.Log("No save file found. Starting fresh.");
        }
    }
}
