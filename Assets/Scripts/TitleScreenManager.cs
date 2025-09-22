//Full class is student creation
using UnityEngine;
using UnityEngine.SceneManagement;
//Deals with the initial titles screen scene UI elements
public class TitleScreenManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void BeginTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}