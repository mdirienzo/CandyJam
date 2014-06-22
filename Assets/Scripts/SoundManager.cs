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
    private bool[] wasPlaying;

	// Use this for initialization
	void Start () {
		instance = this;
        audioSource = GetComponents<AudioSource> ();
        wasPlaying = new bool[audioSource.Length];
		//Start the day music at start of game
		audioSource[0].clip = dayMusic;
		audioSource[1].clip = nightMusic;
	}

	// Update is called once per frame
	void Update () {
		if (GameManager.instance.timeUntilDark < 5.0f && !fadingToDark) {
			StartFadeToDark();
			Debug.Log ("Starting Fade to Dark");
		}
	}

    public void pause() {
        for (int i = 0; i < this.audioSource.Length; ++i) {
            AudioSource source = this.audioSource[i];
            this.wasPlaying[i] = source.isPlaying;
            if (source.isPlaying) source.Pause();
        }
    }

    public void unpause() {
        for (int i = 0; i < this.audioSource.Length; ++i) {
            AudioSource source = this.audioSource[i];
            if (this.wasPlaying[i]) source.Play();
        }
    }

    public void beginRound() {
        this.endRound();
        audioSource[0].Play();
    }

    public void endRound() {
        this.StopAllCoroutines();
        this.fadingToDark = false;
        audioSource[0].Stop();
        audioSource[0].volume = 1.0f;
        audioSource[1].Stop();
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
