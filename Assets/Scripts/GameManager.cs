using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public bool debugBuild = true;
    public int numGhosts = 1;
    public int ghostsPerPlayer = 1;
	public GameObject playerPrefab;
    public GameObject doorPrefab;
	public GameObject explodey;
    public Material closedDoorMaterial;
    public Material openDoorMaterial;
    public LevelParameters initLevelParameters = new LevelParameters {
        players           =  1,
        rows              = 14,
        columns           = 19,
        trapFraction      =  0.05f,
        extraPathFraction =  0.25f,
        keyFraction       =  1.0f
    };

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

    private LevelParameters nextLevelParameters;

    private string[] managedTags = new string[] {
        "Ghost",
        "Door",
        "Key",
        "Player",
        "Torch"
    };

    public int numPlayers {
        get { return LevelManager.instance.parameters.players; }
    }

    public int keysRequired {
        get { return (int)Mathf.Ceil(LevelManager.instance.parameters.players * LevelManager.instance.parameters.keyFraction); }
    }

	public void Awake() {
		this.playerRefs = new List<GameObject>();

        this.nextLevelParameters = this.initLevelParameters;

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

        this.newRound();
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

            int height = 280;
            int width = 200 + (this.debugBuild ? 250 : 0);
            GUI.BeginGroup(new Rect(Screen.width/2 - width/2, Screen.height/2 - height/2, width, height));

            int y = 0;

            if (GUI.Button(new Rect(0, y, 200, 40), "Unpause")) {
                this.unpause();
            }

            y += 40;

            if (GUI.Button(new Rect(0, y, 50, 40), "1P")) {
                this.nextLevelParameters.players = 1;
            }
            if (GUI.Button(new Rect(50, y, 50, 40), "2P")) {
                this.nextLevelParameters.players = 2;
            }
            if (GUI.Button(new Rect(100, y, 50, 40), "3P")) {
                this.nextLevelParameters.players = 3;
            }
            if (GUI.Button(new Rect(150, y, 50, 40), "4P")) {
                this.nextLevelParameters.players = 4;
            }

            y += 40;

            if (GUI.Button(new Rect(0, y, 50, 40), "Easy")) {
                this.nextLevelParameters.trapFraction = 0.05f;
                this.nextLevelParameters.extraPathFraction = 0.35f;
                this.nextLevelParameters.keyFraction = 1.0f;
            }
            if (GUI.Button(new Rect(50, y, 50, 40), "Med.")) {
                this.nextLevelParameters.trapFraction = 0.05f;
                this.nextLevelParameters.extraPathFraction = 0.25f;
                this.nextLevelParameters.keyFraction = 1.0f;
            }
            if (GUI.Button(new Rect(100, y, 50, 40), "Hard")) {
                this.nextLevelParameters.trapFraction = 0.05f;
                this.nextLevelParameters.extraPathFraction = 0.15f;
                this.nextLevelParameters.keyFraction = 1.25f;
            }
            if (GUI.Button(new Rect(150, y, 50, 40), "Noooo")) {
                this.nextLevelParameters.trapFraction = 0.05f;
                this.nextLevelParameters.extraPathFraction = 0.1f;
                this.nextLevelParameters.keyFraction = 1.5f;
            }

            y += 40;

            if (GUI.Button(new Rect(0, y, 40, 40), "-") && this.nextLevelParameters.columns > 10) {
                --this.nextLevelParameters.columns;
            }
            GUI.Label(new Rect(40, y, 120, 40), this.nextLevelParameters.columns + " Columns", countStyle);
            if (GUI.Button(new Rect(160, y, 40, 40), "+") && this.nextLevelParameters.columns < 100) {
                ++this.nextLevelParameters.columns;
            }

            y += 40;

            if (GUI.Button(new Rect(0, y, 40, 40), "-") && this.nextLevelParameters.rows > 10) {
                --this.nextLevelParameters.rows;
            }
            GUI.Label(new Rect(40, y, 120, 40), this.nextLevelParameters.rows + " Rows", countStyle);
            if (GUI.Button(new Rect(160, y, 40, 40), "+") && this.nextLevelParameters.rows < 100) {
                ++this.nextLevelParameters.rows;
            }

            y += 40;

            if (GUI.Button(new Rect(0, y, 200, 40), "Apply / Restart")) {
                this.unpause();
                newRound();
            }

            y += 40;

            if (GUI.Button(new Rect(0, y, 200, 40), "Quit")) {
                Application.Quit();
            }

            if (this.debugBuild) {
                y = 0;

                if (GUI.Button(new Rect(250, y, 40, 40), "-") && this.nextLevelParameters.keyFraction > 1.0f) {
                    this.nextLevelParameters.keyFraction -= 0.25f;
                }
                GUI.Label(new Rect(290, y, 120, 40), this.nextLevelParameters.keyFraction + " Key cf", countStyle);
                if (GUI.Button(new Rect(410, y, 40, 40), "+") && this.nextLevelParameters.keyFraction < 5.0f) {
                    this.nextLevelParameters.keyFraction += 0.25f;
                }

                y += 40;

                if (GUI.Button(new Rect(250, y, 40, 40), "-") && this.nextLevelParameters.trapFraction > 0.01f) {
                    this.nextLevelParameters.trapFraction -= 0.01f;
                }
                GUI.Label(new Rect(290, y, 120, 40), this.nextLevelParameters.trapFraction + " Trap cf", countStyle);
                if (GUI.Button(new Rect(410, y, 40, 40), "+") && this.nextLevelParameters.trapFraction < 0.25f) {
                    this.nextLevelParameters.trapFraction += 0.01f;
                }

                y += 40;

                if (GUI.Button(new Rect(250, y, 40, 40), "-") && this.nextLevelParameters.extraPathFraction > 0.0f) {
                    this.nextLevelParameters.extraPathFraction -= 0.05f;
                }
                GUI.Label(new Rect(290, y, 120, 40), this.nextLevelParameters.extraPathFraction + " Path cf", countStyle);
                if (GUI.Button(new Rect(410, y, 40, 40), "+") && this.nextLevelParameters.extraPathFraction < 0.5f) {
                    this.nextLevelParameters.extraPathFraction += 0.05f;
                }

                y += 40;
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

            GUI.Label(new Rect(Screen.width/2 - 50, Screen.height/2 - 25, 100, 50), text, style);
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
        this.beginRound(this.nextLevelParameters);
    }

    public void beginRound(LevelParameters levelParameters) {
        this.endRound();

        LevelManager level = LevelManager.instance;
        level.beginRound(levelParameters);
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
        int numPlayers = level.parameters.players;

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
        for (int i = 0; i < numGhosts + numPlayers * this.ghostsPerPlayer; ++i) {
            GameObject ghost = (GameObject)Instantiate(ghostPrefab, Vector3.zero, Quaternion.identity);
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
