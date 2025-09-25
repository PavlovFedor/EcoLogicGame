using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PMGwithMask : ProceduralMapGenerationCommon
{
    //Скрипт маски для генерации карт с заданными статичными чанками
    private GameObject chunk;
    private StaticChunk buferChunk;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private enum rotateChunk
    {
        Deg0 = 0,
        Deg90 = 90,
        Deg180 = 180,
        Deg270 = 270,
    }

    //Данные статичного чанка  
    [System.Serializable]
    public struct StaticChunk
    {
        public int x;
        public int y;
        public GameObject prefabChunk;
    }

    public List<StaticChunk> staticChunkMask = new List<StaticChunk>();

    // Метод для получения случайного значения из перечисления
    rotateChunk GetRandomChunkRotation()
    {
        // Получаем массив всех значений перечисления
        rotateChunk[] values = (rotateChunk[])System.Enum.GetValues(typeof(rotateChunk));
        // Выбираем случайное значение
        return values[UnityEngine.Random.Range(0, values.Length)];
    }

    public override void SpawnChunk()
    {
        //Генерируем чанки
        for (int x = 0; x < widthOfChunksGrid; x++)
        {
            for (int y = 0; y < heightOfChunksGrid; y++)
            {
                //Выбираем сллучайный угол поворота чанка

                rotateChunk randomRotateChunk = GetRandomChunkRotation();

                // Задаем координаты и угол спавна
                switch (randomRotateChunk)
                {
                    case rotateChunk.Deg0:
                        spawnPosition = new Vector3(StartSpawnPositionChunks.x + x * stepPrefabChunks, StartSpawnPositionChunks.z, StartSpawnPositionChunks.y + y * stepPrefabChunks);
                        spawnRotation = Quaternion.Euler(0, 0, 0);
                        break;
                    case rotateChunk.Deg90:
                        spawnPosition = new Vector3(StartSpawnPositionChunks.x + x * stepPrefabChunks, StartSpawnPositionChunks.z, StartSpawnPositionChunks.y + (y - 1) * stepPrefabChunks);
                        spawnRotation = Quaternion.Euler(0, 90, 0);
                        break;//y-1
                    case rotateChunk.Deg180:
                        spawnPosition = new Vector3(StartSpawnPositionChunks.x + (x - 1) * stepPrefabChunks, StartSpawnPositionChunks.z, StartSpawnPositionChunks.y + (y - 1) * stepPrefabChunks);
                        spawnRotation = Quaternion.Euler(0, 180, 0);
                        break;//y-1x-1
                    case rotateChunk.Deg270:
                        spawnPosition = new Vector3(StartSpawnPositionChunks.x + (x - 1) * stepPrefabChunks, StartSpawnPositionChunks.z, StartSpawnPositionChunks.y + y * stepPrefabChunks);
                        spawnRotation = Quaternion.Euler(0, 270, 0);
                        break;//x-1
                }

                //Проверяем не находится ли позиция в списке запрещенных
                if (isChunkMasked(x, y))
                {
                    // Создаём префаб маски
                    chunk = Instantiate(buferChunk.prefabChunk, spawnPosition, spawnRotation);
                    chunk.transform.parent = chunks.transform;
                    grid[x, y] = chunk;
                    continue;
                }

                // Выбираем случайный индекс
                int randomIndex = UnityEngine.Random.Range(0, listOfChunks.Length);
                // Создаём экземпляр случайного префаба
                chunk = Instantiate(listOfChunks[randomIndex], spawnPosition, spawnRotation);
                chunk.transform.parent = chunks.transform;
                grid[x, y] = chunk;
            }
        }
    }

    public bool isChunkMasked(int x, int y)
    {
        foreach (var staticChunk in staticChunkMask)
        {
            if (staticChunk.x == x && staticChunk.y == y)
            {
                buferChunk = staticChunk;
                return true;
            }
        }
        return false;
    }
}
