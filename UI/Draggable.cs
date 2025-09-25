using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [HideInInspector]
    public Transform parentToReturnTo; // Объект-родитель для возвращения блока
    [HideInInspector]
    public Transform placeHolderParent; // Объект-родитель, в котором находится буфер

    private GameObject placeHolder; // Буфер перемещения
    private GameObject clone; // Скопированный блок

    [HideInInspector]
    public bool flagForDragging;
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Функция вызывается после начала перемещения блока

        // Проверяем на отношение пермещаемого объекта к SBCFieldOne
        // Когда перемещаем объект из SBCFieldOne, сам объект копируется, объект из другого поля просто перемещается без копирования
        if (transform.parent.parent.parent == GameObject.Find("SBCFieldOne").GetComponent<Transform>())
        {
            flagForDragging = true;
            clone = Instantiate(gameObject, transform.parent); // Копируем перемещаемый блок
        }
        else
        {
            flagForDragging = false;
            clone = gameObject;
        }

        placeHolder = new GameObject(); // Добавляем буфер
        placeHolder.transform.SetParent(clone.transform.parent);

        // Присваеваем буферу размеры от блока
        LayoutElement le = placeHolder.AddComponent<LayoutElement>();
        le.preferredWidth = clone.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = clone.GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        placeHolder.transform.SetSiblingIndex(clone.transform.GetSiblingIndex());
        
        parentToReturnTo = clone.transform.parent; // Присваиваем объект-родителя для возвращения блока
        placeHolderParent = parentToReturnTo;
        clone.transform.SetParent(clone.transform.root); // Временно перемещаем блок в вверхнюю иерархию
        clone.transform.SetAsLastSibling(); // Временно перемещаем в последний индекс, чтобы не уходил под Background
        clone.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Функция вызывается во время перемещения блока
        clone.transform.position = eventData.position; // Синхронизация местоположения перемещаемого объекта с курсором
        // Синхронизация перемещаемого объекта по иерархии
        if (placeHolder.transform.parent != placeHolderParent)
        {
            placeHolder.transform.SetParent(placeHolderParent);
        }

        // Вне зоны выпадения блоков буфер выключается
        if (!flagForDragging)
        {
            placeHolder.SetActive(true);
            int newSiblingIndex = placeHolderParent.childCount;

            // Пробег по уровню ветки
            for (int i = 0; i < placeHolderParent.childCount; i++)
            {
                if (clone.transform.position.y > placeHolderParent.GetChild(i).position.y)
                {
                    newSiblingIndex = i;

                    if (placeHolder.transform.GetSiblingIndex() < newSiblingIndex)
                        newSiblingIndex--;

                    break;
                }
            }
            placeHolder.transform.SetSiblingIndex(newSiblingIndex); // Устанавливаем одноуровневый индекс внутри объекта-родителя буферу
            AutoScrolling();
        }
        else
        {
            placeHolder.SetActive(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Функция вызывается после окончания перемещения блока
        // Блок автоматически удаляется вне зоны выпадения блока
        if (!flagForDragging)
        {
            clone.transform.SetParent(parentToReturnTo);
            clone.transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
            clone.GetComponent<CanvasGroup>().blocksRaycasts = true;
            Destroy(placeHolder); // После перемещения блока удаляем буфер
        }
        else
        {
            Destroy(clone);
            Destroy(placeHolder);
        }
    }

    private void AutoScrolling()
    {
        // Функция вызывается при перемещении блока к верхнему или нижнему края поля

        ScrollRect scrollRect = GameObject.Find("SBCFieldTwo").GetComponent<ScrollRect>();
        RectTransform scrollContent = scrollRect.content;

        // Получаем размеры областей поля
        Vector2 viewportSize = scrollRect.viewport.rect.size;
        Vector2 contentSize = scrollContent.rect.size;

        // Позиция блока относительно области контейнера поляы
        Vector2 localPosition = scrollRect.viewport.InverseTransformPoint(clone.transform.position);
        // Проверка краев поля
        if (localPosition.y > -50)
        {
            scrollRect.verticalNormalizedPosition += 0.05f; // Пролистываем вверх
        }
        else if (localPosition.y < -viewportSize.y + 50)
        {
            scrollRect.verticalNormalizedPosition -= 0.05f; // Пролистываем вниз
        }
    }
}
