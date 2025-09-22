using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using TMPro;

public class AbilityButtonToolTipTests
{
    GameObject buttonObj;
    AbilityButtonToolTip tooltipScript;
    GameObject tooltipObj;
    TMP_Text tooltipText;

    // Creates button and tooltip objects and then adding the AbilityButtonToolTip script.
    [SetUp]
    public void Setup()
    {
        buttonObj = new GameObject("Button");
        tooltipScript = buttonObj.AddComponent<AbilityButtonToolTip>();

        tooltipObj = new GameObject("Tooltip");
        tooltipObj.SetActive(false);
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(tooltipObj.transform);
        tooltipText = textObj.AddComponent<TMP_Text>();
        tooltipScript.tooltipObject = tooltipObj;
        tooltipScript.tooltipMessage = "Test Tooltip";
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(buttonObj);
        Object.DestroyImmediate(tooltipObj);
    }

    // Checks the tooltip is hidden when the pointer exits the button.
    [UnityTest]
    public IEnumerator OnPointerExitTooltipHidden()
    {
        tooltipObj.SetActive(true);
        tooltipScript.OnPointerExit(null);
        yield return null;
        Assert.IsFalse(tooltipObj.activeSelf, "Tooltip is hidden on pointer exit");
    }
}