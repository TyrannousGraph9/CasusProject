using UnityEngine;
using UnityEngine.SceneManagement;
public class LogOut : MonoBehaviour
{
    public void LogOutUser()
    {
        SceneManager.LoadScene("Login");
    }
}
