using UnityEngine;
using UnityEngine.UI;

public class HideNShowField : MonoBehaviour
{
    public GameObject field;
    public Image buttonImage;
    public Sprite hideSprite;
    public Sprite showSprite;
    public void whenButtonClicked()
    {
        if (field.activeInHierarchy)
        {
            field.SetActive(false);
            buttonImage.sprite = showSprite;
        }
        else
        {
            field.SetActive(true);
            buttonImage.sprite = hideSprite;
        }
    }
}
