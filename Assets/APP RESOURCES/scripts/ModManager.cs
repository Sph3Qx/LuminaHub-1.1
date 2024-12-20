using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ModManager : MonoBehaviour
{
    public TMP_InputField modNameInputField; // Input field for mod name
    public TextMeshProUGUI modText; // TextMeshPro element for generic messages
    public Button searchButton; // Button to trigger the search
    public Button sortButton; // Button to sort mods by popularity
    public Button nextPageButton; // Button to navigate to the next page
    public Transform modListParent; // Parent object to hold mod entries
    public GameObject modEntryPrefab; // Prefab for a mod entry (Image + Text)

    private string featuredModsUrl = "https://api.gamebanana.com/Rss/Featured?itemtype=Mod"; // Featured mods URL
    private string searchModsUrl = "https://api.gamebanana.com/Core/List/Section"; // API endpoint for fetching mods

    private List<ModEntry> currentMods = new List<ModEntry>(); // List to store current mods
    private int currentPage = 1;
    private const int maxModsPerPage = 10;
    private bool sortByPopularity = false;

    void Start()
    {
        // Attach button listeners
        searchButton.onClick.AddListener(OnSearchButtonClicked);
        sortButton.onClick.AddListener(OnSortButtonClicked);
        nextPageButton.onClick.AddListener(OnNextPageButtonClicked);

        StartCoroutine(GetMods()); // Load mods when the app starts
    }

    void OnSearchButtonClicked()
    {
        string modName = modNameInputField.text;
        if (!string.IsNullOrEmpty(modName))
        {
            Debug.Log($"Searching for mod: {modName}");
            StartCoroutine(GetMods(modName));
        }
        else
        {
            Debug.LogWarning("Please enter a mod name.");
            modText.text = "Please enter a mod name.";
        }
    }

    void OnSortButtonClicked()
    {
        sortByPopularity = !sortByPopularity;
        Debug.Log("Sort by popularity: " + sortByPopularity);
        StartCoroutine(GetMods());
    }

    void OnNextPageButtonClicked()
    {
        currentPage++;
        Debug.Log("Loading page: " + currentPage);
        StartCoroutine(GetMods());
    }

    IEnumerator GetMods(string searchQuery = "")
    {
        // Clear current mods
        foreach (Transform child in modListParent)
        {
            Destroy(child.gameObject);
        }

        string sortField = sortByPopularity ? "popularity" : "id";
        string url = searchModsUrl + $"?itemtype=Mod&page={currentPage}&perpage={maxModsPerPage}&sort={sortField}&direction=desc";

        if (!string.IsNullOrEmpty(searchQuery))
        {
            url += $"&field=name&match={UnityWebRequest.EscapeURL(searchQuery)}";
        }

        Debug.Log("Sending request to: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("Raw Response: " + json);

            if (string.IsNullOrEmpty(json) || json.Contains("null"))
            {
                Debug.LogWarning("No results found.");
                modText.text = "No mods found.";
                yield break;
            }

            try
            {
                // Parse JSON response to extract mods
                // This example assumes JSON format: [{"name":"ModName","popularity":123,"imageUrl":"..."}, ...]
                var mods = JsonUtility.FromJson<List<ModEntry>>(json);
                currentMods = mods;

                for (int i = 0; i < mods.Count && i < maxModsPerPage; i++)
                {
                    CreateModEntry(mods[i]);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error parsing response: " + ex.Message);
                modText.text = "Error processing mod data.";
            }
        }
        else
        {
            Debug.LogError("Error fetching mods: " + request.error);
            modText.text = "Error fetching mods.";
        }
    }

    void CreateModEntry(ModEntry mod)
    {
        GameObject modEntryObject = Instantiate(modEntryPrefab, modListParent);
        ModEntryUI modUI = modEntryObject.GetComponent<ModEntryUI>();

        if (modUI != null)
        {
            modUI.SetModData(mod);
        }
    }
}

[System.Serializable]
public class ModEntry
{
    public string name;
    public int popularity;
    public string imageUrl;
}

public class ModEntryUI : MonoBehaviour
{
    public TextMeshProUGUI modNameText;
    public Image modImage;

    public void SetModData(ModEntry mod)
    {
        modNameText.text = mod.name;

        StartCoroutine(LoadImage(mod.imageUrl));
    }

    IEnumerator LoadImage(string imageUrl)
    {
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return imageRequest.SendWebRequest();

        if (imageRequest.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            modImage.sprite = sprite;
        }
        else
        {
            Debug.LogError("Error loading image: " + imageRequest.error);
        }
    }
}
