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
    // �������������� ��������� ��� JSON-�����
    public GameData CreateNewGameData()
    {
        GameData newData = new GameData
        {
            biomes = new List<Biome>
            {
                new Biome
                {
                    id = 1,
                    nameBiome = "�������",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "��������� ����", sceneName = "DistinctionOfBees", isCompletedLevel = false },
                        new Level { id = 2, nameLevel = "����������� ���", sceneName = "RiverPollution", isCompletedLevel = false }
                    }
                },
                new Biome
                {
                    id = 2,
                    nameBiome = "�����",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "����������", sceneName = "Utilization", isCompletedLevel = false },
                        new Level { id = 2, nameLevel = "����������� �����", sceneName = "Bonuslvl_SpaceJunk", isCompletedLevel = false}
                    }
                },
                new Biome
                {
                    id = 3,
                    nameBiome = "����",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "�������������� �����", sceneName = "RestorationPark", isCompletedLevel = false }
                    }
                },
                new Biome
                {
                    id = 4,
                    nameBiome = "���",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "����-��������", sceneName = "DroneFirefighter", isCompletedLevel = false },
                        new Level { id = 2, nameLevel = "�������������", sceneName = "Poaching", isCompletedLevel = false }
                    }
                },
                new Biome
                {
                    id = 5,
                    nameBiome = "�����",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "����� �� ������", sceneName = "Mazut", isCompletedLevel = false },
                        new Level { id = 2, nameLevel = "������� ����������� ������", sceneName = "Boat_OceanClean", isCompletedLevel = false },
                        new Level { id = 3, nameLevel = "������� ���������� ���� ������", sceneName = "DroneUnderwater_OceanClean", isCompletedLevel = false }
                    }
                },
                new Biome
                {
                    id = 6,
                    nameBiome = "�������",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "���������� �������", sceneName = "GreeeningDesert", isCompletedLevel = false }
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
        string json = JsonUtility.ToJson(newGameData, prettyPrint: true); // �� ������ ��������� ������ ������� JSON
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
