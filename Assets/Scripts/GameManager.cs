using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public int numPlayers;
	public GameObject playerPrefab;

	public static GameManager instance;

	public GameObject[] playerRefs;


	// Game Control Variables
	private float timeUntilDarkness = 15;
	private float startTime;

	void Awake(){

		if (instance == null) {
				instance = this;
		} else {
				Debug.Log ("Only one copy of gamemanager allowed!");
		}
	}


	// Use this for initialization
	void Start () {

		startTime = Time.time;
		if (playerPrefab == null)
		{
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
					playerRefs[i].transform.position = LevelManager.instance.centerOfMap();
					//playerRefs[i].transform.position+= (Vector3.back);
				}
			}


		}



	}



	// Update is called once per frame
	void Update () {
		if ((Time.time - startTime) > timeUntilDarkness) {
			// Enter darkness mode
		}
	}


	void PlayerPicksUpKeyItem () {

	}
}
