using UnityEngine;
using TMPro;

public class ExitLevel : MonoBehaviour
{
    public Transform playerTransform;
    Vector3 spawnPosition;
    public float collectionRange = 3f;
    private LevelBuilder levelBuilder;
    private CurrentLevel currentLevel;
    private TextMeshProUGUI levelText;

    
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        GameObject textObject = GameObject.Find("LevelText");
        levelText = textObject.GetComponent<TextMeshProUGUI>();
        levelBuilder = FindObjectOfType<LevelBuilder>();
        currentLevel = FindObjectOfType<CurrentLevel>();
        
        
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }

        spawnPosition = transform.position;
    }

    
    void Update()
    {
        if (Vector3.Distance(spawnPosition, playerTransform.position) < collectionRange)
        {
            PlayerCanExit();
        }
    }
    void PlayerCanExit()
    {
        if (levelBuilder != null && currentLevel != null)
        {
            levelBuilder.GenerateRandom();
            currentLevel.currentLevelNumber ++;
            Debug.Log(currentLevel.currentLevelNumber);
            
            if (levelText != null)
            {
                levelText.text = "Level: " + currentLevel.currentLevelNumber;
            }
        }
    }
}
