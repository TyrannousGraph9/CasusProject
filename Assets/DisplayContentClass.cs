using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.UI;

public class DisplayContentClass : MonoBehaviour
{
    public GameObject schermContainer;
    void Start()
    {
    GetAllFromDB((response) => {
        if (response != null)
        {
            foreach (var item in response)
            {
                GameObject scherm = Instantiate(schermContainer);
                
                scherm.transform.SetParent(transform, false);

                Transform imageTransform = scherm.transform.Find("RawImage");
                Transform textTransform = scherm.transform.Find("Text (TMP)");

                if(imageTransform != null)
                {
                    RawImage image = imageTransform.GetComponent<RawImage>();

                    if (image != null)
                    {
                        // convert from binary to texture  
                        byte[] imageBytes = Convert.FromBase64String(item.Foto);
                        Texture2D texture = new Texture2D(2, 2);
                        texture.LoadImage(imageBytes);
                        image.texture = texture;
                    }
                }

                if (textTransform != null)
                {
           
                    TMP_Text text = textTransform.GetComponent<TMP_Text>();

                    if (text != null)
                    {

                        text.text = item.Naam;
                    }
                }
                else
                {
                    Debug.LogError("Image or Text component not found in schermContainer.");
                }
            }
        }
        else
        {
            Debug.LogError("Failed to retrieve data.");
        }
    });
}
    public void GetAllFromDB(Action<List<DataItem>> callback)
    {
        APIClass aPIClass = new APIClass();
        string jsonData = aPIClass.SelectFromDatabase("InheemseSoort");
        StartCoroutine(aPIClass.ConnectToApi(jsonData, (response) => {
            callback(response);
        }));
    }

    public void SearchSpecificFromDB(string searchValue, Action<List<DataItem>> callback)
    {
        APIClass aPIClass = new APIClass();
        string jsonData = aPIClass.SelectFromDatabase("InheemseSoort", searchValue);
        StartCoroutine(aPIClass.ConnectToApi(jsonData, (response) => {
            callback(response);
        }));
    }
    
}
