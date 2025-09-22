//Full class is student creation
using UnityEngine;
using System.Collections.Generic;
using TMPro;

//creates purchases list, updates the list based on if purchased or not and if available to be purchased per level
public class CharacterPurchases : MonoBehaviour
{
    public int attunementCost = 10;
    public List<PurchasableAbilities> abilities = new List<PurchasableAbilities>();

    private Transform playerTransform;
    private CharacterTreasure charTreasureScript;
    public List<UpdatePurchasedAbilitiesButtons> updateAbilitiesButtonsList;
    public int abilitiesPurchasablePerLevel = 2;
    public TextMeshProUGUI[] availablePurchasesTexts;
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
        }
    }

    public void PurchaseSpellByName(string abilityName)
    {
        PurchasableAbilities ability = abilities.Find(a => a.name == abilityName);

        if (ability != null)
        {
            if (ability.hasBeenPurchased)
            {
                return;
            }

            if (charTreasureScript != null && charTreasureScript.crystals >= attunementCost && abilitiesPurchasablePerLevel > 0)
            {
                ability.hasBeenPurchased = true;
                charTreasureScript.RemoveTreasure(attunementCost);
                abilitiesPurchasablePerLevel -= 1;
                UpdateAvailablePurchasesTexts();
                if (updateAbilitiesButtonsList != null)
                {
                    foreach (var button in updateAbilitiesButtonsList)
                    {
                        if (button != null)
                            button.UpdateButtonVisual();
                    }
                }
            }
        }
    }
    //Updates available purchase texts
    public void UpdateAvailablePurchasesTexts()
    {
        if (availablePurchasesTexts != null && availablePurchasesTexts.Length > 0)
        {
            foreach (var text in availablePurchasesTexts)
            {
                if (text != null)
                {
                    text.text = "Ability Attunements Available: " + abilitiesPurchasablePerLevel;
                }
            }
        }
    }
    public int GetTotalPurchases()
    {
        return abilities.FindAll(a => a.hasBeenPurchased).Count;
    }
}