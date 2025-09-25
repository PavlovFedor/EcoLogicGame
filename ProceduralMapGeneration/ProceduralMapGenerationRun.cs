using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProceduralMapGenerationRun : MonoBehaviour
{
    [Header("Chunk Generation Settings")]
    public float moveSpeed = 5f;
    public GameObject[] listOfChunks;
    public int widthOfChunksGrid = 5;
    public int heightOfChunksGrid = 3;
    public int stepPrefabChunks = 10;
    
    [Header("Wall Generation Settings")]
    public GameObject[] listOfWalls;
    public int widthOfWalls = 10;
    public int heightOfWalls = 5;
    public int stepPrefabWalls = 5;
    
    [Header("Other")]
    public bool enableWindSway = true;
    public string swaySearchTag = "Grass";

    [Header("Optimization Settings")]
    public float destroyCheckInterval = 0.1f; // Проверка каждые 0.1 секунды
    public int maxDestructionsPerFrame = 3;   // Макс. уничтожений за кадр
    public int chunksPerFrame = 2; // Оптимально для 60 FPS
    public float destroyDistance = 30f;

    public Vector3 StartSpawnPosition = new Vector3(1f, 1f, 0f);
    public GameObject[,] grid;
    public GameObject triggerObject;
    
    private WindSwaySearch _windSwaySystem;
    private Queue<GameObject> _chunksToDestroy = new Queue<GameObject>();
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private enum rotateChunk
    {
        Deg0 = 0,
        Deg90 = 90,
        Deg180 = 180,
        Deg270 = 270,
    }

    // Метод для получения случайного значения из перечисления
    rotateChunk GetRandomChunkRotation()
    {
        // Получаем массив всех значений перечисления
        rotateChunk[] values = (rotateChunk[])System.Enum.GetValues(typeof(rotateChunk));
        // Выбираем случайное значение
        return values[UnityEngine.Random.Range(0, values.Length)];
    }

    void Start()
    {
        // Инициализация системы покачивания
        if (enableWindSway)
        {
            _windSwaySystem = gameObject.GetComponent<WindSwaySearch>();
            if (_windSwaySystem == null)
            {
                _windSwaySystem = gameObject.AddComponent<WindSwaySearch>();
                _windSwaySystem.searchTag = swaySearchTag;
            }
        }

        grid = new GameObject[widthOfChunksGrid, heightOfChunksGrid];
        
        if (listOfChunks.Length == 0) 
            Debug.LogWarning("Chunks not assigned!");
        else 
            SpawnInitialChunksInstantly();
     
        if (listOfWalls.Length == 0) 
            Debug.LogWarning("Walls not assigned!");
        else 
            SpawnWalls();

        StartCoroutine(CleanupRoutine());
    }

    void Update()
    {
        HandleInput();
        MoveWorld();
        CleanupFarChunks();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    // Уничтожает чанки, которые ушли далеко позади игрока
    private void CleanupFarChunks()
    {
        Vector3 playerPos = Camera.main.transform.position;
        float sqrDestroyDistance = destroyDistance * destroyDistance; // Используем квадрат расстояния для оптимизации
        
        for (int i = 0; i < widthOfChunksGrid; i++)
        {
            for (int j = 0; j < heightOfChunksGrid; j++)
            {
                if (grid[i, j] != null)
                {
                    float sqrDistance = (playerPos - grid[i, j].transform.position).sqrMagnitude;

                    // Если чанк слишком далеко позади
                    if (sqrDistance > sqrDestroyDistance)
                    {
                        _chunksToDestroy.Enqueue(grid[i, j]);
                        grid[i, j] = null;
                    }
                }
            }
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.D))
        {
            ShiftingMap();
        }
    }

    private void SpawnInitialChunksInstantly()
    {
        for (int i = 0; i < widthOfChunksGrid; i++)
        {   
            for (int j = 0; j < heightOfChunksGrid; j++)
            {
                SpawnChunkAtPosition(i, j);
            }
        }
    }

    private IEnumerator SpawnInitialChunks()
    {
        for (int i = 0; i < widthOfChunksGrid; i++)
        {   
            for (int j = 0; j < heightOfChunksGrid; j++)
            {
                SpawnChunkAtPosition(i, j);

                if (j % chunksPerFrame == 0) // Ждём каждый N-ый чанк
                yield return null;
            }
        }
    }

    private void SpawnChunkAtPosition(int x, int y)
    {
        rotateChunk randomRotateChunk = GetRandomChunkRotation();

        // Задаем координаты и угол спавна
        switch (randomRotateChunk)
        {
            case rotateChunk.Deg0:
                spawnPosition = new Vector3(Mathf.Round(StartSpawnPosition.x + x * stepPrefabChunks), StartSpawnPosition.z, Mathf.Round(StartSpawnPosition.y + y * stepPrefabChunks));
                spawnRotation = Quaternion.Euler(0, 0, 0);
                break;
            case rotateChunk.Deg90:
                spawnPosition = new Vector3(Mathf.Round(StartSpawnPosition.x + x * stepPrefabChunks), StartSpawnPosition.z, Mathf.Round(StartSpawnPosition.y + (y - 1) * stepPrefabChunks));
                spawnRotation = Quaternion.Euler(0, 90, 0);
                break;//y-1
            case rotateChunk.Deg180:
                spawnPosition = new Vector3(Mathf.Round(StartSpawnPosition.x + (x - 1) * stepPrefabChunks), StartSpawnPosition.z, Mathf.Round(StartSpawnPosition.y + (y - 1) * stepPrefabChunks));
                spawnRotation = Quaternion.Euler(0, 180, 0);
                break;//y-1x-1
            case rotateChunk.Deg270:
                spawnPosition = new Vector3(Mathf.Round(StartSpawnPosition.x + (x - 1) * stepPrefabChunks), StartSpawnPosition.z, Mathf.Round(StartSpawnPosition.y + y * stepPrefabChunks));
                spawnRotation = Quaternion.Euler(0, 270, 0);
                break;//x-1
        }

        int randomIndex = Random.Range(0, listOfChunks.Length);
        GameObject chunk = Instantiate(listOfChunks[randomIndex], spawnPosition, spawnRotation);
        chunk.transform.parent = transform;
        grid[x, y] = chunk;

        if (enableWindSway && _windSwaySystem != null)
        {
            _windSwaySystem.ScanChunkForSwayObjects(chunk);
        }
    }

    public void ShiftingMap()
    {
        triggerObject.transform.position += Vector3.right * stepPrefabChunks;

        // Уничтожение старых чанков
        for (int j = 0; j < heightOfChunksGrid; j++)
        {
            if (grid[0, j] != null)
            {
                _chunksToDestroy.Enqueue(grid[0, j]);
                grid[0, j] = null;
            }
        }

        // Сдвиг чанков в массиве
        for (int i = 1; i < widthOfChunksGrid; i++)
        {
            for (int j = 0; j < heightOfChunksGrid; j++)
            {
                grid[i-1, j] = grid[i, j];
                grid[i, j] = null;
            }
        }

        // Запуск постепенного спавна новых чанков
        StartCoroutine(SpawnNewChunksAfterShift());
    }

    private IEnumerator SpawnNewChunksAfterShift()
    {
        for (int j = 0; j < heightOfChunksGrid; j++)
        {
            SpawnChunkAtPosition(widthOfChunksGrid-1, j);
            
            if (j % chunksPerFrame == 0) // Замедление, если нужно
                yield return null;
        }
    }

    private IEnumerator CleanupRoutine()
    {
        while (true)
        {
            if (_chunksToDestroy.Count > 0)
            {
                int destroyedThisFrame = 0;

                while (_chunksToDestroy.Count > 0 && destroyedThisFrame < maxDestructionsPerFrame)
                {
                    GameObject chunk = _chunksToDestroy.Dequeue();
                    if(chunk != null)
                    {
                        if (enableWindSway && _windSwaySystem != null)
                        {
                            _windSwaySystem.RemoveObjectsFromChunk(chunk);
                        }
                        DestroyImmediate(chunk);
                        destroyedThisFrame++;   
                    }
                }
            }
            yield return new WaitForSeconds(destroyCheckInterval);
        }
    }

    private void MoveWorld()
    {
        transform.Translate(-moveSpeed * Time.deltaTime, 0, 0);
    }

    private void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(0, moveVertical, moveHorizontal) * moveSpeed * Time.deltaTime;
        Camera.main.transform.Translate(movement);
    }

    private void SpawnWalls()
    {
        for (int i = 0; i < heightOfWalls; i++)
        {   
            for (int j = 0; j < widthOfWalls; j++)
            {
                int randomIndex = Random.Range(0, listOfWalls.Length);
                Vector3 spawnPosition = new Vector3(
                    StartSpawnPosition.x + i * stepPrefabWalls, 
                    StartSpawnPosition.z,  
                    StartSpawnPosition.y + j * stepPrefabWalls
                );

                if (i == 0) // Левая стена
                {
                    InstantiateWall(randomIndex, spawnPosition, 270);
                }          
                else if (i == heightOfWalls-1) // Правая стена
                {
                    InstantiateWall(randomIndex, spawnPosition, 90);
                }          
                else if (j == 0) // Нижняя стена
                {
                    InstantiateWall(randomIndex, spawnPosition, 180);
                }          
                else if (j == widthOfWalls-1) // Верхняя стена
                {
                    InstantiateWall(randomIndex, spawnPosition, 0);
                }
            }
        }        
    }

    private void InstantiateWall(int prefabIndex, Vector3 position, float yRotation)
    {
        Quaternion rotation = Quaternion.Euler(0, yRotation, 0);
        GameObject wall = Instantiate(listOfWalls[prefabIndex], position, rotation);
        wall.transform.parent = transform;
    }
}