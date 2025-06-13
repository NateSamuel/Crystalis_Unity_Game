using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int enemyHealthTotal = 100;
    private int enemyHealthCurrent;

    public Slider healthSlider;

    void Start()
    {
        enemyHealthCurrent = enemyHealthTotal;
        if (healthSlider != null)
        {
            healthSlider.maxValue = enemyHealthTotal;
            healthSlider.value = enemyHealthCurrent;
        }
    }

    public void EnemyDamageTaken(int damageAmount)
    {
        enemyHealthCurrent -= damageAmount;
        if (healthSlider != null)
        {
            healthSlider.value = enemyHealthCurrent;
            Debug.Log("Enemy health" + enemyHealthCurrent);
        }

        if (enemyHealthCurrent < 0)
        {
            EnemyDie();
        }
    }
    void EnemyDie()
    {
         Destroy(gameObject);
    }
}
