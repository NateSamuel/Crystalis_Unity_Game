using UnityEngine;
using UnityEngine.UI;

public class HealthBoost : MonoBehaviour
{
    public int healthBoostAmount = 20;
    public int boostCost = 2;
    private CharacterTreasure charTreasureScript;
    private CharacterHealth charHealthScript;
    private Transform playerTransform;
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
            charHealthScript = playerTransform.GetComponent<CharacterHealth>();
        }

    }
    void Update()
    {
        if (charTreasureScript.crystals >= boostCost)
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

        if (charTreasureScript.crystals >= boostCost)
        {
            charTreasureScript?.RemoveTreasure(boostCost);

            charHealthScript?.AddHealth(healthBoostAmount);
        }
    }
}
