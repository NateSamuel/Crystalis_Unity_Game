//Full class is student creation
using UnityEngine;
using TMPro;
using System.Collections.Generic;

//Adds, removes, updates, resets character treasure amount
public class CharacterTreasure : MonoBehaviour
{
    public int crystals = 200;

    [SerializeField] private List<TextMeshProUGUI> treasureTexts;

    void Start()
    {
        UpdateTreasureUI();
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
    public void ResetTreasure(int treasureAmount)
    {
        if (treasureAmount > crystals)
        {
            crystals = treasureAmount;
        }
        UpdateTreasureUI();
    }

    private void UpdateTreasureUI()
    {
        foreach (var text in treasureTexts)
        {
            if (text != null)
            {
                text.text = " " + crystals;
            }
        }
    }
}