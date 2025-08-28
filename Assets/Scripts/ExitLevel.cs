using UnityEngine;

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
    }

    void Update()
    {
        int enemiesLeft = tracker.activeEnemies + tracker.activeBosses;
        if (playerTransform != null && Vector3.Distance(spawnPosition, playerTransform.position) < collectionRange  && enemiesLeft == 0)
        {
            PlayerCanExit();
        }
    }

    public void PlayerCanExit()
    {
        if (levelBuilder != null && currentLevel != null)
        {
            levelBuilder.GenerateRandom();
            currentLevel.IncrementLevel();
        }

        FollowCamera camera = FindAnyObjectByType<FollowCamera>();
        if (camera != null)
        {
            camera.ResetCameraToStart();
        }
        charTreasureScript?.ApplyTreasure(10);
        uiManager.NewLevelScreenAfterPrevLevel();
    }
    public void PlayerCanRetry()
    {
        if (levelBuilder != null && currentLevel != null)
        {
            currentLevel?.RevertToLevelOne();
            levelBuilder?.GenerateRandom();
            characterHealth?.CharacterComesAliveAgain();
            charTreasureScript?.ResetTreasure(15);
            EnemyHealth[] allEnemies = FindObjectsOfType<EnemyHealth>();

            foreach (EnemyHealth enemy in allEnemies)
            {
                enemy.ResetHealth();
            }
            
        }

        FollowCamera camera = FindAnyObjectByType<FollowCamera>();
        if (camera != null)
        {
            camera.ResetCameraToStart();
        }
    }
}