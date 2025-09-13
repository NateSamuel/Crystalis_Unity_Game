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

    void Start()
    {
        button = GetComponent<Button>(); 
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
        }
    }

    void Update()
    {

        var purchasedAbility = purchases.abilities.Find(a => a.name == spellName);
        if (purchasedAbility != null)
        {

            if (purchasedAbility.hasBeenPurchased && charTreasureScript.crystals >= levelUpCost)
            {
                button.interactable = true;
            }
            else{
                button.interactable = false;
            }
        }
    }

}