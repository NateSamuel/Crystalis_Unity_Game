using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialExitLevel : MonoBehaviour
{
    public Transform playerTransform;
    private Vector3 spawnPosition;
    public float collectionRange = 3f;
    private CharacterTreasure charTreasureScript;
    private CharacterHealth characterHealth;
    private TutorialLevelBuilder levelBuilder;
    private TutorialCurrentLevel currentLevel;
    private EnemyTrackerForObjectives tracker;
    public TutorialMainScreenManager uiManager;


    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
        characterHealth = playerObject.GetComponent<CharacterHealth>();
        levelBuilder = FindAnyObjectByType<TutorialLevelBuilder>();
        currentLevel = FindAnyObjectByType<TutorialCurrentLevel>();
        tracker = FindAnyObjectByType<EnemyTrackerForObjectives>();
        uiManager = FindAnyObjectByType<TutorialMainScreenManager>();
        spawnPosition = transform.position;
        if (uiManager != null)
        {
            uiManager.HideCollectTreasureUI();
        }
    }

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

    public void PlayerCanExit()
    {
        SceneManager.LoadScene("EntryScreen");
    }
    public void PlayerCanRetry()
    {
        SceneManager.LoadScene("TutorialScene");
    }
}