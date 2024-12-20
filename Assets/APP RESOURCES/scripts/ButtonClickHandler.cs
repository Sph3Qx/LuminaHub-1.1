using UnityEngine;
using UnityEngine.UI;

public class ButtonClickHandler : MonoBehaviour
{
    public Button myButton;  // Reference to the UI Button
    public Animator myAnimator;  // Reference to the Animator
    public string boolParameterName = "Anim";  // Name of the boolean parameter in the Animator

    void Start()
    {
        // Ensure the button and animator are assigned
        if (myButton != null && myAnimator != null)
        {
            myButton.onClick.AddListener(OnButtonClick);  // Add listener to the button click event
        }
        else
        {
            Debug.LogError("Button or Animator is not assigned.");
        }
    }

    void OnButtonClick()
    {
        // Set the boolean parameter to true in the Animator
        myAnimator.SetBool(boolParameterName, true);
    }
}
