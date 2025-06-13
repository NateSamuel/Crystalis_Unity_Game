using UnityEngine;
using TMPro;

public class CharacterTreasure : MonoBehaviour
{
    
    int crystals = 0;
    public TextMeshProUGUI treasureText;

    public void ApplyTreasure()
    {
        crystals += 2;
        UpdateTreasureUI();
    }
    void UpdateTreasureUI()
    {
        if (treasureText != null)
        {
            treasureText.text = "Crystals: " + crystals;
        }
    }
}
