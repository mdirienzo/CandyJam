using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	public AudioClip dayMusic;
	public AudioClip nightMusic;
	public AudioClip death;
	public AudioClip thunder;
	public AudioClip laugh;

	public static SoundManager instance;
	private AudioSource[] audioSource;
	private bool fadingToDark = false;

	// Use this for initialization
	void Start () {
		instance = this;
		audioSource = GetComponents<AudioSource> ();
		//Start the day music at start of game
		audioSource[0].clip = dayMusic;
		audioSource[1].clip = nightMusic;
		if(!GameManager.instance.isDark) audioSource[0].Play ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.instance.timeUntilDark < 5.0f && !fadingToDark) {
			StartFadeToDark();
			Debug.Log ("Starting Fade to Dark");
		}
	
	}

	void StartFadeToDark(){

		audioSource [1].volume = 0;
		audioSource [1].Play ();
		fadingToDark = true;
		StartCoroutine(FadeToDark ());


	}

	public void Thunder(){

		audioSource [2].pitch = 1.0f;
		audioSource [2].PlayOneShot (thunder, 1.0f);

		}

	public void Laugh(){
		audioSource [2].pitch = 1.0f;
		audioSource [2].PlayOneShot (laugh, 1.0f);
		}

	public void Death(){
		audioSource [2].pitch = 1.0f;
		audioSource [2].PlayOneShot (death, 1.0f);
		this.Invoke ("Laugh", 1.5f);
	}

	IEnumerator FadeToDark(){
		while (audioSource[0].volume != 0) {
			audioSource[0].volume-=0.02f;
			audioSource[1].volume+=0.02f;
			yield return new WaitForSeconds (0.10f);
		}
	}
}
