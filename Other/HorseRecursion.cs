using UnityEngine;
using System.Collections;//для корутин
using System.Collections.Generic;//для 

public class HorseRecursion : MonoBehaviour
{
    public GameObject checkCell;
    public GameObject prefabHorse;
    public int i = 0;
    public int j = 0;
    public float waitSec = 1f;

    private int startI;
    private int startJ;
    private Vector3 startVector;

    private GameObject goHorse;
    private bool[,] gridCompleted;
    private int[,] gridMoveChoice;
    private GameObject[,] gridGreenCheck;
    private ProceduralMapGenerationCommon scriptProceduralMapGenerationCommon;

    //for move horse
    public float moveDuration = 0.2f;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float moveTimer = 0.0f;

    //flags for while
    private bool isEndPlay = false;
    private bool isGameCompleted = false;

    //создание двух стеков для записи ходов удобней и лакоичней, чем структура  
    private Stack<int> stack_i = new Stack<int>();  
    private Stack<int> stack_j = new Stack<int>();

    //Стартовая функция
    public void SpawnHorse(){ 
        scriptProceduralMapGenerationCommon = gameObject.GetComponent<ProceduralMapGenerationCommon>();
        gridCompletedInitialization();
        gridMoveChoiceInitialization();
        gridGreenCheckInitialization();


        Vector3 spawnPosition;
        // Задаем координаты спавна
        spawnPosition = new Vector3(scriptProceduralMapGenerationCommon.grid[i,j].transform.position.x-1, scriptProceduralMapGenerationCommon.grid[i,j].transform.position.y, scriptProceduralMapGenerationCommon.grid[i,j].transform.position.z-1);
        // Создаём коня
        
        startVector = spawnPosition;
        Debug.Log(startVector);
        startI = i;
        startJ = j;

        goHorse = Instantiate(prefabHorse, spawnPosition, Quaternion.identity);    
        CellComplete();
        StackPush_ij();
        Debug.Log("Конь создан");   
    }

    public void StartHorseCycle(){
        isEndPlay = false;//This is not necessary, but It is required by the soul of the Creator!
        StartCoroutine(StartMoveHorse());
    }

    public void StopHorseCycle(){
        isEndPlay = true;//This is not necessary, but It is required by the soul of the Creator!
        StopCoroutine(StartMoveHorse());
    }

    //Осуществляет передвижение коня по полю
    public void MoveHorse(){
        if (isMoving)
        {
            moveTimer += Time.deltaTime;
            goHorse.transform.position = Vector3.Lerp(startPosition, endPosition, moveTimer / moveDuration);
            if (moveTimer >= moveDuration)
            {
                isMoving = false;
                goHorse.transform.position = endPosition;
            }
            return;
        }
    }

    //Начинает передвижение коня
    IEnumerator StartMoveHorse(){
        while ( !isEndPlay && !isGameCompleted){
            int now_i = i;
            //Получаем коррдинаты для передвижения
            Vector3 coordinateToMove = SearchCell();

            //Задаем координаты и обнуляем флаг с таймером
            startPosition = goHorse.transform.position;
            moveTimer = 0.0f;
            isMoving = true;
            
            //SearchCell изменит i, если найдет подходящую клетку
            if(now_i != i)
            {
                StackPush_ij();
                //Отмечаем кетку пройденной
                CellComplete();
            } else {
                if (stack_i.Count == scriptProceduralMapGenerationCommon.widthOfChunksGrid * scriptProceduralMapGenerationCommon.heightOfChunksGrid || gridMoveChoice[startI, startJ] == 8){
                    isGameCompleted = true;
                    Debug.Log("Игра завершена!");
                    yield return StartCoroutine(WaitSeconds());
                }else{
                    Debug.Log("Start move choice: " + gridMoveChoice[startI, startJ]);
                    //Убираем отметку пройденности
                    CellDisComplete();
                    StackPop_ij();
                    
                    Transform gridCell = scriptProceduralMapGenerationCommon.grid[stack_i.Peek(),stack_j.Peek()].transform;
                    coordinateToMove = new Vector3(gridCell.position.x, gridCell.position.y, gridCell.position.z);
                }
            }
            endPosition = new Vector3(coordinateToMove.x-1, coordinateToMove.y, coordinateToMove.z-1);

            yield return StartCoroutine(WaitSeconds());
        }
    }

