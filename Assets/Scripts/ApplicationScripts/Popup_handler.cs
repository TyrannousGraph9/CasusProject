using System;
using UnityEngine;

public class Popup_handler : MonoBehaviour
{
    public GameObject[] popupHolder;

    public int popupCount;
    
    public GameObject mainScreen;
    void Start()
    {

        if(PlayerPrefs.GetInt("Tutorial") == 0)
        {
            foreach(GameObject popup in popupHolder)
            {
                popup.SetActive(false);
            }   
                
            popupHolder[0].SetActive(true);
            popupCount = 0;
            PlayerPrefs.SetInt("Tutorial", 1);
        }
        else
        {
            mainScreen.SetActive(false);
        }
    }

    public void Deny()
    {
        Destroy(mainScreen);
        PlayerPrefs.SetInt("Tutorial", 1);
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
        int isAdmin = PlayerPrefs.GetInt("IsAdmin");
        if (isAdmin == 1)
        {
            TutorialTextsAdmin tutorialTexts = new TutorialTextsAdmin();
            string popupText = tutorialTexts.GetText(popupCount);
            return popupText;
        }
        else
        {
            TutorialTexts tutorialTexts = new TutorialTexts();
            string popupText = tutorialTexts.GetText(popupCount);
            return popupText;
        }
   }
        
}