using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ProceduralMapGenerationCommon proceduralMapGenerationCommon;
    public HorseRecursion horseRecursion;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        proceduralMapGenerationCommon.SpawnMap();
        horseRecursion.SpawnHorse();
    }

    // Update is called once per frame
    void Update()
    {
        horseRecursion.MoveHorse();
    }
}
