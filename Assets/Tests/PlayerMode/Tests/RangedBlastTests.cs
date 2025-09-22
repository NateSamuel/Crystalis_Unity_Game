using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RangedBlastPlayModeTests
{
    GameObject blastGameObj;
    RangedBlast blast;

    // Creates a RangedBlast GameObject
    [SetUp]
    public void Setup()
    {
        blastGameObj = new GameObject("RangedBlast");
        blast = blastGameObj.AddComponent<RangedBlast>();
    }

    // Destroys RangedBlast game object
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(blastGameObj);
    }

    // Checks that Launch sets the fields correctly
    [UnityTest]
    public IEnumerator LaunchSetsFieldsCorrectly()
    {
        Vector3 direction = Vector3.forward;
        blast.Launch(direction, RangedBlast.CasterType.Player, 50f, true);

        var moveDir = GetPrivateField<Vector3>(blast, "moveDirection");
        var ableToHit = GetPrivateField<bool>(blast, "isAbleToHit");

        Assert.AreEqual(direction, moveDir);
        Assert.AreEqual(RangedBlast.CasterType.Player, blast.caster);
        Assert.AreEqual(50f, blast.damage);
        Assert.IsTrue(ableToHit);

        yield return null;
    }

    // Checks that Update moves the projectile forward over time
    [UnityTest]
    public IEnumerator UpdateMovesProjectile()
    {
        Vector3 direction = Vector3.forward;
        blast.speed = 2f;
        blast.Launch(direction, RangedBlast.CasterType.Player, 40f, true);

        Vector3 initialPos = blast.transform.position;

        yield return null;

        Assert.That(blast.transform.position.z, Is.GreaterThan(initialPos.z));

        yield return null;

        Assert.That(blast.transform.position.z, Is.GreaterThan(initialPos.z + 0.001f));
    }

    // Helper method to access private fields
    private T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (T)field.GetValue(obj);
    }
}