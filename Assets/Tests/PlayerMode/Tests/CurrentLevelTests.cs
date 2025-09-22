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

    // Checks that IncrementLevel increases the currentLevelNumber by 1
    [Test]
    public void IncrementLevelIncreasesNumber()
    {
        int initial = levelScript.currentLevelNumber;
        levelScript.IncrementLevel();
        Assert.AreEqual(initial + 1, levelScript.currentLevelNumber);
    }

    // Checks that it goes backward 1 level
    [Test]
    public void RevertToPreviousLevelCheck()
    {
        levelScript.currentLevelNumber = 5;
        levelScript.RevertToPreviousLevel();
        Assert.AreEqual(5, levelScript.currentLevelNumber);
    }

    // Checks that UpdateLevelText updates all linked TextMeshProUGUI fields correctly
    [Test]
    public void UpdateLevelTextUpdatesAllTextFields()
    {
        levelScript.currentLevelNumber = 3;
        levelScript.UpdateLevelText();

        Assert.AreEqual("Level 3", text1.text);
        Assert.AreEqual("Level 3", text2.text);
    }

    // Checks that IncrementLevel also refreshes the text UI
    [Test]
    public void IncrLevelAlsoUpdatesText()
    {
        levelScript.currentLevelNumber = 2;
        levelScript.IncrementLevel();

        Assert.AreEqual("Level 3", text1.text);
        Assert.AreEqual("Level 3", text2.text);
    }

    // Checks that RevertToPreviousLevel updates all text UI fields
    [Test]
    public void RevertToPreviousLevelAlsoUpdatesText()
    {
        levelScript.currentLevelNumber = 5;
        levelScript.RevertToPreviousLevel();

        Assert.AreEqual("Level 5", text1.text);
        Assert.AreEqual("Level 5", text2.text);
    }
}