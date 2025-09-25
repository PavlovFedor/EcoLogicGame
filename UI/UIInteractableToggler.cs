using UnityEngine;
using UnityEngine.UI;

public class UIInteractableToggler : ButtonToggle
{
    [SerializeField] private Selectable[] uiElements;
    public override void ToggleOff()
    {
        base.ToggleOff();
        foreach (var element in uiElements)
        {
            element.interactable = false;
        }
    }
    public override void ToggleOn()
    {
        base.ToggleOn();
        foreach (var element in uiElements)
        {
            element.interactable = true;
        }
    }
}
