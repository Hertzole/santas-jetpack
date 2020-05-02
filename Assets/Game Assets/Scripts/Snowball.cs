using UnityEngine;
using System.Collections;

public class Snowball : MonoBehaviour {

    public ParticleSystem deathParticles;

	void OnTriggerEnter2D()
    {
        ParticleSystem parts = (ParticleSystem)Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(parts.gameObject, 2);
        Destroy(gameObject);
    }
}