    //Ищет координаты доступной клетки
    private Vector3 SearchCell(){
        int max_x = scriptProceduralMapGenerationCommon.widthOfChunksGrid-1;
        int max_y = scriptProceduralMapGenerationCommon.heightOfChunksGrid-1;
        Debug.Log("SearchCell");
        Vector3 coordinateToMove;
        //UP LEFT
        if(j <= max_y-2 && i >= 1 && gridCompleted[i-1,j+2] == false && gridMoveChoice[i,j] < 1){
            Transform gridCell = scriptProceduralMapGenerationCommon.grid[i-1,j+2].transform;
            coordinateToMove = new Vector3(gridCell.position.x, gridCell.position.y, gridCell.position.z);
            gridMoveChoice[i,j] = 1;
            i = i -1;
            j = j +2;
            return coordinateToMove;
        } else   
        //UP RIGHT
        if(j <= max_y-2 && i <= max_x-1 && gridCompleted[i+1,j+2] == false && gridMoveChoice[i,j] < 2){
            Transform gridCell = scriptProceduralMapGenerationCommon.grid[i+1,j+2].transform;
            coordinateToMove = new Vector3(gridCell.position.x, gridCell.position.y, gridCell.position.z);
            gridMoveChoice[i,j] = 2;
            i = i +1;
            j = j +2;
            return coordinateToMove;
        } else
        //RIGHT UP
        if(j <= max_y-1 && i <= max_x-2 && gridCompleted[i+2,j+1] == false && gridMoveChoice[i,j] < 3){
            Transform gridCell = scriptProceduralMapGenerationCommon.grid[i+2,j+1].transform;
            coordinateToMove = new Vector3(gridCell.position.x, gridCell.position.y, gridCell.position.z);
            gridMoveChoice[i,j] = 3;
            i = i +2;
            j = j +1;
            return coordinateToMove;
        } else
        //RIGHT DOWN
        if(j >= 1 && i <= max_x-2 && gridCompleted[i+2,j-1] == false && gridMoveChoice[i,j] < 4){
            Transform gridCell = scriptProceduralMapGenerationCommon.grid[i+2,j-1].transform;
            coordinateToMove = new Vector3(gridCell.position.x, gridCell.position.y, gridCell.position.z);
            gridMoveChoice[i,j] = 4;
            i = i +2;
            j = j -1;
            return coordinateToMove;
        } else
        //DOWN RGHT
        if(j >= 2 && i <= max_x-1 && gridCompleted[i+1,j-2] == false && gridMoveChoice[i,j] < 5){
            Transform gridCell = scriptProceduralMapGenerationCommon.grid[i+1,j-2].transform;
            coordinateToMove = new Vector3(gridCell.position.x, gridCell.position.y, gridCell.position.z);
            gridMoveChoice[i,j] = 5;
            i = i +1;
            j = j -2;
            return coordinateToMove;
        } else
        //DOWN LEFT
        if(j >= 2 && i >= 1 && gridCompleted[i-1,j-2] == false && gridMoveChoice[i,j] < 6){
            Transform gridCell = scriptProceduralMapGenerationCommon.grid[i-1,j-2].transform;
            coordinateToMove = new Vector3(gridCell.position.x, gridCell.position.y, gridCell.position.z);
            gridMoveChoice[i,j] = 6;
            i = i -1;
            j = j -2;
            return coordinateToMove;
        } else
        //LEFT DOWN
        if(j >= 1 && i >= 2 && gridCompleted[i-2,j-1] == false && gridMoveChoice[i,j] < 7){
            Transform gridCell = scriptProceduralMapGenerationCommon.grid[i-2,j-1].transform;
            coordinateToMove = new Vector3(gridCell.position.x, gridCell.position.y, gridCell.position.z);
            gridMoveChoice[i,j] = 7;
            i = i -2;
            j = j -1;
            return coordinateToMove;
        } else
        //LEFT UP
        if(j <= max_y-1 && i >= 2 && gridCompleted[i-2,j+1] == false && gridMoveChoice[i,j] < 8){
            Transform gridCell = scriptProceduralMapGenerationCommon.grid[i-2,j+1].transform;
            coordinateToMove = new Vector3(gridCell.position.x, gridCell.position.y, gridCell.position.z);
            gridMoveChoice[i,j] = 8;
            i = i -2;
            j = j +1;
            return coordinateToMove;
        }
        
        //Поиск окончен. Успешного пути нет
        if (i == startI && j == startJ){
           gridMoveChoice[startI, startJ] = 8;
        }
        return startVector;
    }

