using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System;

public class CBInteraction : CodeBlock
{
    private ProceduralMapGenerationCommon mapGenerator;
    private CancellationTokenSource cts;
    private TaskCompletionSource<bool> interactionCompletionSource;

    private Animator characterAnimator;
    private float animationLength;
    private bool shouldPlayAnimation = false;

    public override async Task ExecuteAsync(CancellationToken ct = default)
    {
        Debug.Log($"Interaction block started");

        cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        interactionCompletionSource = new TaskCompletionSource<bool>();

        try
        {
            ct.ThrowIfCancellationRequested();

            if (mapGenerator == null)
            {
                Debug.LogError("Map generator reference is null");
                interactionCompletionSource?.TrySetResult(false);
                return;
            }

            // Проверяем условие для блока
            if (CheckInteractionCondition())
            {
                // 1. Воспроизведение анимации только если условие выполнено
                if (shouldPlayAnimation && characterAnimator != null && !string.IsNullOrEmpty(codeBlockManager.interactionNameAnim))
                {
                    try
                    {
                        characterAnimator.Play(codeBlockManager.interactionNameAnim);
                        await Task.Delay((int)(animationLength * 1000), cts.Token);
                    }
                    catch (Exception animEx)
                    {
                        Debug.LogWarning($"Animation error: {animEx.Message}");
                    }
                }
                Debug.Log("Me???");
                // 2. Основное действие
                ProcessChunkInteraction();
                interactionCompletionSource.TrySetResult(true);
            }
            else
            {
                Debug.Log("Interaction condition not met");
                // Устанавливаем результат как false, так как условие не выполнено
                interactionCompletionSource?.TrySetResult(false);
            }

            //await interactionCompletionSource.Task;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Interaction was canceled");
            interactionCompletionSource?.TrySetCanceled();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Interaction error: {ex.Message}");
            interactionCompletionSource?.TrySetException(ex);
        }
        finally
        {
            if (!interactionCompletionSource.Task.IsCompleted)
            {
                interactionCompletionSource?.TrySetResult(false);
            }
            cts?.Dispose();
            cts = null;
        }

        await interactionCompletionSource.Task;
    }

    private bool CheckInteractionCondition()
    {
        try
        {
            int x = codeBlockManager.x_playerCoordinateInGrid;
            int z = codeBlockManager.z_playerCoordinateInGrid;

            GameObject chunk = mapGenerator.GetChunkAt(x, z);

            if (chunk == null)
            {
                Debug.LogWarning($"Chunk not found at coordinates: {x}, {z}");
                return false;
            }

            Transform dirtyObj = chunk.transform.Find("Dirty");
            Transform clearObj = chunk.transform.Find("Clear");

            // Условие: Dirty активен, Clear неактивен
            bool conditionMet = dirtyObj != null && dirtyObj.gameObject.activeSelf &&
                              (clearObj == null || !clearObj.gameObject.activeSelf);

            // Анимация проигрывается только если условие выполнено
            shouldPlayAnimation = conditionMet;

            return conditionMet;
        }
        catch (Exception ex)
        {
            Debug.LogError($"CheckInteractionCondition error: {ex.Message}");
            return false;
        }
    }

    private void ProcessChunkInteraction()
    {
        try
        {
            int x = codeBlockManager.x_playerCoordinateInGrid;
            int z = codeBlockManager.z_playerCoordinateInGrid;

            GameObject chunk = mapGenerator.GetChunkAt(x, z);

            if (chunk == null)
            {
                Debug.LogWarning($"Chunk not found at coordinates: {x}, {z}");
                return;
            }

            Transform dirtyObj = chunk.transform.Find("Dirty");
            Transform clearObj = chunk.transform.Find("Clear");

            if (dirtyObj == null || clearObj == null)
            {
                Debug.LogWarning($"Dirty/Clear objects not found in chunk {chunk.name}");
                return;
            }

            dirtyObj.gameObject.SetActive(false);
            clearObj.gameObject.SetActive(true);
        }
        catch (Exception ex)
        {
            Debug.LogError($"ProcessChunkInteraction error: {ex.Message}");
        }
    }

    void OnDestroy()
    {
        cts?.Cancel();
        interactionCompletionSource?.TrySetResult(true);
    }

    public override void InitializationCB()
    {
        blockName = "CBInteraction";
        mapGenerator = codeBlockManager.scriptProceduralMapGeneration;

        // Получаем аниматор из игрока
        if (codeBlockManager.player != null)
        {
            characterAnimator = codeBlockManager.player.GetComponent<Animator>();

            if (characterAnimator != null && !string.IsNullOrEmpty(codeBlockManager.interactionNameAnim))
            {
                AnimationClip[] clips = characterAnimator.runtimeAnimatorController.animationClips;
                foreach (var clip in clips)
                {
                    if (clip.name == codeBlockManager.interactionNameAnim)
                    {
                        animationLength = clip.length;
                        Debug.Log($"Animation '{codeBlockManager.interactionNameAnim}' found, length: {animationLength}");
                        break;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Animator component or animation name not found");
            }
        }
        else
        {
            Debug.LogError("Player reference is null in codeBlockManager");
        }
    }
}