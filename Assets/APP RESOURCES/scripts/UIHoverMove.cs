using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class UIHoverMove : MonoBehaviour
{
    // Reference to the object to move
    public GameObject objectToMove;

    // The target position to move the object to
    public Vector3 targetPosition;

    // Reference to the Button component
    public Button moveButton;

    // Speed of movement
    public float moveSpeed = 5f;

    private Vector3 originalPosition; // Store the original position of the object
    private bool isMoving = false;
    private bool moveToTarget = true; // Tracks whether to move to the target or back to the original position

    private void Start()
    {
        if (moveButton != null)
        {
            // Attach the OnButtonClick method to the button's onClick event
            moveButton.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("Move button is not assigned!");
        }

        if (objectToMove != null)
        {
            // Save the original position of the object
            originalPosition = objectToMove.transform.position;
        }
        else
        {
            Debug.LogError("Object to move is not assigned!");
        }
    }

    private void Update()
    {
        if (isMoving && objectToMove != null)
        {
            // Determine the target based on the current toggle state
            Vector3 destination = moveToTarget ? targetPosition : originalPosition;

            // Smoothly move the object to the target position
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, destination, moveSpeed * Time.deltaTime);

            // Stop moving when the destination is reached
            if (Vector3.Distance(objectToMove.transform.position, destination) < 0.01f)
            {
                isMoving = false;
            }
        }
    }

    private void OnButtonClick()
    {
        if (objectToMove != null)
        {
            // Toggle the move direction
            moveToTarget = !moveToTarget;

            // Start moving the object
            isMoving = true;
        }
        else
        {
            Debug.LogError("Object to move is not set in the inspector!");
        }
    }
}
