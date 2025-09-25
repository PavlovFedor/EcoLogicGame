using UnityEngine;
using System.Collections.Generic;

public class WindSwaySearch : WindSway
{
    [Header("Search Settings")]
    public string searchTag = "Grass";
    public float updateInterval = 0.3f;
    public float visibleRange = 30f;

    [System.Serializable]
    protected class PlantData
    {
        public Transform transform;
        public Vector3 initialPosition;
        public Quaternion initialRotation;
        public float randomOffset;
        public bool isGrass;
    }

    protected List<PlantData> plants = new List<PlantData>();
    private float _timer;
    private HashSet<GameObject> _processedChunks = new HashSet<GameObject>();

    protected override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= updateInterval)
        {
            _timer = 0;
            ApplySwayToVisiblePlants();
        }
    }

    public void ScanChunkForSwayObjects(GameObject chunk)
    {
        if (chunk == null || _processedChunks.Contains(chunk)) return;

        foreach (Transform child in chunk.transform)
        {
            if (child.CompareTag(searchTag))
            {
                AddPlant(child, true);
            }
        }
        _processedChunks.Add(chunk);
    }

    public void RemoveObjectsFromChunk(GameObject chunk)
    {
        if (chunk == null) return;
        
        plants.RemoveAll(p => p.transform != null && p.transform.IsChildOf(chunk.transform));
        _processedChunks.Remove(chunk);
    }

    private void ApplySwayToVisiblePlants()
    {
        if (Camera.main == null) return;

        Vector3 cameraPos = Camera.main.transform.position;
        
        foreach (var plant in plants)
        {
            if (plant.transform == null) continue;
            
            bool isVisible = Vector3.Distance(plant.transform.position, cameraPos) < visibleRange;
            if (isVisible)
            {
                float sway = GetSway(plant.randomOffset);
                if (plant.isGrass) sway += GetGrassSway(plant.randomOffset);
                ApplySway(plant.transform, plant.initialPosition, plant.initialRotation, sway, plant.isGrass);
            }
        }
    }

    public void AddPlant(Transform plantTransform, bool isGrass)
    {
        if (plantTransform == null || plants.Exists(p => p.transform == plantTransform)) 
            return;

        plants.Add(new PlantData
        {
            transform = plantTransform,
            initialPosition = plantTransform.localPosition,
            initialRotation = plantTransform.localRotation,
            randomOffset = Random.Range(0f, 100f),
            isGrass = isGrass
        });
    }

    public void ClearAllPlants()
    {
        plants.Clear();
        _processedChunks.Clear();
    }
}