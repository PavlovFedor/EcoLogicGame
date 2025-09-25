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
    public GameObject pauseMenuMap;
    public GameObject howToPlay;
    private void Start()
    {
        if(SaveSystem.Instance.GetHowToPlayFlag())
        {
            howToPlay.SetActive(true);
            SaveSystem.Instance.SetHowToPlayFlag(false);
            SetPlayerControls(false);
        }
    }
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
            ShowPanel(psl);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && currentInteractable == null)
        {
            ShowPanel(pauseMenuMap);
        }
        if (Input.GetKeyDown(KeyCode.Tab) && currentInteractable == null)
        {
            ShowPanel(howToPlay);
        }
    }
    public void ShowPanel(GameObject panel)
    {
        // Открываем панель
        panel.SetActive(true);

        // Блокируем управление
        SetPlayerControls(false);
    }

    // В методе закрытия панели
    public void HidePanel(GameObject panel)
    {
        // Закрываем панель
        panel.SetActive(false);

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
