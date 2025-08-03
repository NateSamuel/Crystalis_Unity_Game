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
        else
        {
            Debug.LogWarning("Player not found! Make sure Player GameObject is tagged 'Player'.");
        }

    }
    public void OnButtonClick()
    {

        Debug.Log("Button was clicked!");
        if (charTreasureScript.crystals >= boostCost)
        {
            charTreasureScript?.RemoveTreasure(boostCost);

            charHealthScript?.AddHealth(healthBoostAmount);
        }
    }
}
