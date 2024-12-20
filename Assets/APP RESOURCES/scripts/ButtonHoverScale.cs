using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Reference to the TextMeshProUGUI component
    public TextMeshProUGUI buttonText;

    // Scale values
    public float scaleFactor = 1.1f; // how much to scale when hovering
    public float normalScale = 1f;   // normal scale of the button

    private void Start()
    {
        // Get the TextMeshProUGUI component attached to the button
        buttonText = GetComponent<TextMeshProUGUI>();
    }

    // When the pointer enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Scale up the button text when hovering
        buttonText.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }

    // When the pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset scale when the cursor leaves
        buttonText.transform.localScale = new Vector3(normalScale, normalScale, 1f);
    }
}
