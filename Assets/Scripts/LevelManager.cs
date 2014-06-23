using UnityEngine;
using System.Collections.Generic;

public class TileWall {
    public bool north;
    public bool east;
}

public class LevelManager : MonoBehaviour {
    public static LevelManager instance = null;

    public GameObject borderPrefab;
    public GameObject[] floorPrefabs;
    public GameObject[] xWallPrefabs;
    public GameObject[] yWallPrefabs;
    public GameObject[] trapPrefabs;

    public BoundedTiles tiles;
    private List<TileLocation> trapLocations;
    private GameObject xClipWallPrefab = null;
    private GameObject yClipWallPrefab = null;

    private string[] managedTags = new string[] {
        "Wall",
        "Floor",
        "Trap"
    };


    public void Awake() {
        // just make sure they have some sensible values. the game manager should beginRound() with the real values before it matters though
        this._parameters = new LevelParameters {
            trapFraction = 0.05f,
            extraPathFraction = 0.0f,
            rows = 10,
            columns = 10,
            players = 1
        };

        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("Only one copy of levelmanager allowed!");
        }

        this.xClipWallPrefab = (GameObject)Resources.Load("x-clip-wall", typeof(GameObject));
        this.yClipWallPrefab = (GameObject)Resources.Load("y-clip-wall", typeof(GameObject));
    }

	public void Start() {
		Debug.Log("LevelManager Start");

		// prevalidate that the tile types are populate correctly, otherwise we won't be able to make the map
        if (!this.validateConfiguration()) {
            Debug.LogError("Level manager not configured correctly and cannot start.");
            return;
        }
	}

	public void Update() { }

    public void beginRound(LevelParameters parameters) {
        this.endRound();

        this._parameters = parameters;

        this.tiles = new BoundedTiles(new TileLocation(this.parameters.rows, this.parameters.columns));
        this.computeSpawnRegion();
        this.positionBorder();
        this.positionCameraAndLight();
        this.buildMap();
    }

    public void endRound() {
        foreach (string tag in this.managedTags) {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tag)) {
                Destroy(obj);
            }
        }
    }

    private LevelParameters _parameters;
    public LevelParameters parameters {
        get { return _parameters; }
    }

    private List<TileLocation> _spawnRegion = null;

    public List<TileLocation> spawnRegion { get { return _spawnRegion; } }

    private void computeSpawnRegion() {
        List<TileLocation> l = new List<TileLocation>();
        int columns = (int)Mathf.Ceil(Mathf.Sqrt(this.parameters.players));
        int rows = (int)Mathf.Ceil((float)this.parameters.players / (float)columns);

        TileLocation center = new TileLocation(this.tiles.bound.r / 2, this.tiles.bound.c / 2);
        TileLocation upperLeft = new TileLocation(center.r - rows / 2, center.c - columns / 2);
        TileLocation upperRight = new TileLocation(upperLeft.r, upperLeft.c + columns-1);
        TileLocation pos = upperLeft;

        for (int i = 0; i < this.parameters.players; ++i) {
            l.Add(pos);
            Debug.Log("Spawn point of player " + i + " = " + pos);
            int c = pos.c + 1;
            if (c > upperRight.c) {
                pos = new TileLocation(pos.r + 1, upperLeft.c);
            } else {
                pos = new TileLocation(pos.r, c);
            }
        }

        this._spawnRegion = l;
    }

    public Vector3 spawnPointForPlayer(int playerNumber) {
        return this.centerOfTile(this.spawnRegion[playerNumber]);
    }

    public Vector3 trueCenterOfMap {
        get { return this.extentsOfMap / 2.0f; }
    }

    public Vector3 centerOfTile(TileLocation loc) {
        return new Vector3(loc.c, loc.r, 0);
    }

    public Vector3 centerOfWall(TileLocation loc, CardinalDir side) {
        switch (side) {
            case CardinalDir.NORTH: return this.centerOfTile(loc) + new Vector3( 0.0f,  0.5f, 0.0f);
            case CardinalDir.EAST:  return this.centerOfTile(loc) + new Vector3( 0.5f,  0.0f, 0.0f);
            case CardinalDir.SOUTH: return this.centerOfTile(loc) + new Vector3( 0.0f, -0.5f, 0.0f);
            case CardinalDir.WEST:  return this.centerOfTile(loc) + new Vector3(-0.5f,  0.0f, 0.0f);
            default:                return Vector3.zero;
        }
    }

    public Vector3 extentsOfMap {
        get { return this.centerOfTile(this.tiles.extent) + new Vector3(0.5f, 0.5f, 0.0f); }
    }

    public bool isBadPlaceForThings(TileLocation l) {
        return this.trapLocations.Contains(l) || this.spawnRegion.Contains(l);
    }

    private void buildMap() {
        // create the floors
        foreach (TileLocation l in this.tiles.all) {
            GameObject floorPrefab = this.floorPrefabs[RNG.random.Next(floorPrefabs.Length)];
            Vector3 position = this.centerOfTile(l);
            position.z = 1.0f;
            Object floor = Instantiate(floorPrefab, position, Quaternion.identity);
            floor.name = "Floor " + l;
        }


        LevelBuilder levelBuilder;

        /*
        levelBuilder = new RandomWalkLevelBuilder (
            extraWalks:  0,
            numTraps:    (int)((this.tiles.bound.r * this.tiles.bound.c) * 0.05f),
            tiles:       this.tiles,
            spawnRegion: this.spawnRegion,
        );
        */
        levelBuilder = new GrowthLevelBuilder (
            parameters:  this.parameters,
            tiles:       this.tiles,
            spawnRegion: this.spawnRegion
        );

        levelBuilder.build();

        TileWall[] walls = levelBuilder.walls;
        this.trapLocations = levelBuilder.trapLocations;

        // create the traps
        foreach (TileLocation l in this.trapLocations) {
            GameObject trapPrefab = this.trapPrefabs[RNG.random.Next(trapPrefabs.Length)];
            Instantiate(trapPrefab, this.centerOfTile(l), Quaternion.identity);
        }

        // create the walls
        foreach (TileLocation l in this.tiles.all) {
            TileWall w = walls[this.tiles.indexOf(l)];
            if (w.north && l.north.inBound(this.tiles)) {
                GameObject wallPrefab = this.xWallPrefabs[RNG.random.Next(xWallPrefabs.Length)];
                Vector3 wallPosition = this.centerOfWall(l, CardinalDir.NORTH);
                Object wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity);
                wall.name = "Wall " + l + " north";
            }

            if (w.east && l.east.inBound(this.tiles)) {
                GameObject wallPrefab = this.yWallPrefabs[RNG.random.Next(yWallPrefabs.Length)];
                Vector3 wallPosition = this.centerOfWall(l, CardinalDir.EAST);
                Object wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity);
                wall.name = "Wall " + l + " east";
            }
        }

        // create outside clipping walls
        foreach (TileLocation l in this.tiles.columns[0]) {
            Vector3 pos = this.centerOfWall(l, CardinalDir.WEST);
            Object wall = Instantiate(this.yClipWallPrefab, pos, Quaternion.identity);
            wall.name = "West bounds wall " + l;
        }

        foreach (TileLocation l in this.tiles.columns[this.tiles.bound.c-1]) {
            Vector3 pos = this.centerOfWall(l, CardinalDir.EAST);
            Object wall = Instantiate(this.yClipWallPrefab, pos, Quaternion.identity);
            wall.name = "East bounds wall " + l;
        }

        foreach (TileLocation l in this.tiles.rows[0]) {
            Vector3 pos = this.centerOfWall(l, CardinalDir.SOUTH);
            Object wall = Instantiate(this.xClipWallPrefab, pos, Quaternion.identity);
            wall.name = "South bounds wall " + l;
        }

        foreach (TileLocation l in this.tiles.rows[this.tiles.bound.r-1]) {
            Vector3 pos = this.centerOfWall(l, CardinalDir.NORTH);
            Object wall = Instantiate(this.xClipWallPrefab, pos, Quaternion.identity);
            wall.name = "North bounds wall " + l;
        }
    }

    private void positionBorder() {
        Vector3 position = this.extentsOfMap / 2.0f;
        position.z = 5.0f;
        GameObject borderObj = (GameObject)Instantiate(this.borderPrefab, Vector3.zero, Quaternion.identity);
        borderObj.transform.localScale = this.extentsOfMap + new Vector3(1.1f, 1.1f, 0.0f);
        borderObj.transform.position = position;
    }

    private void positionCameraAndLight() {
        // Compute the center of the board, subtracting 0.5 since each tile is 1.0x1.0 with the origin at the center
        // The camera and light will be -5 Z above just to give plenty of room for walls and things
        Vector3 center = new Vector3(this.tiles.bound.c / 2.0f - 0.5f, this.tiles.bound.r / 2.0f - 0.5f, -10.0f);

        // Reposition the camera at the location we want
        GameObject mainCameraObj = GameObject.FindWithTag("MainCamera");
        mainCameraObj.transform.position = center;

        // Set the ortho size (which is half the vertical) to the total vertical height plus a fudge
        // hope that nobody runs with a portrait orientation, because this won't deal with it
        Camera mainCamera = mainCameraObj.GetComponent<Camera>();
        mainCamera.orthographicSize = this.tiles.bound.r / 2.0f + 0.25f /* nudge to give a little border at the top and bottom */;

        GameObject sunObj = GameObject.FindWithTag("Sun");
        sunObj.transform.position = center;

        GameObject lightningObj = GameObject.FindWithTag("Lightning");
        lightningObj.transform.position = center;
    }

    private bool validateConfiguration() {
        bool invalidConfiguration = false;

        // validate border prefab
        if (this.borderPrefab == null) {
            Debug.LogError("No border prefab set.");
            invalidConfiguration = true;
        }

        // validate floor prefabs
        if (this.floorPrefabs.Length == 0) {
            Debug.LogError("No floor prefabs set.");
            invalidConfiguration = true;
        }


        for (int i = 0; i < this.floorPrefabs.Length; ++i) {
            if (this.floorPrefabs[i] == null) {
                Debug.LogError("Floor prefab #" + i + " not set.");
                invalidConfiguration = true;
            }
        }

        // and the wall prefabs
        if (this.xWallPrefabs.Length == 0) {
            Debug.LogError("X wall prefabs not set.");
            invalidConfiguration = true;
        }

        for (int i = 0; i < this.xWallPrefabs.Length; ++i) {
            if (this.xWallPrefabs[i] == null) {
                Debug.LogError("X wall prefab #" + i + " not set.");
                invalidConfiguration = true;
            }
        }

        if (this.yWallPrefabs.Length == 0) {
            Debug.LogError("Y wall prefab not set.");
            invalidConfiguration = true;
        }

        for (int i = 0; i < this.yWallPrefabs.Length; ++i) {
            if (this.yWallPrefabs[i] == null) {
                Debug.LogError("Y wall prefab #" + i + " not set.");
                invalidConfiguration = true;
            }
        }

        // trap prefabs
        if (this.trapPrefabs.Length == 0) {
            Debug.LogError("Trap prefabs not set.");
            invalidConfiguration = true;
        }

        for (int i = 0; i < this.trapPrefabs.Length; ++i) {
            if (this.trapPrefabs[i] == null) {
                Debug.LogError("Trap prefab #" + i + " not set.");
                invalidConfiguration = true;
            }
        }

        return !invalidConfiguration;
    }
}

public abstract class LevelBuilder {
    public abstract TileWall[] walls { get; }
    public abstract List<TileLocation> trapLocations { get; }
    public abstract void build();
}

[System.Serializable]
public struct LevelParameters {
    public float keyFraction;
    public float extraPathFraction;
    public float trapFraction;
    public int   players;
    public int   rows;
    public int   columns;
}
