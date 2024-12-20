using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class InstanceFileManager : MonoBehaviour
{
    public Button openFilesButtonPrefab; // Reference to the prefab of a button
    public GameObject filesCanvas; // The Canvas that will display the files
    public Transform filesPanel; // The panel inside the canvas that will hold the file buttons

    private string currentInstanceFolderPath; // Path to the current instance's folder

    // Method to open the file list UI when an instance is clicked
    public void OpenFilesUI(string instanceFolderPath)
    {
        currentInstanceFolderPath = instanceFolderPath;

        // Enable the files canvas
        filesCanvas.SetActive(true);

        // Clear previous file buttons
        foreach (Transform child in filesPanel)
        {
            Destroy(child.gameObject); // Destroy previous buttons
        }

        // Get files in the instance folder and create buttons for them
        string[] files = Directory.GetFiles(currentInstanceFolderPath);
        foreach (string file in files)
        {
            CreateFileButton(file);
        }
    }

    // Method to create a button for each file in the folder
    private void CreateFileButton(string filePath)
    {
        // Instantiate a new button
        Button fileButton = Instantiate(openFilesButtonPrefab, filesPanel);

        // Get the file name from the path
        string fileName = Path.GetFileName(filePath);

        // Set the button's text to the file name
        fileButton.GetComponentInChildren<TMP_Text>().text = fileName;

        // Add listener to open the file when the button is clicked
        fileButton.onClick.AddListener(() => OpenFile(filePath));
    }

    // Method to open the file (for now, it logs the file path to the console)
    private void OpenFile(string filePath)
    {
        // You can extend this method to open the file in a relevant application
        Debug.Log($"Opening file: {filePath}");
    }

    // Close the files canvas
    public void CloseFilesUI()
    {
        filesCanvas.SetActive(false);
    }
}
