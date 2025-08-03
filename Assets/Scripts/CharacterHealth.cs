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
    public GameObject spellEffectPrefab;
    public Transform castPoint;

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

    public void AddHealth(int healthBoostAmount)
    {
        if (spellEffectPrefab != null && castPoint != null)
        {
            GameObject effect = Instantiate(spellEffectPrefab, castPoint.position, castPoint.rotation);
            Destroy(effect, 1f);
        }
        int addedHealth = healthBoostAmount + characterHealthCurrent;
        if (addedHealth < 100)
        {
            characterHealthCurrent = addedHealth;
        }
        else{
            characterHealthCurrent = 100;
        }
        UpdateHealthUI();
    }
}
