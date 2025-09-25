using UnityEngine;
using TMPro;
using System.IO;
using System;
using System.Linq;

public class SaveCellProcessing : MonoBehaviour
{
    public GameObject noDataContent;
    public GameObject dataContent;
    public TextMeshProUGUI lastBiomeText;
    public TextMeshProUGUI percentCompletedText;
    public TextMeshProUGUI dateText;

    public string saveFileName = "saveGame.json";
    private void Update()
    {
        SetSaveData();
    }
    public void SetSaveData()
    {
        string savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        if (File.Exists(savePath))
        {
            noDataContent.SetActive(false);
            dataContent.SetActive(true);

            // Извлекаем данные из JSON
            string json = File.ReadAllText(savePath);
            GameData saveData = JsonUtility.FromJson<GameData>(json);
            // Вставляем текст с последним доступным биомом
            Biome lastBiome = GetLastBiome(saveData);
            lastBiomeText.text = $"{lastBiome.nameBiome}";
            // Вставляем текст с процентом пройденых уровней
            int percentCompleted = CalculatePercentComplete(saveData);
            percentCompletedText.text = $"{percentCompleted}% ЗАВЕРШЕНО";
            // Вставляем текст с датой сохранения
            DateTime lastSaveTime = File.GetLastWriteTime(savePath);
            dateText.text = $"{lastSaveTime:dd.MM.yyyy HH:mm}";
        }
        else
        {
            noDataContent.SetActive(true);
            dataContent.SetActive(false);
        }
    }
    private int CalculatePercentComplete(GameData data)
    {
        int totalLevels = 0;
        int completedLevels = 0;
        foreach (var b in data.biomes)
        {
            foreach (var lvl in b.levels)
            {
                totalLevels++;
                if (lvl.isCompletedLevel)
                {
                    completedLevels++;
                }
            }
        }
        return completedLevels * 100 / totalLevels;
    }
    private Biome GetLastBiome(GameData data)
    {
        foreach (var b in data.biomes)
        {
            if(!b.isCompletedBiome)
            {
                return b;
            }
        }
        return data.biomes.Last();
    }
}
