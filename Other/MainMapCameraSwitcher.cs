using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class MainMapCameraSwitcher : MonoBehaviour
{
    [Header("Камеры")]
    public Camera[] staticCameras;        // Для быстрого откючения всех камер
    public Camera playerCamera;          // Камера игрока
    private Camera travelerCamera;        // Камера-путешественник (должна быть создана заранее)

    [Header("Настройки")]
    public float moveSpeed = 3f;         // Скорость перемещения
    public float rotationSpeed = 3f;     // Скорость вращения
    public float arrivalThreshold = 0.1f; // Порог прибытия
    public bool copyFOV = true;          // Копировать ли поле зрения

    [Header("Скорость кмаеры в катсценах")]
    public float moveSpeedToStatic = 15f;
    public float moveSpeedToPlayer = 25f;

    private Camera currentActiveCamera;  // Текущая активная камера
    private Camera targetCamera;         // Целевая камера для перехода
    private bool isTransitioning = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float journeyLength;
    private float startTime;

    private TaskCompletionSource<bool> switchCompletionSource;

    void Start()
    {
        // Инициализация камер
        if (travelerCamera == null)
        {
            GameObject travelerObj = new GameObject("Traveler Camera");
            travelerCamera = travelerObj.AddComponent<Camera>();
            travelerCamera.enabled = false;
            travelerCamera.gameObject.AddComponent<AudioListener>();
        }

        // По умолчанию активна камера игрока
        SetActiveCamera(playerCamera);
    }

    public void SwitchToCamera(Camera targetCam)
    {
        if (isTransitioning || targetCam == currentActiveCamera) return;

        // Начинаем переход
        targetCamera = targetCam;
        isTransitioning = true;

        // Настраиваем камеру-путешественник
        travelerCamera.transform.SetPositionAndRotation(
            currentActiveCamera.transform.position,
            currentActiveCamera.transform.rotation
        );
        
        CopyCameraParams(currentActiveCamera, travelerCamera);
        travelerCamera.enabled = true;
        currentActiveCamera.enabled = false;

        // Обновляем AudioListener
        UpdateAudioListener(currentActiveCamera, travelerCamera);

        // Запоминаем параметры для плавного перехода
        startPosition = travelerCamera.transform.position;
        startRotation = travelerCamera.transform.rotation;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, targetCam.transform.position);
    }

    public async Task SwitchToStaticCamera(Camera cam)
    {
        if (cam == null)
        {
            Debug.LogWarning("Invalid camera!");
            return;
        }

        moveSpeed = moveSpeedToStatic;
        switchCompletionSource?.TrySetCanceled();
        switchCompletionSource = new TaskCompletionSource<bool>();

        SwitchToCamera(cam);
        await switchCompletionSource.Task;
    }

    public async Task SwitchToPlayerCamera()
    {
        moveSpeed = moveSpeedToPlayer;

        switchCompletionSource?.TrySetCanceled();
        switchCompletionSource = new TaskCompletionSource<bool>();

        SwitchToCamera(playerCamera);
        await switchCompletionSource.Task;
    }

    void Update()
    {
        if (isTransitioning)
        {
            // Вычисляем прогресс перехода (0-1)
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = journeyLength > 0 ? distanceCovered / journeyLength : 1f;

            // Плавное перемещение и вращение
            travelerCamera.transform.position = Vector3.Lerp(
                startPosition,
                targetCamera.transform.position,
                fractionOfJourney
            );

            travelerCamera.transform.rotation = Quaternion.Slerp(
                startRotation,
                targetCamera.transform.rotation,
                fractionOfJourney
            );

            // Проверка завершения перехода
            if (fractionOfJourney >= 1f || 
                Vector3.Distance(travelerCamera.transform.position, targetCamera.transform.position) < arrivalThreshold)
            {
                CompleteTransition();
            }
        }
    }

    void CompleteTransition()
    {
        // Завершаем переход
        CopyCameraParams(targetCamera, travelerCamera);
        SetActiveCamera(targetCamera);
        travelerCamera.enabled = false;
        isTransitioning = false;
        switchCompletionSource?.TrySetResult(true);
    }

    void SetActiveCamera(Camera cam)
    {
        // Отключаем все камеры
        foreach (var staticCam in staticCameras)
        {
            staticCam.enabled = false;
            SetAudioListener(staticCam, false);
        }
        
        if (playerCamera != null) 
        {
            playerCamera.enabled = false;
            SetAudioListener(playerCamera, false);
        }
        
        travelerCamera.enabled = false;
        SetAudioListener(travelerCamera, false);

        // Включаем целевую камеру
        cam.enabled = true;
        SetAudioListener(cam, true);
        currentActiveCamera = cam;
    }

    void CopyCameraParams(Camera from, Camera to)
    {
        if (from == null || to == null) return;

        to.transform.SetPositionAndRotation(from.transform.position, from.transform.rotation);
        if (copyFOV) to.fieldOfView = from.fieldOfView;
        to.orthographic = from.orthographic;
        if (from.orthographic) to.orthographicSize = from.orthographicSize;
        to.nearClipPlane = from.nearClipPlane;
        to.farClipPlane = from.farClipPlane;
        // Можно добавить копирование других параметров по необходимости
    }

    // Обновляет AudioListener при переходе между камерами
    void UpdateAudioListener(Camera fromCamera, Camera toCamera)
    {
        SetAudioListener(fromCamera, false);
        SetAudioListener(toCamera, true);
    }

    // Включает или выключает AudioListener у камеры
    void SetAudioListener(Camera camera, bool enabled)
    {
        if (camera == null) return;
        
        var audioListener = camera.GetComponent<AudioListener>();
        if (audioListener != null)
        {
            audioListener.enabled = enabled;
        }
    }
}