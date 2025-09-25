using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using System;

public class CBRotate : CodeBlock
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private float rotationSpeed = 5f;
    
    private float targetAngle;
    private bool isRotating;
    private const float RotationThreshold = 0.1f;
    private TaskCompletionSource<bool> rotateCompletionSource;

    private void Awake()
    {
        dropdown = GetComponentInChildren<TMP_Dropdown>(true);
        if (dropdown == null)
        {
            Debug.LogError("Dropdown не найден среди дочерних объектов!");
        }
    }

    private void FixedUpdate()
    {
        if (!isRotating) return;

        var targetRotation = Quaternion.Euler(0, targetAngle, 0);
        player.transform.rotation = Quaternion.Slerp(
            player.transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        if (Quaternion.Angle(player.transform.rotation, targetRotation) < RotationThreshold)
        {
            CompleteRotation();
        }
    }

    private void CompleteRotation()
    {
        isRotating = false;
        Debug.Log($"Поворот завершен. Текущий угол: {player.transform.eulerAngles.y}");
        rotateCompletionSource?.TrySetResult(true);
    }

    public override async Task ExecuteAsync(CancellationToken ct)
    {
        Debug.Log("Rotation started");
        
        try
        {
            ct.ThrowIfCancellationRequested();
            
            if (!ValidateReferences()) return;
            if (!SetTargetAngleAndDirection()) return;
            
            // Создаем TaskCompletionSource для ожидания завершения вращения
            rotateCompletionSource = new TaskCompletionSource<bool>();
            isRotating = true;
            
                try
                {
                    await WaitForTaskWithCancellation(rotateCompletionSource.Task, ct);
                }
                finally
                {
                    rotateCompletionSource = null;
                }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Rotation was canceled");
            CompleteRotation();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Rotation error: {ex.Message}");
            CompleteRotation();
        }
    }

    private async Task WaitForTaskWithCancellation(Task task, CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<bool>();
        
        using (ct.Register(() => tcs.TrySetCanceled()))
        {
            var completedTask = await Task.WhenAny(task, tcs.Task);
            if (completedTask == tcs.Task)
            {
                throw new OperationCanceledException(ct);
            }
            await task; // Пробрасываем исключения из исходной задачи
        }
    }

    private bool ValidateReferences()
    {
        if (dropdown == null)
        {
            Debug.LogError($"Dropdown not assigned in {blockName}");
            return false;
        }

        if (player == null)
        {
            Debug.LogError($"Player not assigned in {blockName}");
            return false;
        }

        if (codeBlockManager == null)
        {
            Debug.LogError($"CodeBlockManager not assigned in {blockName}");
            return false;
        }

        return true;
    }

    private bool SetTargetAngleAndDirection()
    {
        try
        {
            switch (dropdown.value)
            {
                case 0: // Вверх
                    targetAngle = 0f;
                    codeBlockManager.directionsPlayer = "up";
                    break;
                case 1: // Направо
                    targetAngle = 90f;
                    codeBlockManager.directionsPlayer = "right";
                    break;
                case 2: // Вниз
                    targetAngle = 180f;
                    codeBlockManager.directionsPlayer = "down";
                    break;
                case 3: // Налево
                    targetAngle = 270f;
                    codeBlockManager.directionsPlayer = "left";
                    break;
                default:
                    Debug.LogError($"Unknown dropdown value: {dropdown.value}");
                    return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task PerformRotationAsync(CancellationToken ct)
    {
        isRotating = true;
        rotateCompletionSource = new TaskCompletionSource<bool>();

        try
        {
            using (ct.Register(() => rotateCompletionSource?.TrySetCanceled()))
            {
                await rotateCompletionSource.Task;
            }
        }
        finally
        {
            rotateCompletionSource = null;
        }
    }

    public override void InitializationCB()
    {
        blockName = "CBRotate";
    }
}