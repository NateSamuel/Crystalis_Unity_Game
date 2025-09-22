//Full class is student creation
using UnityEngine;
using TMPro;
//Stores the current level and updates all text components that use it
//Also increments the level, and reverts it back to level 1 if the player has died.
public class CurrentLevel : MonoBehaviour
{
    public int currentLevelNumber = 1;

    public TextMeshProUGUI[] levelTexts;
    public EnemyDifficultyIncrease difficultyManager;

    void Start()
    {
        UpdateLevelText();
    }

    public void IncrementLevel()
    {
        currentLevelNumber++;
        UpdateLevelText();
        difficultyManager?.UpdateLevelDifficultyInfo(currentLevelNumber);

    }

    public void RevertToLevelOne()
    {
        currentLevelNumber = 1;
        UpdateLevelText();
        difficultyManager?.UpdateLevelDifficultyInfo(currentLevelNumber);
    }

    public void UpdateLevelText()
    {
        if (levelTexts != null && levelTexts.Length > 0)
        {
            foreach (var text in levelTexts)
            {
                if (text != null)
                {
                    text.text = "Level " + currentLevelNumber;
                }
            }
        }
    }
}