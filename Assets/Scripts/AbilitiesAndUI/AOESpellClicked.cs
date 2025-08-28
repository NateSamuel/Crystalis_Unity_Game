using UnityEngine;
using UnityEngine.UI;

public class AOESpellClicked : MonoBehaviour
{
    private CharacterAttack charAttackScript;
    private CharacterTreasure charTreasureScript;
    private Transform playerTransform;
    public int spellCost = 2;
    public float spellDamage = 30f;
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

    }
    void Update()
    {
        if (charTreasureScript.crystals >= spellCost)
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
        if (charTreasureScript.crystals >= spellCost)
        {
            charTreasureScript?.RemoveTreasure(spellCost);
            charAttackScript?.AOEAttack(spellDamage);
        }
    }
}
