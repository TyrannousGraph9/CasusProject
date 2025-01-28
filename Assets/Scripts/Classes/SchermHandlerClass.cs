using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SchermHandlerClass : MonoBehaviour
{
    public GameObject[] schermen;
    public int huidigScherm = 0;

    public TMP_InputField naamInput, beschrijvingInput, locatieNaamInput;

    public TMP_Text ErrorText;

    public GameObject parentObject;

    public TextMeshProUGUI Naam, Beschrijving, Locatie;

    public RawImage capturedImage;
    
    void OnAwake() => schermen[huidigScherm].SetActive(true);

    void Start() => schermen[huidigScherm].SetActive(true);

    public void VorigScherm()
    {
        schermen[huidigScherm].SetActive(false);
        huidigScherm = (huidigScherm - 1 + schermen.Length) % schermen.Length;
        schermen[huidigScherm].SetActive(true);
        DataPerStap(huidigScherm);
    }

    public void VolgendScherm()
    {
        schermen[huidigScherm].SetActive(false);
        huidigScherm = (huidigScherm + 1) % schermen.Length;
        schermen[huidigScherm].SetActive(true);
        DataPerStap(huidigScherm);
    }

    public void DataPerStap(int huidigScherm)
    {
        switch (huidigScherm)
        {
            case 1: VerwerkAfbeelding(); break;
            case 2: SlaTekstinvoerOp(); break;
            case 3: VerwerkLocatie(); LaatResultatenZien(); break;
            case 4: CombineerEnSlaDataOp(); break;
            case 5: Reset(); break;
        }
    }
    void VerwerkAfbeelding()
    {
        var takePicture = FindFirstObjectByType<TakePicture>();
        var imageBytes = (takePicture.image.texture as Texture2D).EncodeToPNG();
        PlayerPrefs.SetString("ImageBytes", System.Convert.ToBase64String(imageBytes));
    }

    void SlaTekstinvoerOp()
    {
        PlayerPrefs.SetString("Naam", naamInput.text);
        PlayerPrefs.SetString("Beschrijving", beschrijvingInput.text);
        PlayerPrefs.SetString("LocatieNaam", locatieNaamInput.text);

        if(string.IsNullOrEmpty(PlayerPrefs.GetString("Naam")) || string.IsNullOrEmpty(PlayerPrefs.GetString("Beschrijving")) || string.IsNullOrEmpty(PlayerPrefs.GetString("LocatieNaam")))
        {
            ErrorText.text = "Vul alle velden in!";
            VorigScherm();
            return;
        }
    }

    void VerwerkLocatie()
    {
        Input.location.Start();
        PlayerPrefs.SetFloat("Longitude", Input.location.lastData.longitude);
        PlayerPrefs.SetFloat("Latitude", Input.location.lastData.latitude);
        Input.location.Stop();
    }

    public void LaatResultatenZien()
    {
        capturedImage.texture = FindFirstObjectByType<TakePicture>().image.texture;

        Naam.text = "Naam:" + PlayerPrefs.GetString("Naam");

        Beschrijving.text = "Beschrijving:" + PlayerPrefs.GetString("Beschrijving");

        Locatie.text = "Locatienaam:" + PlayerPrefs.GetString("LocatieNaam");


    }

    void CombineerEnSlaDataOp()
    {
        var combinedValues = new Dictionary<string, string>
        {
            { "Foto", System.Convert.ToBase64String(System.Convert.FromBase64String(PlayerPrefs.GetString("ImageBytes"))) },
            { "Beschrijving", PlayerPrefs.GetString("Beschrijving") },
            { "Status", "In Beoordeling" },
            { "UploadDatum", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
            { "Naam", PlayerPrefs.GetString("Naam") },
            { "LocatieNaam", PlayerPrefs.GetString("LocatieNaam") },
            { "Longitude", PlayerPrefs.GetFloat("Longitude").ToString() },
            { "Latitude", PlayerPrefs.GetFloat("Latitude").ToString() },
        };
        Debug.Log(combinedValues);
        Opslaan(combinedValues);
    }

    public void Opslaan(Dictionary<string, string> combinedValues)
    {
        var aPIClass = new APIClass();
        StartCoroutine(aPIClass.ConnectToApi(aPIClass.InsertIntoDatabase("InheemseSoort", combinedValues), response =>
            Debug.Log("API Response: " + response)));

        Invoke("Reset", 3);
    }

    public void Reset()
    {
        SceneManager.LoadScene("Gast");
        return;
    }
}



