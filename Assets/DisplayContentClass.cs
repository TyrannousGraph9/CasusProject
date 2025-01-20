using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayContentClass : MonoBehaviour
{
    public GameObject schermContainer;

    void OnEnable()
    {
        // Display all items from the database when the object is enabled
        DisplayAllItems();
    }

    void OnDisable()
    {
        // Clear all children when the object is disabled
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void DisplayAllItems()
    {
        GetAllFromDB((response) =>
        {
            if (response != null)
            {
                DisplayItems(response);
            }
            else
            {
                Debug.LogError("Failed to retrieve all items from the database.");
            }
        });
    }

    public void DisplaySpecificItems(string searchValue)
    {
        SearchSpecificFromDB(searchValue, (response) =>
        {
            if (response != null)
            {
                DisplayItems(response);
            }
            else
            {
                Debug.LogError($"Failed to retrieve specific items for search value: {searchValue}");
            }
        });
    }

    private void DisplayItems(List<DataItem> items)
    {
        foreach (var item in items)
        {
            GameObject scherm = Instantiate(schermContainer);
            scherm.transform.SetParent(transform, false);

            // Set Image
            SetImage(item, scherm);

            // Set Text
            SetText(item, scherm);
        }
    }

    private void SetImage(DataItem item, GameObject scherm)
    {
        Transform imageTransform = scherm.transform.Find("Logo_image");
        if (imageTransform != null)
        {
            RawImage image = imageTransform.GetComponent<RawImage>();
            if (image != null)
            {
                if (string.IsNullOrEmpty(item.Foto))
                {
                    Debug.LogWarning($"Foto field is empty or null for item: {item.Naam}");
                    return;
                }

                byte[] imageBytes;
                if (IsBase64String(item.Foto))
                {
                    imageBytes = Convert.FromBase64String(item.Foto);
                    Debug.Log($"Decoded image size for item '{item.Naam}': {imageBytes.Length} bytes");
                }
                else
                {
                    Debug.LogWarning($"Foto for item '{item.Naam}' is not Base64 encoded. Assuming raw binary.");
                    imageBytes = System.Text.Encoding.UTF8.GetBytes(item.Foto);
                }

                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(imageBytes))
                {
                    Debug.Log($"Texture created successfully for item '{item.Naam}'. Width: {texture.width}, Height: {texture.height}");
                    image.texture = texture;
                }
                else
                {
                    Debug.LogError($"Failed to create texture for item '{item.Naam}'.");
                }
            }
            else
            {
                Debug.LogError("RawImage component not found in schermContainer.");
            }
        }
        else
        {
            Debug.LogError("RawImage transform not found in schermContainer.");
        }
    }

    private void SetText(DataItem item, GameObject scherm)
    {
        Transform textTransform = scherm.transform.Find("Text (TMP)");
        if (textTransform != null)
        {
            TMP_Text text = textTransform.GetComponent<TMP_Text>();
            if (text != null)
            {
                text.text = item.Naam;
            }
            else
            {
                Debug.LogError("TMP_Text component not found in 'Text (TMP)'.");
            }
        }
        else
        {
            Debug.LogError("Text transform not found in schermContainer.");
        }
    }

    public void GetAllFromDB(Action<List<DataItem>> callback)
    {
        APIClass aPIClass = new APIClass();
        string jsonData = aPIClass.SelectFromDatabase("InheemseSoort");
        StartCoroutine(aPIClass.ConnectToApi(jsonData, (response) =>
        {
            callback(response);
        }));
    }

    public void SearchSpecificFromDB(string searchValue, Action<List<DataItem>> callback)
    {
        APIClass aPIClass = new APIClass();
        string jsonData = aPIClass.SelectFromDatabase("InheemseSoort", searchValue);
        StartCoroutine(aPIClass.ConnectToApi(jsonData, (response) =>
        {
            callback(response);
        }));
    }

    private bool IsBase64String(string base64String)
    {
        if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0)
        {
            return false;
        }

        try
        {
            Convert.FromBase64String(base64String);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
