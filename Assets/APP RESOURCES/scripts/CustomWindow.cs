using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class CustomWindow : MonoBehaviour
{
    // Import the necessary Windows APIs
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    
    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowLongPtr(IntPtr hwnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hwnd, int nIndex, IntPtr dwNewLong);
    
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int X, int Y, int Width, int Height, uint uFlags);

    private const int GWL_STYLE = -16;
    private const uint SWP_FRAMECHANGED = 0x0020;
    private const uint WS_SYSMENU = 0x00080000;
    private const uint WS_THICKFRAME = 0x00040000;

    private void Start()
    {
        // Set the initial resolution (500x500) windowed
        Screen.SetResolution(500, 500, false);
        RemoveWindowBorder(); // Call to remove default window title bar
    }

    private void RemoveWindowBorder()
    {
        IntPtr hwnd = GetActiveWindow();
        
        // Get current window style
        IntPtr currentStyle = GetWindowLongPtr(hwnd, GWL_STYLE);

        // Remove the title bar, minimize, and maximize buttons, but keep close button
        IntPtr newStyle = (IntPtr)(currentStyle.ToInt32() & ~(WS_THICKFRAME | WS_SYSMENU));
        
        // Set new window style
        SetWindowLongPtr(hwnd, GWL_STYLE, newStyle);

        // Apply the changes
        SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAMECHANGED);
    }
}
