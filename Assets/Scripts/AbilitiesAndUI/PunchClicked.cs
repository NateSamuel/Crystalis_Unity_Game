using UnityEngine;
using UnityEngine.UI;

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