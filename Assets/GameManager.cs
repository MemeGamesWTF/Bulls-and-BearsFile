using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Static instance for the singleton
    public static GameManager Instance { get; private set; }

    public int GameID = 0;

    public GameObject GameOverScreen, GameWinScreen, InfoScreen;
    public GameObject  Information;
    public bool GameState = false;
    public BasePlayer Player;

    private ScoreObj Score;
    public GameObject[] face;
    public GameObject[] face2;
    public Transform[] spawnPoints;
  
    public float spawnDelay = 1f;
    public float spawnDelay2 = 1f;
   
  



    private Vector2 tapPosition;
    public ParticleSystem poff;
    private List<GameObject> spawnedFaces = new List<GameObject>();
    

    public GameObject OldGame;
    public GameObject NewGame;

    public Text ScoreText;
    private int currentScore;

    public AudioSource GameOverReal;
    public AudioSource GameOverCartoon;
    public AudioSource Coin;
    public AudioSource Tap;
    public AudioSource UISound;
    public float timeScale = 1.0f;
    private float scoreTimer = 0f;
    public float scrollSpeed = 2f;
    private float scoreInterval = 2f;
    [Tooltip("Array of background transforms (stacked vertically). Order doesn't matter.")]
    public Transform[] backgrounds;
    private Vector3[] backgroundStartPositions;
    private float backgroundHeight;

    [DllImport("__Internal")]
    private static extern void SendScore(int score, int game);

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Another instance of GameManager already exists. Destroying this instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    private void Start()
    {
        InfoScreen.SetActive(true);
        StartCoroutine(SpawnFacesCoroutine());
        
        if (backgrounds == null || backgrounds.Length < 2)
        {
            Debug.LogError("Please assign at least two background transforms.");
            enabled = false;
            return;
        }

        // Assumes all backgrounds have the same height (from SpriteRenderer bounds)
        SpriteRenderer sr = backgrounds[0].GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("Background objects need a SpriteRenderer component.");
            enabled = false;
            return;
        }

        backgroundHeight = sr.bounds.size.y;

        backgroundStartPositions = new Vector3[backgrounds.Length];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgroundStartPositions[i] = backgrounds[i].position;
        }

    }

    void Update()
    {
        if (!GameState)
            return;

        scoreTimer += Time.deltaTime;
        if (scoreTimer >= scoreInterval)
        {
            AddScore(); // Increase score at regular intervals
            scoreTimer = 0f;
        }


        timeScale += Time.deltaTime / 150f;
        timeScale = Mathf.Clamp(timeScale, 0f, 2f);
        Time.timeScale = timeScale;


        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].position += Vector3.down * scrollSpeed * Time.deltaTime;

            // If the background has moved fully off-screen below
            if (backgrounds[i].position.y <= -backgroundHeight)
            {
                // Find the highest background
                float highestY = backgrounds[0].position.y;
                for (int j = 1; j < backgrounds.Length; j++)
                {
                    if (backgrounds[j].position.y > highestY)
                        highestY = backgrounds[j].position.y;
                }

                // Reposition this background above the highest
                Vector3 newPos = backgrounds[i].position;
                newPos.y = highestY-1f + backgroundHeight;
                backgrounds[i].position = newPos;
            }
        }
    

    }

    IEnumerator SpawnFacesCoroutine()
    {
        while (true)
        {
            if (GameState)
            {
                int randomIndex = Random.Range(0, face.Length);
                GameObject randomFace = face[randomIndex];

                // Ensure there is at least one spawn point available
                if (spawnPoints.Length > 0)
                {
                  //  Shoot.Play();

                    int spawnIndex = Random.Range(0, spawnPoints.Length);
                    GameObject spawnedFace = Instantiate(randomFace, spawnPoints[spawnIndex].position, Quaternion.identity);
                    spawnedFaces.Add(spawnedFace);
                }
                else
                {
                    // Debug.LogWarning("No spawn points assigned!");
                }
                yield return new WaitForSeconds(spawnDelay);
            }
            else
            {
                yield return null;
            }
        }
    }


   


    public void PlayGame()
    {
        GameState = true;
        Time.timeScale = 1;
        Information.SetActive(false);
    }
    public void PauseGame()
    {

        Information.SetActive(true);
        StartCoroutine(Pause());
    }
    public void uiSound()
    {
        UISound.Play();
    }

    IEnumerator Pause()
    {
      GameState = false;

        // Wait for a specified duration (adjust the delay as needed)
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 0;


    }


    


   
    public void GameWin()
    {
        GameState = false;
        GameWinScreen.SetActive(true);
        Debug.Log(currentScore);
        SendScore(currentScore, 139);
    }

    public void GameOver()
    {
       
        GameState = false;
        GameOverScreen.SetActive(true);
        Debug.Log(currentScore);
        SendScore(currentScore, 139);
    }

    public void GameResetScreen()
    {
        timeScale = 1.0f;
        Time.timeScale = timeScale;
        DestroySpawnedFaces();
        ScoreText.text = "0";
        Score.score = 0;
        currentScore = 0;
        InfoScreen.SetActive(false);
        GameOverScreen.SetActive(false);
        GameWinScreen.SetActive(false);

        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].position = backgroundStartPositions[i];
        }


        GameState = true;
        Player.Reset();
    }

    public void AddScore()
    {


        if (int.TryParse(ScoreText.text, out currentScore))
        {
            currentScore += 10;
            ScoreText.text = currentScore.ToString();
        }
        else
        {

            ScoreText.text = "0";
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bear"))
        {
            collision.gameObject.SetActive(false);
            GameOver();
            
        }
    }


    public void AddScore(float f)
    {
        Score.score += f;
    }

    public void DestroySpawnedFaces()
    {
        foreach (GameObject face in spawnedFaces)
        {
            if (face != null)
            {
                Destroy(face);
            }
        }
        spawnedFaces.Clear(); // Clear the list so it doesn't hold references to destroyed objects
    }



    //HELPER FUNTION TO GET SPAWN POINT
    public Vector2 GetRandomPointInsideSprite(SpriteRenderer SpawnBounds)
    {
        if (SpawnBounds == null || SpawnBounds.sprite == null)
        {
            Debug.LogWarning("Invalid sprite renderer or sprite.");
            return Vector2.zero;
        }

        Bounds bounds = SpawnBounds.sprite.bounds;
        Vector2 randomPoint = new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );

        // Transform local point to world space
        return SpawnBounds.transform.TransformPoint(randomPoint);
    }


    public struct ScoreObj
    {
        public float score;
    }
}
