using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Функция вызывается когда курсор с блоком входит в зону выпадения блока

        if (eventData.pointerDrag == null)
        {
            return;
        }
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null) // Предусматриваем от NullException
        {
            d.placeHolderParent = transform.GetChild(0);
            d.flagForDragging = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Функция вызывается когда курсор с блоком выходит за пределы зоны выпадения блока

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
        // Функция вызывается при высадке блока в зону выпадения

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null) // Предусматриваем от NullException
        {
            d.parentToReturnTo = transform.GetChild(0);
        }
    }
}