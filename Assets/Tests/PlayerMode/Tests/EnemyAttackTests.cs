using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public class EnemyAttackTests
{
    private GameObject enemyGameObj;
    private EnemyAttack attack;

    // Creates a new GameObject with EnemyAttack
    [SetUp]
    public void SetUp()
    {
        enemyGameObj = new GameObject();
        attack = enemyGameObj.AddComponent<EnemyAttack>();
    }

    // Destroys game object
    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(enemyGameObj);
    }

    // Checks that IsAbleToHitPlayer sets 'isAbleToHit' bool to true
    [Test]
    public void IsAbleToHitPlayerSetsBoolTrue()
    {
        attack.IsAbleToHitPlayer();

        bool value = (bool)typeof(EnemyAttack)
            .GetField("isAbleToHit", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(attack);

        Assert.IsTrue(value);
    }

    // Checks that IsNotAbleToDamagePlayer sets 'isAbleToDamage' bool to false.
    [Test]
    public void IsNotAbleToDamagePlayerSetsBoolFalse()
    {
        attack.IsNotAbleToDamagePlayer();

        bool value = (bool)typeof(EnemyAttack)
            .GetField("isAbleToDamage", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(attack);

        Assert.IsFalse(value);
    }

    // Checks that SetDead true sets 'isDead' bool and disables 'isAbleToHit'.
    [Test]
    public void SetDeadSetsIsDeadAndDisablesHit()
    {
        attack.IsAbleToHitPlayer();
        attack.SetDead(true);

        bool isDead = (bool)typeof(EnemyAttack)
            .GetField("isDead", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(attack);

        bool isAbleToHit = (bool)typeof(EnemyAttack)
            .GetField("isAbleToHit", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(attack);

        Assert.IsTrue(isDead);
        Assert.IsFalse(isAbleToHit);
    }

    // Checks that RotateDirection rotates a Vector3 by the specific angle.
    [Test]
    public void RotateDirectionRotatesVectorCorrectly()
    {
        Vector3 original = Vector3.forward;

        var method = typeof(EnemyAttack)
            .GetMethod("RotateDirection", BindingFlags.NonPublic | BindingFlags.Instance);

        Vector3 rotated = (Vector3)method.Invoke(attack, new object[] { original, 90f });

        Assert.AreEqual(Vector3.right.x, Mathf.Round(rotated.x));
        Assert.AreEqual(Vector3.right.z, Mathf.Round(rotated.z));
    }
}