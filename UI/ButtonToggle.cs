using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    [SerializeField] private Sprite[] buttonSprites;
    [SerializeField] private Image targetButton;
    [SerializeField] protected bool startOn = false;

    protected virtual void Start()
    {
        //targetButton.sprite = startOn ? buttonSprites[1] : buttonSprites[0];
        ApplyInitialState();
    }

    public void ChangeSprite()
    {
        bool isFirstSprite = targetButton.sprite == buttonSprites[0];
        targetButton.sprite = isFirstSprite ? buttonSprites[1] : buttonSprites[0];

        // Вызываем виртуальные методы
        if (isFirstSprite)
        {
            ToggleOn();
        }
        else
        {
            ToggleOff();
        }
    }

    public virtual void ToggleOff()
    {

    }

    public virtual void ToggleOn()
    {

    }
    
    public virtual void ApplyInitialState()
    {
        if (startOn) ToggleOn();
        else ToggleOff();
    }
}
