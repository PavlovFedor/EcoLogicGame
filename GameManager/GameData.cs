using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int id;
    public string nameLevel;
    public string sceneName;
    public bool isCompletedLevel;
}

[System.Serializable]
public class Biome
{
    public int id;
    public string nameBiome;
    public bool isCompletedBiome;
    public List<Level> levels;
}

public class GameData
{
    public List<Biome> biomes;
}
