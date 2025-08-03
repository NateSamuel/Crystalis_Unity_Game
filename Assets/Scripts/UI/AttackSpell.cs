using UnityEngine;

public class AttackSpell : MonoBehaviour
{
    private CharacterAttack charAttackScript;
    private CharacterTreasure charTreasureScript;
    private Transform playerTransform;
    public int spellCost = 2;
    public int spellDamage = 30;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
            charAttackScript = playerTransform.GetComponent<CharacterAttack>();
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure Player GameObject is tagged 'Player'.");
        }

    }
    public void OnButtonClick()
    {
        Debug.Log("Button was clicked!");
        if (charTreasureScript.crystals >= spellCost)
        {
            charTreasureScript?.RemoveTreasure(spellCost);
            charAttackScript?.AOEAttack(spellDamage);
        }
    }
}
