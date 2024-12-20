using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextCopy : MonoBehaviour
{
    // Reference to the TextMeshPro UI component where the copied text will be shown
    public TMP_Text displayText;

    void Start()
    {
        // Ensure displayText is assigned
        if (displayText == null)
        {
            Debug.LogError("Display Text is not assigned.");
            return;
        }

        // Add listener for button click (this script needs to be attached to each button created)
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    // This method will be triggered when any button is clicked
    void OnButtonClick()
    {
        // Get the TextMeshPro component from the button's label (child of button)
        TMP_Text buttonText = GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            // Copy the button's text to the displayText
            displayText.text = buttonText.text;
        }
    }
}
