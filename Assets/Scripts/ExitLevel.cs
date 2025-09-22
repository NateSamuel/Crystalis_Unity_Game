//Full class is student creation
using UnityEngine;
//Stores methods about exiting the level
public class ExitLevel : MonoBehaviour
{
    public Transform playerTransform;
    private Vector3 spawnPosition;
    public float collectionRange = 3f;
    private CharacterTreasure charTreasureScript;
    private CharacterHealth characterHealth;
    private LevelBuilder levelBuilder;
    private CurrentLevel currentLevel;
    private EnemyTrackerForObjectives tracker;
    public MainScreenManager uiManager;
    private CharacterLevelUps charLevelUps;
    private CharacterPurchases charPurchases;
    private EnemyDifficultyIncrease difficultyManager;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
        characterHealth = playerObject.GetComponent<CharacterHealth>();
        levelBuilder = FindAnyObjectByType<LevelBuilder>();
        currentLevel = FindAnyObjectByType<CurrentLevel>();
        tracker = FindAnyObjectByType<EnemyTrackerForObjectives>();
        uiManager = FindAnyObjectByType<MainScreenManager>();
        spawnPosition = transform.position;
        if (uiManager != null)
        {
            uiManager.HideCollectTreasureUI();
        }
        charLevelUps = FindAnyObjectByType<CharacterLevelUps>();
        charPurchases = FindAnyObjectByType<CharacterPurchases>();
        difficultyManager = FindAnyObjectByType<EnemyDifficultyIncrease>();
    }
    //if near level exit point, button appears if you have completed objectives
    void Update()
    {
        int enemiesLeft = tracker.activeEnemies + tracker.activeBosses;
        if (playerTransform != null && Vector3.Distance(spawnPosition, playerTransform.position) < collectionRange && enemiesLeft == 0)
        {
            uiManager?.ShowCompleteLevelUI(PlayerCanExit);
        }
        else
        {
            uiManager?.HideCompleteLevelUI();
        }
    }
    //Updates available purchases and level ups for new level
    public void PlayerCanExit()
    {

        charLevelUps.levelUpsPurchasablePerLevel += 3;
        charLevelUps.UpdateAvailableLevelUps();
        if (charPurchases.GetTotalPurchases() - 4 + charPurchases.abilitiesPurchasablePerLevel < 6)
        {
            int minForPurchasableAmounts = Mathf.Min(2, 5 - (charPurchases.GetTotalPurchases() - 4 + charPurchases.abilitiesPurchasablePerLevel));
            charPurchases.abilitiesPurchasablePerLevel += minForPurchasableAmounts;
            charPurchases.UpdateAvailablePurchasesTexts();
        }

        if (levelBuilder != null && currentLevel != null)
        {
            levelBuilder.DeactivateAllUnits();
            currentLevel.IncrementLevel();
            levelBuilder.GenerateRandom();
        }

        FollowCamera camera = FindAnyObjectByType<FollowCamera>();
        camera?.ResetCameraToStart();
        charTreasureScript?.ApplyTreasure(10);
        uiManager?.HideCompleteLevelUI();
        uiManager.NewLevelScreenAfterPrevLevel();

    }

    //if player has died, exits level in a different way that preserves the abilities and level ups they purchased
    public void PlayerCanRetry()
    {
        if (levelBuilder != null && currentLevel != null)
        {
            levelBuilder.DeactivateAllUnits();
            currentLevel.RevertToPreviousLevel();
            levelBuilder.GenerateRandom();
            characterHealth?.CharacterComesAliveAgain();
            charTreasureScript?.ResetTreasure(15);
        }

        FollowCamera camera = FindAnyObjectByType<FollowCamera>();
        camera?.ResetCameraToStart();

    }
}