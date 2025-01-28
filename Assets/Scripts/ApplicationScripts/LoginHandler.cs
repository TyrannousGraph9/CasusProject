using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoginHandler : MonoBehaviour
{
   public TMP_InputField gebruikerField, wachtwoordField;

   public TextMeshProUGUI errorText;
   public Button GebruikerButton;	

   public void Start()
   {
      PlayerPrefs.SetInt("IsAdmin", 0);
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
            errorText.text = "Vul alle velden in!";
            return;
         }
   
         // Check of de gebruiker bestaat
         if (gebruiker == "admin" && wachtwoord == "Z4yDH0G3Sch00l!@###")
         {
            PlayerPrefs.SetInt("IsAdmin", 1);
            SceneManager.LoadScene("Admin");
         }
         else
         {
            errorText.text = "Je hebt de foute inloggegevens ingevoerd!";
         }
   }
}
