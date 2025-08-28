using UnityEngine;
using UnityEngine.UI;

public class PunchClicked : MonoBehaviour
{
    private CharacterAttack charAttackScript;
    private CharacterTreasure charTreasureScript;
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
        if (Time.time - lastAbilityTime >= globalAbilityCooldown)
        {
            charAttackScript?.Attack();
            lastAbilityTime = Time.time;

            button.interactable = false;

        }
    }
}