using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class AbilityButtonToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipObject;
    public string tooltipMessage = "This is a tooltip.";

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipObject.SetActive(true);
        tooltipObject.GetComponentInChildren<TMP_Text>().text = tooltipMessage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipObject.SetActive(false);
    }
}