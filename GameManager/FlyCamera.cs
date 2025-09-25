using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float moveSpeed = 10f; // Базовая скорость движения
    public float lookSpeed = 2f; // Скорость вращения
    public float sprintMultiplier = 2f; // Множитель скорости при удержании Shift
    public float smoothTime = 0.1f; // Время сглаживания для плавного движения

    private float rotationX = 0f;
    private float rotationY = 0f;
    private Vector3 targetPosition; // Целевая позиция для плавного движения
    private Vector3 velocity = Vector3.zero; // Вектор скорости для SmoothDamp

    void Start()
    {
        // Инициализация целевой позиции
        targetPosition = transform.position;
    }

    void Update()
    {
        // Управление движением
        HandleMovement();

        // Управление вращением
        if (Input.GetMouseButton(1)) // Правая кнопка мыши зажата
        {
            HandleRotation();
        }

        // Плавное перемещение к целевой позиции
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    void HandleMovement()
    {
        float speed = moveSpeed;

        // Ускорение при удержании Shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= sprintMultiplier;
        }

        // Вычисляем направление движения
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += transform.right;
        }
        if (Input.GetKey(KeyCode.E))
        {
            moveDirection += transform.up;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            moveDirection -= transform.up;
        }

        // Нормализуем вектор направления, чтобы избежать ускорения при движении по диагонали
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
        }

        // Обновляем целевую позицию
        targetPosition += moveDirection * speed * Time.deltaTime;
    }

    void HandleRotation()
    {
        // Получаем ввод от мыши
        rotationX += Input.GetAxis("Mouse X") * lookSpeed;
        rotationY -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f); // Ограничиваем угол по вертикали

        // Применяем вращение
        transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);
    }
}