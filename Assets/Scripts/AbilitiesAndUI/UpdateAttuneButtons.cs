//Full class is student creation
using UnityEngine;
using UnityEngine.UI;
//this updates the buttons in the purchase abilities interface from being interactable to not interactable
public class UpdateAttuneButtons : MonoBehaviour
{
    public string spellName;
    private Button button;
    public CharacterPurchases purchases;
    void Start()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        var ability = purchases.abilities.Find(a => a.name == spellName);
        if (ability == null) return;

        if (ability.hasBeenPurchased)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

}