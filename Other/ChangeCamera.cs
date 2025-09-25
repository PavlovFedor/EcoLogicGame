using UnityEngine;
using UnityEngine.UI;

public class ChangeCamera : MonoBehaviour
{
    [Header("Статичные камеры")]
    public Camera[] staticCameras;       // Камеры, между которыми будем перемещаться
    [Header("Настройки")]
    public Button switchButton;           // Кнопка переключения
    public float moveSpeed = 3f;          // Скорость перемещения
    public bool copyFOV = true;           // Копировать ли поле зрения

    private Camera movingCamera;          // Камера, которая будет перемещаться
    private int targetCameraIndex = 0;   // Индекс целевой камеры
    private bool isMoving = false;

    void Start()
    {
        // Создаем камеру-путешественника
        movingCamera = new GameObject("Moving Camera").AddComponent<Camera>();
        movingCamera.enabled = true;

        // Копируем параметры из первой камеры
        CopyCameraParams(staticCameras[0], movingCamera);

        // Назначаем кнопке переключение
        switchButton.onClick.AddListener(StartCameraMove);
    }

    void StartCameraMove()
    {
        if (!isMoving)
        {
            // Выбираем следующую камеру (с зацикливанием)
            targetCameraIndex = (targetCameraIndex + 1) % staticCameras.Length;
            isMoving = true;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            Camera targetCam = staticCameras[targetCameraIndex];

            // Плавное перемещение
            movingCamera.transform.position = Vector3.Lerp(
                movingCamera.transform.position,
                targetCam.transform.position,
                Time.deltaTime * moveSpeed
            );

            // Плавное вращение
            movingCamera.transform.rotation = Quaternion.Lerp(
                movingCamera.transform.rotation,
                targetCam.transform.rotation,
                Time.deltaTime * moveSpeed
            );

            // Проверка завершения движения
            if (Vector3.Distance(movingCamera.transform.position, targetCam.transform.position) < 0.1f)
            {
                // Точное копирование параметров при достижении цели
                CopyCameraParams(targetCam, movingCamera);
                isMoving = false;
            }
        }
    }

    // Копирует важные параметры камеры
    void CopyCameraParams(Camera from, Camera to)
    {
        to.transform.SetPositionAndRotation(from.transform.position, from.transform.rotation);
        if (copyFOV) to.fieldOfView = from.fieldOfView;
        to.orthographic = from.orthographic;
        if (from.orthographic) to.orthographicSize = from.orthographicSize;
        to.nearClipPlane = from.nearClipPlane;
        to.farClipPlane = from.farClipPlane;
    }
}