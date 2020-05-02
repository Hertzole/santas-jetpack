using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    //[HideInInspector]
    public bool menu = true, tutorial = false, countdown = false, started = false, gameover = false, settings = false, gameJolt = false;

    public AudioClip continueSound;

    Animator uiAnim;

    public AudioMixer masterMixer;

    Text gpText, versionText, paText;

    GameObject snowParticles, trophiesButton, loginButton;

    private static MenuManager instance;

    public static MenuManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<MenuManager>();
            }
            return instance;
        }
    }

    public Sprite soundOnSprite, soundOffSprite, musicOnSprite, musicOffSprite;

    Image soundToggle, musicToggle;

    // Use this for initialization
    void Start()
    {
        uiAnim = GameObject.Find("GameUI").GetComponent<Animator>();
        gpText = GameObject.Find("GoldenPresentText").GetComponent<Text>();
        versionText = GameObject.Find("VersionText").GetComponent<Text>();
        paText = GameObject.Find("PlayingAsText").GetComponent<Text>();

        versionText.text = "Version " + Application.version;
        paText.text = "Playing as Guest";

        loginButton = GameObject.Find("GameJoltLogin");
        trophiesButton = GameObject.Find("GameJoltTrophies");

        soundToggle = GameObject.Find("SoundToggle").GetComponent<Image>();
        musicToggle = GameObject.Find("MusicToggle").GetComponent<Image>();

#if UNITY_WEBGL  
        loginButton.gameObject.SetActive(false);
#endif

        if (PlayerPrefs.GetInt("SFXVolume") == 0)
        {
            masterMixer.SetFloat("SFXVolume", -80);
            soundToggle.sprite = soundOffSprite;
        }
        else
        {
            masterMixer.SetFloat("SFXVolume", 0);
            soundToggle.sprite = soundOnSprite;
        }

        if (PlayerPrefs.GetInt("MusicVolume") == 0)
        {
            masterMixer.SetFloat("MusicVolume", -80);
            musicToggle.sprite = musicOffSprite;
        }
        else
        {
            masterMixer.SetFloat("MusicVolume", 0);
            musicToggle.sprite = musicOnSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameJolt_CheckLogIn();

        if (Input.GetButtonDown("Confirm"))
        {
            if (!settings && !gameJolt)
            {
                if (gameover)
                {
                    AudioUtlity.Instance.PlaySoundEffect(continueSound, transform.position, 1);
                    uiAnim.SetTrigger("Restart");
                    GameController.Instance.Invoke("RestartGame", 0.5f);
                }

                if (tutorial)
                {
                    AudioUtlity.Instance.PlaySoundEffect(continueSound, transform.position, 1);
                    tutorial = false;
                    uiAnim.SetTrigger("Countdown");
                    GameController.Instance.Invoke("StartGame", 3);
                }

                if (menu)
                {
                    AudioUtlity.Instance.PlaySoundEffect(continueSound, transform.position, 1);
                    menu = false;
                    uiAnim.SetTrigger("Tutorial");
                    tutorial = true;
                }
            }
        }

        if (Input.GetButtonDown("Toggle Sound"))
        {
            if (!gameJolt)
            {
                ToggleSound();
            }
        }

        if (Input.GetButtonDown("Toggle Music"))
        {
            if (!gameJolt)
            {
                ToggleMusic();
            }
        }
    }

    public void SetGameOver()
    {
        gameover = true;
    }

    public void ToggleSound()
    {
        PlayerPrefs.SetInt("SFXVolume", PlayerPrefs.GetInt("SFXVolume") == 0 ? 1 : 0);

        if (PlayerPrefs.GetInt("SFXVolume") == 0)
        {
            masterMixer.SetFloat("SFXVolume", -80);
            soundToggle.sprite = soundOffSprite;
        }
        else
        {
            masterMixer.SetFloat("SFXVolume", 0);
            soundToggle.sprite = soundOnSprite;
        }
    }

    public void ToggleMusic()
    {
        PlayerPrefs.SetInt("MusicVolume", PlayerPrefs.GetInt("MusicVolume") == 0 ? 1 : 0);

        if (PlayerPrefs.GetInt("MusicVolume") == 0)
        {
            masterMixer.SetFloat("MusicVolume", -80);
            musicToggle.sprite = musicOffSprite;
        }
        else
        {
            masterMixer.SetFloat("MusicVolume", 0);
            musicToggle.sprite = musicOnSprite;
        }
    }

    public void SetUITrigger(string triggerName)
    {
        uiAnim.SetTrigger(triggerName);
    }

    public void SetGoldenPresentText(string text, Color textColor)
    {
        gpText.text = text;
        gpText.color = textColor;
    }

    public void SetSoundVolume(float volume)
    {
        masterMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        masterMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetMasterVolume(float volume)
    {
        masterMixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void GameJolt_LoginWindow()
    {
        //TODO: Show GameJolt login.
    }

    public void GameJolt_Leaderboards()
    {
        //TODO: Show GameJolt leaderboards.
    }

    public void GameJolt_Trophies()
    {
        //TODO: Show GameJolt trophies.
    }

    public void ToggleGameJolt(bool gameJoltState)
    {
        // Stop here because GameJolt is not present here.
        //gameJolt = gameJoltState;
    }

    public void ToggleSettings(bool settingsState)
    {
        settings = settingsState;
    }

    public void GameJolt_CheckLogIn()
    {
        //TODO: Check if GameJolt user is signed in.
        bool signedIn = false;

        trophiesButton.SetActive(signedIn);

        if (signedIn)
        {
            //TODO: Replace user with GameJolt username.
            paText.text = "Playing as user";
        }
        else
        {
            paText.text = "Playing as Guest";
        }
    }
}
