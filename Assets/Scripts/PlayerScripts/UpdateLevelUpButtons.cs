//Full class is student creation
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UpdateLevelUpButtons : MonoBehaviour
{
    public int levelUpCost = 5;
    public CharacterPurchases purchases;
    private Transform playerTransform;
    private CharacterTreasure charTreasureScript;
    public string spellName;
    private Button button;
    private CharacterLevelUps charLevelUps;

    void Start()
    {
        button = GetComponent<Button>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
        }
        charLevelUps = FindAnyObjectByType<CharacterLevelUps>();
    }
    //checks if button is interactable, if ability has been purchased, if player has enough crystals, and if level up is available still at this level
    void Update()
    {

        var purchasedAbility = purchases.abilities.Find(a => a.name == spellName);
        if (purchasedAbility != null)
        {

            if (purchasedAbility.hasBeenPurchased && charTreasureScript.crystals >= levelUpCost && charLevelUps.levelUpsPurchasablePerLevel > 0)
            {
                button.interactable = true;
            }
            else{
                button.interactable = false;
            }
        }
    }

}