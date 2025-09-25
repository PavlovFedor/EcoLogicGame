using UnityEngine; 
using System.Collections.Generic;

public class WindSwayGroup : WindSway
{
    [Header("Group Settings")]
    public bool affectChildren = true;
    public string grassTag = "Grass"; // Тег для идентификации травы

    private List<PlantData> plants = new List<PlantData>();

    private class PlantData
    {
        public Transform transform;
        public Vector3 initialPosition;
        public Quaternion initialRotation;
        public float randomOffset;
        public bool isGrass;
    }

    void Start()
    {
        if (!affectChildren)
            return;

        foreach (Transform child in transform)
        {
            plants.Add(new PlantData
            {
                transform = child,
                initialPosition = child.localPosition,
                initialRotation = child.localRotation,
                randomOffset = Random.Range(0f, 100f),
                isGrass = child.CompareTag(grassTag)
            });
        }
    }

    protected override void Update()
    {
        foreach (var plant in plants)
        {
            float sway = GetSway(plant.randomOffset);
            
            if (plant.isGrass)
                sway += GetGrassSway(plant.randomOffset);
            
            ApplySway(plant.transform, plant.initialPosition, plant.initialRotation, sway, plant.isGrass);
        }
    }
}