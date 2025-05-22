using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BasePlayer : MonoBehaviour
{
    [Header("Effects")]
    public ParticleSystem hit;
    public AudioSource coin;

    [Header("Bounds")]
    public float minX = -5f;
    public float maxX = 5f;

    [Header("Physics Movement")]
    public float acceleration = 50f;
    public float maxSpeed = 5f;
    // Set Linear Drag in the Rigidbody2D component (try ~5�10 to start)

    Rigidbody2D rb;

    [Tooltip("Constant upward speed")]
    public float forwardSpeed = 2f;

    Vector2 startPosition;
    float startRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
        startRotation = rb.rotation;
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.GameState)
            return;

        // 1) Horizontal input-driven movement
        float moveDir = GetHoldDirection();            // –1, 0 or +1
        rb.AddForce(Vector2.right * moveDir * acceleration);

        // 2) Clamp horizontal speed
        float vx = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);

        // 3) Enforce constant forward (Y) speed
        float vy = forwardSpeed;

        rb.linearVelocity = new Vector2(vx, vy);

        // 4) Clamp position to horizontal bounds
        Vector2 pos = rb.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        rb.position = pos;
    }

    private float GetHoldDirection()
    {
        if (Input.GetMouseButton(0))
            return (Input.mousePosition.x < Screen.width * 0.5f) ? -1f : 1f;

        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began ||
                t.phase == TouchPhase.Moved ||
                t.phase == TouchPhase.Stationary)
                return (t.position.x < Screen.width * 0.5f) ? -1f : 1f;
        }
        return 0f;
    }
    public void GameOver()
    {
        GameManager.Instance.GameOver();
    }

    public void Reset()
    {
        rb.position = startPosition;
        rb.rotation = startRotation;

        // 2) Clear out any velocity or momentum
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // e.g. snap back to center lane
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
    }

  /*  private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bear"))
        {
            coin.Play();
            hit.Play();
            collision.gameObject.SetActive(false);
            // GameManager.Instance.AddScore();
        }
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hit.Play();
            GameOver();
        }
    }
}

    // � your GameOver(), Reset(), collision methods �

