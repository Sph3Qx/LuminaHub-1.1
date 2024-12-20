using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class InstanceDetailsUIManager : MonoBehaviour
{
    public GameObject fileListUIPrefab; // Prefab for displaying file list UI
    public Transform fileListUIParent; // Parent object where file list UI will be instantiated

    private Dictionary<string, string> instanceFolderMappings = new Dictionary<string, string>();

    public void RegisterInstance(string instanceName, string folderPath)
    {
        // Add the instance and its folder path to the dictionary
        if (!instanceFolderMappings.ContainsKey(instanceName))
        {
            instanceFolderMappings.Add(instanceName, folderPath);
        }
    }

    public void OnInstanceClicked(string instanceName)
    {
        if (instanceFolderMappings.TryGetValue(instanceName, out string folderPath))
        {
            // Generate the UI for displaying files
            CreateFileListUI(instanceName, folderPath);
        }
        else
        {
            Debug.LogError($"No folder mapping found for instance: {instanceName}");
        }
    }

    private void CreateFileListUI(string instanceName, string folderPath)
    {
        // Clear any existing file list UI
        foreach (Transform child in fileListUIParent)
        {
            Destroy(child.gameObject);
        }

        // Instantiate the file list UI prefab
        GameObject fileListUI = Instantiate(fileListUIPrefab, fileListUIParent);
        fileListUI.name = instanceName + "_FileListUI";

        // Update the title with the instance name
        TMP_Text titleText = fileListUI.transform.Find("TitleText").GetComponent<TMP_Text>();
        if (titleText != null)
        {
            titleText.text = $"Files in {instanceName}";
        }

        // Find the content area for displaying file names
        Transform contentArea = fileListUI.transform.Find("Content");
        if (contentArea == null)
        {
            Debug.LogError("Content area not found in file list UI prefab.");
            return;
        }

        // Get all files in the instance's folder
        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                // Create a UI element for each file
                GameObject fileTextObject = new GameObject("FileText", typeof(TMP_Text));
                TMP_Text fileText = fileTextObject.GetComponent<TMP_Text>();

                fileText.text = Path.GetFileName(file); // Display only the file name
                fileText.fontSize = 24; // Set font size for better readability
                fileText.color = Color.black;

                // Add the text object to the content area
                fileTextObject.transform.SetParent(contentArea, false);
            }
        }
        else
        {
            Debug.LogError($"Folder not found: {folderPath}");
        }
    }
}
