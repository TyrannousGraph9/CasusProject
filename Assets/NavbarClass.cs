using UnityEngine;

public class NavbarClass : MonoBehaviour
{
    public GameObject cameraPanel, infoPanel, landingScherm;
    
    public void SwitchScreen(string activeScreen)
    {
        switch(activeScreen)
        {
            case "camera":
                cameraPanel.SetActive(true);
                infoPanel.SetActive(false);
                landingScherm.SetActive(false);
                break;

            case "info":
                infoPanel.SetActive(true);      
                cameraPanel.SetActive(false);
                landingScherm.SetActive(false);
                break;
        }
    }


    public void OpenInfo()
    {
       SwitchScreen("info");
    }


    public void OpenCamera()
    {
        SwitchScreen("camera");
    }
}