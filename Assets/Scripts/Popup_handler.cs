using System;
using UnityEngine;

public class Popup_handler : MonoBehaviour
{
    public GameObject[] popupHolder;

    public int popupCount;
    
    public GameObject popupHolderToDelete;
    void Start()
    {   
        int isTutorialChecked = PlayerPrefs.GetInt("tutorial");
        
        if(isTutorialChecked == 0)
        {
            foreach(GameObject popup in popupHolder)
            {
                popup.SetActive(false);
            }   
            
            popupHolder[0].SetActive(true);
            popupCount = 0;
        }
        else
        {
            foreach(GameObject popup in popupHolder)
            {
                popup.SetActive(false);
            }
        }
    }

    public void Deny()
    {
        Destroy(popupHolderToDelete);
        PlayerPrefs.SetInt("tutorial", 1);
    }

    public void Confirm(int setActive)
    {
        for(int i = 0; i < popupHolder.Length; i++)
        {
            if(i == setActive)
            {
                popupHolder[i].SetActive(true);
                popupCount++;
                string textToChange = ChangeText(popupCount);
                popupHolder[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = textToChange;
                popupHolder[i-1].SetActive(false);
            }
            else
            {
                popupHolder[i].SetActive(false);
            }
        }
   }

   public string ChangeText(int popupCount)
   {
        TutorialTexts tutorialTexts = new TutorialTexts();
        string popupText = tutorialTexts.GetText(popupCount);
        
        return popupText;
   }
        
}
