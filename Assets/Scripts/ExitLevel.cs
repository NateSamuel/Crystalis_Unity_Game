using UnityEngine;

public class ExitLevel : MonoBehaviour
{
    public Transform playerTransform;
    private Vector3 spawnPosition;
    public float collectionRange = 3f;

    private LevelBuilder levelBuilder;
    private CurrentLevel currentLevel;

    void Start()
    {
        // Find the player in the scene
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure it's tagged as 'Player'.");
        }

        // Grab required references
        levelBuilder = FindAnyObjectByType<LevelBuilder>();
        currentLevel = FindAnyObjectByType<CurrentLevel>();

        spawnPosition = transform.position;
    }

    void Update()
    {
        if (playerTransform != null && Vector3.Distance(spawnPosition, playerTransform.position) < collectionRange)
        {
            PlayerCanExit();
        }
    }

    void PlayerCanExit()
    {
        if (levelBuilder != null && currentLevel != null)
        {
            levelBuilder.GenerateRandom();     // Regenerate or load new content
            currentLevel.IncrementLevel();     // Update level and UI
        }
    }
}