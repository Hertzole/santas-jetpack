using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class SnowmanAI : MonoBehaviour {

	public GameObject projectile;
	public float fireRate;

	GameObject player;

	Transform head;

	bool facingRight = true;

	bool playerIsOnTheRight = false;

	float nextFire;

	GameController gc;

	Animator anim;

    public AudioClip shootSoundEffect;

    void Awake()
	{
		anim = GetComponent<Animator>();

		player = GameObject.FindGameObjectWithTag("Player");

		head = transform.Find("SnowmanBottom/SnowmanBody/SnowmanHead");

		GameObject GameC = GameObject.FindGameObjectWithTag("GameController");
		gc = GameC.GetComponent<GameController>();

        
	}

	// Update is called once per frame
	void Update () {
		if(player != null)
		{
			nextFire += fireRate * GameController.Instance.difficulty * Time.deltaTime;

            anim.speed = GameController.Instance.difficulty;

            if (nextFire >= 1 && gc.started == true && gc.gameover == false && Vector3.Distance(player.transform.position, transform.position) < 12)
			{
				Shoot();
			}

			if(Vector3.Distance(player.transform.position, transform.position) < 12)
			{
				if(player.transform.position.x > transform.position.x)
				{
					playerIsOnTheRight = true;
					
					if(playerIsOnTheRight == true && facingRight == false)
					{
						Flip ();
					}
				} else 
				{
					playerIsOnTheRight = false;
					
					if(playerIsOnTheRight == false && facingRight == true)
					{
						Flip ();
					}
				}
			}

			/*Vector3 headDifference = player.transform.position - head.transform.position;
			headDifference.Normalize();
			
			float rotZ = Mathf.Atan2(headDifference.y, headDifference.x) * Mathf.Rad2Deg;
			if(playerIsOnTheRight == true)
			{
				head.transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 90);
			} else
			{
				head.transform.rotation = Quaternion.Euler(0f, 0f, rotZ + 180);
			}*/
		}
	}

	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void Shoot()
	{
		anim.SetTrigger("Shoot");
        AudioUtlity.Instance.PlaySoundEffect(shootSoundEffect, transform.position, 1);
		nextFire = 0;
		Vector3 dir = (player.transform.position - transform.position).normalized;

		GameObject proj = (GameObject)Instantiate(projectile, head.transform.position, Quaternion.identity);
		proj.GetComponent<Rigidbody2D>().AddForce(dir * 700);
	}
}
