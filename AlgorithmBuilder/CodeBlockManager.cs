using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using System;
//Этот скрипт будет управлять системой блочного кода, 
//собирая и выполняя код, созданный пользователем из блоков.

public class CodeBlockManager : MonoBehaviour
{
    public List<CodeBlock> codeBlocks; // Список всех блоков кода
    //Игрок и его данные
    public GameObject player;
    public string directionsPlayer = "up";
    // Скрипт Генератора карт
    public ProceduralMapGenerationCommon scriptProceduralMapGeneration;

    // Настройки анимации
    [Header("Block Animation")]
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float animationSpeed = 2f;   
    private CodeBlock currentlyExecutingBlock;
    private int currentBlockIndex = -1;
    private Coroutine activeAnimation;
    // Координаты игрока;
    [Header("Настройки игрока")]
    public float xOffset_playerCoordinateInGrid = 0.5f;
    public float zOffset_playerCoordinateInGrid = 0.5f;
    public int x_playerCoordinateInGrid = 0;
    public int z_playerCoordinateInGrid = 0;
    public string interactionNameAnim;

    //флаг выолнения блока
    private bool isExecuting = false;
    private bool isPaused = false;
    private bool isAnimating = false;
    private CancellationTokenSource executionCts;

    private void Awake()
    {
        // Сброс масштаба всех блоков при старте
        ResetAllBlocksVisuals();
    }

    //Вставляет блок кода в нужное место в массиве
    public void InsertCodeBlock(int index, GameObject goBlock, List<CodeBlock> listCodeBlock)
    {
        Debug.Log($"Вызван InsertCodeBlock c id {index}/{listCodeBlock.Count}");
        if (index < 0 || index > listCodeBlock.Count && listCodeBlock.Count != 0)
        {
            // Автоматически корректируем индекс, если он вне диапазона
            index = Mathf.Clamp(index, 0, listCodeBlock.Count);
            Debug.LogWarning($"Ошибка индекса при вставке блока: {index}");
            return;
        }

        CodeBlock block =  goBlock.GetComponent<CodeBlock>();

        if(index != 0 && index != listCodeBlock.Count){  
            //Убираем связь у блоков, где вставим наш блок
            DisconnectBlocks(listCodeBlock[index-1], listCodeBlock[index]);

            //Вставляем наш блок. Блок с этого места сдвинется вперед
            listCodeBlock.Insert(index, block);

            //Подключаем его с обоих сторон
            CodeBlock previousCB = listCodeBlock[index-1];
            ConnectBlocks(previousCB, block);
            CodeBlock nextCB = listCodeBlock[index+1];
            ConnectBlocks(block, nextCB);
        }
        else if(index == 0)//Вставяем первый блок
        {
            if (listCodeBlock.Count != 0){

                //Вставляем наш блок. Блок с этого места сдвинется вперед
                listCodeBlock.Insert(index, block);

                //Подключаем его спереди
                CodeBlock nextCB = listCodeBlock[index+1];
                ConnectBlocks(block, nextCB);
            }
            else// Значит массив пуст
            {
                //Вставляем наш блок.
                listCodeBlock.Insert(index, block);                
            }
        }
        else if(index == listCodeBlock.Count)//Вставляем последний блок
        {    
            //Вставляем наш блок. Блок с этого места сдвинется вперед
            listCodeBlock.Insert(index, block);

            //Подключаем его сзади
            CodeBlock previousCB = listCodeBlock[index-1];
            ConnectBlocks(previousCB, block);
        }
    }

    //Метод для удаления блока
    public void DeleteCodeBlock(int index, List<CodeBlock> listCodeBlock)
    {
        if (listCodeBlock.Count == 0){
            return;
        }

        if (index != 0 && index != listCodeBlock.Count-1)
        {

            ConnectBlocks(listCodeBlock[index-1], listCodeBlock[index+1]);

            //Удаяем блок по индексу. Блок со след места сдвинется назад
            listCodeBlock.RemoveAt(index);

        }
        else if (index == 0)
        {
            if (listCodeBlock.Count == 1){
            //Удаяем блок по индексу.
            listCodeBlock.RemoveAt(index);
            return;
            }

            //Отключаем его спереди
            CodeBlock nextCB = listCodeBlock[index+1];
            DisconnectBlocks(listCodeBlock[index], nextCB);

            //Удаяем блок по индексу. Блок со след места сдвинется назад
            listCodeBlock.RemoveAt(index);
        }
        else if (index == listCodeBlock.Count-1)
        {
            //Отключаем его сзади
            CodeBlock previousCB = listCodeBlock[index-1];
            DisconnectBlocks(previousCB, listCodeBlock[index]);
            
            //Удаяем блок по индексу.
            listCodeBlock.RemoveAt(index);
        }
    }

    // Метод для выполнения всех блоков кода
    // Основной метод выполнения
    public async void ExecuteAll()
    {
        if (isExecuting) return;
        
        // Полный сброс предыдущего состояния
        executionCts?.Dispose();
        executionCts = new CancellationTokenSource();

        isExecuting = true;
        isPaused = false;
        currentBlockIndex = -1;
        ResetAllBlocksVisuals();

        try
        {
            for (currentBlockIndex = 0; currentBlockIndex < codeBlocks.Count; currentBlockIndex++)
            {
                if (isPaused || executionCts.IsCancellationRequested)
                    break;

                var block = codeBlocks[currentBlockIndex];
                StartBlockAnimation(block.transform);

                try
                {
                    await block.ExecuteAsync(executionCts.Token);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log($"Блок {block.blockName} был прерван");
                    continue;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Ошибка в блоке {block.blockName}: {ex.Message}");
                    continue;
                }             
                finally
                {
                    if (!isPaused)
                        StopBlockAnimation(block.transform);
                }
            }
        }
        finally
        {
            if (!isPaused)
            {
                ResetExecutionState();
            }
        }
    }

