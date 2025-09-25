using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

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

    // Скрипт менеджера блоков кода
    private CodeBlockManager codeBlockManager;

    void Start(){
        GameObject canvas = GameObject.Find("Canvas");

        // Проверяем, что объект был найден
        if (canvas != null)
        {
            // Получаем компонент CodeBlockManager
            codeBlockManager = canvas.GetComponent<CodeBlockManager>();

            // Проверяем, что компонент был найден
            if (codeBlockManager == null)
            {
                Debug.LogError("Компонент CodeBlockManager не найден на объекте Canvas.");
            }
        }
        else
        {
            Debug.LogError("Объект Canvas не найден в сцене.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Функция вызывается после начала перемещения блока

        // Проверяем на отношение перемещаемого объекта к SBCFieldOne
        // Когда перемещаем объект из SBCFieldOne, сам объект копируется, объект из другого поля просто перемещается без копирования
        if (transform.parent == null || transform.parent.parent == null)
        {
            Debug.LogError("Invalid object hierarchy");
            return;
        }

        if (transform.parent.parent.parent != GameObject.Find("SBCFieldTwo").GetComponent<Transform>())
        {
            flagForDragging = true;
            clone = Instantiate(gameObject, transform.parent); // Копируем перемещаемый блок
        }
        else
        {
            flagForDragging = false;
            clone = gameObject;
            // Удаляем блок из текущей позиции (так как мы его перемещаем) 
            List<CodeBlock> CB = FindScriptListBlockCode(gameObject);  
            codeBlockManager.DeleteCodeBlock(transform.GetSiblingIndex(), CB);
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
        clone.transform.SetParent(clone.transform.root); // Временно перемещаем блок в верхнюю иерархию
        clone.transform.SetAsLastSibling(); // Временно перемещаем в последний индекс, чтобы не уходил под Background
        clone.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Функция вызывается во время перемещения блока
        
        // Проверяем, что clone существует
        if (clone == null)
        {
            Debug.LogError("Clone is null in OnDrag");
            return;
        }

        // Синхронизация местоположения перемещаемого объекта с курсором
        clone.transform.position = eventData.position; 
        // Синхронизация перемещаемого объекта по иерархии
        if (placeHolder.transform.parent != placeHolderParent)
        {
            placeHolder.transform.SetParent(placeHolderParent);
        }

        // Вне зоны выпадения блоков буфер выключается
        if (!flagForDragging)
        {
            // Проверяем активность placeHolder
            if (!placeHolder.activeSelf)
                placeHolder.SetActive(true);
            
            int newSiblingIndex = placeHolderParent.childCount;

            // Пробег по уровню ветки
            for (int i = 0; i < placeHolderParent.childCount; i++)
            {
                Transform child = placeHolderParent.GetChild(i);
                if (child == null) continue;

                if (clone.transform.position.y > child.position.y)
                {
                    newSiblingIndex = i;

                    if (placeHolder.transform.GetSiblingIndex() < newSiblingIndex)
                        newSiblingIndex--;

                    break;
                }
            }
            // Устанавливаем одноуровневый индекс внутри объекта-родителя буферу
            placeHolder.transform.SetSiblingIndex(newSiblingIndex); 
            AutoScrolling();
        }
        else
        {
            if (placeHolder != null && placeHolder.activeSelf)
                placeHolder.SetActive(false);        
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!flagForDragging && parentToReturnTo.parent.parent == GameObject.Find("SBCFieldTwo").GetComponent<Transform>()) // Если перетаскивали блок внутри зоны строителя
        {
            Debug.LogWarning("Блок перетащили внутрь строителя");
            List<CodeBlock> CB = FindScriptListBlockCode(placeHolder);              
 
            clone.transform.SetParent(parentToReturnTo);
            clone.transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
            clone.GetComponent<CanvasGroup>().blocksRaycasts = true;
            Destroy(placeHolder);
            
            codeBlockManager.InsertCodeBlock(clone.transform.GetSiblingIndex(), clone, CB);
        }
        else // Если перетаскивали из зоны выбора
        {
            // Добавляем в список только если успешно поместили в зону строителя
            if (!flagForDragging || placeHolder.transform.parent.parent.CompareTag("Multiblock"))
            {
                Debug.LogWarning("Перетащили в мультиблок");
                List<CodeBlock> CB = FindScriptListBlockCode(placeHolder);  
            
                clone.transform.SetParent(parentToReturnTo);
                clone.transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
                clone.GetComponent<CanvasGroup>().blocksRaycasts = true;     
                Destroy(placeHolder);

                codeBlockManager.InsertCodeBlock(clone.transform.GetSiblingIndex(), clone, CB);
            }
            else
            {
                // Если не попали в зону строителя - уничтожаем клон
                Debug.LogWarning("Вне зон доступа");
                Destroy(clone);
                Destroy(placeHolder);
            }
        }
    }

    private List<CodeBlock> FindScriptListBlockCode(GameObject go)
    {
        //это основа
        if( go.transform.parent.parent.parent == GameObject.Find("SBCFieldTwo").GetComponent<Transform>())
        {
            CodeBlock scriptCB = GetComponent<CodeBlock>();
            return codeBlockManager.codeBlocks;
        }//это блок если
        else if( go.transform.parent.parent.CompareTag("Multiblock"))
        {
            CodeBlock scriptCBmulti = go.transform.parent.parent.GetComponent<CodeBlock>();
            if(go.transform.parent.gameObject.name == "Container1") return scriptCBmulti.codeBlockList1;
            if(go.transform.parent.gameObject.name == "Container2") return scriptCBmulti.codeBlockList2;
        }
        return null;
    }

    private void AutoScrolling()
    {
        // Функция вызывается при перемещении блока к верхнему или нижнему краю поля

        GameObject sbcFieldTwo = GameObject.Find("SBCFieldTwo");
        if (sbcFieldTwo == null)
        {
            Debug.LogError("SBCFieldTwo not found");
            return;
        }

        ScrollRect scrollRect = sbcFieldTwo.GetComponent<ScrollRect>();
        if (scrollRect == null || scrollRect.viewport == null || scrollRect.content == null)
        {
            Debug.LogError("ScrollRect components not properly initialized");
            return;
        }

        RectTransform scrollContent = scrollRect.content;

        // Получаем размеры областей поля
        Vector2 viewportSize = scrollRect.viewport.rect.size;
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