using UnityEngine;

public class ExitLevel : MonoBehaviour
{
    public Transform playerTransform;
    private Vector3 spawnPosition;
    public float collectionRange = 3f;

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

    void PlayerCanExit()
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

        uiManager.NewLevelScreenAfterPrevLevel();
    }
}