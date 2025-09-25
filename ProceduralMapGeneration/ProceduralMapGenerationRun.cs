using UnityEngine;
using System.Collections;

public class ProceduralMapGenerationRun : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject[] listOfChunks;
    public int widthOfChunksGrid;
    public int heightOfChunksGrid;
    public int stepPrefabChunks;
    
    public GameObject[] listOfWalls;
    public int widthOfWalls;
    public int heightOfWalls;
    public int stepPrefabWalls;
    
    public Vector3 StartSpawnPosition = new Vector3(1f, 1f, 0f);

    public GameObject[,] grid;
    // Префаб, который будет спавниться как дочерний объект
    private GameObject childPrefab;

    public GameObject triggerObject;
    
    void Start()
    {       
        grid = new GameObject[widthOfChunksGrid, heightOfChunksGrid];

        // Проверяем массив и спавним чанки
        if (listOfChunks.Length == 0) Debug.LogWarning("Чанки не заданы!");
        else SpawnChunk();
     
        // Проверяем массив и спавним стены
        if (listOfWalls.Length == 0) Debug.LogWarning("Стены не заданы!");
        else SpawnWalls();

    }

    void Update(){
        // Спавн дочернего объекта при нажатии пробела
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShiftingMap();
        }
        MoveWorld();
    }

    void FixedUpdate()
    {
        // Перемещение родительского объекта с помощью клавиш
        MovePlayer();
        MoveWorld();
    }

    public void ShiftingMap()
    {
        Vector3 positionNext = new Vector3(triggerObject.transform.position.x + stepPrefabChunks, triggerObject.transform.position.y, triggerObject.transform.position.z);
        triggerObject.transform.position = positionNext;
        Debug.Log("ShiftingMap");
        //Генерируем чанки
        for (int i = 0; i < widthOfChunksGrid; i++){   
            for (int j = 0; j < heightOfChunksGrid; j++)
            {   
                if( i == 0){
                    Destroy(grid[i,j], 1f);
                }
                else if(i > 0 && i < widthOfChunksGrid-1){ 
                    grid[i-1,j] = grid[i,j];
                }
                else if(i == widthOfChunksGrid-1){
                    grid[i-1,j] = grid[i,j];

                    // Выбираем случайный индекс
                    Vector3 spawnPosition;
                    int randomIndex = Random.Range(0, listOfChunks.Length);
                    // Задаем координаты спавна
                    spawnPosition = new Vector3(grid[i,j].transform.position.x + stepPrefabChunks, grid[i-1,j].transform.position.y, StartSpawnPosition.z + grid[i-1,j].transform.position.z);
                    // Создаём экземпляр случайного префаба
                    GameObject chunk = Instantiate(listOfChunks[randomIndex], spawnPosition, Quaternion.identity);    
                    
                    chunk.transform.parent = gameObject.transform;
                    grid[i, j] = chunk;   
                }
            }
        }
    }

    private void MoveWorld(){
        if (Input.GetKeyDown(KeyCode.D))
        {
            ShiftingMap();
        }

        Vector3 movement = new Vector3(-moveSpeed, 0.0f, 0.0f) * Time.deltaTime;
        gameObject.transform.Translate(movement);
    }

    private void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShiftingMap();
        }

        Vector3 movement = new Vector3( 0.0f, moveVertical, moveHorizontal) * moveSpeed * Time.deltaTime;
        //player.transform.Translate(movement);
    }

    private void SpawnChild()
    {
        if (childPrefab != null)
        {
            // Создаем дочерний объект как ребенка родителя
            GameObject child = Instantiate(childPrefab);
            child.transform.parent = gameObject.transform;
            child.transform.localPosition = new Vector3(1f, 1f, 0f); // Позиция дочернего объекта относительно родителя
        }
        else
        {
            Debug.LogWarning("Child prefab is not assigned!");
        }
    }

    private void SpawnChunk(){
        //Генерируем чанки
        for (int i = 0; i < widthOfChunksGrid; i++){   
            for (int j = 0; j < heightOfChunksGrid; j++)
            {
                // Выбираем случайный индекс
                int randomIndex = Random.Range(0, listOfChunks.Length);
                // Задаем координаты спавна
                Vector3 spawnPosition = new Vector3(StartSpawnPosition.x + i*stepPrefabChunks, StartSpawnPosition.z,  StartSpawnPosition.y + j*stepPrefabChunks);
                // Создаём экземпляр случайного префаба
                GameObject chunk = Instantiate(listOfChunks[randomIndex], spawnPosition, Quaternion.identity);   
                chunk.transform.parent = gameObject.transform;
                grid[i, j] = chunk;
            }
        }
    }

    private void SpawnWalls(){
        //Генерируем стены
        for (int i = 0; i < heightOfWalls; i++){   
            for (int j = 0; j < widthOfWalls; j++)
            {
                int randomIndex = Random.Range(0, listOfWalls.Length);
                //Спавн стен слева
                if (i == 0){
                    Vector3 spawnPosition = new Vector3(StartSpawnPosition.x + i*stepPrefabWalls, StartSpawnPosition.z,  StartSpawnPosition.y + j*stepPrefabWalls);
                    Quaternion spawnRotation = Quaternion.Euler(0, 270, 0);
                    Instantiate(listOfWalls[randomIndex], spawnPosition, spawnRotation);
                }          

                //Спавн стен справа
                if (i == heightOfWalls-1){
                    Vector3 spawnPosition = new Vector3(StartSpawnPosition.x + i*stepPrefabWalls, StartSpawnPosition.z,  StartSpawnPosition.y + j*stepPrefabWalls);
                    Quaternion spawnRotation = Quaternion.Euler(0, 90, 0);
                    Instantiate(listOfWalls[randomIndex], spawnPosition, spawnRotation);
                }          

                //Спавн стен снизу
                if (j == 0){
                    Vector3 spawnPosition = new Vector3(StartSpawnPosition.x + i*stepPrefabWalls, StartSpawnPosition.z,  StartSpawnPosition.y + j*stepPrefabWalls);
                    Quaternion spawnRotation = Quaternion.Euler(0, 180, 0);
                    Instantiate(listOfWalls[randomIndex], spawnPosition, spawnRotation);
                }          

                //Спавн стен сверху
                if (j == widthOfWalls-1){
                    Vector3 spawnPosition = new Vector3(StartSpawnPosition.x + i*stepPrefabWalls, StartSpawnPosition.z,  StartSpawnPosition.y + j*stepPrefabWalls);
                    Quaternion spawnRotation = Quaternion.Euler(0, 0, 0);
                    Instantiate(listOfWalls[randomIndex], spawnPosition, spawnRotation);
                }          

            }
        }        
    }

}
