using Polyperfect.Universal;
using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 3f; // Дистанция взаимодействия
    public LayerMask interactableLayer;
    public TextMeshProUGUI interactionText;
    private ObjectID currentInteractable;
    public MonoBehaviour movementScript; // Скрипт перемещения (например, FirstPersonController)
    public MonoBehaviour cameraScript; // Скрипт вращения камеры
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
                interactionText.text = $"Нажмите E для взаимодействия";
                interactionText.gameObject.SetActive(true);
                return;
            }
        }

        // Если ничего не нашли
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
        // Открываем панель
        psl.SetActive(true);

        // Блокируем управление
        SetPlayerControls(false);
    }

    // В методе закрытия панели
    public void HideLevelsPanel()
    {
        // Закрываем панель
        psl.SetActive(false);

        // Восстанавливаем управление
        SetPlayerControls(true);
    }
    private void SetPlayerControls(bool enabled)
    {
        // Блокируем/разблокируем перемещение
        if (movementScript != null)
            movementScript.enabled = enabled;

        // Блокируем/разблокируем камеру
        if (cameraScript != null)
            cameraScript.enabled = enabled;

        // Также можно управлять курсором
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enabled;
    }
}
