using UnityEngine;

public class WorldRebaser : MonoBehaviour
{
   /* public float threshold = 10f; // Расстояние, после которого происходит смещение

    private Vector3 lastPlayerPosition;
    private GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (player.transform.position.magnitude > threshold)
        {
            RebaseWorld();
        }
    }

    void RebaseWorld()
    {
        Vector3 offset = player.transform.position;
        player.transform.position -= offset;

        foreach (Transform obj in FindObjectsOfType<Transform>())
        {
            if (obj != player.transform)
            {   
                //obj.position -= offset;
                Debug.Log($"Произошло смещение мира на {threshold}");
            }
        }
    }*/
}