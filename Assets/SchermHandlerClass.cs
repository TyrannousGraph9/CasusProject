using UnityEngine;

public class SchermHandlerClass : MonoBehaviour
{
    public GameObject[] schermen;
    public int huidigScherm = 0;
    
    

    public void VorigScherm()
    {
        schermen[huidigScherm].SetActive(false);
        huidigScherm--;
        if (huidigScherm < 0)
        {
            huidigScherm = schermen.Length - 1;
        }
        schermen[huidigScherm].SetActive(true);
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
    }

    public void Opslaan()
    {
        // opslaan
    }
}
