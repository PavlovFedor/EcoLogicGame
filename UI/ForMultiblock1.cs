using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using NUnit.Framework;

public class ForMultiblock1 : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform multiBlock;
    public RectTransform[] containers;
    public float containerMargin;
    public float containerHeightOffset;
    public float multiblockHeightOffset;
    private void FixedUpdate()
    {
        UpdateBlockSize();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Функция вызывается когда курсор с блоком входит в зону выпадения блока
        if (eventData.pointerDrag == null)
        {
            return;
        }
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (transform.parent.parent.parent == GameObject.Find("SBCFieldOne")) // Предусматриваем от нежелательного перемещения в хранилище блоков
        {
            return;
        }
        else if (d != null) // Предусматриваем от NullException
        {
            d.placeHolderParent = transform.GetChild(0);
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
        else if (d != null && d.placeHolderParent == transform.GetChild(0))
        {
            d.placeHolderParent = transform.parent; // Перемещаем объект на уровень выше по иерархии
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        // Функция вызывается при высадке блока в зону выпадения
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (transform.parent.parent.parent == GameObject.Find("SBCFieldOne")) // Предусматриваем от нежелательного перемещения в хранилище блоков
        {
            return;
        }
        else if (d != null) // Предусматриваем от NullException
        {
            d.parentToReturnTo = transform.GetChild(0);
        }
    }
    public void UpdateBlockSize()
    {
        // Функция обновляет размеры блока

        foreach (RectTransform container in containers)
        {
            float contentHeight = 0;
            // Пробег по объекту в иерархии
            foreach (Transform child in container)
            {
                contentHeight += child.GetComponent<RectTransform>().rect.height + containerMargin;
            }
            if (container.childCount == 0) // Если нету блоков, предусматриваем от уменьшения высоты
            {
                container.sizeDelta = new Vector2(container.sizeDelta.x, contentHeight + containerHeightOffset);
            }
            else // В зависимости от количества блоков размер мультиблока расширяется
            {
                container.sizeDelta = new Vector2(container.sizeDelta.x, contentHeight + containerMargin);
                multiBlock.sizeDelta = new Vector2(multiBlock.sizeDelta.x, contentHeight + multiblockHeightOffset);
            }
        }
    }
}
