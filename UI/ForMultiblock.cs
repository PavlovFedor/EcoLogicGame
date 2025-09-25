using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ForMultiblock : MonoBehaviour
{
    public RectTransform multiBlock;
    public List<RectTransform> containers = new List<RectTransform>();
    public float containerMargin;
    public float containerHeightOffset;
    public float multiblockHeightOffset;
    private void FixedUpdate()
    {
        UpdateBlockSize();
    }
    public void UpdateBlockSize()
    {
        // ������� ��������� ������� �����
        float totalContentHeight = 0;

        foreach (RectTransform container in containers)
        {
            float containerContentHeight = 0;

            foreach (Transform child in container) // ������ �� ������� � ��������
            {
                containerContentHeight += child.GetComponent<RectTransform>().rect.height + containerMargin;
            }

            if (container.childCount == 0) // ���� ���� ������, ��������������� �� ���������� ������
            {
                containerContentHeight += containerHeightOffset;
            }
            else // � ����������� �� ���������� ������ ������ ����������� �����������
            {
                containerContentHeight += containerMargin;
            }
            container.sizeDelta = new Vector2(container.sizeDelta.x, containerContentHeight);
            totalContentHeight += containerContentHeight;
        }
        multiBlock.sizeDelta = new Vector2(multiBlock.sizeDelta.x, totalContentHeight + multiblockHeightOffset);
    }
}
