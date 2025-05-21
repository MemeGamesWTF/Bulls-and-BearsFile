using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class BasePlayer : MonoBehaviour
{
    [Header("Lane Settings")]
    // X positions for each lane
    public float[] laneXPositions = new float[] { -2f, 0f, 2f };
    // How fast to interpolate between lanes
    public float laneSwitchSpeed = 10f;

    private int currentLane = 1;    // start in middle lane (index 1)
    private Vector3 targetPosition;
    public float forwardSpeed = 5f;

    public ParticleSystem hit;

    public AudioSource coin;

    void Start()
    {
        // Initialize car to middle lane
        currentLane = 1;
        targetPosition = new Vector3(laneXPositions[currentLane], transform.position.y, transform.position.z);
        transform.position = targetPosition;
    }

    void Update()
    {
        if (!GameManager.Instance.GameState)
            return;


        HandleInput();
        MoveToLane();
        MoveForward();
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.up * forwardSpeed * Time.deltaTime); // Moves upward on Y-axis
    }

    private void HandleInput()
    {
        // Move left
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            currentLane = Mathf.Clamp(currentLane - 1, 0, laneXPositions.Length - 1);
            targetPosition = new Vector3(laneXPositions[currentLane], transform.position.y, transform.position.z);
        }

        // Move right
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            currentLane = Mathf.Clamp(currentLane + 1, 0, laneXPositions.Length - 1);
            targetPosition = new Vector3(laneXPositions[currentLane], transform.position.y, transform.position.z);
        }
    }

    private void MoveToLane()
    {
        // Smoothly interpolate position towards the target lane
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * laneSwitchSpeed
        );
    }

    public void MoveLeft() { 
        ChangeLane(-1);
        GameManager.Instance.Tap.Play();
    }
    public void MoveRight() { 
        ChangeLane(+1);
        GameManager.Instance.Tap.Play();
    }

    private void ChangeLane(int delta)
    {
        currentLane = Mathf.Clamp(currentLane + delta, 0, laneXPositions.Length - 1);
        targetPosition = new Vector3(laneXPositions[currentLane], transform.position.y, transform.position.z);
    }

    public void GameOver()
    {
        GameManager.Instance.GameOver();
    }

    public void Reset()
    {
        // Reset to middle lane on restart, if desired
        currentLane = 1;
        float startY = -2.38f;
        targetPosition = new Vector3(laneXPositions[currentLane], startY, transform.position.z);
        transform.position = targetPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bear"))
        {
            coin.Play();
            hit.Play();
            collision.gameObject.SetActive(false);
            //GameManager.Instance.AddScore
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameOver();
        }
    }
}
