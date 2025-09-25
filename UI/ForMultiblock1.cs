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
        // ������� ���������� ����� ������ � ������ ������ � ���� ��������� �����
        if (eventData.pointerDrag == null)
        {
            return;
        }
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (transform.parent.parent.parent == GameObject.Find("SBCFieldOne")) // ��������������� �� �������������� ����������� � ��������� ������
        {
            return;
        }
        else if (d != null) // ��������������� �� NullException
        {
            d.placeHolderParent = transform.GetChild(0);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // ������� ���������� ����� ������ � ������ ������� �� ������� ���� ��������� �����

        if (eventData.pointerDrag == null)
        {
            return;
        }
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d.placeHolderParent == GameObject.Find("SBCFieldOne")) // ��������������� �� �������������� ����������� � ��������� ������
        {
            return;
        }
        else if (d != null && d.placeHolderParent == transform.GetChild(0))
        {
            d.placeHolderParent = transform.parent; // ���������� ������ �� ������� ���� �� ��������
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        // ������� ���������� ��� ������� ����� � ���� ���������
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (transform.parent.parent.parent == GameObject.Find("SBCFieldOne")) // ��������������� �� �������������� ����������� � ��������� ������
        {
            return;
        }
        else if (d != null) // ��������������� �� NullException
        {
            d.parentToReturnTo = transform.GetChild(0);
        }
    }
    public void UpdateBlockSize()
    {
        // ������� ��������� ������� �����

        foreach (RectTransform container in containers)
        {
            float contentHeight = 0;
            // ������ �� ������� � ��������
            foreach (Transform child in container)
            {
                contentHeight += child.GetComponent<RectTransform>().rect.height + containerMargin;
            }
            if (container.childCount == 0) // ���� ���� ������, ��������������� �� ���������� ������
            {
                container.sizeDelta = new Vector2(container.sizeDelta.x, contentHeight + containerHeightOffset);
            }
            else // � ����������� �� ���������� ������ ������ ����������� �����������
            {
                container.sizeDelta = new Vector2(container.sizeDelta.x, contentHeight + containerMargin);
                multiBlock.sizeDelta = new Vector2(multiBlock.sizeDelta.x, contentHeight + multiblockHeightOffset);
            }
        }
    }
}
