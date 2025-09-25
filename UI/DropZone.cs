using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ������� ���������� ����� ������ � ������ ������ � ���� ��������� �����

        if (eventData.pointerDrag == null)
        {
            return;
        }
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null) // ��������������� �� NullException
        {
            d.placeHolderParent = transform.GetChild(0);
            d.flagForDragging = false;
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
        if (d != null && d.placeHolderParent == transform.GetChild(0))
        {
            d.placeHolderParent = d.parentToReturnTo;
            d.flagForDragging = true;
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        // ������� ���������� ��� ������� ����� � ���� ���������

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null) // ��������������� �� NullException
        {
            d.parentToReturnTo = transform.GetChild(0);
        }
    }
}