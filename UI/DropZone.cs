using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Ôóíêöèÿ âûçûâàåòñÿ êîãäà êóðñîð ñ áëîêîì âõîäèò â çîíó âûïàäåíèÿ áëîêà

        if (eventData.pointerDrag == null)
        {
            return;
        }
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null) // Ïðåäóñìàòðèâàåì îò NullException
        {
            d.placeHolderParent = transform.GetChild(0);
            d.flagForDragging = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Ôóíêöèÿ âûçûâàåòñÿ êîãäà êóðñîð ñ áëîêîì âûõîäèò çà ïðåäåëû çîíû âûïàäåíèÿ áëîêà

        if (eventData.pointerDrag == null)
        {
            return;
        }
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null && d.placeHolderParent == transform.GetChild(0))
        {
            d.placeHolderParent = d.parentToReturnTo;
            d.flagForDragging = true;
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        // Ôóíêöèÿ âûçûâàåòñÿ ïðè âûñàäêå áëîêà â çîíó âûïàäåíèÿ

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null) // Ïðåäóñìàòðèâàåì îò NullException
        {
            d.parentToReturnTo = transform.GetChild(0);
        }
    }
}