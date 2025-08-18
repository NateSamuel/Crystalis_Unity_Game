using UnityEngine;
using UnityEngine.UI;

public class UpdatePurchasedAbilitiesButtons : MonoBehaviour
{
    public string spellName;
    public Sprite lockedSprite;
    public Sprite purchasedSprite;
    public CharacterPurchases purchases;
    private Button button;
    private Image buttonImage;
    private CharacterAttack charAttackScript;
    private CharacterTreasure charTreasureScript;
    private Transform playerTransform;
    public int spellCost = 2;
    public float spellDamage = 30f;

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

    public void OnButtonClicked()
    {
        if (spellName == "RangedBlast" && charTreasureScript.crystals >= spellCost)
        {
            charTreasureScript?.RemoveTreasure(spellCost);
            charAttackScript?.RangedBlastAttack(spellDamage);
        }
        else if (spellName == "PowerUp" && charTreasureScript.crystals >= spellCost)
        {
            charTreasureScript?.RemoveTreasure(spellCost);
            charAttackScript?.PowerUpAbility();
        }
        else if (spellName == "Freeze" && charTreasureScript.crystals >= spellCost)
        {
            charTreasureScript?.RemoveTreasure(spellCost);
            charAttackScript?.FreezeAbility();
        }
        else if (spellName == "ForceField" && charTreasureScript.crystals >= spellCost)
        {
            charTreasureScript?.RemoveTreasure(spellCost);
            charAttackScript?.ForceFieldAbility();
        }
        else if (spellName == "CrystalStab" && charTreasureScript.crystals >= spellCost)
        {
            charTreasureScript?.RemoveTreasure(spellCost);
            charAttackScript?.CrystalStabAttack();
        }
    }
}