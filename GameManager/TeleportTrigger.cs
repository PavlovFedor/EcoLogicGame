using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    public Transform teleportTarget; // Объект, к которому будем телепортировать
    public GameObject player; // Игрок, которого будем телепортировать
    
    // При вхождении в триггер
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что вошел именно игрок (можно проверить по тегу или по наличию компонента)
        if (other.gameObject == player)
        {
            TeleportPlayer();
        }
    }
    
    private void TeleportPlayer()
    {
        if (player != null && teleportTarget != null)
        {
            // Телепортируем игрока к цели
            player.transform.position = teleportTarget.position;
            
            // Если нужно, также можно изменить вращение игрока
            player.transform.rotation = teleportTarget.rotation;
            
            Debug.Log("Player teleported to " + teleportTarget.name);
        }
        else
        {
            Debug.LogWarning("Player or Teleport Target is not assigned!");
        }
    }
}