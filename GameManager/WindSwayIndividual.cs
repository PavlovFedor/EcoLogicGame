using UnityEngine; 

public class WindSwayIndividual : WindSway
{
    [Header("Individual Settings")]
    public bool isGrass = false;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float randomOffset;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        randomOffset = Random.Range(0f, 100f);
    }

    protected override void Update()
    {
        float sway = GetSway(randomOffset);
        
        if (isGrass)
            sway += GetGrassSway(randomOffset);
        
        ApplySway(transform, initialPosition, initialRotation, sway, isGrass);
    }
}