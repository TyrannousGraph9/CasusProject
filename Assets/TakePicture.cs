using System.IO;
using UnityEngine;
using UnityEngine.UI;
using NativeCameraNamespace;
using UnityEngine.Android;
using TMPro;
using Unity.VisualScripting;
public class TakePicture : MonoBehaviour
{
    public RawImage image;


    void OnEnable()
    {
        CapturePicture(2400);
    }

    public void CapturePicture(int maxSize)
    {

        if (!PlayerPrefs.HasKey("ImageId"))
        {
            PlayerPrefs.SetInt("ImageId", 1);
        }
        else
        {
            int imageId = PlayerPrefs.GetInt("ImageId");
            PlayerPrefs.SetInt("ImageId", imageId + 1);
            PlayerPrefs.Save();
        }


        if (NativeCamera.CheckPermission(true) != NativeCamera.Permission.Granted)
        {
            NativeCamera.RequestPermission(true);
        }
        Permission.RequestUserPermission(Permission.Camera);
        Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        Permission.RequestUserPermission(Permission.ExternalStorageRead);




        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {

            if (path != null)
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                // Assign texture to the RawImage
                image.texture = texture;

                


                // Save the texture to the local device
                SaveTextureToFile(texture, "CapturedImage" + PlayerPrefs.GetInt("ImageId") + ".png");
            }
        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }

    private void SaveTextureToFile(Texture2D texture, string fileName)
    {

        byte[] bytes = texture.EncodeToPNG();
        string directoryPath = Path.Combine("/storage/emulated/0/DCIM/Camera");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        string filePath = Path.Combine(directoryPath, fileName);
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Image saved to: " + filePath);

        // Save the path in PlayerPrefs
        PlayerPrefs.SetString("SavedImagePath", filePath);
        PlayerPrefs.Save();

        // Refresh Android gallery to show the saved image
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