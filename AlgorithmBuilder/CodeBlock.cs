using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using TMPro;

//Этот скрипт будет представлять собой шаблон блока кода, 
//который будет иметь внутренние и внешние порты для соединений с другими блоками.

public class CodeBlock : MonoBehaviour
{
    public string blockName; // Имя блока

    // Порты сделаны списками дя отлова багов, но в них всегда хранится 1 элемент
    public List<CodeBlock> inputPorts = new List<CodeBlock>();
    public List<CodeBlock> outputPorts = new List<CodeBlock>();
    // Ссылки на игрока и строитель алгоритмоы
    protected GameObject player;
    protected CodeBlockManager codeBlockManager;

    public List<CodeBlock> listCodeBlock = new List<CodeBlock>();

    public List<CodeBlock> codeBlockList1;
    public List<CodeBlock> codeBlockList2;

    void Start()
    {
        inputPorts = new List<CodeBlock>();
        outputPorts = new List<CodeBlock>();

        player = GameObject.FindWithTag("Player");
        GameObject canvas = GameObject.Find("Canvas");

        // Проверяем, что объект был найден
        if (canvas != null)
        {
            // Получаем компонент CodeBlockManager
            codeBlockManager = canvas.GetComponent<CodeBlockManager>();

            // Проверяем, что компонент был найден
            if (codeBlockManager == null)
            {
                Debug.LogError("Компонент CodeBlockManager не найден на объекте Canvas.");
            }
        }
        else
        {
            Debug.LogError("Объект Canvas не найден в сцене.");
        }

        InitializationCB();
    }

    // Метод для подключения этого блока к следующему
    public void ConnectTo(CodeBlock targetBlock)
    {
        if (targetBlock == null)
        {
            Debug.LogError("Target block is null");
            return;
        }

        // Initialize lists if they're null (defensive programming)
        if (outputPorts == null) outputPorts = new List<CodeBlock>();
        if (targetBlock.inputPorts == null) targetBlock.inputPorts = new List<CodeBlock>();


        if (!outputPorts.Contains(targetBlock))
        {
            outputPorts.Add(targetBlock);
            targetBlock.inputPorts.Add(this);
        }
    }

    // Метод дя разрыва между этим и следующим
    public void DisconnectTo(CodeBlock targetBlock)
    {
        if (outputPorts != null && outputPorts.Contains(targetBlock))
        {
            outputPorts.Remove(targetBlock);
        }

        if (targetBlock.inputPorts != null && targetBlock.inputPorts.Contains(this))
        {
            targetBlock.inputPorts.Remove(this);
        }
    }

    // Виртуальный метод с поддержкой CancellationToken
    public virtual async Task ExecuteAsync(CancellationToken ct = default)
    {
        await Task.Run(() => ExecuteAsync(), ct);
    }

    public virtual void InitializationCB()
    {
        blockName = "CodeBlock";
    }
}

/*
### Описание скрипта
- **blockName**: Имя блока, которое можно задать в инспекторе.
- **inputPorts** и **outputPorts**: Списки для хранения подключенных блоков.
- **ConnectTo()**: Метод для соединения блоков.
- **Execute()**: Метод, который будет выполняться при активации блока. Логику выполнения можно расширить в наследуемых классах.
*/