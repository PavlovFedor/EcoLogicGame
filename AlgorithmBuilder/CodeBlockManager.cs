using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
//Этот скрипт будет управлять системой блочного кода, 
//собирая и выполняя код, созданный пользователем из блоков.

// Здесь был новый комментарий

public class CodeBlockManager : MonoBehaviour
{
    public List<CodeBlock> codeBlocks; // Список всех блоков кода
    public GameObject player;

    //Вставляет блок кода в нужное место в массиве
    public void InsertCodeBlock(int index, GameObject goBlock)
    {
        CodeBlock block =  goBlock.GetComponent<CodeBlock>();

        if(index != 0 && index != codeBlocks.Count){            
            //Убираем связь у блоков, где вставим наш блок
            DisconnectBlocks(codeBlocks[index-1], codeBlocks[index]);

            //Вставляем наш блок. Блок с этого места сдвинется вперед
            codeBlocks.Insert(index, block);

            //Подключаем его с обоих сторон
            CodeBlock previousCB = codeBlocks[index-1];
            ConnectBlocks(previousCB, block);
            CodeBlock nextCB = codeBlocks[index+1];
            ConnectBlocks(block, nextCB);
        }
        else if(index == 0)//Вставяем первый блок
        {
            if (codeBlocks.Count != 0){

                //Вставляем наш блок. Блок с этого места сдвинется вперед
                codeBlocks.Insert(index, block);

                //Подключаем его спереди
                CodeBlock nextCB = codeBlocks[index+1];
                ConnectBlocks(block, nextCB);
            }
            else// Значит массив пуст
            {
                //Вставляем наш блок.
                codeBlocks.Insert(index, block);                
            }
        }
        else if(index == codeBlocks.Count)//Вставляем последний блок
        {
            //Вставляем наш блок. Блок с этого места сдвинется вперед
            codeBlocks.Insert(index, block);

            //Подключаем его сзади
            CodeBlock previousCB = codeBlocks[index-1];
            ConnectBlocks(previousCB, block);
        }
    }

    //Метод для удаления блока
    public void DeleteCodeBlock(int index)
    {
        if (codeBlocks.Count == 0){
            return;
        }

        if (index != 0 && index != codeBlocks.Count-1)
        {

            ConnectBlocks(codeBlocks[index-1], codeBlocks[index+1]);
/*
            //Отключаем его с обоих сторон
            CodeBlock previousCB = codeBlocks[index-1];
            DisconnectBlocks(previousCB, codeBlocks[index]);
            CodeBlock nextCB = codeBlocks[index+1];
            DisconnectBlocks(codeBlocks[index], nextCB);
*/

            //Удаяем блок по индексу. Блок со след места сдвинется назад
            codeBlocks.RemoveAt(index);

        }
        else if (index == 0)
        {
            if (codeBlocks.Count == 1){
            //Удаяем блок по индексу.
            codeBlocks.RemoveAt(index);
            return;
            }

            //Отключаем его спереди
            CodeBlock nextCB = codeBlocks[index+1];
            DisconnectBlocks(codeBlocks[index], nextCB);

            //Удаяем блок по индексу. Блок со след места сдвинется назад
            codeBlocks.RemoveAt(index);
        }
        else if (index == codeBlocks.Count-1)
        {
            //Отключаем его сзади
            CodeBlock previousCB = codeBlocks[index-1];
            DisconnectBlocks(previousCB, codeBlocks[index]);
            
            //Удаяем блок по индексу.
            codeBlocks.RemoveAt(index);
        }
    }

    // Метод для выполнения всех блоков кода
    public async void ExecuteAll()
    {
        foreach (var block in codeBlocks)
        {
            await Task.Delay(1000);
            block.Execute();
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