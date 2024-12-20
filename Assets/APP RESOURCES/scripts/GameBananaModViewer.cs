using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class GameBananaModViewer : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text exampleTitleText;  // Example TextMeshPro title reference
    public TMP_Text exampleIdText;     // Example TextMeshPro ID reference
    public Image exampleThumbnailImage; // Example Image reference
    public Transform contentPanel;     // Scroll View Content panel

    private string modListUrl = "https://api.gamebanana.com/Core/List/New";
    private string modDetailsUrl = "https://api.gamebanana.com/Core/Item/Data";

    void Start()
    {
        StartCoroutine(FetchMods());
    }

    IEnumerator FetchMods()
    {
        string requestUrl = $"{modListUrl}?itemtype=Mod&gameid=8552&page=1&format=json";
        UnityWebRequest request = UnityWebRequest.Get(requestUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching mod list: " + request.error);
            yield break;
        }

        string json = request.downloadHandler.text;
        Debug.Log("Received JSON: " + json);

        // Parse the JSON into a list of IDs
        List<int> modIds = ParseModIds(json);
        if (modIds == null || modIds.Count == 0)
        {
            Debug.LogWarning("No mods found.");
            yield break;
        }

        foreach (int modId in modIds)
        {
            yield return FetchModDetails(modId);

            // Add a delay between requests to avoid rate limiting
            yield return new WaitForSeconds(1f);
        }
    }

    List<int> ParseModIds(string json)
    {
        try
        {
            // Parse the nested JSON array
            var modIds = new List<int>();
            var wrapper = JsonUtilityArrayHelper.ParseNestedArray(json);
            foreach (var entry in wrapper)
            {
                if (entry.Length > 1 && int.TryParse(entry[1], out int id))
                {
                    modIds.Add(id);
                }
            }
            return modIds;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing mod IDs: {e.Message}");
            return null;
        }
    }

    IEnumerator FetchModDetails(int modId)
    {
        string requestUrl = $"{modDetailsUrl}?itemtype=Mod&itemid={modId}&fields=name,preview";
        UnityWebRequest request = UnityWebRequest.Get(requestUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error fetching mod details for ID {modId}: {request.error}");
            yield break;
        }

        string json = request.downloadHandler.text;
        Debug.Log($"Received details for Mod ID {modId}: {json}");

        ModDetails mod = ParseModDetails(json);
        if (mod != null)
        {
            AddModToUI(mod);
        }
    }

    ModDetails ParseModDetails(string json)
    {
        try
        {
            return JsonUtility.FromJson<ModDetails>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing mod details JSON: {e.Message}");
            return null;
        }
    }

    void AddModToUI(ModDetails mod)
    {
        TMP_Text titleText = Instantiate(exampleTitleText, contentPanel);
        TMP_Text idText = Instantiate(exampleIdText, contentPanel);
        Image thumbnailImage = Instantiate(exampleThumbnailImage, contentPanel);

        titleText.text = string.IsNullOrEmpty(mod.name) ? "No Title Available" : mod.name;
        idText.text = $"ID: {mod.id}";

        if (!string.IsNullOrEmpty(mod.previewUrl))
        {
            StartCoroutine(LoadThumbnail(mod.previewUrl, thumbnailImage));
        }
    }

    IEnumerator LoadThumbnail(string url, Image image)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogError($"Error loading thumbnail: {request.error}");
        }
    }
}

[System.Serializable]
public class ModDetails
{
    public int id;
    public string name;
    public string previewUrl;
}

public static class JsonUtilityArrayHelper
{
    public static string[][] ParseNestedArray(string json)
    {
        // Convert the JSON array into a deserializable format
        return JsonUtility.FromJson<RawWrapper>($"{{\"data\":{json}}}").data;
    }

    [System.Serializable]
    private class RawWrapper
    {
        public string[][] data;
    }
}
