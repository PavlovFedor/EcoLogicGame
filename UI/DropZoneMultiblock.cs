using UnityEngine;
using UnityEngine.EventSystems;

public class DropZoneMultiblock : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Функция вызывается когда курсор с блоком входит в зону выпадения блока
        if (eventData.pointerDrag == null)
        {
            return;
        }
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (transform.parent.parent.parent.parent == GameObject.Find("SBCFieldOne")) // Предусматриваем от нежелательного перемещения в хранилище блоков
        {
            return;
        }
        else if (d != null) // Предусматриваем от NullException
        {
            d.placeHolderParent = transform;
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
        if (d.placeHolderParent == GameObject.Find("SBCFieldOne")) // Предусматриваем от нежелательного перемещения в хранилище блоков
        {
            return;
        }
        else if (d != null && d.placeHolderParent == transform)
        {
            d.placeHolderParent = transform.parent.parent; // Перемещаем объект на уровень выше по иерархии
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        // Функция вызывается при высадке блока в зону выпадения
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (transform.parent.parent.parent.parent == GameObject.Find("SBCFieldOne")) // Предусматриваем от нежелательного перемещения в хранилище блоков
        {
            return;
        }
        else if (d != null) // Предусматриваем от NullException
        {
            d.parentToReturnTo = transform;
        }
    }
}
