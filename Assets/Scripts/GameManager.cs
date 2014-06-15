using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public int numPlayers;
    public int keysRequired = 1;
    public int numGhosts = 1;
    public int ghostsPerPlayer = 1;
	public GameObject playerPrefab;
    public GameObject doorPrefab;
	public GameObject explodey;
    public Material closedDoorMaterial;
    public Material openDoorMaterial;

	public static GameManager instance;

	public List<GameObject> playerRefs;

	// Game Control Variables
	private float theDarkTime = 15;
	private float startTime;
    private GameObject ghostPrefab;
    private GameObject sun;
    private GameObject lightning;
    private GameObject keyPrefab;
    private float sunRotation = 0.0f;
    private int keysObtained = 0;

    public bool isDark = false;
    public bool isDoorOpen = false;
    public float timeUntilDark = 15;
    public bool winCondition = false;

	void Awake() {
		this.playerRefs = new List<GameObject> ();

        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("Only one copy of gamemanager allowed!");
        }

        if (this.doorPrefab == null) {
            Debug.LogError("No door prefab!");
        }
        if (this.closedDoorMaterial == null) {
            Debug.LogError("no closed door material!");
        }
        if (this.openDoorMaterial == null) {
            Debug.LogError("no open door material!");
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

        this.keyPrefab = (GameObject)Resources.Load("Key", typeof(GameObject));
        if (this.keyPrefab == null) {
            Debug.LogError("No key!");
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

				//playerRefs = new GameObject[numPlayers];

				for(int i = 0; i < numPlayers; i++){
					GameObject player = (GameObject)Instantiate(playerPrefab);
					playerRefs.Add (player);
					player.GetComponent<InputController>().axisName = "Player" + (i+1) + "_";
					//playerRefs[i] = Instantiate(playerPrefab) as GameObject;
					player.transform.position = LevelManager.instance.centerOfMap;

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


        if (this.playerRefs.Count == 0) {
            GUIStyle style = new GUIStyle();
            style.fontSize = 36;
            string text = "";
            if (this.winCondition) {
                style.normal.textColor = Color.green;
                text = "You win!";
            } else {
                style.normal.textColor = Color.red;
                text = "YOU LOSE!";
            }

            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50), text, style);
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

            float playerLightIntensity = 0.3f * (1.0f - this.timeUntilDark / (theDarkTime / 2.0f));
            foreach (GameObject light in GameObject.FindGameObjectsWithTag("PlayerLight")) {
                light.GetComponent<Light>().intensity = playerLightIntensity;
            }
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
        this.Invoke("placeDoorAndKeys", 1.0f);
    }

    void spawnGhosts() {
        LevelManager level = LevelManager.instance;
        for (int i = 0; i < numGhosts + numPlayers * this.ghostsPerPlayer; ++i) {
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

    void placeDoorAndKeys() {
        LevelManager level = LevelManager.instance;
        CardinalDir side = (CardinalDir)RNG.random.Next(4);
        TileLocation loc;
        Quaternion rot = Quaternion.identity;
        Vector3 scale = Vector3.one;
        do {
            switch (side) {
                case CardinalDir.NORTH: loc = new TileLocation(level.tiles.bound.r-1, RNG.random.Next(level.tiles.bound.c)); rot.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f); scale.y = -1.0f; break;
                case CardinalDir.EAST:  loc = new TileLocation(RNG.random.Next(level.tiles.bound.r), level.tiles.bound.c-1); scale.x = -1.0f; break;
                case CardinalDir.SOUTH: loc = new TileLocation(level.tiles.bound.r-1, RNG.random.Next(level.tiles.bound.c)); rot.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f); break;
                default:                loc = new TileLocation(RNG.random.Next(level.tiles.bound.r), level.tiles.bound.c-1); break;
            }
        } while (level.isTrapped(loc));

        Instantiate(this.doorPrefab, level.centerOfTile(loc), rot);
        this.closeDoor();

        for (int i = 0; i < keysRequired; ++i) {
            do {
                loc = level.tiles.random();
            } while (level.isTrapped(loc));
            Vector3 pos = level.centerOfTile(loc);
            GameObject key = (GameObject)Instantiate(this.keyPrefab, pos, Quaternion.identity);
            key.name = "Key " + i;
        }
    }

    void openDoor() {
        this.isDoorOpen = true;
        GameObject.FindWithTag("Door").GetComponent<MeshRenderer>().material = this.openDoorMaterial;
    }

    void closeDoor() {
        this.isDoorOpen = false;
        GameObject.FindWithTag("Door").GetComponent<MeshRenderer>().material = this.closedDoorMaterial;
    }

	public void keyPickedUp(GameObject keyObj) {
        this.keysObtained += 1;
        if (this.keysObtained >= this.keysRequired) {
            this.openDoor();
        }
        Destroy(keyObj);
	}

    public void doorTouched(GameObject player) {
        if (this.isDoorOpen) {
            this.winCondition = true;
            this.KillPlayer(player);
        }
    }

	public void KillPlayer(GameObject playerObject){

		/*foreach (GameObject player in playerRefs) {
			if(player == playerObject){
				player = null;
				Destroy(playerObject);
			}
		}*/

		if (playerRefs.Remove (playerObject)) {
			Instantiate (explodey,playerObject.transform.position, explodey.transform.rotation);
			Destroy(playerObject);
			Debug.Log ("Player Destroyed.");
		}
	}
}
