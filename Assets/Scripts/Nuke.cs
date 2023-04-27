using UnityEngine;

public class Nuke : MonoBehaviour
{
    [SerializeField] GameObject nukeFX;
    [SerializeField] GameObject winScreen;
    
    private float timer = 5;
    private bool gameWon = false;

    //To pan the camera out during the win screen
    private Camera mainCamera;
    private Camera winCam;
    private float cameraPanY;
    private float cameraPanZ;

    //Audio
    private AudioSource src;
    public AudioClip clip;

    private void Awake()
    {
        src = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        mainCamera.enabled = true;
        winCam = GameObject.Find("WinCamera").GetComponent<Camera>();
        winCam.enabled = false;
        
    }


    // Update is called once per frame
    void Update()
    {
        if (gameWon)
        {
            //Countdown
            timer -= Time.deltaTime;

            //Pan out camera
            if (Vector3.Distance(winCam.transform.position, transform.position) < 100f)
            {
                cameraPanZ += 0.01f;
                cameraPanY += 0.01f;
                winCam.transform.position = new Vector3(winCam.transform.position.x, cameraPanY, cameraPanZ);//, winCam.transform.position.z);
            }

            //After countdown
            if (timer <= 0)
            {
                //Display win screen
                winScreen.SetActive(true);
            }
        }

    }


    //If player gets to the Nuke
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Disable pausing
            MenuController.Instance.pausable = false;
            
            //Play explosion
            src.PlayOneShot(clip);
            Instantiate(nukeFX, transform);

            //Swap to win camera for view of explosion
            mainCamera.enabled = false;
            winCam.enabled = true;
            cameraPanY = winCam.transform.position.y;
            cameraPanZ = winCam.transform.position.z;

            //Destroy all guards
            GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard");
            foreach (GameObject i in guards)
            {
                Destroy(i);
            }

            //Disable game UI
            GameObject[] ui = GameObject.FindGameObjectsWithTag("GameUI");
            foreach (GameObject i in ui)
            {
                i.SetActive(false);
            }

            gameWon = true;
        }
    }
}
