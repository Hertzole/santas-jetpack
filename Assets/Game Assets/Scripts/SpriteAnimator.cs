using UnityEngine;
using System.Collections;

public class SpriteAnimator : MonoBehaviour {

    public Sprite[] normalSprites;
    public Sprite[] highresSprites;

    public float animationSpeed;

    public bool highres;

    SpriteRenderer sr;

    public int spriteIndex = 0;

	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();

        StartCoroutine(Animate());
	}
	
	IEnumerator Animate()
    {
        while(true)
        {
            if(highres)
            {
                sr.sprite = highresSprites[spriteIndex];
                spriteIndex++;

                if(spriteIndex == highresSprites.Length)
                {
                    spriteIndex = 0;
                }

                yield return new WaitForSeconds(animationSpeed);
            }
            else
            {
                sr.sprite = normalSprites[spriteIndex];
                spriteIndex++;

                if (spriteIndex == normalSprites.Length)
                {
                    spriteIndex = 0;
                }

                yield return new WaitForSeconds(animationSpeed);
            }
        }
    }
}
