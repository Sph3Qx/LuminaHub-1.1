using UnityEngine;

public class RotatingBorder : MonoBehaviour
{
    public float moveSpeed = 200f;  // Speed of the movement
    public float scaleFactor = 1.5f; // Maximum scale factor
    public RectTransform targetImage; // The image (square) the bar follows
    private RectTransform rectTransform;
    private Vector3[] corners; // The corners of the target image

    private int currentCorner = 0; // The current corner the bar is at
    private Vector3 targetScale; // Target scale based on position

    void Start()
    {
        if (targetImage == null)
        {
            Debug.LogError("Target Image not assigned!");
            return;
        }

        rectTransform = GetComponent<RectTransform>();

        // Get the local corners of the target image
        corners = new Vector3[4];
        targetImage.GetLocalCorners(corners);

        // Set initial scale
        targetScale = rectTransform.localScale;
    }

    void Update()
    {
        if (targetImage == null) return;

        // Move the bar along the corners of the target image
        Vector3 targetPosition = corners[currentCorner];

        // Move the bar smoothly to the target position
        rectTransform.position = Vector3.MoveTowards(rectTransform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Smoothly scale the bar depending on its position
        SmoothScale();

        // Once the current corner is reached, move to the next corner
        if (rectTransform.position == targetPosition)
        {
            currentCorner = (currentCorner + 1) % 4; // Cycle through the 4 corners
        }
    }

    // Smoothly change the scale based on the position
    void SmoothScale()
    {
        if (currentCorner == 0 || currentCorner == 2) // Top and Bottom sides (ceiling/floor)
        {
            targetScale = new Vector3(scaleFactor, 1f, 1f); // Scale more in width
        }
        else // Left and Right sides (walls)
        {
            targetScale = new Vector3(1f, scaleFactor, 1f); // Scale more in height
        }

        // Apply smooth scaling
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, moveSpeed * Time.deltaTime);
    }
}
