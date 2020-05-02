using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    Text randomMessageText;
    Animator anim;

    float changeLevelTimer;
    float skipTimer;

    public bool skippable = true;
    public float skipTime = 1f;
    public float changeLevelDelay = 0.5f;

    public string[] randomMessages;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        randomMessageText = GameObject.Find("RandomMessage").GetComponent<Text>();
        randomMessageText.text = randomMessages[Random.Range(0, randomMessages.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
        {
            changeLevelTimer += 1 * Time.deltaTime;
        }

        if (changeLevelTimer >= changeLevelDelay)
        {
            //Application.LoadLevel(Application.loadedLevel + 1); // OLD
            // New code
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        skipTimer += 1 * Time.deltaTime;

        if (Input.GetButtonDown("Quit"))
        {
            if (skipTimer >= skipTime && skippable)
            {
                //Application.LoadLevel(Application.loadedLevel + 1); // OLD
                // New code
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
            }
        }
    }
}