    // Метод паузы
    public void PauseExecution()
    {
        if (!isExecuting || isPaused) return;
        
        isPaused = true;
        executionCts?.Cancel();
        // Не останавливаем анимацию - она продолжается на текущем блоке
        Debug.Log("Execution paused (animation continues)");
    }

    // Метод продолжения
    public void ResumeExecution()
    {
        if (!isPaused || !isExecuting) return;
        // Сначала сбрасываем анимацию, затем запускаем заново
        if (currentlyExecutingBlock != null)
        {
            StopBlockAnimation(currentlyExecutingBlock.transform);
            StartBlockAnimation(currentlyExecutingBlock.transform);
        }

        isPaused = false;
        executionCts = new CancellationTokenSource();
        ExecuteFromCurrentIndex();
    }

    public void StopExecution()
    {
        if (!isExecuting) return;
        
        executionCts?.Cancel();
        if (currentlyExecutingBlock != null)
        {
            StopBlockAnimation(currentlyExecutingBlock.transform);
            currentlyExecutingBlock = null;
        }

        isExecuting = false;
        isPaused = false;
        currentBlockIndex = -1;
        executionCts?.Dispose();
        executionCts = null;
        
        Debug.Log("Execution stopped completely");
    }
    // Метод перезапуска
    public void RestartExecution()
    {
        StopExecution();
        ExecuteAll();
        Debug.Log("Выполнение перезапущено");
    }
  
    private async void ExecuteFromCurrentIndex()
    {
        try
        {
            for (; currentBlockIndex < codeBlocks.Count; currentBlockIndex++)
            {
                if (isPaused || executionCts.IsCancellationRequested)
                    break;

                currentlyExecutingBlock = codeBlocks[currentBlockIndex];
                
                // Всегда запускаем анимацию для текущего блока
                StartBlockAnimation(currentlyExecutingBlock.transform);

                try
                {
                    await currentlyExecutingBlock.ExecuteAsync(executionCts.Token);
                }
                finally
                {
                    if (!isPaused)
                        StopBlockAnimation(currentlyExecutingBlock.transform);
                }
            }
        }
        finally
        {
            if (!isPaused)
            {
                ResetExecutionState();
            }
        }
    }

    private bool IsBlockAnimating(Transform blockTransform)
    {
        // Проверяем, идет ли уже анимация для этого блока
        return activeAnimation != null && blockTransform != null && blockTransform.localScale != Vector3.one;
    }

    private IEnumerator AnimateBlock(Transform blockTransform)
    {
        if (blockTransform == null) yield break;
        
        Vector3 originalScale = blockTransform.localScale;
        float timer = 0f;

        // Главное изменение - отдельный флаг для анимации
        while (true)
        {
            // Плавная пульсация масштаба
            timer += Time.deltaTime * animationSpeed;
            float scale = Mathf.Lerp(1f, pulseScale, Mathf.PingPong(timer, 1f));
            blockTransform.localScale = originalScale * scale;
            
            // Выходим только при явной остановке
            if (!isAnimating) break;
            
            yield return null;
        }

        // Автоматический сброс при завершении
        if (!isAnimating && blockTransform != null)
            blockTransform.localScale = originalScale;
    }

    private void StartBlockAnimation(Transform blockTransform)
    {    
        if (blockTransform == null) return;

        if (activeAnimation != null)
            StopCoroutine(activeAnimation);
        
        isAnimating = true;   
        activeAnimation = StartCoroutine(AnimateBlock(blockTransform));
    }

    private void StopBlockAnimation(Transform blockTransform)
    {
        isAnimating = false;

        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
            activeAnimation = null;
        }
        
        if(blockTransform != null)
            blockTransform.localScale = Vector3.one;
    }

    private void ResetBlockVisual(Transform blockTransform)
    {
        if (blockTransform != null)
            blockTransform.localScale = Vector3.one;
    }

    private void ResetExecutionState()
    {
        isExecuting = false;
        executionCts?.Dispose();
        currentBlockIndex = -1;
    }

    private void ResetAllBlocksVisuals()
    {
        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
            activeAnimation = null;
        }

        foreach (var block in codeBlocks)
        {
            if (block != null)
                block.transform.localScale = Vector3.one;
        }
    }

    // Метод для подключения блоков
    public void ConnectBlocks(CodeBlock blockA, CodeBlock blockB)
    {
        blockA.ConnectTo(blockB);
        Debug.Log($"{blockA.blockName} connected to {blockB.blockName}.");
    }

    // Метод для отключения блоков
    public void DisconnectBlocks(CodeBlock blockA, CodeBlock blockB)
    {
        blockA.DisconnectTo(blockB);
        Debug.Log($"{blockA.blockName} disconnected to {blockB.blockName}.");
    }
}
/*
### Описание скрипта
- **codeBlocks**: Список всех блоков кода, которые управляются менеджером.
- **AddCodeBlock()**: Метод для добавления нового блока кода в менеджер.
- **ExecuteAll()**: Метод для выполнения всех блоков кода в списке.
- **ConnectBlocks()**: Метод для подключения одного блока к другому.

## Использование

1. Создайте пустой объект в Unity и добавьте к нему компонент `CodeBlockManager`.
2. Создайте несколько объектов, добавьте к ним компонент `CodeBlock` и настройте их в инспекторе.
3. Используйте методы `AddCodeBlock()` и `ConnectBlocks()` для управления блоками и их соединениями.
4. Вызовите метод `ExecuteAll()` для выполнения всех подключенных блоков.

Эта система блочного кода может быть расширена для добавления различных типов блоков, логики выполнения и визуального представления.
*/