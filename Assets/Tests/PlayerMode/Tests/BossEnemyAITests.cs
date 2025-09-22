using NUnit.Framework;
using UnityEngine;

public class BossEnemyAITests
{
    // Checks that OnTeleportComplete resets 'isTeleporting'
    [Test]
    public void OnTeleportCompleteResetsIsTeleporting()
    {
        var gameObj = new GameObject();
        var boss = gameObj.AddComponent<BossEnemyAI>();

        var field = typeof(BossEnemyAI).GetField("isTeleporting", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(boss, true);

        boss.OnTeleportComplete();

        bool isTeleportingValue = (bool)field.GetValue(boss);
        Assert.IsFalse(isTeleportingValue);
    }
}