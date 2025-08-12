using UnityEngine;

public class HealthBoost : MonoBehaviour
{
    public int healthBoostAmount = 20;
    public int boostCost = 2;
    private CharacterTreasure charTreasureScript;
    private CharacterHealth charHealthScript;
    private Transform playerTransform;


    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
            charHealthScript = playerTransform.GetComponent<CharacterHealth>();
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
