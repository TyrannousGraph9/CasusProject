using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class GetUserLocation : MonoBehaviour
{
    public TextMeshProUGUI locationText;
    public RawImage mapImage;
    private string mapUrl = "https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/pin-s-l+000({0},{1})/{0},{1},14,0/600x300?access_token=pk.eyJ1IjoibmllbHNjcmVtZXJzIiwiYSI6ImNtNHJsdjNxZzA2cWoya3BkcXp0M2l3N3EifQ.7g4Ms4TNd9ZJPD0EcDM_yw";
    
    void OnEnable()
    {
        // On startup request for location access.
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.Camera);
        }
    }
    
    void Start()
    {
        StartCoroutine(GetLocation());
        Input.location.Start();
    }

    IEnumerator GetLocation()
    {
        if (!Input.location.isEnabledByUser)
        {
            locationText.text = "Location services are disabled.";
            yield break;
        }

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            locationText.text = "Timed out.";
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            locationText.text = "Unable to determine device location.";
            yield break;
        }
        else
        {
            float latitude = Input.location.lastData.latitude;
            float longitude = Input.location.lastData.longitude;
            locationText.text = "Location: " + latitude + ", " + longitude;
            StartCoroutine(LoadMap(latitude, longitude));
            StartCoroutine(UpdateLocation());
        }
    }

    IEnumerator LoadMap(float latitude, float longitude)
    {
        string url = string.Format(mapUrl, longitude, latitude);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            locationText.text = "Failed to load map.";
        }
        else
        {
            mapImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    IEnumerator UpdateLocation()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            if (Input.location.status == LocationServiceStatus.Running)
            {
                float latitude = Input.location.lastData.latitude;
                float longitude = Input.location.lastData.longitude;
                locationText.text = "Location: " + latitude + ", " + longitude;
                StartCoroutine(LoadMap(latitude, longitude));
            }
            else
            {
                locationText.text = "Unable to determine device location.";
            }
        }
    }
}
