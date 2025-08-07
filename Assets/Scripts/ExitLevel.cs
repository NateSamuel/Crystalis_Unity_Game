using System.Collections;
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

    
    IEnumerator Start()
    {
        yield return null;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }

        GameObject textObject = GameObject.Find("LevelText");
        if (textObject != null)
            levelText = textObject.GetComponent<TextMeshProUGUI>();

        levelBuilder = FindAnyObjectByType<LevelBuilder>();
        currentLevel = FindAnyObjectByType<CurrentLevel>();

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
                levelText.text = "Level " + currentLevel.currentLevelNumber;
            }
        }
    }
}
