using UnityEngine;

public class Face : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float moveSpeed = 1f;
    private Rigidbody2D rb;
    public float fallForce = 5f;
    private bool isHit = false;

    public enum MoveDirection { Down = -1, Up = 1 }
    public MoveDirection direction = MoveDirection.Down;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // Move the object downward
        float dirValue = (float)direction;  // -1 for Down, +1 for Up
        transform.Translate(Vector2.up * dirValue * moveSpeed * Time.deltaTime);
    }

    

}
