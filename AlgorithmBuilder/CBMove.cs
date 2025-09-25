using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using TMPro;

// Блок Передвижения
public class CBMove: CodeBlock
{
   //for move
    public float moveDuration = 0.2f;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float moveTimer = 0.0f;

    private int x_nextPosition;
    private int z_nextPosition;

    // Добавляем TaskCompletionSource для отслеживания завершения движения
    private TaskCompletionSource<bool> moveCompletionSource;
    private bool isCompletionSourceActive = false;

    public override async Task ExecuteAsync(CancellationToken ct)
    {
        Debug.Log("Move вызван");


        if (player == null)
        {
            Debug.LogError("Player reference is null");
            return;
        }

        try
        {
            ct.ThrowIfCancellationRequested();
            // Создаем TaskCompletionSource для ожидания завершения движения
            var moveTask = new TaskCompletionSource<bool>();
            StartMovePlayer(moveTask);
            await WaitForTaskWithCancellation(moveTask.Task, ct);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Move operation was canceled");
            ResetMoveState();
            // Не бросаем исключение дальше, чтобы менеджер мог продолжить
            return;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка в Move: {ex.Message}");
            // Не бросаем исключение дальше
            return;
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

    private async Task WaitForMoveCompletion(CancellationToken ct)
    {
        // Реинициализация TaskCompletionSource если нужно
        if (moveCompletionSource == null || isCompletionSourceActive)
        {
            moveCompletionSource = new TaskCompletionSource<bool>();
        }
        isCompletionSourceActive = true;

        // Регистрация callback для отмены
        using (ct.Register(() => 
        {
            if (moveCompletionSource != null && !moveCompletionSource.Task.IsCompleted)
            {
                moveCompletionSource.TrySetCanceled(ct);
            }
        }))
        {
            await moveCompletionSource.Task;
        }
        
        isCompletionSourceActive = false;
    }

    private void StartMovePlayer(TaskCompletionSource<bool> completionSource)
    {
        //Узнает текущие координаты игрока
        //Узнает куда нужно переместить
        //Проверяет можно ли переместить
        //Перемещает

        this.moveCompletionSource = completionSource;

        if (codeBlockManager == null || 
            codeBlockManager.scriptProceduralMapGeneration == null || 
            player == null)
        {
            Debug.LogError("Critical references are null");
            CompleteMove(false);
            return;
        }

        //Текущие координаты игрока
        Vector3 playerPosition = player.transform.position; 

        // Вычисляем новые коррдинаты игрока
        switch (codeBlockManager.directionsPlayer)
        {
            case "up": // Вверх
                z_nextPosition = codeBlockManager.z_playerCoordinateInGrid + codeBlockManager.scriptProceduralMapGeneration.stepPrefabChunks;
                x_nextPosition = codeBlockManager.x_playerCoordinateInGrid;
                break;
            case "right": // Направо
                z_nextPosition = codeBlockManager.z_playerCoordinateInGrid;
                x_nextPosition = codeBlockManager.x_playerCoordinateInGrid + codeBlockManager.scriptProceduralMapGeneration.stepPrefabChunks;
                break;
            case "down": // Вниз
                z_nextPosition = codeBlockManager.z_playerCoordinateInGrid - codeBlockManager.scriptProceduralMapGeneration.stepPrefabChunks;
                x_nextPosition = codeBlockManager.x_playerCoordinateInGrid;
                break;
            case "left": // Налево
                z_nextPosition = codeBlockManager.z_playerCoordinateInGrid;
                x_nextPosition = codeBlockManager.x_playerCoordinateInGrid - codeBlockManager.scriptProceduralMapGeneration.stepPrefabChunks;
                break;
            default:
                Debug.LogWarning($"Неизвестное направление игрока в блоке {blockName} = {codeBlockManager.directionsPlayer}");
                CompleteMove(false);
                break;
        }

        //Проверка доступности клеток
        if(!isGoodCell()) 
        {
            Debug.LogWarning($"Движение вперед отменено. Клетка за границей карты");
            // Уведомляем, что движение завершено
            CompleteMove(false);
            return; 
        }
        //Задаем координаты и обнуляем флаг с таймером
        startPosition = playerPosition;
        endPosition = new Vector3( 
            x_nextPosition + codeBlockManager.xOffset_playerCoordinateInGrid,
            playerPosition.y, 
            z_nextPosition + codeBlockManager.zOffset_playerCoordinateInGrid);
        
        Debug.LogError($"Движению быть: {startPosition}, {endPosition}");
        moveTimer = 0.0f;
        isMoving = true;
    }

    //Осуществяет передвижение игрока
    void Update()
    { 
        if (isMoving)
        {
            moveTimer += Time.deltaTime;
            player.transform.position = Vector3.Lerp(startPosition, endPosition, moveTimer / moveDuration);
            if (moveTimer >= moveDuration)
            {
                player.transform.position = endPosition;
                isMoving = false;
                CompleteMove(true);
            }
            return;
        }
    }

    private void CompleteMove(bool success)
    {
        if (moveCompletionSource != null && !moveCompletionSource.Task.IsCompleted)
        {
            moveCompletionSource.TrySetResult(success);
        }
    }

    private void ResetMoveState()
    {
        isMoving = false;
        if (moveCompletionSource != null && !moveCompletionSource.Task.IsCompleted)
        {
            moveCompletionSource.TrySetCanceled();
        }
    }

    public override void InitializationCB()
    {
        blockName = "CBMove";
    }

    private bool isGoodCell()
    {
        if (codeBlockManager?.scriptProceduralMapGeneration == null)
        {
            Debug.LogError("Map generation reference is null");
            return false;
        }

        bool isXgood = false;
        bool isZgood = false;

        if (x_nextPosition < codeBlockManager.scriptProceduralMapGeneration.widthOfChunksGrid && x_nextPosition >= 0)
            isXgood = true;
        if (z_nextPosition < codeBlockManager.scriptProceduralMapGeneration.heightOfChunksGrid && z_nextPosition >= 0)
            isZgood = true;

        Debug.Log($"isGoodCell x={isXgood} and z={isZgood}. x z {x_nextPosition} {z_nextPosition}");
      
        if (isXgood && isZgood)
        {
            codeBlockManager.x_playerCoordinateInGrid = x_nextPosition;
            codeBlockManager.z_playerCoordinateInGrid = z_nextPosition;
            return true;
        }
        else
        {
            return false;
        }
    }

    
}