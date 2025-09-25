using Polyperfect.Universal;
using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 3f; // ��������� ��������������
    public LayerMask interactableLayer;
    public TextMeshProUGUI interactionText;
    private ObjectID currentInteractable;
    public MonoBehaviour movementScript; // ������ ����������� (��������, FirstPersonController)
    public MonoBehaviour cameraScript; // ������ �������� ������
    public GameObject psl;
    private void Update()
    {
        CheckForInteractable();
        HandleInteractionInput();
    }
    void CheckForInteractable()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            ObjectID interactable = hit.collider.GetComponent<ObjectID>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                interactionText.text = $"������� E ��� ��������������";
                interactionText.gameObject.SetActive(true);
                return;
            }
        }

        // ���� ������ �� �����
        currentInteractable = null;
        interactionText.gameObject.SetActive(false);
    }
    void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.ObjSelector();
            ShowLevelsPanel();
        }
    }
    public void ShowLevelsPanel()
    {
        // ��������� ������
        psl.SetActive(true);

        // ��������� ����������
        SetPlayerControls(false);
    }

    // � ������ �������� ������
    public void HideLevelsPanel()
    {
        // ��������� ������
        psl.SetActive(false);

        // ��������������� ����������
        SetPlayerControls(true);
    }
    private void SetPlayerControls(bool enabled)
    {
        // ���������/������������ �����������
        if (movementScript != null)
            movementScript.enabled = enabled;

        // ���������/������������ ������
        if (cameraScript != null)
            cameraScript.enabled = enabled;

        // ����� ����� ��������� ��������
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enabled;
    }
}
