//Full class is student creation
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

//Applies multiplier to LevelUpAbilities List when button clicked if enough money and if there are enough available level us for that level
public class CharacterLevelUps : MonoBehaviour
{
    public int levelUpCost = 5;
    public List<LevelUpAbilities> abilities = new List<LevelUpAbilities>();
    private Transform playerTransform;
    private CharacterTreasure charTreasureScript;
    private CharacterHealth charHealthScript;
    private CharacterAttack charAttackScript;
    public int levelUpsPurchasablePerLevel = 3;
    public TextMeshProUGUI[] availableLevelUpTexts;
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
            charHealthScript = playerTransform.GetComponent<CharacterHealth>();
            charAttackScript = playerTransform.GetComponent<CharacterAttack>();
        }
    }
    //checks if spell is able to be levelled up
    public void LevelUpSpellByName(string spellName)
    {
        if (charTreasureScript != null && charTreasureScript.crystals >= levelUpCost)
        {
            charTreasureScript.RemoveTreasure(levelUpCost);
            ApplyMultiplier(spellName);
            levelUpsPurchasablePerLevel -= 1;
            UpdateAvailableLevelUps();
        }
    }

    //applies multiplier to that specific ability
    private void ApplyMultiplier(string spellName)
    {
        LevelUpAbilities ability = abilities.Find(a => a.name == spellName);
        if (ability != null)
        {
            ability.levelUpIncreaseMultiplier += 1;
            ability.currentStatAmount = ability.originalStatAmount * (1 + (ability.levelUpIncreaseMultiplier * ability.baseMultiplier));
        }

        if (spellName == "OverallHealth")
        {
            charHealthScript.UpdateOverallHealth(ability.currentStatAmount);
        }
    }

    //updates available level up text
    public void UpdateAvailableLevelUps()
    {
        if (availableLevelUpTexts != null && availableLevelUpTexts.Length > 0)
        {
            foreach (var text in availableLevelUpTexts)
            {
                if (text != null)
                {
                    text.text = "Level Ups Available: " + levelUpsPurchasablePerLevel;
                }
            }
        }
    }
}