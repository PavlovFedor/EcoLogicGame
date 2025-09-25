using UnityEngine;
using System.Collections.Generic;
//Этот скрипт будет представлять собой шаблон блока кода, 
//который будет иметь внутренние и внешние порты для соединений с другими блоками.

public class CodeBlock : MonoBehaviour
{
    public string blockName; // Имя блока

    // Порты сделаны списками дя отлова багов, но в них всегда хранится 1 элемент
    public List<CodeBlock> inputPorts; 
    public List<CodeBlock> outputPorts;

    protected GameObject player;

    void Start()
    {
        inputPorts = new List<CodeBlock>();
        outputPorts = new List<CodeBlock>();
        InitializationCB();
        player = GameObject.Find("Player");
    }

    // Метод для подключения этого блока к следующему
    public void ConnectTo(CodeBlock targetBlock)
    {
        Debug.Log(targetBlock);
        if (!outputPorts.Contains(targetBlock))
        {
            outputPorts.Add(targetBlock);
            targetBlock.inputPorts.Add(this);
        }
    }

    // Метод дя разрыва между этим и следующим
    public void DisconnectTo(CodeBlock targetBlock)
    {   
        outputPorts = null;
        targetBlock.inputPorts = null;
    }

    // Метод для выполнения кода блока
    public virtual void Execute()
    {
        // Здесь будет логика выполнения блока
        Debug.Log($"Блок {blockName} выполнен.");
    }

    public virtual void InitializationCB(){
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