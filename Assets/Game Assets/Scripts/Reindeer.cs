using UnityEngine;
using System.Collections;

public class Reindeer : MonoBehaviour {

    public float maxSpeed;
    public float minSpeed;

    float speed;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed) + GameController.Instance.difficulty;
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
	}
}
