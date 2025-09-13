using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterLevelUps : MonoBehaviour
{
    public int levelUpCost = 5;
    public List<LevelUpAbilities> abilities = new List<LevelUpAbilities>();
    private Transform playerTransform;
    private CharacterTreasure charTreasureScript;
    private CharacterHealth charHealthScript;
    private CharacterAttack charAttackScript;


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

    public void LevelUpSpellByName(string spellName)
    {

        if (charTreasureScript != null && charTreasureScript.crystals >= levelUpCost)
        {
            charTreasureScript.RemoveTreasure(levelUpCost);
            ApplyMultiplier(spellName);
        }

    }

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
}