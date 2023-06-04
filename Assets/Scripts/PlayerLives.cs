using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using StarterAssets;

public class PlayerLives : MonoBehaviour
{
    [Header("Player Lives")]
    [SerializeField] public int lives = 3;
    [SerializeField] public bool isAlive = true;

    private StarterAssetsInputs inputs;


    [Header("UI")]
    [SerializeField] private GameObject captureText;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private TextMeshProUGUI livesCount;
    public static PlayerLives Instance;

    [Header("Respawn")]
    private bool respawn = false;
    [SerializeField] private GameObject respawnPoint;
    [SerializeField] private GameObject body;
    [SerializeField] private float spawnDelay = 3;
    [SerializeField] private float timer = 0;
    [SerializeField] private GameObject player;

    [Header("Audio")]
    private AudioSource src;
    [SerializeField] AudioClip caughtSound;


    private void Awake()
    {
        src = GetComponent<AudioSource>();
        Instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        inputs = player.GetComponent<StarterAssetsInputs>();

    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateLivesCount();
    }

    // Update is called once per frame
    void Update()
    {
        //If respawn is started
        if (respawn)
        {
            //Start a timer for when to respawn
            timer -= Time.deltaTime;

            //After timer
            if (timer <= 0)
            {
                //Respawn player
                player.gameObject.transform.position = respawnPoint.gameObject.transform.position;
                player.SetActive(true);
                captureText.SetActive(false);
                respawn = false;
            }
        }
    }

    public void FirstSpawn()
    {
        respawnPoint = GameObject.Find("RespawnPoint");
        if (respawnPoint != null)
        {
            player.gameObject.transform.position = respawnPoint.gameObject.transform.position;
        }
    }

    //When the player gets caught
    public void PlayerCaught()
    {
        if (!respawn && isAlive)
        {
            //Deduct lives count
            lives--;

            //If no more lives left player is killed
            if (lives <= 0)
            {
                isAlive = false;
                Instantiate(body, player.transform.position, player.transform.rotation, player.transform.parent.transform);
                src.PlayOneShot(caughtSound, 0.8f);

                //Disable pausing
                MenuController.Instance.pausable = false;
                Time.timeScale = 0;

                //Hide in game UI
                GameObject[] ui = GameObject.FindGameObjectsWithTag("GameUI");
                foreach (GameObject i in ui)
                {
                    i.SetActive(false);
                }

                //Destroy the player and display game over screen
                Destroy(GameObject.FindGameObjectWithTag("Player"));
                gameOverText.SetActive(true);
            }

            //If player still has lives remaining, begin respawning and display captured text
            if (lives > 0)
            {
                Instantiate(body, player.transform.position, player.transform.rotation, player.transform.parent.transform);
                src.PlayOneShot(caughtSound, 0.8f);
                UpdateLivesCount();
                player.SetActive(false);
                captureText.SetActive(true);
                timer = spawnDelay;
                //Reset player movement inputs
                inputs.move = Vector2.zero;
                inputs.sprint = false;
                inputs.jump = false;
                respawn = true;
            }
        }
    }


    //Update the lives text UI
    public void UpdateLivesCount()
    {
        livesCount.text = "Lives: " + lives;
    }
}
