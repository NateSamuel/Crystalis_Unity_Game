using UnityEngine;
using TMPro;

public class CharacterHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int characterHealthTotal = 100;
    private int characterHealthCurrent;
    public TextMeshProUGUI healthText;
    

    void Start()
    {
        characterHealthCurrent = characterHealthTotal;
        UpdateHealthUI();
    }

    public void CharacterDamageTaken(int damageAmount)
    {
        characterHealthCurrent -= damageAmount;
        Debug.Log(characterHealthCurrent);
        UpdateHealthUI();
        if (characterHealthCurrent < 0)
        {
            CharacterDie();
        }
    }

    void CharacterDie(){
        Debug.Log("You have died");
    }
    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + characterHealthCurrent;
        }
    }

}
