using UnityEngine;
using System.Collections;

public class GameIntro : MonoBehaviour
{
    public float splashDuration = 5f; // Duration for the splash screen in seconds

    private void Start()
    {
        // Set initial square resolution to 500x500 and windowed mode
        SetInitialResolution();
        
        // Start the splash screen timer
        StartCoroutine(SplashScreenTimer());
    }

    private void SetInitialResolution()
    {
        // Set window resolution to 500x500 (square resolution) and windowed mode
        Screen.SetResolution(500, 500, false); // false ensures it's windowed, not fullscreen
        Debug.Log("Initial resolution set to 500x500");

        // Ensure the window is resizable (this is default behavior for windowed mode)
        Screen.fullScreen = false;
    }

    private IEnumerator SplashScreenTimer()
    {
        // Wait for the splash duration (simulating the splash screen)
        yield return new WaitForSeconds(splashDuration);

        // After the splash screen ends, reset the resolution to the native screen resolution
        ResetResolutionToNative();
    }

    private void ResetResolutionToNative()
    {
        // Get the native screen resolution (not based on the game window, but the actual display)
        int screenWidth = Screen.currentResolution.width;
        int screenHeight = Screen.currentResolution.height;

        // Set the resolution to the native screen resolution in windowed mode
        Screen.SetResolution(screenWidth, screenHeight, false); // false ensures it's windowed, not fullscreen
        Debug.Log("Resolution set to native screen resolution: " + screenWidth + "x" + screenHeight);
    }
}
