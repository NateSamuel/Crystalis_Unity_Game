using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LevelBuilderTests
{
    // Creates a GameObject with a LevelBuilder
    GameObject builderObj;
    LevelBuilder builder;

    [SetUp]
    public void Setup()
    {
        builderObj = new GameObject("builder");
        builder = builderObj.AddComponent<LevelBuilder>();
    }
    //Destroys game object
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(builderObj);
    }

    // Checks that two similar colors are approximately equal.
    [Test]
    public void ColorsAlmostEqual()
    {
        var method = builder.GetType().GetMethod("ColorsApproximatelyEqual",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var res = method.Invoke(builder, new object[] {
            Color.red, new Color(1f, 0f, 0.01f), 0.05f
        });

        Assert.IsTrue((bool)res, "close colors should be same enough");
    }
}