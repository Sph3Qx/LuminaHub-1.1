using UnityEngine;

public class LogoDisplay : MonoBehaviour
{
    public GameObject logo;  // Reference to the logo GameObject
    public float displayDuration = 3f; // Time in seconds to display the logo
    public int targetResolutionWidth = 800; // Desired resolution width
    public int targetResolutionHeight = 600; // Desired resolution height
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera
        logo.SetActive(true); // Make sure the logo is visible at the start
        // Set the resolution to the desired size before displaying the logo
        Screen.SetResolution(targetResolutionWidth, targetResolutionHeight, false); 
        StartCoroutine(DisplayLogoAndResize());
    }

    private System.Collections.IEnumerator DisplayLogoAndResize()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(displayDuration);

        // Get the logo's size in world space
        Vector3 logoSize = logo.GetComponent<SpriteRenderer>().bounds.size;

        // Calculate the orthographic size of the camera based on the logo's height
        float cameraSize = logoSize.y / 2;

        // Set the camera's orthographic size to match the logo's size
        mainCamera.orthographicSize = cameraSize;

        // Optionally, center the camera on the logo
        Vector3 logoPosition = logo.transform.position;
        mainCamera.transform.position = new Vector3(logoPosition.x, logoPosition.y, mainCamera.transform.position.z);

        // Optionally, hide all other objects except the logo
        // You can do this by disabling other objects or cameras if desired
        // Example: Hide all objects except the logo
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("HideOnLogo"))
        {
            obj.SetActive(false);
        }
    }

    // Method to change the resolution manually at any point
    public void ChangeResolution(int width, int height)
    {
        Screen.SetResolution(width, height, false); 
    }
}
