//Full class is student creation
using UnityEngine;
using TMPro;
using System.Collections.Generic;

//adds, removes, and resets keys
public class CharacterKeys : MonoBehaviour
{
    public int keys = 0;

    [SerializeField] private List<TextMeshProUGUI> keyTexts;

    void Start()
    {
        UpdateKeyUI();
    }

    public void ApplyKey(int keyAmount)
    {
        keys += keyAmount;
        UpdateKeyUI();
    }

    public void RemoveKey(int keyAmount)
    {
        keys -= keyAmount;
        UpdateKeyUI();
    }
    public void ResetKey(int keyAmount)
    {
        keys = keyAmount;

        UpdateKeyUI();
    }

    private void UpdateKeyUI()
    {
        foreach (var text in keyTexts)
        {
            if (text != null)
            {
                text.text = " " + keys;
            }
        }
    }
}