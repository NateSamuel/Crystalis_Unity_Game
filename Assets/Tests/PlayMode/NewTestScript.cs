using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    [Test]
    public void LayoutGenTestsSimplePasses()
    {
        int result = 2 + 2;
        Assert.AreEqual(4, result); // this will pass
    }

    [UnityTest]
    public IEnumerator LayoutGenTestsWithEnumeratorPasses()
    {
        var go = new GameObject();
        go.transform.position = Vector3.zero;

        // simulate moving right
        go.transform.position += Vector3.right;

        yield return null; // wait a frame

        Assert.AreEqual(Vector3.right, go.transform.position);
    }
}