using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace

public class GameBananaModsLoader : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform contentArea; // The ScrollView Content Panel
    public GameObject modPrefab;      // Prefab with TextMeshPro, Image, and Button for each mod
    public List<Transform> spawnPoints; // List of predefined spawn points for the mods
    public Button nextPageButton; // Button to load the next page

    private string baseRssUrl = "https://api.gamebanana.com/Rss/Featured?gameid=19123";
    private int currentPage = 1;
    private const int modsPerPage = 10; // Assuming each page has 10 mods

    void Start()
    {
        nextPageButton.onClick.AddListener(LoadNextPage);
        StartCoroutine(LoadModsFromRSS(currentPage));
    }

    void LoadNextPage()
    {
        currentPage++;
        StartCoroutine(LoadModsFromRSS(currentPage));
    }

    IEnumerator LoadModsFromRSS(int page)
    {
        string rssUrl = baseRssUrl + "&page=" + page;

        using (UnityWebRequest request = UnityWebRequest.Get(rssUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load RSS: " + request.error);
                yield break;
            }

            // Parse the XML response
            string xml = request.downloadHandler.text;
            List<ModInfo> mods = ParseModsFromXML(xml);

            // Populate the ScrollView
            PopulateScrollView(mods);
        }
    }

    List<ModInfo> ParseModsFromXML(string xml)
    {
        List<ModInfo> mods = new List<ModInfo>();

        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        doc.LoadXml(xml);

        foreach (System.Xml.XmlNode item in doc.GetElementsByTagName("item"))
        {
            string title = item["title"]?.InnerText;
            string link = item["link"]?.InnerText;
            string image = item["image"]?.InnerText;

            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(link) && !string.IsNullOrEmpty(image))
            {
                mods.Add(new ModInfo(title, link, image));
            }
        }

        return mods;
    }

    void PopulateScrollView(List<ModInfo> mods)
    {
        // Clear existing content
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        // Ensure we have enough spawn points
        if (spawnPoints.Count < mods.Count)
        {
            Debug.LogError("Not enough spawn points for the mods!");
            return;
        }

        for (int i = 0; i < mods.Count; i++)
        {
            var mod = mods[i];

            GameObject modItem = Instantiate(modPrefab, spawnPoints[i].position, Quaternion.identity, contentArea);

            TMP_Text titleText = modItem.transform.Find("TitleText").GetComponent<TMP_Text>();
            Image modImage = modItem.transform.Find("ModImage").GetComponent<Image>();
            Button linkButton = modItem.transform.Find("LinkButton").GetComponent<Button>();

            titleText.text = mod.Title;

            // Load the mod image
            StartCoroutine(LoadImage(mod.ImageUrl, modImage));

            // Set up the link button
            linkButton.onClick.AddListener(() => Application.OpenURL(mod.Link));
        }
    }

    IEnumerator LoadImage(string url, Image targetImage)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                targetImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Failed to load image: " + request.error);
            }
        }
    }

    // Class to hold mod data
    public class ModInfo
    {
        public string Title;
        public string Link;
        public string ImageUrl;

        public ModInfo(string title, string link, string imageUrl)
        {
            Title = title;
            Link = link;
            ImageUrl = imageUrl;
        }
    }
}
