using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenManager : MonoBehaviour
{
    public GameObject LevelEntryPanel;
    public GameObject LevelObjectivesPanel;
    public GameObject MainUIPanel;
    public GameObject PlayerDeathPanel;
    public GameObject PauseMenuPanel;
    public GameObject PurchaseAbilities;

    void Start()
    {
        LevelEntryPanel.SetActive(true);
        LevelObjectivesPanel.SetActive(false);
        MainUIPanel.SetActive(false);
        PlayerDeathPanel.SetActive(false);
        PauseMenuPanel.SetActive(false);
        PurchaseAbilities.SetActive(false);
    }
    public void BackToEntryScreen()
    {
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }
        SceneManager.LoadScene("EntryScreen");
    }
    
    public void ShowObjectives()
    {
        LevelEntryPanel.SetActive(false);
        LevelObjectivesPanel.SetActive(true);
    }
    public void ShowLevelPanel()
    {
        LevelEntryPanel.SetActive(true);
        LevelObjectivesPanel.SetActive(false);
    }
    public void StartMainGameUI()
    {
        LevelObjectivesPanel.SetActive(false);
        MainUIPanel.SetActive(true);
    }
    public void ShowObjectivesFromMainScreen()
    {
        MainUIPanel.SetActive(false);
        LevelEntryPanel.SetActive(false);
        LevelObjectivesPanel.SetActive(true);
    }
    public void PauseGame()
    {
        PauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void UnpauseGame()
    {
        PauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void ShowPurchaseAbilitiesPanel()
    {
        MainUIPanel.SetActive(false);
        PurchaseAbilities.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ExitPurchaseAbilitiesPanel()
    {
        MainUIPanel.SetActive(true);
        PurchaseAbilities.SetActive(false);
        Time.timeScale = 1f;
    }

}