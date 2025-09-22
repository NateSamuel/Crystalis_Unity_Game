//Full class is student creation
using UnityEngine;
using UnityEngine.SceneManagement;

//This deals with enabling an disabling buttons and panels from the UI based on the states in the game
//please see the method titles for their specific uses
public class MainScreenManager : MonoBehaviour
{
    public GameObject LevelEntryPanel;
    public GameObject LevelObjectivesPanel;
    public GameObject MainUIPanel;
    public GameObject PlayerDeathPanel;
    public GameObject PauseMenuPanel;
    public GameObject PurchaseAbilities;
    public GameObject LevelUpAbilities;

    private ExitLevel exitLevel;
    [SerializeField] LayoutGeneratorRooms layoutGeneratorRooms;
    public UnityEngine.UI.Button CollectKeyButton;
    public UnityEngine.UI.Button FreePrisonerButton;
    public UnityEngine.UI.Button CollectTreasureButton;
    public UnityEngine.UI.Button CompleteLevelButton;
    public UnityEngine.UI.Button OpenDoorButton;

    void Start()
    {
        LevelEntryPanel.SetActive(true);
        LevelObjectivesPanel.SetActive(false);
        MainUIPanel.SetActive(false);
        PlayerDeathPanel.SetActive(false);
        PauseMenuPanel.SetActive(false);
        PurchaseAbilities.SetActive(false);
        LevelUpAbilities.SetActive(false);

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
        Time.timeScale = 1f;
    }
    public void ShowLevelPanel()
    {
        LevelEntryPanel.SetActive(true);
        LevelObjectivesPanel.SetActive(false);
        Time.timeScale = 0f;
    }
    public void StartMainGameUI()
    {
        LevelObjectivesPanel.SetActive(false);
        MainUIPanel.SetActive(true);
        FreePrisonerButton.gameObject.SetActive(false);
        CollectKeyButton.gameObject.SetActive(false);
        CollectTreasureButton.gameObject.SetActive(false);
        CompleteLevelButton.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
    public void ShowObjectivesFromMainScreen()
    {
        MainUIPanel.SetActive(false);
        LevelEntryPanel.SetActive(false);
        LevelObjectivesPanel.SetActive(true);
        Time.timeScale = 0f;
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

    public void NewLevelScreenAfterPrevLevel()
    {
        LevelEntryPanel.SetActive(true);
        LevelObjectivesPanel.SetActive(false);
        MainUIPanel.SetActive(false);
        PlayerDeathPanel.SetActive(false);
        PauseMenuPanel.SetActive(false);
        PurchaseAbilities.SetActive(false);
        LevelUpAbilities.SetActive(false);
    }
    public void ShowLevelUpAbilitiesPanel()
    {
        MainUIPanel.SetActive(false);
        LevelUpAbilities.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ExitLevelUpAbilitiesPanel()
    {
        MainUIPanel.SetActive(true);
        LevelUpAbilities.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RetryScreenAfterDeath()
    {
        Debug.Log("RetryScreenAfterDeath() CALLED");

        layoutGeneratorRooms.LevelConfig.ResetMaxRoomCount();

        exitLevel = FindAnyObjectByType<ExitLevel>();

        if (exitLevel != null)
        {
            exitLevel.PlayerCanRetry();
        }
        else
        {
            Debug.LogError("ExitLevel component NOT FOUND in the scene.");
        }

        LevelEntryPanel.SetActive(true);
        LevelObjectivesPanel.SetActive(false);
        MainUIPanel.SetActive(false);
        PlayerDeathPanel.SetActive(false);
        PauseMenuPanel.SetActive(false);
        PurchaseAbilities.SetActive(false);
        LevelUpAbilities.SetActive(false);
    }

    public void ShowCollectKeyUI(UnityEngine.Events.UnityAction onClick)
    {
        CollectKeyButton.gameObject.SetActive(true);

        CollectKeyButton.onClick.RemoveAllListeners();
        CollectKeyButton.onClick.AddListener(onClick);
    }

    public void HideCollectKeyUI()
    {
        CollectKeyButton.gameObject.SetActive(false);
        CollectKeyButton.onClick.RemoveAllListeners();
    }

    public void ShowFreePrisonerUI(UnityEngine.Events.UnityAction onClick)
    {
        FreePrisonerButton.gameObject.SetActive(true);

        FreePrisonerButton.onClick.RemoveAllListeners();
        FreePrisonerButton.onClick.AddListener(onClick);
    }
    public void HideFreePrisonerUI()
    {
        FreePrisonerButton.gameObject.SetActive(false);
        FreePrisonerButton.onClick.RemoveAllListeners();
    }
    public void ShowCollectTreasureUI(UnityEngine.Events.UnityAction onClick)
    {
        CollectTreasureButton.gameObject.SetActive(true);

        CollectTreasureButton.onClick.RemoveAllListeners();
        CollectTreasureButton.onClick.AddListener(onClick);
    }

    public void HideCollectTreasureUI()
    {
        CollectTreasureButton.gameObject.SetActive(false);
        CollectTreasureButton.onClick.RemoveAllListeners();
    }
    public void ShowCompleteLevelUI(UnityEngine.Events.UnityAction onClick)
    {
        CompleteLevelButton.gameObject.SetActive(true);

        CompleteLevelButton.onClick.RemoveAllListeners();
        CompleteLevelButton.onClick.AddListener(onClick);
    }

    public void HideCompleteLevelUI()
    {
        CompleteLevelButton.gameObject.SetActive(false);
        CompleteLevelButton.onClick.RemoveAllListeners();
    }
    public void ShowDoorOpenUI(UnityEngine.Events.UnityAction onClick)
    {
        OpenDoorButton.gameObject.SetActive(true);

        OpenDoorButton.onClick.RemoveAllListeners();
        OpenDoorButton.onClick.AddListener(onClick);
    }
    public void HideDoorOpenUI()
    {
        OpenDoorButton.gameObject.SetActive(false);
        OpenDoorButton.onClick.RemoveAllListeners();
    }
}