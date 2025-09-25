using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private const float tile_size = 1.0f;
    private const float tile_offset = 0.5f;
    //private float speed = 5.0f;

    public GameObject characterPrefab;
    private GameObject activeCharacter;

    public int positionX;
    public int positionY;

    public float moveDistance = 1.0f;
    public float moveDuration = 0.2f;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float moveTimer = 0.0f;
    void Start()
    {
        SpawnCharacter(MoveToTileCenter(positionX - 1, positionY - 1));
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            moveTimer += Time.deltaTime;
            activeCharacter.transform.position = Vector3.Lerp(startPosition, endPosition, moveTimer / moveDuration);
            if (moveTimer >= moveDuration)
            {
                isMoving = false;
                activeCharacter.transform.position = endPosition;
            }
            return;
        }
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        if (moveHorizontal != 0 || moveVertical != 0)
        {
            Vector3 direction = new Vector3(moveHorizontal, 0, moveVertical).normalized;
            Vector3 targetPosition = activeCharacter.transform.position + direction * moveDistance;
            StartMove(targetPosition);
        }
    }
    private void SpawnCharacter(Vector3 position)
    {
        activeCharacter = Instantiate(characterPrefab, position, Quaternion.identity);
    }
    private Vector3 MoveToTileCenter(int x, int y)
    {
        Vector3 origin = new Vector3(0, 1, 0);
        origin.x += (tile_size * x) + tile_offset;
        origin.z += (tile_size * y) + tile_offset;
        return origin;
    }
    private void StartMove(Vector3 targetPosition)
    {
        startPosition = activeCharacter.transform.position;
        endPosition = targetPosition;
        moveTimer = 0.0f;
        isMoving = true;
    }
}
