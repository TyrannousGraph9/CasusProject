using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoginHandler : MonoBehaviour
{
   public TMP_InputField gebruikerField, wachtwoordField;
   public Button GebruikerButton;	

   public void Start()
   {
      GebruikerButton.onClick.AddListener(GebruikerLogin);
   }
   public void GebruikerLogin()
   {
      SceneManager.LoadScene("Gast");
   }
   public void SubmitLogin()
   {
         string gebruiker = gebruikerField.text;
         string wachtwoord = wachtwoordField.text;
   
         if (string.IsNullOrEmpty(gebruiker) || string.IsNullOrEmpty(wachtwoord))
         {
            Debug.LogError("Vul alle velden in.");
            return;
         }
   
         // Check of de gebruiker bestaat
         if (gebruiker == "admin" && wachtwoord == "admin")
         {
            Debug.Log("Inloggen gelukt!");
            SceneManager.LoadScene("Admin");
         }
         else
         {
            Debug.LogError("Inloggen mislukt.");
         }


   }
}
