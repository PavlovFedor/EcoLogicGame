using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    public ProceduralMapGenerationRun script;

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, какой объект вошел в триггер
        if (other.CompareTag("Player")) // Пример проверки по тегу
        {
            script.ShiftingMap();
            Debug.Log("Игрок вошел в триггер!");
        }
    }
}
