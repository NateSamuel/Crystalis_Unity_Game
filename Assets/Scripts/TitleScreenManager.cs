using UnityEngine;
using UnityEngine.SceneManagement;

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