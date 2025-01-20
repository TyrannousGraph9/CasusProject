using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class LoginHandler : MonoBehaviour
{
   public TMP_InputField gebruikerField, wachtwoordField;

   public void SubmitLogin()
   {
      APIClass aPIClass = new APIClass();
      
      string response = aPIClass.DatabaseLoginHandler(gebruikerField.text, wachtwoordField.text);

      switch(response)
      {
         case "Gebruiker":
            Debug.Log("Gebruiker ingelogd");
            SceneManager.LoadScene("gast");
            break;
         case "Admin":
            Debug.Log("Admin ingelogd");
            SceneManager.LoadScene("admin");
            break;
         default:
            Debug.Log("Gebruiker niet gevonden");
            // errortxt.text = "Gebruiker niet gevonden";
            break;
      }


   }
}