    //Отмечаем клетку на карте и в массиве пройденными
    private void CellComplete(){
        gridCompleted[i,j] = true;
        Vector3 spawnPosition;
        // Задаем координаты спавна
        spawnPosition = new Vector3(scriptProceduralMapGenerationCommon.grid[i,j].transform.position.x-1, scriptProceduralMapGenerationCommon.grid[i,j].transform.position.y, scriptProceduralMapGenerationCommon.grid[i,j].transform.position.z-1);
        // Создаём отметку
        gridGreenCheck[i,j] = Instantiate(checkCell, spawnPosition, Quaternion.identity);    
    }

    //Убираем отметку пройденности
    private void CellDisComplete(){
        gridCompleted[i,j] = false;
        // Удаляем отметку
        Destroy(gridGreenCheck[i,j]);
    }

    //Инициаизируем массив запоминающий выбор путей клеток
    private void gridMoveChoiceInitialization(){
        gridMoveChoice = new int[scriptProceduralMapGenerationCommon.widthOfChunksGrid, scriptProceduralMapGenerationCommon.heightOfChunksGrid];
        for (int i = 0; i < scriptProceduralMapGenerationCommon.widthOfChunksGrid; i++){   
            for (int j = 0; j < scriptProceduralMapGenerationCommon.heightOfChunksGrid; j++)
            {
                gridMoveChoice[i, j] = 0;
            }
        }
    }

    //Инициаизируем массив проверки прохождения клеток
    private void gridCompletedInitialization(){
        gridCompleted = new bool[scriptProceduralMapGenerationCommon.widthOfChunksGrid, scriptProceduralMapGenerationCommon.heightOfChunksGrid];
        for (int i = 0; i < scriptProceduralMapGenerationCommon.widthOfChunksGrid; i++){   
            for (int j = 0; j < scriptProceduralMapGenerationCommon.heightOfChunksGrid; j++)
            {
                gridCompleted[i, j] = false;
            }
        }
    }

    //Инициаизируем массив зеленых отметок пройденности
    private void gridGreenCheckInitialization(){
        gridGreenCheck = new GameObject[scriptProceduralMapGenerationCommon.widthOfChunksGrid, scriptProceduralMapGenerationCommon.heightOfChunksGrid];
        /*
        for (int i = 0; i < scriptProceduralMapGenerationCommon.widthOfChunksGrid; i++){   
            for (int j = 0; j < scriptProceduralMapGenerationCommon.heightOfChunksGrid; j++)
            {
                gridGreenCheck[i, j] = false;
            }
        }*/
    }
    
    // Приостанавливает выполнение на n секунд
    IEnumerator WaitSeconds()
    {
        yield return new WaitForSeconds(waitSec); 
     }


    //Добавяем запись хода в стеки
    private void StackPush_ij(){
        stack_i.Push(i);
        stack_j.Push(j);
        Debug.Log("Push: " + i + " " + j);
    }

    //Убираем запись хода из стеков
    private void StackPop_ij(){
        gridMoveChoice[i,j] = 0;

        stack_i.Pop();
        stack_j.Pop();

        i = stack_i.Peek();
        j = stack_j.Peek();
    }
}