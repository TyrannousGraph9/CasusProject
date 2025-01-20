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
        
        // Dictionary<string, string> inheemseSoortValues = new Dictionary<string, string>
        // {
        //     { "Naam", "Hond" },
        //     { "LocatieNaam", "Parkstad" },
        //     { "Longitude", "5.6929" },
        //     { "Latitude", "52.4740" },
        //     { "Datum", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") } // Huidige datum en tijd
        // };

        // string jsonData = InsertIntoDatabase("InheemseSoort", inheemseSoortValues);
        // StartCoroutine(ConnectToApi(jsonData));
        
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

        
        if (databaseTable == "InheemseSoort")
        {
            // Controleer of alle verplichte velden voor 'InheemseSoort' aanwezig zijn
            if (!values.ContainsKey("Naam") || !values.ContainsKey("LocatieNaam") ||
                !values.ContainsKey("Longitude") || !values.ContainsKey("Latitude") ||
                !values.ContainsKey("UploadDatum"))
            {
                throw new ArgumentException("Naam, LocatieNaam, Longitude, Latitude en Datum zijn verplicht voor de InheemseSoort-tabel.");
            }

            // Bouw de SQL-query
            string columns = string.Join(", ", values.Keys); // Kolomnamen combineren
            string valuesList = string.Join(", ", values.Values.Select(value => $"'{value}'")); // Waarden combineren met quotes
            query = $"INSERT INTO InheemseSoort ({columns}) VALUES ({valuesList})";
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
            query = $"SELECT {columnToSearch} FROM {databaseTable} WHERE {columnToSearch} = '{searchValue}' AND Status = 'Goedgekeurd'";
        }
        else if (columnToSearch != "*")
        {
            query = $"SELECT {columnToSearch} FROM {databaseTable} WHERE Status = 'Goedgekeurd'";
        }
        else
        {
            query = $"SELECT * FROM {databaseTable} WHERE Status = 'Goedgekeurd'";
        }

        // Zet de query om in JSON-formaat
        if (!string.IsNullOrEmpty(query))
        {
            jsonData = JsonUtility.ToJson(new QueryData { query = query });
        }

        return jsonData;
    }

    public string DatabaseLoginHandler(string gebruikersnaam, string wachtwoord)
    {
        string jsonData = JsonUtility.ToJson(new QueryData { query = "SELECT Role FROM Gebruiker WHERE Gebruikersnaam = " + gebruikersnaam +" AND Wachtwoord = " + wachtwoord +"" });
        // change password dat we joinen vanuit een andere table
        StartCoroutine(ConnectToApi(jsonData, (response) => {
        Debug.Log("API Response: " + response);}));

        return jsonData;        
    }

    // Coroutine om de SQL-query naar de API te sturen
    public IEnumerator ConnectToApi(string jsonData, Action<List<DataItem>> callback)
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
            callback(null);
        }
        else
        {
            Debug.Log("Reactie: " + request.downloadHandler.text);
            List<DataItem> filteredData = FilterResponse(request.downloadHandler.text);
            callback(filteredData);
        }
    }

    private List<DataItem> FilterResponse(string response)
    {
        response = response.Replace("\n", "");

        ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(response);

        if (apiResponse.success)
        {
            return apiResponse.data;
        }
        else
        {
            Debug.LogError("API response indicates failure.");
            return null;
        }
    }
}

// Klasse om de querydata op te slaan voor JSON-serialisatie
[System.Serializable]
public class QueryData
{
    public string query;
}

// serialize data zodat we precieser kunnen inzien en bekijken wat we terugkrijgen
[System.Serializable]
public class ApiResponse
{
    public bool success;
    public List<DataItem> data;
}

[System.Serializable]
public class DataItem
{   
    public string Foto;
    public string Naam;
    public string LocatieNaam;
    public string Longitude;
    public string Latitude;
    public string Datum;
}