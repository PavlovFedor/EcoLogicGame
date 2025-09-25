using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Polyperfect.Universal;

public class PSLDataProcessing : MonoBehaviour
{
    public TextMeshProUGUI titleBiomeText;
    public TextMeshProUGUI countLvls;
    public GameObject levelButtonPrefab;
    public Sprite completedLvlSprite;
    public Sprite sourceLvlSprite;
    public Transform levelList;
    public void ShowPSL(Biome biome)
    {
        int totalLevelsInObj = 0;
        int completedLevelsInObj = 0;
        foreach (Transform child in levelList) // Очищаем список уровней
        {
            Destroy(child.gameObject);
        }
        titleBiomeText.text = biome.nameBiome;
        foreach(var lvl in biome.levels)
        {
            totalLevelsInObj++;
            if (lvl.isCompletedLevel)
            {
                completedLevelsInObj++;
                levelButtonPrefab.GetComponent<Image>().sprite = completedLvlSprite;
            }
            else
            {
                levelButtonPrefab.GetComponent<Image>().sprite = sourceLvlSprite;
            }
            GameObject btnlvl = Instantiate(levelButtonPrefab, levelList);
            btnlvl.GetComponent<LevelButtonProcessing>().lvlDataProcessing(lvl);
        }
        countLvls.text = $"{completedLevelsInObj}/{totalLevelsInObj}";
    }
}
