using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class ProceduralMapGenerationCommon : MonoBehaviour
{
    //Для чанков
    public GameObject[] listOfChunks;
    public int widthOfChunksGrid;
    public int heightOfChunksGrid;
    public int stepPrefabChunks;
    //Для стен
    public GameObject[] listOfWalls;
    public int widthOfWalls;
    public int heightOfWalls;
    public int stepPrefabWalls;

    //Проверка на привальный поворот префаба стен
    //Только по осям x и y. Другие углы не учитываются
    public int rotateWall = 0;

    public Vector3 StartSpawnPositionChunks = new Vector3(0f, 0f, 0f);
    public Vector3 StartSpawnPositionWalls = new Vector3(0f, 0f, 0f);

    //Offset для спавна стен
    //Right
    public int RightXWallsOffset_StartSpawnPosition = 0;
    public int RightYWallsOffset_StartSpawnPosition = 0;
    //Left
    public int LeftXWallsOffset_StartSpawnPosition = 0;
    public int LeftYWallsOffset_StartSpawnPosition = 0;
    //Up
    public int UpXWallsOffset_StartSpawnPosition = 0;
    public int UpYWallsOffset_StartSpawnPosition = 0;
    //Down
    public int DownXWallsOffset_StartSpawnPosition = 0;
    public int DownYWallsOffset_StartSpawnPosition = 0;
    
    public GameObject[,] grid;
    
    // Префаб, который будет спавниться как дочерний объект
    private GameObject childPrefab;

    private GameObject map;
    private GameObject walls;
    protected GameObject chunks;

    private void Start()
    {
        SpawnMap();
    }

    public void SpawnMap()
    {
        grid = new GameObject[widthOfChunksGrid, heightOfChunksGrid];

        // Создаем пустые объекты, чтобы складывать в них элементы карты
        map = new GameObject("Map");
        map.transform.localPosition = new Vector3(0f, 0f, 0f);
        walls = new GameObject("Walls");
        map.transform.localPosition = new Vector3(0f, 0f, 0f);
        chunks = new GameObject("Chunks");
        map.transform.localPosition = new Vector3(0f, 0f, 0f);

        chunks.transform.parent = map.transform;
        walls.transform.parent = map.transform;

        // Проверяем массив и спавним чанки
        if (listOfChunks.Length == 0) Debug.LogWarning("Чанки не заданы!");
        else SpawnChunk();
     
        // Проверяем массив и спавним стены
        if (listOfWalls.Length == 0) Debug.LogWarning("Стены не заданы!");
        else SpawnWalls();
    }

//        child.transform.parent = gameObject.transform;

    public virtual void SpawnChunk(){
        //Генерируем чанки
        for (int i = 0; i < widthOfChunksGrid; i++){   
            for (int j = 0; j < heightOfChunksGrid; j++)
            {
                // Выбираем случайный индекс
                int randomIndex = Random.Range(0, listOfChunks.Length);
                // Задаем координаты спавна
                Vector3 spawnPosition = new Vector3(StartSpawnPositionChunks.x + i*stepPrefabChunks, StartSpawnPositionChunks.z,  StartSpawnPositionChunks.y + j*stepPrefabChunks);
                // Создаём экземпляр случайного префаба
                GameObject chunk = Instantiate(listOfChunks[randomIndex], spawnPosition, Quaternion.identity);   
                chunk.transform.parent = chunks.transform;
                grid[i, j] = chunk;
            }
        }
    }

    public void SpawnWalls(){
        //Генерируем стены
        for (int i = 0; i < heightOfWalls; i++){   
            for (int j = 0; j < widthOfWalls; j++)
            {
                int randomIndex = Random.Range(0, listOfWalls.Length);
                //Спавн стен слева
                if (i == 0 && j != widthOfWalls-1){
                    Vector3 spawnPosition = new Vector3(StartSpawnPositionWalls.x + i * stepPrefabWalls + LeftXWallsOffset_StartSpawnPosition, StartSpawnPositionWalls.z, StartSpawnPositionWalls.y + j * stepPrefabWalls + LeftYWallsOffset_StartSpawnPosition);
                    Quaternion spawnRotation = Quaternion.Euler(0, 270 + rotateWall, 0);
                    GameObject wall = Instantiate(listOfWalls[randomIndex], spawnPosition, spawnRotation);
                    wall.transform.parent = walls.transform;
                }

                //Спавн стен справа
                if (i == heightOfWalls - 1 && j != 0)
                {
                    Vector3 spawnPosition = new Vector3(StartSpawnPositionWalls.x + i*stepPrefabWalls + RightXWallsOffset_StartSpawnPosition, StartSpawnPositionWalls.z, StartSpawnPositionWalls.y + j*stepPrefabWalls + RightYWallsOffset_StartSpawnPosition);
                    Quaternion spawnRotation = Quaternion.Euler(0, 90 + rotateWall, 0);
                    GameObject wall = Instantiate(listOfWalls[randomIndex], spawnPosition, spawnRotation);
                    wall.transform.parent = walls.transform;
                }          

                //Спавн стен снизу
                if (j == 0 && i != 0){
                    Vector3 spawnPosition = new Vector3(StartSpawnPositionWalls.x + i * stepPrefabWalls + DownXWallsOffset_StartSpawnPosition, StartSpawnPositionWalls.z, StartSpawnPositionWalls.y + j * stepPrefabWalls + DownYWallsOffset_StartSpawnPosition);
                    Quaternion spawnRotation = Quaternion.Euler(0, 180 + rotateWall, 0);
                    GameObject wall = Instantiate(listOfWalls[randomIndex], spawnPosition, spawnRotation);
                    wall.transform.parent = walls.transform;
                }          

                //Спавн стен сверху
                if (j == widthOfWalls-1 && i != heightOfWalls-1){
                    Vector3 spawnPosition = new Vector3(StartSpawnPositionWalls.x + i * stepPrefabWalls + UpXWallsOffset_StartSpawnPosition, StartSpawnPositionWalls.z, StartSpawnPositionWalls.y + j * stepPrefabWalls + UpYWallsOffset_StartSpawnPosition);
                    Quaternion spawnRotation = Quaternion.Euler(0, 0 + rotateWall, 0);
                    GameObject wall = Instantiate(listOfWalls[randomIndex], spawnPosition, spawnRotation);
                    wall.transform.parent = walls.transform;
                }          
            }
        }        
    }

    public GameObject GetChunkAt(int x, int z)
    {
        return grid[x,z];
    }
}