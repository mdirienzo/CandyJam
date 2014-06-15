using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour {
	public int numPlayers;
	public GameObject playerPrefab;

	public static GameManager instance;

	public GameObject[] playerRefs;

	// Game Control Variables
	private float theDarkTime = 15;
	private float startTime;
    private GameObject ghostPrefab;
    private GameObject sun;
    private GameObject lightning;
    private float sunRotation = 0.0f;

    public bool isDark = false;
    public float timeUntilDark = 15;

	void Awake() {
		if (instance == null) {
				instance = this;
		} else {
				Debug.Log ("Only one copy of gamemanager allowed!");
		}
	}


	void Start() {
        this.sun = GameObject.FindWithTag("Sun");
        if (this.sun == null) {
            Debug.LogError("No sun!");
        }
        this.lightning = GameObject.FindWithTag("Lightning");
        if (this.lightning == null) {
            Debug.LogError("No lightning!");
        }

        this.ghostPrefab = (GameObject)Resources.Load("Ghost", typeof(GameObject));
        if (this.ghostPrefab == null) {
            Debug.LogError("No ghost!");
        }

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

    void OnGUI() {
        int secondsLeft = (int)this.timeUntilDark;
        if (secondsLeft >= 0) {
            GUI.Label(new Rect(10, 10, 100, 200), ":" + secondsLeft.ToString("D2"));
        }
    }

	// Update is called once per frame
	void Update () {
        float timeElapsed = Time.time - startTime;
        this.timeUntilDark = theDarkTime - timeElapsed;

        if (this.timeUntilDark < theDarkTime / 2.0f && this.timeUntilDark > 0.0f) {
            float r = -110.0f * (1.0f - this.timeUntilDark / (theDarkTime / 2.0f));
            float dr = r - sunRotation;
            sunRotation = r;
            sun.transform.RotateAround(LevelManager.instance.trueCenterOfMap, Vector3.up, dr);
        }

        if (this.timeUntilDark < 0.0f && !this.isDark) {
            this.isDark = true;
            this.Invoke("flashLightning1", 0.5f);
		}
	}

    void setLightning(float intensity) {
        this.lightning.GetComponent<Light>().intensity = intensity;
    }

    void dimLightning() {
        this.setLightning(0.0f);
    }

    void flashLightning1() {
        this.setLightning(2.0f);
        this.Invoke("dimLightning", 0.01f);
        this.Invoke("flashLightning2", 1.0f);
    }

    void flashLightning2() {
        this.spawnGhosts();
        this.setLightning(2.0f);
        this.Invoke("dimLightning", 0.1f);
        this.Invoke("placeDoor", 1.0f);
    }

    void spawnGhosts() {
        LevelManager level = LevelManager.instance;
        for (int i = 0; i < numPlayers * 2; ++i) {
            Vector3 pos;
            float nearestDistance;
            do {
                nearestDistance = 0.0f;
                pos = level.centerOfTile(level.tiles.random());
                GameObject nearestPlayer = null;
                foreach (GameObject player in playerRefs) {
                    float distance = Vector3.Distance(player.transform.position, pos);
                    if (nearestPlayer == null || distance < nearestDistance) {
                        nearestPlayer = player;
                        nearestDistance = distance;
                    }
                }
            } while (nearestDistance <= 1.0f);

            GameObject ghost = (GameObject)Instantiate(ghostPrefab, pos, Quaternion.identity);
            ghost.name = "Ghost " + i;
        }
    }

    void placeDoor() {
    }


	void PlayerPicksUpKeyItem () {

	}
}
