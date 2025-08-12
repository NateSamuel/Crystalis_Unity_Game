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
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

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
            levelBuilder.GenerateRandom();
            currentLevel.IncrementLevel();
        }
    }
}