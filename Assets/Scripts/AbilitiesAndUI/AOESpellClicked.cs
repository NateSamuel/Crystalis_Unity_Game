//Full class is student creation
using UnityEngine;
using UnityEngine.UI;
//Button creation and info for Crystal Spiral AOE spell
public class AOESpellClicked : MonoBehaviour
{
    private CharacterAttack charAttackScript;
    private CharacterTreasure charTreasureScript;
    public CharacterLevelUps levelUps;
    private Transform playerTransform;
    public int spellCost = 2;
    public float spellDamage = 30f;
    private Button button;
    public float globalAbilityCooldown = 1f;
    private float lastAbilityTime = -Mathf.Infinity;
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
            charAttackScript = playerTransform.GetComponent<CharacterAttack>();
        }

    }
    //cooldowns enacted for the buttons so it gives the player animation time to play etc.
    void Update()
    {
        float cooldownRemaining = globalAbilityCooldown - (Time.time - lastAbilityTime);

        if (cooldownRemaining <= 0f && charTreasureScript.crystals >= spellCost)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
    //When button is clicked the spell is called from the character attack and crystals are removed from character treasure
    public void OnButtonClick()
    {
        LevelUpAbilities upgradedAbility = levelUps.abilities.Find(a => a.name == "CrystalSpiral");

        if (Time.time - lastAbilityTime >= globalAbilityCooldown && charTreasureScript.crystals >= spellCost)
        {
            charTreasureScript?.RemoveTreasure(spellCost);
            charAttackScript?.AOEAttack(upgradedAbility.currentStatAmount);
            lastAbilityTime = Time.time;

            button.interactable = false;
        }
    }
}
