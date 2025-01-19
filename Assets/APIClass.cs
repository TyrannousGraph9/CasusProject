using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class APIClass : MonoBehaviour
{
    // De API-endpoint voor de databaseverbinding
    private string connectionString = "http://98.71.252.54/apinederland.php";

    void Start()
    {
        // Voorbeeld 1: Data toevoegen aan de 'InheemseSoort'-tabel
        
        Dictionary<string, string> inheemseSoortValues = new Dictionary<string, string>
        {
            { "Naam", "Hond" },
            { "LocatieNaam", "Parkstad" },
            { "Longitude", "5.6929" },
            { "Latitude", "52.4740" },
            { "Datum", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") } // Huidige datum en tijd
        };

        string jsonData = InsertIntoDatabase("InheemseSoort", inheemseSoortValues);
        StartCoroutine(ConnectToApi(jsonData));
        

        // Voorbeeld 2: Data toevoegen aan de 'Fotos'-tabel
        /*
        Dictionary<string, string> fotosValues = new Dictionary<string, string>
        {
            { "FotoURL", "https://example.com/images/golden_eagle.jpg" },
            { "Beschrijving", "Een prachtige foto van een Golden Eagle." },
            { "Status", "In Beoordeling" }, // Standaardstatus
            { "UploadDatum", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") } // Huidige datum en tijd
        };

        string jsonData = InsertIntoDatabase("Fotos", fotosValues);
        StartCoroutine(ConnectToApi(jsonData));
        */
    }

    // Methode om een SQL INSERT-query te bouwen en om te zetten naar JSON
    public string InsertIntoDatabase(string databaseTable, Dictionary<string, string> values)
    {
        string query = null; // Hier wordt de SQL-query opgeslagen
        string jsonData = null; // Hier wordt de query als JSON opgeslagen

        // Controleren of de invoer geldig is
        if (string.IsNullOrEmpty(databaseTable) || values == null || values.Count == 0)
        {
            throw new ArgumentException("De tabelnaam en waarden moeten worden opgegeven.");
        }

        if (databaseTable == "Fotos")
        {
            // Controleer of alle verplichte velden voor 'Fotos' aanwezig zijn
            if (!values.ContainsKey("FotoURL") || !values.ContainsKey("Beschrijving") ||
                !values.ContainsKey("Status")) // Optioneel: controleer ook op 'GebruikerID'
            {
                throw new ArgumentException("FotoURL, Beschrijving, Status en GebruikerID zijn verplicht voor de Fotos-tabel.");
            }

            // Bouw de SQL-query
            if (values.Count == 4)
            {
                string columns = string.Join(", ", values.Keys); // Kolomnamen combineren
                string valuesList = string.Join(", ", values.Values.Select(value => $"'{value}'")); // Waarden combineren met quotes
                query = $"INSERT INTO Fotos ({columns}) VALUES ({valuesList})";
            }
            else
            {
                throw new ArgumentException("Ongeldig aantal waarden voor de Fotos-tabel.");
            }
        }
        else if (databaseTable == "InheemseSoort")
        {
            // Controleer of alle verplichte velden voor 'InheemseSoort' aanwezig zijn
            if (!values.ContainsKey("Naam") || !values.ContainsKey("LocatieNaam") ||
                !values.ContainsKey("Longitude") || !values.ContainsKey("Latitude") ||
                !values.ContainsKey("Datum"))
            {
                throw new ArgumentException("Naam, LocatieNaam, Longitude, Latitude en Datum zijn verplicht voor de InheemseSoort-tabel.");
            }

            // Bouw de SQL-query
            if (values.Count == 5)
            {
                string columns = string.Join(", ", values.Keys); // Kolomnamen combineren
                string valuesList = string.Join(", ", values.Values.Select(value => $"'{value}'")); // Waarden combineren met quotes
                query = $"INSERT INTO InheemseSoort ({columns}) VALUES ({valuesList})";
            }
            else
            {
                throw new ArgumentException("Ongeldig aantal waarden voor de InheemseSoort-tabel.");
            }
        }
        else
        {
            throw new ArgumentException("Ongeldige tabelnaam.");
        }

        // Zet de query om in JSON-formaat
        if (!string.IsNullOrEmpty(query))
        {
            jsonData = JsonUtility.ToJson(new QueryData { query = query });
        }

        return jsonData;
    }

    // Methode om een SQL SELECT-query te bouwen en om te zetten naar JSON
    public string SelectFromDatabase(string databaseTable, string columnToSearch = "*", string searchValue = null)
    {
        string query = null; // Hier wordt de SQL-query opgeslagen
        string jsonData = null; // Hier wordt de query als JSON opgeslagen

        // Controleren of de invoer geldig is
        if (string.IsNullOrEmpty(databaseTable))
        {
            throw new ArgumentException("De tabelnaam moet worden opgegeven.");
        }

        // Bouw de SQL-query op basis van de invoer
        if (!string.IsNullOrEmpty(searchValue) && columnToSearch != "*")
        {
            query = $"SELECT {columnToSearch} FROM {databaseTable} WHERE {columnToSearch} = '{searchValue}'";
        }
        else if (columnToSearch != "*")
        {
            query = $"SELECT {columnToSearch} FROM {databaseTable}";
        }
        else if (string.IsNullOrEmpty(searchValue))
        {
            query = $"SELECT * FROM {databaseTable}";
        }

        // Zet de query om in JSON-formaat
        if (!string.IsNullOrEmpty(query))
        {
            jsonData = JsonUtility.ToJson(new QueryData { query = query });
        }

        return jsonData;
    }

    // Coroutine om de SQL-query naar de API te sturen
    private IEnumerator ConnectToApi(string jsonData)
    {
        // Instellen van de UnityWebRequest voor een POST-aanvraag
        UnityWebRequest request = new UnityWebRequest(connectionString, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData); // Converteer JSON-data naar bytes
        request.uploadHandler = new UploadHandlerRaw(bodyRaw); // Voeg de data toe
        request.downloadHandler = new DownloadHandlerBuffer(); // Ontvang de reactie
        request.SetRequestHeader("Content-Type", "application/json"); // Stel de content type in
        request.SetRequestHeader("X-API-Key", "veiligekey123"); // Voeg een beveiligingssleutel toe

        // Wacht op de reactie van de API
        yield return request.SendWebRequest();

        // Verwerk de reactie
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Fout: " + request.error);
            Debug.LogError("Reactiecode: " + request.responseCode);
            Debug.LogError("Reactie: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Reactie: " + request.downloadHandler.text);
        }
    }
}

// Klasse om de querydata op te slaan voor JSON-serialisatie
[System.Serializable]
public class QueryData
{
    public string query;
}
