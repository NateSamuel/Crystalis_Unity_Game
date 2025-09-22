using NUnit.Framework;
using UnityEngine;
using TMPro;

public class CurrentLevelTests
{
    GameObject gameObj;
    CurrentLevel levelScript;
    TextMeshProUGUI text1;
    TextMeshProUGUI text2;

    // Creates a CurrentLevel and two TMP text fields
    [SetUp]
    public void Setup()
    {
        gameObj = new GameObject();
        levelScript = gameObj.AddComponent<CurrentLevel>();

        // create simple TMP objects
        var textGameObj1 = new GameObject();
        text1 = textGameObj1.AddComponent<TextMeshProUGUI>();

        var textGameObj2 = new GameObject();
        text2 = textGameObj2.AddComponent<TextMeshProUGUI>();

        levelScript.levelTexts = new TextMeshProUGUI[] { text1, text2 };
    }

    // Destroys after the test
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameObj);
        Object.DestroyImmediate(text1.gameObject);
        Object.DestroyImmediate(text2.gameObject);
    }

    // Verifies that IncrementLevel increases the currentLevelNumber by 1.
    [Test]
    public void IncrementLevelIncreasesNumber()
    {
        int initial = levelScript.currentLevelNumber;
        levelScript.IncrementLevel();
        Assert.AreEqual(initial + 1, levelScript.currentLevelNumber);
    }

    // Verifies that RevertToLevelOne sets currentLevelNumber to 1.
    [Test]
    public void RevertToLevelOneSetsNumberToOne()
    {
        levelScript.currentLevelNumber = 5;
        levelScript.RevertToLevelOne();
        Assert.AreEqual(1, levelScript.currentLevelNumber);
    }

    // Checks that UpdateLevelText updates all linked TextMeshProUGUI fields correctly.
    [Test]
    public void UpdateLevelTextUpdatesAllTextFields()
    {
        levelScript.currentLevelNumber = 3;
        levelScript.UpdateLevelText();

        Assert.AreEqual("Level 3", text1.text);
        Assert.AreEqual("Level 3", text2.text);
    }

    // Ensures that IncrementLevel not only updates the number but also refreshes the text UI.
    [Test]
    public void IncrLevelAlsoUpdatesText()
    {
        levelScript.currentLevelNumber = 2;
        levelScript.IncrementLevel();

        Assert.AreEqual("Level 3", text1.text);
        Assert.AreEqual("Level 3", text2.text);
    }

    // Ensures that RevertToLevelOne not only sets the number to 1 but also updates all text UI fields.
    [Test]
    public void RevertToLevelOneAlsoUpdatesText()
    {
        levelScript.currentLevelNumber = 5;
        levelScript.RevertToLevelOne();

        Assert.AreEqual("Level 1", text1.text);
        Assert.AreEqual("Level 1", text2.text);
    }
}