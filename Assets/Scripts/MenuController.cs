using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


//Controls the Main and paused menu also handles loading scenes
public class MenuController : MonoBehaviour
{
    public static MenuController Instance;

    //Menu Screens
    public GameObject menu;
    public GameObject mainScreen;
    public GameObject helpScreen;
    public GameObject aboutScreen;
    public GameObject pauseScreen;

    //Loading
    public GameObject Loading;
    public Image LoadingBar;
    public TextMeshProUGUI progressTxt;

    public bool mainMenu;
    private bool paused;
    public bool pausable = true;
    public GameObject followCam;

    //Sound and Music
    public AudioClip buttonClick;
    public AudioClip backgroundMusic;
    public AudioSource clipPlayer;

    AsyncOperation scene;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        paused = false;
        ChangeScreen("Main");
    }

    // Update is called once per frame
    void Update()
    {
        //If player his the pause key and they are not at the menu screen and the game is pausable
        if (Input.GetKeyDown(KeyCode.Escape) && !mainMenu && pausable)
        {
            //If currently paused, unpause
            if (paused)
            {
                followCam.SetActive(true);
                paused = false;
                clipPlayer.PlayOneShot(buttonClick);
                ChangeScreen("Close Menu");
                menu.SetActive(false);
                Time.timeScale = 1;
            }
            //If currently unpaused, pause
            else if (!paused)
            {
                followCam.SetActive(false);
                paused = true;
                clipPlayer.PlayOneShot(buttonClick);
                menu.SetActive(true);
                Time.timeScale = 0;
                ChangeScreen("Pause");
            }
        }
    }

    public void ChangeScreen(string screen)
    {
        //Toggle different menu screens
        switch (screen)
        {
            case "Main":
                mainScreen.SetActive(true);
                helpScreen.SetActive(false);
                aboutScreen.SetActive(false);
                pauseScreen.SetActive(false);
                break;            
            
            case "Pause":
                mainScreen.SetActive(false);
                helpScreen.SetActive(false);
                aboutScreen.SetActive(false);
                pauseScreen.SetActive(true);
                break;

            case "Help":
                mainScreen.SetActive(false);
                helpScreen.SetActive(true);
                aboutScreen.SetActive(false);
                pauseScreen.SetActive(false);
                break;

            case "About":
                mainScreen.SetActive(false);
                helpScreen.SetActive(false);
                aboutScreen.SetActive(true);
                pauseScreen.SetActive(false);
                break;            
            
            case "Close Menu":
                mainScreen.SetActive(false);
                helpScreen.SetActive(false);
                aboutScreen.SetActive(false);
                pauseScreen.SetActive(false);
                break;

            default:
                break;
        }
    }

    //Play game
    public void PlayClicked()
    {
        clipPlayer.PlayOneShot(buttonClick);
        
        //Load game scene
        scene = SceneManager.LoadSceneAsync(1);
        ChangeScreen("Close Menu");
        Loading.SetActive(true);
        StartCoroutine(LoadingScreen());
    }    
    
    IEnumerator LoadingScreen()
    {
        float loadProgress = 0;
        //While loading update loading bar
        while (!scene.isDone)
        {
            //Loading bar
            loadProgress += scene.progress;
            LoadingBar.fillAmount = loadProgress;

            //progress percentage
            progressTxt.text = (int)(scene.progress * 100) + "%";
            yield return null;
        }
    }

    //Unpause game
    public void ResumeClicked()
    {
        paused = false;
        clipPlayer.PlayOneShot(buttonClick);
        ChangeScreen("Close Menu");
        menu.SetActive(false);
        Time.timeScale = 1;
    }

    //Open help screen
    public void HelpClicked()
    {
        clipPlayer.PlayOneShot(buttonClick);
        ChangeScreen("Help");
    }

    //Open about screen
    public void AboutClicked()
    {
        clipPlayer.PlayOneShot(buttonClick);
        ChangeScreen("About");
    }

    //Go back to main screen
    public void BackClicked()
    {
        clipPlayer.PlayOneShot(buttonClick);
        if (mainMenu)
        {
            ChangeScreen("Main");
        }
        else if(!mainMenu)
        {
            ChangeScreen("Pause");
        }
    }

    //Exit game
    public void QuitClicked()
    {
        clipPlayer.PlayOneShot(buttonClick);
        Application.Quit();
    }
}
