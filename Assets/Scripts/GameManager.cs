using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public int numGhosts = 1;
    public int ghostsPerPlayer = 1;
	public GameObject playerPrefab;
    public GameObject doorPrefab;
	public GameObject explodey;
    public Material closedDoorMaterial;
    public Material openDoorMaterial;
    public int initNumPlayers = 1;
    public int initColumns = 19;
    public int initRows = 14;

	public static GameManager instance;

	public List<GameObject> playerRefs;

	// Game Control Variables
	private float theDarkTime = 15;
    private float normalIntensity = 0.3f;
    private float torchIntensity = 0.75f;

    private GameObject ghostPrefab;
    private GameObject sun;
    private GameObject lightning;
    private GameObject keyPrefab;
    private GameObject torchPrefab;

    private bool paused;
    private float startTime;
    private float sunRotation = 0.0f;
    private int keysObtained = 0;
    private bool _isDark = false;
    private bool _isDoorOpen = false;
    private float _timeUntilDark = 15;
    private bool _winCondition = false;
    public bool isDark { get { return _isDark; } }
    public bool isDoorOpen { get { return _isDoorOpen; } }
    public float timeUntilDark { get { return _timeUntilDark; } }
    public bool winCondition { get { return _winCondition; } }

    private int nextNumPlayers;
    private int nextRows;
    private int nextColumns;

    private string[] managedTags = new string[] {
        "Ghost",
        "Door",
        "Key",
        "Player",
        "Torch"
    };

    public int numPlayers {
        get { return LevelManager.instance.numPlayers; }
    }

    public int keysRequired {
        get { return LevelManager.instance.numPlayers; }
    }

	public void Awake() {
		this.playerRefs = new List<GameObject>();

        this.nextNumPlayers = this.initNumPlayers;
        this.nextRows = this.initRows;
        this.nextColumns = this.initColumns;

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

	public void Start() {
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

        this.torchPrefab = (GameObject)Resources.Load("Torch", typeof(GameObject));
        if (this.keyPrefab == null) {
            Debug.LogError("No key!");
        }

		if (this.playerPrefab == null) {
			Debug.LogError ("Player prefab not set!");
		}

        this.beginRound(this.initNumPlayers, this.initRows, this.initColumns);
    }

    public void OnGUI() {
        int secondsLeft = (int)this.timeUntilDark;
        if (secondsLeft >= 0) {
            GUI.Label(new Rect(10, 10, 100, 200), ":" + secondsLeft.ToString("D2"));
        }

        if (this.paused) {
            GUIStyle countStyle = new GUIStyle();
            countStyle.alignment = TextAnchor.MiddleCenter;
            countStyle.fontSize = 20;

            GUI.BeginGroup(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 120, 200, 240));
            if (GUI.Button(new Rect(0, 0, 200, 40), "Unpause")) {
                this.unpause();
            }

            if (GUI.Button(new Rect(0, 40, 50, 40), "1P")) {
                this.nextNumPlayers = 1;
            }
            if (GUI.Button(new Rect(50, 40, 50, 40), "2P")) {
                this.nextNumPlayers = 2;
            }
            if (GUI.Button(new Rect(100, 40, 50, 40), "3P")) {
                this.nextNumPlayers = 3;
            }
            if (GUI.Button(new Rect(150, 40, 50, 40), "4P")) {
                this.nextNumPlayers = 4;
            }

            if (GUI.Button(new Rect(0, 80, 40, 40), "-") && this.nextColumns > 10) {
                --this.nextColumns;
            }
            GUI.Label(new Rect(40, 80, 120, 40), this.nextColumns + " Columns", countStyle);
            if (GUI.Button(new Rect(160, 80, 40, 40), "+") && this.nextColumns < 100) {
                ++this.nextColumns;
            }

            if (GUI.Button(new Rect(0, 120, 40, 40), "-") && this.nextRows > 10) {
                --this.nextRows;
            }
            GUI.Label(new Rect(40, 120, 120, 40), this.nextRows + " Rows", countStyle);
            if (GUI.Button(new Rect(160, 120, 40, 40), "+") && this.nextRows < 100) {
                ++this.nextRows;
            }

            if (GUI.Button(new Rect(0, 160, 200, 40), "Apply / Restart")) {
                this.unpause();
                newRound();
            }

            if (GUI.Button(new Rect(0, 200, 200, 40), "Quit")) {
                Application.Quit();
            }

            GUI.EndGroup();
        } else {
            if (GUI.Button(new Rect(10, 30, 100, 20), "Pause")) {
                this.pause();
            }
        }

        if (this.playerRefs.Count == 0) {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
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
    public void Update() {
        float timeElapsed = Time.time - startTime;
        this._timeUntilDark = theDarkTime - timeElapsed;

        if (this.timeUntilDark < theDarkTime / 2.0f && this.timeUntilDark > 0.0f) {
            float r = -110.0f * (1.0f - this.timeUntilDark / (theDarkTime / 2.0f));
            float dr = r - sunRotation;
            sunRotation = r;
            sun.transform.RotateAround(LevelManager.instance.trueCenterOfMap, Vector3.up, dr);

            foreach (GameObject player in this.playerRefs) {
                PlayerManager manager = player.GetComponent<PlayerManager>();
                Light light = player.GetComponentInChildren<Light>();
                float maxIntensity = manager.hasLantern ? torchIntensity : normalIntensity;
                float playerLightIntensity = maxIntensity * (1.0f - this.timeUntilDark / (theDarkTime / 2.0f));
                light.GetComponent<Light>().intensity = playerLightIntensity;
            }
        }

        if (this.timeUntilDark < 0.0f && !this.isDark) {
            this._isDark = true;
            this.Invoke("flashLightning1", 0.5f);
        }

        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetButtonUp("Pause")) {
            if (this.paused) {
                this.unpause();
            } else {
                this.pause();
            }
        }
    }

    public void newRound() {
        this.beginRound(this.nextNumPlayers, this.nextRows, this.nextColumns);
    }

    public void beginRound(int numPlayers, int rows, int columns) {
        this.endRound();

        LevelManager level = LevelManager.instance;
        level.beginRound(numPlayers, rows, columns);
        this.spawnPlayers();
        this.spawnTorch();
        this.startTime = Time.time;
        SoundManager.instance.beginRound();
    }

    public void endRound() {
        this.CancelInvoke();

        SoundManager.instance.endRound();

        this._isDark = false;
        this._isDoorOpen = false;
        this._timeUntilDark = this.theDarkTime;
        this._winCondition = false;
        this.keysObtained = 0;

        sun.transform.RotateAround(LevelManager.instance.trueCenterOfMap, Vector3.up, -sunRotation);
        sunRotation = 0.0f;

        this.playerRefs.Clear();

        foreach (string tag in this.managedTags) {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tag)) {
                Destroy(obj);
            }
        }

        LevelManager.instance.endRound();
    }

    public void pause() {
        this.paused = true;
        Time.timeScale = 0.0f;
        SoundManager.instance.pause();
    }

    public void unpause() {
        this.paused = false;
        Time.timeScale = 1.0f;
        SoundManager.instance.unpause();
    }

    private void spawnPlayers() {
        LevelManager level = LevelManager.instance;
        int numPlayers = level.numPlayers;

        Debug.Log("Spawning " + numPlayers + " players!");

        for (int i = 0; i < numPlayers; ++i) {
            GameObject player = (GameObject)Instantiate(playerPrefab);
            playerRefs.Add(player);
            player.GetComponent<InputController>().axisName = "Player" + (i+1) + "_";
            player.transform.position = level.spawnPointForPlayer(i);
        }
    }

    private void spawnTorch() {
        LevelManager level = LevelManager.instance;
        TileLocation torchLoc;
        do {
            torchLoc = level.tiles.random ();
        } while (level.isBadPlaceForThings(torchLoc));
        Vector3 torchPos = level.centerOfTile(torchLoc);
        Instantiate(this.torchPrefab, torchPos, Quaternion.identity);
    }

    void setLightning(float intensity) {
        this.lightning.GetComponent<Light>().intensity = intensity;
    }

    void dimLightning() {
        this.setLightning(0.0f);
    }

    void flashLightning1() {
		SoundManager.instance.Thunder ();
        this.setLightning(2.0f);
        this.Invoke("dimLightning", 0.01f);
        this.Invoke("flashLightning2", 1.0f);
    }

    void flashLightning2() {
		//SoundManager.instance.Thunder ();
        this.spawnGhosts();
        this.setLightning(2.0f);
        this.Invoke("dimLightning", 0.1f);
        this.Invoke("placeDoorAndKeys", 1.0f);
    }

    void spawnGhosts() {
		//har har har
		//SoundManager.instance.Laugh ();
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
        } while (level.isBadPlaceForThings(loc));

        Instantiate(this.doorPrefab, level.centerOfTile(loc), rot);
        this.closeDoor();

        for (int i = 0; i < keysRequired; ++i) {
            do {
                loc = level.tiles.random();
            } while (level.isBadPlaceForThings(loc));
            Vector3 pos = level.centerOfTile(loc);
            GameObject key = (GameObject)Instantiate(this.keyPrefab, pos, Quaternion.identity);
            key.name = "Key " + i;
        }
    }

    void openDoor() {
        this._isDoorOpen = true;
        GameObject.FindWithTag("Door").GetComponent<MeshRenderer>().material = this.openDoorMaterial;
    }

    void closeDoor() {
        this._isDoorOpen = false;
        GameObject.FindWithTag("Door").GetComponent<MeshRenderer>().material = this.closedDoorMaterial;
    }

	public void keyPickedUp(GameObject player, GameObject key) {
        this.keysObtained += 1;
        if (this.keysObtained >= this.keysRequired) {
            this.openDoor();
        }
        Destroy(key);
	}

    public void torchPickedUp(GameObject player, GameObject torch) {
        Destroy(torch);
        player.GetComponent<PlayerManager>().hasLantern = true;
        Light light = player.GetComponentInChildren<Light>();
        light.spotAngle = 120;
        if (this.isDark) {
            // the update loop is no longer touching the light intensity, so touch directly
            light.intensity = torchIntensity;
        }
    }

    public void doorTouched(GameObject player) {
        if (this.isDoorOpen) {
            this._winCondition = true;
            this.RemovePlayer(player, false);
        }
    }

	public void RemovePlayer(GameObject playerObject, bool wasKilled){

		/*foreach (GameObject player in playerRefs) {
			if(player == playerObject){
				player = null;
				Destroy(playerObject);
			}
		}*/

		if (playerRefs.Remove (playerObject)) {
			if ( wasKilled){
				Instantiate (explodey,playerObject.transform.position, explodey.transform.rotation);
				SoundManager.instance.Death();
			}
			Destroy(playerObject);
			Debug.Log ("Player Destroyed.");
		}
	}
}
