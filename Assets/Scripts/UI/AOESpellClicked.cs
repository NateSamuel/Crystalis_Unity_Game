using UnityEngine;

public class AOESpellClicked : MonoBehaviour
{
    private CharacterAttack charAttackScript;
    private CharacterTreasure charTreasureScript;
    private Transform playerTransform;
    public int spellCost = 2;
    public float spellDamage = 30f;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
            charAttackScript = playerTransform.GetComponent<CharacterAttack>();
        }

    }
    public void OnButtonClick()
    {
        if (charTreasureScript.crystals >= spellCost)
        {
            charTreasureScript?.RemoveTreasure(spellCost);
            charAttackScript?.AOEAttack(spellDamage);
        }
    }
}
