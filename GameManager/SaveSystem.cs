using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    [HideInInspector] public GameData gameDataset;
    public static SaveSystem Instance;
    private int currentCellId = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Подготавливаем структуру для JSON-файла
    public GameData CreateNewGameData()
    {
        GameData newData = new GameData
        {
            biomes = new List<Biome>
            {
                new Biome
                {
                    id = 1,
                    nameBiome = "Равнина",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "Вымирание пчел", sceneName = "DistinctionOfBees", isCompletedLevel = false },
                        new Level { id = 2, nameLevel = "Загрязнение рек", sceneName = "RiverPollution", isCompletedLevel = false }
                    }
                },
                new Biome
                {
                    id = 2,
                    nameBiome = "Завод",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "Утилизация", sceneName = "Utilization", isCompletedLevel = false },
                        new Level { id = 2, nameLevel = "Космический мусор", sceneName = "Bonuslvl_SpaceJunk", isCompletedLevel = false}
                    }
                },
                new Biome
                {
                    id = 3,
                    nameBiome = "Парк",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "Восстановление парка", sceneName = "RestorationPark", isCompletedLevel = false }
                    }
                },
                new Biome
                {
                    id = 4,
                    nameBiome = "Лес",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "Дрон-пожарный", sceneName = "DroneFirefighter", isCompletedLevel = false },
                        new Level { id = 2, nameLevel = "Браконьерство", sceneName = "Poaching", isCompletedLevel = false }
                    }
                },
                new Biome
                {
                    id = 5,
                    nameBiome = "Океан",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "Мазут на берегу", sceneName = "Mazut", isCompletedLevel = false },
                        new Level { id = 2, nameLevel = "Очистка поверхности океана", sceneName = "Boat_OceanClean", isCompletedLevel = false },
                        new Level { id = 3, nameLevel = "Очистка подводного мира океана", sceneName = "DroneUnderwater_OceanClean", isCompletedLevel = false }
                    }
                },
                new Biome
                {
                    id = 6,
                    nameBiome = "Пустыня",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "Озеленение пустыни", sceneName = "GreeeningDesert", isCompletedLevel = false }
                    }
                }
            }
        };
        return newData;
    }
    public void SaveNewGame(int CellId)
    {
        currentCellId = CellId;
        GameData newGameData = CreateNewGameData();
        string json = JsonUtility.ToJson(newGameData, prettyPrint: true); // На основе структуры данных создаем JSON
        string savePath = Path.Combine(Application.persistentDataPath, $"saveGame{CellId}.json");
        File.WriteAllText(savePath, json);
    }
    public void LoadGameData(int CellId)
    {
        currentCellId = CellId;
        string path = Path.Combine(Application.persistentDataPath, $"saveGame{CellId}.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            gameDataset = JsonUtility.FromJson<GameData>(json);
        }
    }
    public void OnObjectSelected(int objId)
    {
        Biome selectedObj = gameDataset.biomes.Find(bs => bs.id == objId);
        if (selectedObj != null)
        {
            FindAnyObjectByType<PSLDataProcessing>().ShowPSL(selectedObj);
        }
    }
}
