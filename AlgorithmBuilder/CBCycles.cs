using UnityEngine;
using System.Collections.Generic;
//Этот скрипт будет управлять системой блочного кода, 
//собирая и выполняя код, созданный пользователем из блоков.

public class CBCycles : CodeBlock
{
    public List<CodeBlock> codeBlocks; // Список всех блоков кода
    public CodeBlock test1;
    public CodeBlock test2;


    public override void InitializationCB()
    {
         blockName = "CBCycles";
    }

    void Start()
    { 
        InitializationCB();
        AddCodeBlock(test1);
        test1.ConnectTo(test2);
        AddCodeBlock(test2);
        test2.ConnectTo(test1);    
        AddCodeBlock(test1);
    }

    // Метод для добавления блока кода
    public void AddCodeBlock(CodeBlock block)
    {
        if (!codeBlocks.Contains(block))
        {
            codeBlocks.Add(block);
            Debug.Log($"{block.blockName} added to the manager.");
        }
    }

    // Метод для подключения блоков
    public void ConnectBlocks(CodeBlock blockA, CodeBlock blockB)
    {
        blockA.ConnectTo(blockB);
        Debug.Log($"{blockA.blockName} connected to {blockB.blockName}.");
    }

    /*public override void Execute()
    {
        Debug.Log($"Блок {blockName} выполнен.");
    }*/
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