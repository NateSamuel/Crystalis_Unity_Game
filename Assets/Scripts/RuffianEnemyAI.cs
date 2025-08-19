using UnityEngine;

public class RuffianEnemyAI : BaseEnemyAI
{
    public override void Initialize()
    {
        base.Initialize();
    }
    protected override void UseAbility()
    {
        float roll = Random.Range(0f, 1f);

        if (roll < 0.1f)
        {
            attackScript?.EnemyAOEAttack(10f);
        }
        else
        {
            base.UseAbility();
        }
    }
}