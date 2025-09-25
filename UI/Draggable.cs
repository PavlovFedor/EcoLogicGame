using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [HideInInspector]
    public Transform parentToReturnTo; // ������-�������� ��� ����������� �����
    [HideInInspector]
    public Transform placeHolderParent; // ������-��������, � ������� ��������� �����

    private GameObject placeHolder; // ����� �����������
    private GameObject clone; // ������������� ����

    [HideInInspector]
    public bool flagForDragging;
    public void OnBeginDrag(PointerEventData eventData)
    {
        // ������� ���������� ����� ������ ����������� �����

        // ��������� �� ��������� ������������ ������� � SBCFieldOne
        // ����� ���������� ������ �� SBCFieldOne, ��� ������ ����������, ������ �� ������� ���� ������ ������������ ��� �����������
        if (transform.parent.parent.parent == GameObject.Find("SBCFieldOne").GetComponent<Transform>())
        {
            flagForDragging = true;
            clone = Instantiate(gameObject, transform.parent); // �������� ������������ ����
        }
        else
        {
            flagForDragging = false;
            clone = gameObject;
        }

        placeHolder = new GameObject(); // ��������� �����
        placeHolder.transform.SetParent(clone.transform.parent);

        // ����������� ������ ������� �� �����
        LayoutElement le = placeHolder.AddComponent<LayoutElement>();
        le.preferredWidth = clone.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = clone.GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        placeHolder.transform.SetSiblingIndex(clone.transform.GetSiblingIndex());
        
        parentToReturnTo = clone.transform.parent; // ����������� ������-�������� ��� ����������� �����
        placeHolderParent = parentToReturnTo;
        clone.transform.SetParent(clone.transform.root); // �������� ���������� ���� � �������� ��������
        clone.transform.SetAsLastSibling(); // �������� ���������� � ��������� ������, ����� �� ������ ��� Background
        clone.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ������� ���������� �� ����� ����������� �����
        clone.transform.position = eventData.position; // ������������� �������������� ������������� ������� � ��������
        // ������������� ������������� ������� �� ��������
        if (placeHolder.transform.parent != placeHolderParent)
        {
            placeHolder.transform.SetParent(placeHolderParent);
        }

        // ��� ���� ��������� ������ ����� �����������
        if (!flagForDragging)
        {
            placeHolder.SetActive(true);
            int newSiblingIndex = placeHolderParent.childCount;

            // ������ �� ������ �����
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
            placeHolder.transform.SetSiblingIndex(newSiblingIndex); // ������������� ������������� ������ ������ �������-�������� ������
            AutoScrolling();
        }
        else
        {
            placeHolder.SetActive(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ������� ���������� ����� ��������� ����������� �����
        // ���� ������������� ��������� ��� ���� ��������� �����
        if (!flagForDragging)
        {
            clone.transform.SetParent(parentToReturnTo);
            clone.transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
            clone.GetComponent<CanvasGroup>().blocksRaycasts = true;
            Destroy(placeHolder); // ����� ����������� ����� ������� �����
        }
        else
        {
            Destroy(clone);
            Destroy(placeHolder);
        }
    }

    private void AutoScrolling()
    {
        // ������� ���������� ��� ����������� ����� � �������� ��� ������� ���� ����

        ScrollRect scrollRect = GameObject.Find("SBCFieldTwo").GetComponent<ScrollRect>();
        RectTransform scrollContent = scrollRect.content;

        // �������� ������� �������� ����
        Vector2 viewportSize = scrollRect.viewport.rect.size;
        Vector2 contentSize = scrollContent.rect.size;

        // ������� ����� ������������ ������� ���������� �����
        Vector2 localPosition = scrollRect.viewport.InverseTransformPoint(clone.transform.position);
        // �������� ����� ����
        if (localPosition.y > -50)
        {
            scrollRect.verticalNormalizedPosition += 0.05f; // ������������ �����
        }
        else if (localPosition.y < -viewportSize.y + 50)
        {
            scrollRect.verticalNormalizedPosition -= 0.05f; // ������������ ����
        }
    }
}
