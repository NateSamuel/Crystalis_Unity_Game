//Full class is student creation
using UnityEngine;
using UnityEngine.UI;
//deals with the buttons for all abilities that are purchasable
//i.e. if they are interactable, the cooldowns for the abilities etc.
public class UpdatePurchasedAbilitiesButtons : MonoBehaviour
{
    public string spellName;
    public Sprite lockedSprite;
    public Sprite purchasedSprite;
    public CharacterPurchases purchases;
    public CharacterLevelUps levelUps;
    private Button button;
    private Image buttonImage;
    private CharacterAttack charAttackScript;
    private CharacterTreasure charTreasureScript;
    private Transform playerTransform;
    public int spellCost = 2;
    public float spellDamage = 30f;

    public float abilityCooldown = 2f;
    private float lastAbilityTime = -Mathf.Infinity;


    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        if (purchases == null)
        {
            return;
        }

        UpdateButtonVisual();

        button.onClick.AddListener(OnButtonClicked);
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
            charAttackScript = playerTransform.GetComponent<CharacterAttack>();
        }
    }
    //adds in cooldown for abilities
    void Update()
    {
        var ability = purchases.abilities.Find(a => a.name == spellName);
        if (ability == null) return;

        float cooldownRemaining = abilityCooldown - (Time.time - lastAbilityTime);

        if (ability.hasBeenPurchased && charTreasureScript.crystals >= spellCost && cooldownRemaining <= 0f)
        {
            button.interactable = true;
        }
        else if (ability.hasBeenPurchased)
        {
            button.interactable = false;
        }
    }
    //if button has been purchased it is now iteractable and sprite changes
    public void UpdateButtonVisual()
    {
        var ability = purchases.abilities.Find(a => a.name == spellName);
        if (ability != null)
        {
            if (ability.hasBeenPurchased)
            {
                buttonImage.sprite = purchasedSprite;
                button.interactable = true;
            }
            else
            {
                buttonImage.sprite = lockedSprite;
                button.interactable = false;
            }
        }
    }
    //if specific button is clicked then it is called in character attack and crystals are removed from character treasure
    public void OnButtonClicked()
    {
        if (Time.time - lastAbilityTime < abilityCooldown) return;

        LevelUpAbilities upgradedAbility = levelUps.abilities.Find(a => a.name == spellName);
        if (upgradedAbility == null) return;

        if (charTreasureScript.crystals >= spellCost)
        {
            charTreasureScript.RemoveTreasure(spellCost);

            switch (spellName)
            {
                case "RangedBlast":
                    charAttackScript?.RangedBlastAttack(upgradedAbility.currentStatAmount);
                    break;
                case "PowerUp":
                    charAttackScript?.PowerUpAbility(upgradedAbility.currentStatAmount);
                    break;
                case "Freeze":
                    charAttackScript?.FreezeAbility(upgradedAbility.currentStatAmount);
                    break;
                case "ForceField":
                    charAttackScript?.ForceFieldAbility(upgradedAbility.currentStatAmount);
                    break;
                case "CrystalStab":
                    charAttackScript?.CrystalStabAttack(upgradedAbility.currentStatAmount);
                    break;
            }

            lastAbilityTime = Time.time;
            button.interactable = false;
        }
    }
}