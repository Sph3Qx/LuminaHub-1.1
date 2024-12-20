using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ModDisplay : MonoBehaviour
{
    public string apiUrl = "https://api.gamebanana.com/v1/mods"; // Replace with the actual API endpoint
    public GameObject modPrefab; // Prefab for displaying mod information
    public Transform contentPanel; // Content panel of the Scroll View

    void Start()
    {
        StartCoroutine(FetchMods());
    }

    IEnumerator FetchMods()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            // Send the request and wait for a response
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching mods: " + request.error);
            }
            else
            {
                // Parse the response (Assuming JSON format)
                string jsonResponse = request.downloadHandler.text;
                List<Mod> mods = ParseMods(jsonResponse);

                // Populate the Scroll View
                foreach (Mod mod in mods)
                {
                    AddModToScrollView(mod);
                }
            }
        }
    }

    List<Mod> ParseMods(string jsonResponse)
    {
        // Deserialize JSON into a list of mods (adjust this according to API's response structure)
        // Assuming JSONUtility or a similar library is used
        ModList modList = JsonUtility.FromJson<ModList>(jsonResponse);
        return modList.mods;
    }

    void AddModToScrollView(Mod mod)
    {
        // Instantiate the prefab and set its parent
        GameObject modEntry = Instantiate(modPrefab, contentPanel);
        modEntry.transform.localScale = Vector3.one;

        // Set mod details (assuming prefab has a Text component for name and description)
        Text[] textComponents = modEntry.GetComponentsInChildren<Text>();
        if (textComponents.Length >= 2)
        {
            textComponents[0].text = mod.name;
            textComponents[1].text = mod.description;
        }
    }

    // Define a class to match the JSON structure
    [System.Serializable]
    public class Mod
    {
        public string name;
        public string description;
    }

    [System.Serializable]
    public class ModList
    {
        public List<Mod> mods;
    }
}
