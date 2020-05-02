using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [System.Serializable]
    public class SpawnGroup
    {
        public string displayName;
        public float minSpawnTime;
        public float maxSpawnTime;
        public float minYPos;
        public float maxYPos;
        public enum Type { Prop, Enemy, Pickup };
        public Type type;
        public GameObject[] prefabs;
    }

    public SpawnGroup[] spawnGroups;
    GameObject groupsHolder;
    GameObject groundGroup;
    public GameObject groundPrefab;
    GameObject previousGround;

    public bool started = false, gameover = false;

    PlayerController pc;

    public ParticleSystem deathEffect;
    public ParticleSystem looseLifePartiles;
    Transform cameraHolder;
    CameraShake cs;
    public Renderer background;

    public float points;
    public float difficultyIncreaseRate = 0.001f;

    Text pointsText;
    Text endPointsText;
    Text endHighscoreText;

    Slider gpSlider;

    MenuManager menu;

    public AudioMixer masterMixer;

    public GameObject musicPrefab;

    public AudioClip gameOverSound;
    public AudioClip continueSound;

    public float difficulty = 1;

    private static GameController instance;

    public static GameController Instance { get { if (!instance) { instance = FindObjectOfType<GameController>(); } return instance; } }

    public float backgroundSpeed;

    public Vector2 backgroundOffset;
    float backgroundFloat = 0;
    [HideInInspector]
    public float oldDifficulty;

    bool debugMode = false;

    float goldenSliderCurrentDuration;
    public bool increaseEnemies;
    public bool increasePickups;

    // Use this for initialization
    void Start()
    {
        if (!GameObject.Find("Anti-Cheat Toolkit Detectors"))
        {
            //TODO: Start anti cheat detection.
        }

        pc = FindObjectOfType<PlayerController>();
        menu = GetComponent<MenuManager>();
        pc.canControl = false;

        groupsHolder = new GameObject("Spawn Groups");
        groundGroup = new GameObject("Ground Group");

        cameraHolder = GameObject.Find("CameraHolder").transform;
        cs = Camera.main.GetComponent<CameraShake>();
        pointsText = GameObject.Find("PointsText").GetComponent<Text>();
        endPointsText = GameObject.Find("EndPointsText").GetComponent<Text>();
        endHighscoreText = GameObject.Find("EndHighscoreText").GetComponent<Text>();

        gpSlider = GameObject.Find("GoldenPresentSlider").GetComponent<Slider>();


        previousGround = Instantiate(groundPrefab, new Vector3(-12, -5, 0), Quaternion.identity);
        previousGround.transform.SetParent(groundGroup.transform);

        if (!GameObject.FindGameObjectWithTag("Music"))
        {
            GameObject music = Instantiate(musicPrefab);
            DontDestroyOnLoad(music);
        }
    }

    public void OnCheaterDetected()
    {
        // Use int as a bool. 1 is true.
        PlayerPrefs.SetInt("Chearer", 1);
    }

    void ResetDifficulty()
    {
        difficulty = oldDifficulty;
    }

    void ResetEnemyMultplier()
    {
        increaseEnemies = false;
    }

    void ResetPickupsMultiplier()
    {
        increasePickups = false;
    }

    void Update()
    {
        if (previousGround)
        {
            if (Vector3.Distance(new Vector3(Camera.main.transform.position.x, -5, 0), previousGround.transform.position) < 15)
            {
                Vector3 spawnPos = new Vector3(previousGround.transform.Find("Connection").transform.position.x, -5, 0);
                previousGround = Instantiate(groundPrefab, spawnPos, Quaternion.identity);
                previousGround.transform.SetParent(groundGroup.transform);

            }
        }

        if (started && !gameover)
        {
            if (!increaseEnemies)
            {
                points += 1 * difficulty * Time.deltaTime;
            }
            else
            {
                points += 3 * difficulty * Time.deltaTime;
            }
            difficulty += difficultyIncreaseRate * Time.deltaTime;
            pointsText.text = "SCORE: " + points.ToString("0");

            if (Input.GetButtonDown("Quit"))
            {
                PlayerDie();
            }
        }

        if (started)
        {
            backgroundFloat += 1 * backgroundSpeed * difficulty * Time.deltaTime;
            backgroundOffset = new Vector2(backgroundFloat, 0);
            background.material.mainTextureOffset = backgroundOffset;
        }

        goldenSliderCurrentDuration -= 1 * Time.deltaTime;
        gpSlider.value = goldenSliderCurrentDuration;

        if (goldenSliderCurrentDuration > 0)
        {
            gpSlider.gameObject.SetActive(true);
        }
        else
        {
            gpSlider.gameObject.SetActive(false);
        }

        if (!started && !gameover)
        {
            if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.N))
            {
                pc.GetComponent<SpriteAnimator>().highres = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    void StartGame()
    {
        started = true;
        pc.canControl = true;
        foreach (SpawnGroup group in spawnGroups)
        {
            StartCoroutine(GroupSpawner(group.displayName, group.minSpawnTime, group.maxSpawnTime, group.minYPos, group.maxYPos, group.prefabs, group.type));
        }
    }

    void RestartGame()
    {
        //Application.LoadLevel(Application.loadedLevel); OLD
        // New code
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void FixedUpdate()
    {
        if (started)
        {
            cameraHolder.Translate(Vector3.right * 5 * difficulty * Time.deltaTime);
        }
    }

    public void AddPoints(float amount)
    {
        points += amount;
    }

    public void PlayerLooseLife()
    {
        cs.Shake(0.2f, 0.1f);
        ParticleSystem parts = Instantiate(looseLifePartiles, pc.transform.position, Quaternion.identity);
        Destroy(parts.gameObject, 2);
    }

    public void SetGoldenSlider(float duration)
    {
        goldenSliderCurrentDuration = duration;
        gpSlider.maxValue = duration;
    }

    public void PlayerDie()
    {
        float endPoints = points;

        cs.Shake(0.3f, 0.4f);
        gameover = true;
        Destroy(pc.gameObject);
        ParticleSystem parts = Instantiate(deathEffect, pc.transform.position, Quaternion.identity);
        Destroy(parts.gameObject, 2);
        endPointsText.text = "SCORE: " + endPoints.ToString("0");
        menu.SetUITrigger("GameOver");
        menu.gameover = true;
        AudioUtlity.Instance.PlaySoundEffect(gameOverSound, transform.position, 1);

        if (endPoints > PlayerPrefs.GetFloat("Highscore"))
        {
            PlayerPrefs.SetFloat("Highscore", endPoints);
            endHighscoreText.text = "<color=green>[NEW!]</color> HIGHSCORE: " + PlayerPrefs.GetFloat("Highscore").ToString("0");
        }
        else
        {
            endHighscoreText.text = "HIGHSCORE: " + PlayerPrefs.GetFloat("Highscore").ToString("0");
        }

        //Unlock "Bronze Scores"
        if (endPoints >= 150)
        {
            UnlockTrophy(0);
        }

        //Unlock "Silver Scores"
        if (endPoints >= 700)
        {
            UnlockTrophy(0);
        }

        //Unlock "Gold Scores"
        if (endPoints >= 2500)
        {
            UnlockTrophy(0);
        }

        //Unlock "Platinum Scores"
        if (endPoints >= 7500)
        {
            UnlockTrophy(0);
        }

        UploadToLeaderboard((int)endPoints);
    }

    IEnumerator GroupSpawner(string groupName, float minSpawnTime, float maxSpawnTime, float minYPos, float maxYPos, GameObject[] prefab, SpawnGroup.Type type)
    {
        GameObject groupHolder = new GameObject(groupName);
        groupHolder.transform.SetParent(groupsHolder.transform);
        while (true)
        {
            float spawnTime = 0;

            if (type == SpawnGroup.Type.Prop)
            {
                spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
            }
            else if (type == SpawnGroup.Type.Pickup)
            {
                if (increasePickups)
                {
                    spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
                    spawnTime /= 4;
                }
                else
                {
                    spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
                }
            }
            else if (type == SpawnGroup.Type.Enemy)
            {
                if (increaseEnemies)
                {
                    spawnTime = Random.Range(minSpawnTime, maxSpawnTime) / 2;
                    spawnTime /= 2;
                }
                else
                {
                    spawnTime = Random.Range(minSpawnTime, maxSpawnTime) / 2;
                }
            }

            yield return new WaitForSeconds(spawnTime);

            Vector3 spawnPos = new Vector3(Camera.main.transform.position.x + 20, Random.Range(minYPos, maxYPos), 0);

            GameObject newObject = Instantiate(prefab[Random.Range(0, prefab.Length)], spawnPos, Quaternion.identity);
            newObject.transform.SetParent(groupHolder.transform);
        }
    }

    private void UnlockTrophy(int id)
    {
        //TODO: GameJolt API trophy unlock.
        //if (GameJolt.API.Manager.Instance.CurrentUser != null)
        //{
        //    GameJolt.API.Trophies.Get(id, (GameJolt.API.Objects.Trophy trophy) =>
        //    {
        //        if (trophy.Unlocked == false)
        //        {
        //            GameJolt.API.Trophies.Unlock(id);
        //        }
        //    });
        //}
    }

    private void UploadToLeaderboard(int score)
    {
        //TODO: GameJolt API upload to leaderboard.
        //if (GameJolt.API.Manager.Instance.CurrentUser != null)
        //{
        //    if (PlayerPrefs.GetInt("Cheater") == 0)
        //    {
        //        GameJolt.API.Scores.Add(score, score.ToString("0") + " Points", 0, "", (bool success) =>
        //        {
        //            Debug.Log(string.Format("Score Add {0}.", success ? "Successful" : "Failed"));
        //        });
        //    }
        //    else
        //    {
        //        GameJolt.API.Scores.Add(score, score.ToString("0") + " Points <color=red>(!)</color>", 0, "", (bool success) =>
        //        {
        //            Debug.Log(string.Format("Score Add {0}.", success ? "Successful" : "Failed"));
        //        });
        //    }
        //}
    }

    void OnGUI()
    {
        if (debugMode)
        {
            GUILayout.BeginVertical();
            GUILayout.Box("");
            GUILayout.Box("Difficulty: " + difficulty.ToString());
            GUILayout.Box("Highscore: " + PlayerPrefs.GetFloat("Highscore"));
            //GUILayout.Box(GameJolt.API.Manager.Instance.CurrentUser != null ? "GameJolt Logged In: True" : "GameJolt Logged In: False");
            //if (GameJolt.API.Manager.Instance.CurrentUser != null)
            //{
            //    GUILayout.Box("GameJolt Username: " + GameJolt.API.Manager.Instance.CurrentUser.Name);
            //    GUILayout.Box("GameJolt User ID: " + GameJolt.API.Manager.Instance.CurrentUser.ID);
            //}
            GUILayout.EndVertical();
        }
    }
}
