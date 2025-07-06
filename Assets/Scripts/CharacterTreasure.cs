using UnityEngine;
using TMPro;

public class CharacterTreasure : MonoBehaviour
{
    
    int crystals = 0;
    private TextMeshProUGUI treasureText;
    void Start()
    {
        GameObject treasureTextObject = GameObject.Find("PlayerTreasureText");
        treasureText = treasureTextObject.GetComponent<TextMeshProUGUI>();
        if (treasureText != null)
        {
            treasureText.text = "Crystals: " + crystals;
        }
    }
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
