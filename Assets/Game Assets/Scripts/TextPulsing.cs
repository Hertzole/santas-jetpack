using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextPulsing : MonoBehaviour {

	public float tickDelay;

	Text textComp;

	// Use this for initialization
	void Start () {
        textComp = GetComponent<Text>();
		StartCoroutine(PulseTheText());
	}
	
	IEnumerator PulseTheText()
	{
		while(true)
		{
            textComp.color = Color.white;
			yield return new WaitForSeconds(tickDelay);
            textComp.color = Color.clear;
			yield return new WaitForSeconds(tickDelay);
		}
	}
}
