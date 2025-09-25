using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InputFieldToggler : ButtonToggle
{
    [SerializeField] private TMP_InputField tmpInputField;

    public override void ToggleOn()
    {
        base.ToggleOn();
        tmpInputField.interactable = true;
    }

    public override void ToggleOff()
    {
        base.ToggleOff();
        tmpInputField.interactable = false;
        tmpInputField.text = "";
    }
    public override void ApplyInitialState()
    {
        tmpInputField.interactable = startOn;
        base.ApplyInitialState();
    }
}
