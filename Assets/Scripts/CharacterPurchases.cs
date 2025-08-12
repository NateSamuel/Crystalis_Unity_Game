using UnityEngine;
using System.Collections.Generic;

public class CharacterPurchases : MonoBehaviour
{
    public int attunementCost = 10;
    public List<PurchasableAbilities> abilities = new List<PurchasableAbilities>();

    private Transform playerTransform;
    private CharacterTreasure charTreasureScript;
    public UpdatePurchasedAbilitiesButtons updateAbilitiesButtons;
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

            if (charTreasureScript != null && charTreasureScript.crystals >= attunementCost)
            {
                ability.hasBeenPurchased = true;
                charTreasureScript.RemoveTreasure(attunementCost);
                if (updateAbilitiesButtons != null)
                {
                    updateAbilitiesButtons.UpdateButtonVisual();
                }
            }

        }
    }
}