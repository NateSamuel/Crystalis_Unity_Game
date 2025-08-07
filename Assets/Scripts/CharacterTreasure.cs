using UnityEngine;
using TMPro;

public class CharacterTreasure : MonoBehaviour
{
    
    public int crystals = 0;
    private TextMeshProUGUI treasureText;
    void Start()
    {
        GameObject treasureTextObject = GameObject.Find("PlayerTreasureText");
        treasureText = treasureTextObject.GetComponent<TextMeshProUGUI>();
        if (treasureText != null)
        {
            treasureText.text = " " + crystals;
        }
    }
    public void ApplyTreasure(int treasureAmount)
    {
        crystals += treasureAmount;
        UpdateTreasureUI();
    }
    public void RemoveTreasure(int treasureAmount)
    {
        if (crystals >= treasureAmount)
        {
            crystals -= treasureAmount;
            UpdateTreasureUI();
        }
    }
    void UpdateTreasureUI()
    {
        if (treasureText != null)
        {
            treasureText.text = " " + crystals;
        }
    }
}
