using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public int numPlayers;
	public GameObject playerPrefab;

	public static GameManager instance;

	public GameObject[] playerRefs;

	// Game Control Variables
	private float theDarkTime = 15;
	private float startTime;
    private GameObject sun;
    private float sunRotation = 0.0f;

	void Awake() {
		if (instance == null) {
				instance = this;
		} else {
				Debug.Log ("Only one copy of gamemanager allowed!");
		}
	}


	// Use this for initialization
	void Start() {
        this.sun = GameObject.FindWithTag("Sun");
		startTime = Time.time;
		if (playerPrefab == null) {
			Debug.LogError ("Player prefab not set!");
		} else {
			if(numPlayers < 1){
				Debug.LogError ("Players = 0! :(");
			}else{
				Debug.Log ("Spawning " + numPlayers + " players!");
				//spawn players based on size of map

				playerRefs = new GameObject[numPlayers];

				for(int i = 0; i < numPlayers; i++){
					playerRefs[i] = Instantiate(playerPrefab) as GameObject;
					playerRefs[i].transform.position = LevelManager.instance.centerOfMap;
					//playerRefs[i].transform.position+= (Vector3.back);
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
        float timeElapsed = Time.time - startTime;
        float leftUntilDark = theDarkTime - timeElapsed;

        if (leftUntilDark < theDarkTime / 2.0f && leftUntilDark > 0.0f) {
            float r = -110.0f * (1.0f - leftUntilDark / (theDarkTime / 2.0f));
            float dr = r - sunRotation;
            sunRotation = r;
            sun.transform.RotateAround(LevelManager.instance.trueCenterOfMap, Vector3.up, dr);
        }

        if (leftUntilDark < 0.0f && false /* we have not en-dark-ened yet */) {
			// Enter darkness mode
		}
	}


	void PlayerPicksUpKeyItem () {

	}
}
