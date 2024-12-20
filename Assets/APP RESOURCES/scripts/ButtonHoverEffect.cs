using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform buttonRectTransform;  // Reference to the button's RectTransform

    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f);  // Scale when hovering
    public Vector3 normalScale = new Vector3(1f, 1f, 1f);     // Default scale
    public float scaleSpeed = 5f;  // Speed of scaling transition

    private Vector3 targetScale;

    void Start()
    {
        // Get the RectTransform of the button this script is attached to
        buttonRectTransform = GetComponent<RectTransform>();
        buttonRectTransform.localScale = normalScale;  // Initialize scale to normal
        targetScale = normalScale;  // Set the initial target scale to normal
    }

    void Update()
    {
        // Smoothly transition towards the target scale using Lerp
        if (buttonRectTransform != null)
        {
            buttonRectTransform.localScale = Vector3.Lerp(buttonRectTransform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        }
    }

    // This method is called when the mouse enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale;  // Set the target scale to hoverScale when hovered
    }

    // This method is called when the mouse exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = normalScale;  // Set the target scale back to normalScale when the mouse exits
    }
}
