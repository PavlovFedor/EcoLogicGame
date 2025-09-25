using UnityEngine;
using UnityEngine.EventSystems;

public class DropZoneMultiblock : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ������� ���������� ����� ������ � ������ ������ � ���� ��������� �����
        if (eventData.pointerDrag == null)
        {
            return;
        }
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (transform.parent.parent.parent.parent == GameObject.Find("SBCFieldOne")) // ��������������� �� �������������� ����������� � ��������� ������
        {
            return;
        }
        else if (d != null) // ��������������� �� NullException
        {
            d.placeHolderParent = transform;
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
        else if (d != null && d.placeHolderParent == transform)
        {
            d.placeHolderParent = transform.parent.parent; // ���������� ������ �� ������� ���� �� ��������
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        // ������� ���������� ��� ������� ����� � ���� ���������
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (transform.parent.parent.parent.parent == GameObject.Find("SBCFieldOne")) // ��������������� �� �������������� ����������� � ��������� ������
        {
            return;
        }
        else if (d != null) // ��������������� �� NullException
        {
            d.parentToReturnTo = transform;
        }
    }
}
