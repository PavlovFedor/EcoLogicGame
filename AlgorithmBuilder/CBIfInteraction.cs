using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

public class CBIfCondition : CodeBlock
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown tagDropdown;
    [SerializeField] private TMP_Dropdown directionDropdown;

    private ProceduralMapGenerationCommon mapGenerator;
    private bool elseBranchEnabled = false;
    private bool isAnimationComplete = false;

    public override async Task ExecuteAsync(CancellationToken ct = default)
    {
        Debug.Log("Вызван IfElseBee");
        // Проверяем условие
        bool conditionMet = CheckCondition();

        // Ожидаем окончания анимации, если она есть
        await WaitForAnimationComplete(ct);

        // Выполняем соответствующую ветку
        if (conditionMet)
        {
            Debug.Log("Переход в 1 ветку");
            await ExecuteBranch(codeBlockList1, ct);
        }
        else if (elseBranchEnabled)
        {
            Debug.Log("Переход в 2 ветку");
            await ExecuteBranch(codeBlockList2, ct);
        }
    }

    private async Task WaitForAnimationComplete(CancellationToken ct)
    {
        // Проверяем, нужно ли ожидать анимацию
        if (CheckInteractionCondition())
        {
            isAnimationComplete = false;

            // Подписываемся на событие окончания анимации
            // Предполагается, что где-то в другом месте есть вызов AnimationComplete()

            // Ожидаем, пока флаг isAnimationComplete не станет true
            while (!isAnimationComplete)
            {
                if (ct.IsCancellationRequested) return;
                await Task.Yield();
            }
        }
    }

    // Этот метод должен вызываться по окончании анимации
    public void AnimationComplete()
    {
        isAnimationComplete = true;
    }

    // Метод для проверки необходимости ожидания анимации
    private bool CheckInteractionCondition()
    {
        // Здесь должна быть логика, определяющая, нужно ли ожидать анимацию
        // Например, проверка наличия анимации или определенного состояния
        return true; // Временная заглушка - замените на реальную логику
    }

    private async Task ExecuteBranch(List<CodeBlock> codeBlocks, CancellationToken ct)
    {
        foreach (var block in codeBlocks)
        {
            Debug.Log($"IfElse Выполняется {block}");
            if (ct.IsCancellationRequested) return;
            await block.ExecuteAsync(ct);
        }
        Debug.Log("IfElse Ветка выполнена");
    }

    private bool CheckCondition()
    {
        if (tagDropdown == null || directionDropdown == null)
        {
            Debug.LogError("Dropdowns not assigned in CBIfCondition");
            return false;
        }

        string selectedTag = tagDropdown.options[tagDropdown.value].text;
        string selectedDirection = directionDropdown.options[directionDropdown.value].text;

        int checkX = codeBlockManager.x_playerCoordinateInGrid;
        int checkZ = codeBlockManager.z_playerCoordinateInGrid;

        switch (selectedDirection)
        {
            case "СВЕРХУ":
                checkZ = codeBlockManager.z_playerCoordinateInGrid + codeBlockManager.scriptProceduralMapGeneration.stepPrefabChunks;
                checkX = codeBlockManager.x_playerCoordinateInGrid;
                break;
            case "СПРАВА":
                checkZ = codeBlockManager.z_playerCoordinateInGrid;
                checkX = codeBlockManager.x_playerCoordinateInGrid + codeBlockManager.scriptProceduralMapGeneration.stepPrefabChunks;
                break;
            case "СНИЗУ":
                checkZ = codeBlockManager.z_playerCoordinateInGrid - codeBlockManager.scriptProceduralMapGeneration.stepPrefabChunks;
                checkX = codeBlockManager.x_playerCoordinateInGrid;
                break;
            case "СЛЕВА":
                checkZ = codeBlockManager.z_playerCoordinateInGrid;
                checkX = codeBlockManager.x_playerCoordinateInGrid - codeBlockManager.scriptProceduralMapGeneration.stepPrefabChunks;
                break;
            default:
                Debug.LogWarning($"Неизвестное направление игрока в блоке {blockName} = {codeBlockManager.directionsPlayer}");
                break;
        }

        Debug.Log($"IfElse Условия {isWall(checkX, checkZ)}, {isFlower(checkX, checkZ)}");
        bool isTrueTag = false;
        switch (selectedTag)
        {
            case "СТЕНА":
                isTrueTag = isWall(checkX, checkZ);
                break;
            case "ЦВЕТОК":
                isTrueTag = isFlower(checkX, checkZ);
                break;
            case "ДРУГОЕ":
                isTrueTag = !isWall(checkX, checkZ) && !isFlower(checkX, checkZ);
                break;
            default:
                Debug.LogWarning("IfElseBee Неивестный тег");
                break;
        }
        return isTrueTag;
    }

    private bool isWall(int checkX, int checkZ)
    {
        if (checkX < 0 || checkX >= mapGenerator.widthOfChunksGrid ||
            checkZ < 0 || checkZ >= mapGenerator.heightOfChunksGrid)
            return true;
        else
            return false;
    }

    private bool isFlower(int checkX, int checkZ)
    {
        if (checkX < 0 || checkX >= mapGenerator.widthOfChunksGrid || checkZ < 0 || checkZ >= mapGenerator.heightOfChunksGrid)
            return false;
        else if (!codeBlockManager.scriptProceduralMapGeneration.GetChunkAt(checkX, checkZ).CompareTag("Flower"))
            return false;
        else
            return true;
    }

    public override void InitializationCB()
    {
        blockName = "CBIfCondition";
        mapGenerator = codeBlockManager.scriptProceduralMapGeneration;
    }
}