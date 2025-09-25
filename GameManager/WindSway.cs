using UnityEngine;

public abstract class WindSway : MonoBehaviour
{
    [Header("Base Sway Settings")]
    public float swaySpeed = 1f;
    public float swayAmount = 0.1f;
    public float grassSwayMultiplier = 2f;

    protected float GetSway(float timeOffset)
    {
        float sway = Mathf.Sin((Time.time + timeOffset) * swaySpeed) * swayAmount;
        return sway;
    }

    protected float GetGrassSway(float timeOffset)
    {
        float grassSway = Mathf.PerlinNoise(Time.time * swaySpeed, timeOffset) * swayAmount * grassSwayMultiplier;
        return grassSway;
    }

    protected void ApplySway(Transform target, Vector3 initialPosition, Quaternion initialRotation, float sway, bool isGrass)
    {
        // Позиция
        target.localPosition = initialPosition + new Vector3(sway, 0, sway);

        // Вращение (только для не-травы)
        if (!isGrass)
        {
            Quaternion swayRotation = Quaternion.Euler(sway * 5f, 0, sway * 5f);
            target.localRotation = Quaternion.Lerp(target.localRotation, swayRotation, Time.deltaTime * swaySpeed);
        }
    }

    protected abstract void Update();
}