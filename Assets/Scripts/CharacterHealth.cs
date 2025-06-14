using UnityEngine;
using TMPro;
using System.Collections;

public class CharacterHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int characterHealthTotal = 100;
    private int characterHealthCurrent;
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI deathMessageText;
    

    void Start()
    {
        characterHealthCurrent = characterHealthTotal;
        UpdateHealthUI();
        GameObject healthTextObject = GameObject.Find("PlayerHealthText");
        healthText = healthTextObject.GetComponent<TextMeshProUGUI>();
        GameObject deathTextObject = GameObject.Find("PlayerDeathText");
        deathMessageText = deathTextObject.GetComponent<TextMeshProUGUI>();
        if (healthText != null)
        {
            healthText.text = "Health: " + characterHealthTotal;
        }
        if (deathMessageText != null)
        {
            deathMessageText.text = " ";
        }


    }

    public void CharacterDamageTaken(int damageAmount)
    {
        characterHealthCurrent -= damageAmount;
        Debug.Log(characterHealthCurrent);
        UpdateHealthUI();
        if (characterHealthCurrent < 1)
        {
            CharacterDie();
        }
    }

    void CharacterDie()
    {
        
        StartCoroutine(ShowDeathMessageAndPause());

    }

    IEnumerator ShowDeathMessageAndPause()
    {
        if (deathMessageText != null)
        {
            deathMessageText.gameObject.SetActive(true);
            deathMessageText.text = "PLAYER DIED";
        }

        yield return null;

        Time.timeScale = 0f; 
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + characterHealthCurrent;
        }
    }

}
