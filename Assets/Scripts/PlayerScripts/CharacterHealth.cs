//Full class is student creation
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

//Deals with health, damage and death of player
public class CharacterHealth : MonoBehaviour
{
    public int characterHealthTotal = 100;
    public int characterHealthCurrent;
    [SerializeField] private TextMeshProUGUI healthText;

    public GameObject spellEffectPrefab;
    public Transform castPoint;
    public Slider healthSlider;
    public GameObject PlayerDeathPanel;
    private Animator animator;
    private bool hitAnimationAvailable = true;
    public bool characterhasDied = false;

    [SerializeField] public CharacterLevelUps levelUp;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterHealthCurrent = characterHealthTotal;
        GameObject healthTextObject = GameObject.Find("PlayerHealthText");
        UpdateHealthUI();
    }
    void Update()
    {
        isHitAnimationAvailable();
    }

    //Updates health based on level up of health
    public void UpdateOverallHealth(float currentModifiedHealth)
    {
        int roundedcurrentModifiedHealth = Mathf.RoundToInt(currentModifiedHealth);
        characterHealthCurrent += roundedcurrentModifiedHealth - characterHealthTotal;
        characterHealthTotal = roundedcurrentModifiedHealth;
        UpdateHealthUI();
    }

    //Checks if is hit animation is available or if other animations are currently in use
    public void isHitAnimationAvailable()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Standing React Death Backward") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Standing 2H Magic Area Attack 01") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Standing 2H Magic Area Attack 02") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Standing 2H Magic Attack 04"))
        {
            hitAnimationAvailable = false;
        }
        else
        {
            hitAnimationAvailable = true;
        }
    }
    //Players damage is taken and if health is below 1, the character dies is called
    public void CharacterDamageTaken(int damageAmount)
    {
        if (hitAnimationAvailable)
        {
            animator.SetTrigger("playerHit");
        }
        characterHealthCurrent -= damageAmount;
        Debug.Log(characterHealthCurrent);
        UpdateHealthUI();
        if (characterHealthCurrent < 1)
        {
            CharacterDie();
        }
    }
    //Cancels ongoing spells and animations, starts death coroutine
    void CharacterDie()
    {
        characterhasDied = true;
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Standing React Death Backward"))
        {

            var attack = GetComponent<CharacterAttack>();
            if (attack != null)
            {
                attack.CancelAllAttacks();
            }

            animator.ResetTrigger("punch");
            animator.ResetTrigger("rangedBlastTrigger");
            animator.ResetTrigger("aoeAttackTrigger");
            animator.ResetTrigger("freezeTrigger");

            animator.SetTrigger("playerDeath");
            animator.CrossFade("Standing React Death Backward", 0.05f);

            GetComponent<DirectedAgent>().canMove = false;
            StartCoroutine(WaitForDeathAnimationThenContinue());
        }
    }

    //Once died, character gets reset for new level
    public void CharacterComesAliveAgain()
    {
        characterHealthCurrent = characterHealthTotal;
        UpdateHealthUI();
        GetComponent<DirectedAgent>().canMove = true;
        animator.Rebind();
        animator.Play("Locomotion", 0, 0f);
        characterhasDied = false;
    }

    //Death animation is evoked
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

    //Death panel is activated
    IEnumerator ShowDeathMessageAndPause()
    {
        if (PlayerDeathPanel != null)
        {
            PlayerDeathPanel.SetActive(true);
        }

        yield return null;

        Time.timeScale = 0f; 
    }

    //updates text and slider that shows health
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

    //Add health is enacted when health boost is clicked
    public void AddHealth(float healthBoostAmount)
    {
        if (characterhasDied == false)
        {
            if (spellEffectPrefab != null && castPoint != null)
            {
                GameObject effect = Instantiate(spellEffectPrefab, castPoint.position, castPoint.rotation);
                Destroy(effect, 1f);
            }
            int addedHealth = Mathf.RoundToInt(healthBoostAmount) + characterHealthCurrent;
            if (addedHealth < characterHealthTotal)
            {
                characterHealthCurrent = addedHealth;
            }
            else
            {
                characterHealthCurrent = characterHealthTotal;
            }
            UpdateHealthUI();
        }
    }
}
