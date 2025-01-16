using System.IO;
using UnityEngine;
using UnityEngine.UI;
using NativeCameraNamespace;
using UnityEngine.Android;
using TMPro;
public class TakePicture : MonoBehaviour
{
    public RawImage image;

    public TextMeshProUGUI locationText, locationText2;



    public void CapturePicture(int maxSize)
    {
        if (NativeCamera.CheckPermission(true) != NativeCamera.Permission.Granted)
        {
            NativeCamera.RequestPermission(true);
        }
        Permission.RequestUserPermission(Permission.Camera);
        Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        Permission.RequestUserPermission(Permission.ExternalStorageRead);




        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            locationText.text = "Image path: " + path;
            if (path != null)
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                // Assign texture to the RawImage
                image.texture = texture;

                // Save the texture to the local device
                SaveTextureToFile(texture, "CapturedImage.png");
            }
        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }

    // not working yet
    private void SaveTextureToFile(Texture2D texture, string fileName)
    {
        byte[] bytes = texture.EncodeToPNG();
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(filePath, bytes);
        locationText2.text = "Image saved to: " + filePath;

        
        #if UNITY_ANDROID
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (AndroidJavaObject mediaScannerConnection = new AndroidJavaClass("android.media.MediaScannerConnection"))
                {
                    mediaScannerConnection.CallStatic("scanFile", currentActivity, new string[] { filePath }, null, null);
                }
            }
        }
        #endif
    }
}