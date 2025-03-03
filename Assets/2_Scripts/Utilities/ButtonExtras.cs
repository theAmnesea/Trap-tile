using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonExtras : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private TMP_Text text;

    private Color selectedColor;

    private void Awake()
    {
        text = GetComponentInChildren<TMP_Text>(); // Assuming the TextMeshPro text is a child of the button

        if (ColorUtility.TryParseHtmlString("#dcdcdc", out selectedColor))
        {
            // Successfully converted hex to Color
            Debug.Log("Color Converted Successfully");
        }
        else
        {
            Debug.LogError("Invalid hex string");
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (text != null)
        {
            // Make text underline when selected
            text.fontStyle = FontStyles.Underline;
            text.color = Color.white;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (text != null)
        {
            // Remove underline when deselected
            text.fontStyle = FontStyles.Normal;
            text.color = selectedColor;
        }
    }
}
