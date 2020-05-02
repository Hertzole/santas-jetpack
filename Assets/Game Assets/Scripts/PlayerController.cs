using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 8;
    public float jetpackForce = 10;

    [HideInInspector]
    public ParticleSystem jetpackParticles;

    bool facingRight = true;

    public bool canControl = false;

    public float maxJetpackPower = 100;
    public float jetpackPowerUsage = 2;
    [HideInInspector]
    public float currentJetpackPower = 100;

    GameController gc;
    Rigidbody2D rig;

    Slider jetpackFuelSlider;
    Text jetpackFuelText;

    Vector3 screenPos;

    public AudioClip pickupSound;
    public AudioClip looseLifeSound;

    public bool invincible = true;

    public int lives;

    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    Image heart1, heart2, heart3;

    public Color fullFuelColor = Color.green;
    public Color emptyFuelColor = Color.red;

    public Image fuelFillImage;

    SpriteRenderer sr;

    Vector3 originalPos;
    public AudioMixerGroup sfxMixGroup;

    // This is new code.
    ParticleSystem.EmissionModule jetpackEmission;

    // Use this for initialization
    void Start()
    {

        originalPos = transform.position;

        jetpackParticles = transform.Find("Jetpack Particles").gameObject.GetComponent<ParticleSystem>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        sr = GetComponent<SpriteRenderer>();

        jetpackFuelSlider = GameObject.Find("FuelSlider").GetComponent<Slider>();
        jetpackFuelText = GameObject.Find("FuelText").GetComponent<Text>();

        rig = GetComponent<Rigidbody2D>();

        if (canControl)
        {
            rig.gravityScale = 1;
        }
        else
        {
            rig.gravityScale = 0;
        }

        currentJetpackPower = maxJetpackPower;

        jetpackFuelSlider.maxValue = maxJetpackPower;

        jetpackFuelSlider.value = currentJetpackPower;
        jetpackFuelText.text = "Fuel - " + currentJetpackPower.ToString("0") + "%";

        heart1 = GameObject.Find("Heart1").GetComponent<Image>();
        heart2 = GameObject.Find("Heart2").GetComponent<Image>();
        heart3 = GameObject.Find("Heart3").GetComponent<Image>();

        UpdateLives(4);

        fuelFillImage.color = Color.Lerp(emptyFuelColor, fullFuelColor, currentJetpackPower / maxJetpackPower);

        // New code.
        jetpackEmission = jetpackParticles.emission;
    }

    // Update is called once per frame
    void Update()
    {
        // Noted 2/5/2020: THIS IS EXTREMELY BAD. FOR THE LOVE OF EVERYTHING, DON'T DO THIS
        // CACHE YOUR CAMERA!!
        screenPos = Camera.main.WorldToScreenPoint(transform.position);

        if (canControl)
        {
            rig.gravityScale = 1;

            if (Input.GetButton("Jetpack") && currentJetpackPower > 0f)
            {
                //jetpackParticles.enableEmission = true; OLD
                // New code
                {
                    jetpackEmission.enabled = true;
                }
                rig.AddForce(Vector2.up * jetpackForce * 100 * Time.deltaTime);

                currentJetpackPower -= jetpackPowerUsage * Time.deltaTime;
            }

            if (Input.GetButtonUp("Jetpack"))
            {
                //jetpackParticles.enableEmission = false; OLD
                // New code
                {
                    jetpackEmission.enabled = false;
                }
            }
        }
        else
        {
            rig.gravityScale = 0;
            if (gc.started)
            {
                //jetpackParticles.enableEmission = false; OLD
                // New code
                {
                    jetpackEmission.enabled = false;
                }
            }
            rig.velocity = Vector3.zero;
            transform.position = new Vector3(GameObject.Find("CameraHolder").transform.position.x + originalPos.x, originalPos.y, originalPos.z);
        }

        if (currentJetpackPower > maxJetpackPower)
        {
            currentJetpackPower = maxJetpackPower;
        }

        if (currentJetpackPower < 0)
        {
            currentJetpackPower = 0;
        }

        if (screenPos.x < 0 || screenPos.y < 0 || screenPos.x > Screen.width || screenPos.y > Screen.height)
        {
            if (gc.started)
            {
                LooseLife();
            }
        }

        if (gc.started)
        {
            jetpackFuelSlider.value = currentJetpackPower;
            jetpackFuelText.text = "Fuel - " + currentJetpackPower.ToString("0") + "%";
            fuelFillImage.color = Color.Lerp(emptyFuelColor, fullFuelColor, currentJetpackPower / maxJetpackPower);
        }
    }

    void LooseLife()
    {
        if (!invincible)
        {
            invincible = true;
            UpdateLives(lives -= 1);
            canControl = false;

            if (lives > 0)
            {
                gc.PlayerLooseLife();
                sr.enabled = false;
                Invoke("Respawn", 1);
                AudioUtlity.Instance.PlaySoundEffect(looseLifeSound, transform.position, 1);
            }
        }
    }

    void Respawn()
    {
        if (!facingRight)
        {
            Flip();
        }
        sr.enabled = true;
        GetComponent<Animator>().SetTrigger("Respawn");
        Invoke("Respawned", 1);
    }

    void Respawned()
    {
        invincible = false;
        canControl = true;
    }

    public void UpdateLives(int newAmount)
    {
        if (newAmount > 3)
        {
            lives = 3;
        }
        else
        {
            lives = newAmount;
        }

        if (currentJetpackPower <= 0)
        {
            lives = 0;
        }

        if (lives == 3)
        {
            heart1.sprite = fullHeartSprite;
            heart2.sprite = fullHeartSprite;
            heart3.sprite = fullHeartSprite;
        }
        else if (lives == 2)
        {
            heart1.sprite = emptyHeartSprite;
            heart2.sprite = fullHeartSprite;
            heart3.sprite = fullHeartSprite;
        }
        else if (lives == 1)
        {

            heart1.sprite = emptyHeartSprite;
            heart2.sprite = emptyHeartSprite;
            heart3.sprite = fullHeartSprite;
        }
        else if (lives == 0)
        {
            heart1.sprite = emptyHeartSprite;
            heart2.sprite = emptyHeartSprite;
            heart3.sprite = emptyHeartSprite;
            gc.PlayerDie();
        }
    }

    void FixedUpdate()
    {
        if (canControl == true)
        {
            float move = Input.GetAxis("Horizontal");
            rig.velocity = new Vector2(move * moveSpeed * GameController.Instance.difficulty, rig.velocity.y);

            if (move > 0 && !facingRight)
            {
                Flip();
            }
            else if (move < 0 && facingRight)
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Enemy")
        {
            if (!invincible)
            {
                LooseLife();
            }
        }

        if (other.tag == "Present")
        {
            currentJetpackPower += Random.Range(10, 20);
            Destroy(other.gameObject);

            AudioUtlity.Instance.PlaySoundEffect(pickupSound, transform.position, 1);

            gc.AddPoints(20);
        }

        if (other.tag == "GoldenPresent")
        {
            other.GetComponent<GoldenPresent>().GetRandomPowerup();
            Destroy(other.gameObject);
            gc.AddPoints(50);

            AudioUtlity.Instance.PlaySoundEffect(pickupSound, transform.position, 1);
        }
    }
}
