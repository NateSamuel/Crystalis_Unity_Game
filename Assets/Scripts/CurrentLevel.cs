using UnityEngine;
using TMPro;

public class CurrentLevel : MonoBehaviour
{
    public int currentLevelNumber = 1;

    public TextMeshProUGUI[] levelTexts;

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