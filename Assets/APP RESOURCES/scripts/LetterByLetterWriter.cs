using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;          // For TextMeshPro support

public class LetterByLetterWriter : MonoBehaviour
{
    public TextMeshProUGUI tmpText; // TextMeshPro UI Text
    public string[] textLines;      // Array of text lines to display
    public float typingSpeed = 0.05f; // Time delay between each letter
    public AudioSource typingSound;  // Sound to play for each letter
    public List<GameObject> objectsToSpawn; // List of objects to spawn after all text is displayed

    private void Start()
    {
        if (tmpText != null && textLines.Length > 0)
        {
            StartCoroutine(TypeMultipleLines(tmpText));
        }
        else
        {
            Debug.LogError("Please assign a TextMeshProUGUI component and provide text lines.");
        }
    }

    private IEnumerator TypeMultipleLines(TextMeshProUGUI textComponent)
    {
        foreach (string line in textLines)
        {
            textComponent.text = ""; // Clear the text for the new line

            foreach (char letter in line)
            {
                textComponent.text += letter;
                if (typingSound != null)
                {
                    typingSound.Play(); // Play the typing sound
                }
                yield return new WaitForSeconds(typingSpeed);
            }

            yield return new WaitForSeconds(typingSpeed * 10); // Pause before the next line
        }

        SpawnObjects(); // Spawn objects after all text lines are displayed
    }

    private void SpawnObjects()
    {
        foreach (GameObject obj in objectsToSpawn)
        {
            Instantiate(obj, Vector3.zero, Quaternion.identity); // Spawn each object at origin (adjust as needed)
        }
    }
}
