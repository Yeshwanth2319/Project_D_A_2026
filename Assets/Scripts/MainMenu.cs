using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadGame()
    {
        Debug.Log("Load Game button clicked");
    }
    public void Options()
    {
        Debug.Log("Options button clicked");
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game button clicked");
        Application.Quit();
    }
}
