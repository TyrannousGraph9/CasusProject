using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayContentClass : MonoBehaviour
{
    public GameObject schermContainer;

    public TMP_InputField searchInput;

    public GameObject mainScreen;

    public GameObject infoScreen;
    public Button backButton;

    void OnEnable()
    {
        
        DisplayAllItems();
    }

    void OnDisable()
    {

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

    public void Search()
    {
        DisplaySpecificItems(searchInput.text);
    }

    private void DisplayItems(List<DataItem> items)
    {
        
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in items)
        {
            Debug.Log(item);
            GameObject scherm = Instantiate(schermContainer);
            scherm.transform.SetParent(transform, false);

            // Set Image
            SetImage(item, scherm);

            // Set Text
            SetText(item, scherm);

            // Set Button
            SetButton(item, scherm);
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
                    return;
                }

                byte[] imageBytes;
                if (IsBase64String(item.Foto))
                {
                    imageBytes = Convert.FromBase64String(item.Foto);
                }
                else
                {
                    imageBytes = System.Text.Encoding.UTF8.GetBytes(item.Foto);
                }

                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(imageBytes))
                {
                    image.texture = texture;
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        else
        {
           return;
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
        string jsonData = aPIClass.SelectFromDatabase("InheemseSoort", "Naam", searchValue);
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
    private void SetButton(DataItem item, GameObject scherm)
    {
        Transform buttonTransform = scherm.transform.Find("Info_icon");
        if (buttonTransform != null)
        {
            Button button = buttonTransform.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnButtonClick(item));
            }
        }
    }

    private void OnButtonClick(DataItem item)
    {
        // Hide the current screen and show the info screen
        mainScreen.gameObject.SetActive(false);
        infoScreen.SetActive(true);

        // Display the specific data for the item in the info screen
        Transform naamTransform = infoScreen.transform.Find("Naam_text");
        if (naamTransform != null)
        {
            TMP_Text naamText = naamTransform.GetComponent<TMP_Text>();
            if (naamText != null)
            {
            naamText.text = item.Naam;
            }
        }

        Transform beschrijvingTransform = infoScreen.transform.Find("Beschrijving_text");
        if (beschrijvingTransform != null)
        {
            TMP_Text beschrijvingText = beschrijvingTransform.GetComponent<TMP_Text>();
            if (beschrijvingText != null)
            {
            beschrijvingText.text = item.Beschrijving;
            }
        }

        Transform locatieTransform = infoScreen.transform.Find("Locatie_text");
        if (locatieTransform != null)
        {
            TMP_Text locatieText = locatieTransform.GetComponent<TMP_Text>();
            if (locatieText != null)
            {
            locatieText.text = item.LocatieNaam;
            }
        }
        RawImage image = infoScreen.transform.Find("Inheemsesoort_foto").GetComponent<RawImage>();
        if (image != null)
        {
            if(item.Foto != null)
            {
                byte[] imageBytes;
                
                if (IsBase64String(item.Foto))
                {
                    imageBytes = Convert.FromBase64String(item.Foto);
                }
                else
                {
                    imageBytes = System.Text.Encoding.UTF8.GetBytes(item.Foto);
                }
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(imageBytes))
                {
                    image.texture = texture;
                }
                else
                {
                    image.texture = Resources.Load<Texture2D>("Images/Template_Foto");
                }
            }
        }
        // Set up the back button to switch back to the current screen
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => OnBackButtonClick());
    }

    private void OnBackButtonClick()
    {
        // Hide the info screen and show the current screen
        infoScreen.SetActive(false);
        mainScreen.gameObject.SetActive(true);
    }
}
