using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SchermHandlerClass : MonoBehaviour
{
    public GameObject[] schermen;
    public int huidigScherm = 0;    

    public TMPro.TMP_InputField naamInput;
    public TMPro.TMP_InputField beschrijvingInput;

    public TMPro.TMP_InputField locatieNaamInput;
    
    void Start()
    {
        schermen[huidigScherm].SetActive(true);
    }

    public void VorigScherm()
    {
        schermen[huidigScherm].SetActive(false);
        huidigScherm--;
        if (huidigScherm < 0)
        {
            huidigScherm = schermen.Length - 1;
        }
        schermen[huidigScherm].SetActive(true);
        DataPerStap(huidigScherm);
    }

    public void VolgendScherm()
    {
        schermen[huidigScherm].SetActive(false);
        huidigScherm++;
        if (huidigScherm >= schermen.Length)
        {
            huidigScherm = 0;
        }
        schermen[huidigScherm].SetActive(true);
        DataPerStap(huidigScherm);
    }
    
    public void DataPerStap(int stap)
    {
        switch (huidigScherm)
        {
            case 1:
            TakePicture takePicture = FindObjectOfType<TakePicture>();
            Texture2D image = takePicture.image.texture as Texture2D;
            byte[] imageBytes = image.EncodeToPNG();
            PlayerPrefs.SetString("ImageBytes", System.Convert.ToBase64String(imageBytes));
            break;

            case 2:

            string naam = naamInput.text;
            string beschrijving = beschrijvingInput.text;
            string locatieNaam = locatieNaamInput.text;
            PlayerPrefs.SetString("Naam", naam);
            PlayerPrefs.SetString("Beschrijving", beschrijving);
            PlayerPrefs.SetString("LocatieNaam", locatieNaam);
            break;

            case 3:
            Input.location.Start();
            float longitude = Input.location.lastData.longitude;
            float latitude = Input.location.lastData.latitude;
            Input.location.Stop();
            PlayerPrefs.SetFloat("Longitude", longitude);
            PlayerPrefs.SetFloat("Latitude", latitude);
            break;

            case 4:

            byte[] savedImageBytes = System.Convert.FromBase64String(PlayerPrefs.GetString("ImageBytes"));
            string savedNaam = PlayerPrefs.GetString("Naam");
            string savedLocatieNaam = PlayerPrefs.GetString("LocatieNaam");
            string savedBeschrijving = PlayerPrefs.GetString("Beschrijving");
            float savedLongitude = PlayerPrefs.GetFloat("Longitude");
            float savedLatitude = PlayerPrefs.GetFloat("Latitude");

            Dictionary<string, string> combinedValues = new Dictionary<string, string>
            {
                { "Foto", System.Convert.ToBase64String(savedImageBytes) },
                { "Beschrijving", savedBeschrijving },
                { "Status", "In Beoordeling" }, // Default status
                { "UploadDatum", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }, // Current date and time
                { "Naam", savedNaam },
                { "LocatieNaam", savedLocatieNaam },
                { "Longitude", savedLongitude.ToString() },
                { "Latitude", savedLatitude.ToString() },
            };
            Debug.Log(combinedValues);
            Opslaan(combinedValues);
            break;
        }
    }

    public void Opslaan(Dictionary<string, string> combinedValues)
    {
        APIClass aPIClass = new APIClass();
        string jsonData = aPIClass.InsertIntoDatabase("InheemseSoort", combinedValues);
        StartCoroutine(aPIClass.ConnectToApi(jsonData, (response) => {
            Debug.Log("API Response: " + response);
        }));
    }
}
