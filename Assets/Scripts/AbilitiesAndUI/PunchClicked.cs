//Full class is student creation
using UnityEngine;
using UnityEngine.UI;

//Button creation and info for the punch button
public class PunchClicked : MonoBehaviour
{
    private CharacterAttack charAttackScript;
    private CharacterTreasure charTreasureScript;
    public CharacterLevelUps levelUps;
    private Transform playerTransform;

    public float globalAbilityCooldown = 1f;
    private float lastAbilityTime = -Mathf.Infinity;

    private Button button;

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

        button.interactable = true;

    }
    //cooldowns enacted for the buttons so it gives the player animation time to play etc.
    void Update()
    {
        float cooldownRemaining = globalAbilityCooldown - (Time.time - lastAbilityTime);

        if (cooldownRemaining <= 0f)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
    //When button is clicked the punch is called from the character attack
    public void OnButtonClick()
    {
        LevelUpAbilities upgradedAbility = levelUps.abilities.Find(a => a.name == "Punch");

        if (Time.time - lastAbilityTime >= globalAbilityCooldown)
        {
            charAttackScript?.Attack(upgradedAbility.currentStatAmount);
            lastAbilityTime = Time.time;

            button.interactable = false;

        }
    }
}