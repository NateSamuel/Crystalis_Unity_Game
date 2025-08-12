using UnityEngine;
using TMPro;

public class CurrentLevel : MonoBehaviour
{
    public int currentLevelNumber = 1;
    public TextMeshProUGUI levelText;

    void Start()
    {
        UpdateLevelText();
    }

    public void IncrementLevel()
    {
        currentLevelNumber++;
        UpdateLevelText();
    }

    public void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "Level " + currentLevelNumber;
        }
    }
}