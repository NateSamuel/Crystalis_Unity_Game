using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;


public class CharacterHealth : MonoBehaviour
{
    public int characterHealthTotal = 100;
    public int characterHealthCurrent;
    private TextMeshProUGUI healthText;
    public GameObject spellEffectPrefab;
    public Transform castPoint;
    public Slider healthSlider;
    public GameObject PlayerDeathPanel;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterHealthCurrent = characterHealthTotal;
        UpdateHealthUI();
        GameObject healthTextObject = GameObject.Find("PlayerHealthText");
        healthText = healthTextObject.GetComponent<TextMeshProUGUI>();
        if (healthText != null)
        {
            healthText.text = " " + characterHealthTotal;
        }
        if (healthSlider != null)
        {
            healthSlider.maxValue = characterHealthTotal;
            healthSlider.value = characterHealthCurrent;
        }
    }

    public void CharacterDamageTaken(int damageAmount)
    {
        animator.SetTrigger("playerHit");
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
        if( !animator.GetCurrentAnimatorStateInfo(0).IsName("Standing React Death Backward"))
        {
            animator.SetTrigger("playerDeath");
            StartCoroutine(WaitForDeathAnimationThenContinue());
            GetComponent<DirectedAgent>().canMove = false;
        }

    }

    IEnumerator WaitForDeathAnimationThenContinue()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Standing React Death Backward"))
        {
            yield return null;
        }
        
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        StartCoroutine(ShowDeathMessageAndPause());
    }

    IEnumerator ShowDeathMessageAndPause()
    {
        if (PlayerDeathPanel != null)
        {
            PlayerDeathPanel.SetActive(true);
        }

        yield return null;

        Time.timeScale = 0f; 
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = " " + characterHealthCurrent;
        }

        if (healthSlider != null)
        {
            healthSlider.value = characterHealthCurrent;
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
