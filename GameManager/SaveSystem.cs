using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class SaveSystem : MonoBehaviour
{
    [HideInInspector] public GameData gameDataset;
    public static SaveSystem Instance;
    private int currentCellId = 0;
    private bool howToPlayFlag = false;
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
                        new Level { id = 1, nameLevel = "��������� ����", sceneName = "BeesExtinguise", isCompletedLevel = false }
                    }
                },
                new Biome
                {
                    id = 2,
                    nameBiome = "�����",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "����������� ���", sceneName = "1lvl_RiverPollution", isCompletedLevel = false }
                    }
                },
                new Biome
                {
                    id = 3,
                    nameBiome = "����",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "�������������� �����", sceneName = "7lvl_RestorationPark", isCompletedLevel = false }
                    }
                },
                new Biome
                {
                    id = 4,
                    nameBiome = "���",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "��������� ���� 2", sceneName = "BeesExtinguise2", isCompletedLevel = false }
                    }
                },
                new Biome
                {
                    id = 5,
                    nameBiome = "�����",
                    isCompletedBiome = false,
                    levels = new List<Level>
                    {
                        new Level { id = 1, nameLevel = "������� ������", sceneName = "OceanCleaning", isCompletedLevel = false }
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
        howToPlayFlag = true;
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
    public void AutoSave()
    {
        if (gameDataset == null) return;

        string json = JsonUtility.ToJson(gameDataset, true);
        string savePath = Path.Combine(Application.persistentDataPath, $"saveGame{currentCellId}.json");
        File.WriteAllText(savePath, json);
    }
    public void MarkLevelCompleted(int biomeId, int levelId)
    {
        // ������� ��� ������� ���������� �������
        var biome = gameDataset.biomes.Find(b => b.id == biomeId);
        if (biome != null)
        {
            var level = biome.levels.Find(l => l.id == levelId);
            if (level != null)
            {
                level.isCompletedLevel = true;
            }
        }
    }
    public bool CheckBiomeCompleted(int biomeId)
    {
        bool flagBiome = false;
        var biome = gameDataset.biomes.Find(b => b.id == biomeId);
        if (biome != null)
        {
            if (biome.isCompletedBiome)
            {
                flagBiome = true;
            }
            else
            {
                flagBiome = false;
            }
        }
        return flagBiome;
    }
    public bool MarkPlayCutscene(int biomeId)
    {
        // ������� ��� ������� ������� �������� (����������� ���� ��� �� ��� ����)
        bool flagToPlay = false;
        var biome = gameDataset.biomes.Find(b => b.id == biomeId);
        if (biome.levels.All(l => l.isCompletedLevel) && !biome.isCompletedBiome)
        {
            flagToPlay = true;
            biome.isCompletedBiome = biome.levels.TrueForAll(l => l.isCompletedLevel);
        }
        else
        {
            flagToPlay = false;
        }
        return flagToPlay;
    }
    public void SetHowToPlayFlag(bool flag)
    {
        howToPlayFlag = flag;
    }
    public bool GetHowToPlayFlag()
    {
        return howToPlayFlag;
    }
}
