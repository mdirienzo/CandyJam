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
    public int columns;
    public int rows;

    private BoundedTiles tiles;
    private GameObject xClipWallPrefab = null;
    private GameObject yClipWallPrefab = null;

    void Awake() {
        this.tiles = new BoundedTiles(new TileLocation(rows, columns));
        LevelManager.instance = this;
        this.xClipWallPrefab = (GameObject)Resources.Load("x-clip-wall", typeof(GameObject));
        this.yClipWallPrefab = (GameObject)Resources.Load("y-clip-wall", typeof(GameObject));
    }

	void Start() {
		Debug.Log("LevelManager Start");

		// prevalidate that the tile types are populate correctly, otherwise we won't be able to make the map
        if (!this.validateConfiguration()) {
            Debug.LogError("Level manager not configured correctly and cannot start.");
            return;
        }

        this.buildMap();
        this.positionBorder();
        this.positionCameraAndLight();
	}

	void Update() { }

    public TileLocation spawnPoint {
        get {
            return new TileLocation(this.tiles.bound.r / 2, this.tiles.bound.c / 2);
        }
    }

    public Vector3 centerOfMap() {
        return this.centerOfTile(this.spawnPoint);
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

    public Vector3 extentsOfMap() {
        return this.centerOfTile(this.tiles.extent) + new Vector3(0.5f, 0.5f, 0.0f);
    }

    private void buildMap() {
        System.Random rng = new System.Random();

        // create the floors
        foreach (TileLocation l in this.tiles.all) {
            GameObject floorPrefab = this.floorPrefabs[RNG.random.Next(floorPrefabs.Length)];
            Vector3 position = this.centerOfTile(l);
            position.z = 1.0f;
            Object floor = Instantiate(floorPrefab, position, Quaternion.identity);
            floor.name = "Floor " + l;
        }

        // create the traps
        for (int i = 0; i < (this.tiles.bound.r * this.tiles.bound.c) * 0.1f; ++i) {
            TileLocation l;
            do { l = this.tiles.random(); } while (l == this.spawnPoint);

            GameObject trapPrefab = this.trapPrefabs[RNG.random.Next(trapPrefabs.Length)];
            Instantiate(trapPrefab, this.centerOfTile(l), Quaternion.identity);
        }

        TileWall[] walls = new WallBuilder(this.tiles, this.spawnPoint, rng).build();

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
        Vector3 position = this.extentsOfMap() / 2.0f;
        position.z = 5.0f;
        GameObject borderObj = (GameObject)Instantiate(this.borderPrefab, Vector3.zero, Quaternion.identity);
        borderObj.transform.localScale = this.extentsOfMap() + new Vector3(1.1f, 1.1f, 0.0f);
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

        // and the row/column count
        if (this.tiles.bound.r <= 2) {
            Debug.LogError("Invalid row count.");
            invalidConfiguration = true;
        }
        if (this.tiles.bound.c <= 2) {
            Debug.LogError("Invalid column count.");
            invalidConfiguration = true;
        }

        return !invalidConfiguration;
    }
}

public class WallBuilder {
    private BoundedTiles tiles;
    private TileLocation spawnPoint;
    private TileWall[] walls;
    private bool[] reachable;
    private bool[] flood;
    private System.Random rng;
    private List<TileLocation> randomLocations;


    public WallBuilder(BoundedTiles tiles, TileLocation spawnPoint, System.Random rng) {
        this.tiles = tiles;
        this.spawnPoint = spawnPoint;
        this.walls = new TileWall[this.tiles.count];
        this.reachable = new bool[this.tiles.count];
        this.flood = new bool[this.tiles.count];
        this.randomLocations = this.tiles.shuffleAll();

        foreach (TileLocation l in tiles.all) {
            TileWall w = new TileWall();
            this.walls[this.tiles.indexOf(l)] = w;
            w.north = true;
            w.east  = true;
        }
    }

    private void debugWalls() {
        string s = "";
        for (int r = this.tiles.bound.r-1; r >= 0; --r) {
            for (int c = 0; c < this.tiles.bound.c; ++c) {
                TileWall w = this.walls[r*this.tiles.bound.c+c];
                s += w.north ? "^" : ".";
                s += w.east ? ">" : ".";
            }
            s += "\n";
        }
        Debug.Log(s);
    }


    public TileWall[] build() {

        // create walls. start by making everything a wall, then pick random points and
        // randomly walk from them bashing down walls for a finite number of steps. in
        // order to remove isolated clusters keep on picking points that cannot be reached
        // from the player origin

        int iterations = 0;
        while (true) {
            TileLocation isolatedLocation = this.findIsolatedLocation(); // to off 'em

            if (isolatedLocation == TileLocation.notFound) {
                // no more isolated locations, so be done
                break;
            }

            this.randomWalkFrom(isolatedLocation);
            ++iterations;
        }

        Debug.Log("finished building walls after " + iterations + " iterations");
        return this.walls;
    }

    private bool isReachable(TileLocation loc) {
        return this.reachable[this.tiles.indexOf(loc)];
    }

    private TileLocation findIsolatedLocation() {
        this.markReachable(this.spawnPoint, this.reachable);

        TileLocation bestLoc = TileLocation.notFound;
        int bestReach = this.tiles.count;
        IEnumerator<TileLocation> locs = this.randomLocations.GetEnumerator();

        while (bestReach > 1 && locs.MoveNext()) {
            TileLocation loc = locs.Current;
            this.markReachable(loc, this.flood);
            int reach = 0;
            for (int i = 0; i < this.flood.Length; ++i) {
                if (this.flood[i]) { reach += 1; }
            }
            if (reach < bestReach) {
                bestReach = reach;
                bestLoc = loc;
            }
        }

        return bestLoc;
    }

    private bool walled(TileLocation l, CardinalDir d) {
        switch (d) {
            case CardinalDir.NORTH: return this.walls[this.tiles.indexOf(l)].north;
            case CardinalDir.EAST:  return this.walls[this.tiles.indexOf(l)].east;
            case CardinalDir.SOUTH: return !l.south.inBound(this.tiles) || this.walls[this.tiles.indexOf(l.south)].north;
            default:                return !l.west .inBound(this.tiles) || this.walls[this.tiles.indexOf(l.west)].east;
        }
    }

    private void markReachable(TileLocation initial, bool[] marks) {
        for (int i = 0; i < marks.Length; ++i) {
            marks[i] = false;
        }

        marks[this.tiles.indexOf(initial)] = true;

        bool updated;
        do {
            updated = false;

            foreach (TileLocation l in this.tiles.all) {
                bool wasMarked = marks[this.tiles.indexOf(l)];
                if (wasMarked) continue;

                TileLocation n = l.north;
                TileLocation e = l.east;
                TileLocation s = l.south;
                TileLocation w = l.west;

                bool nt = (n.inBound(this.tiles) && !this.walled(l, CardinalDir.NORTH)) ? marks[this.tiles.indexOf(n)] : false;
                bool et = (e.inBound(this.tiles) && !this.walled(l, CardinalDir.EAST )) ? marks[this.tiles.indexOf(e)] : false;
                bool st = (s.inBound(this.tiles) && !this.walled(l, CardinalDir.SOUTH)) ? marks[this.tiles.indexOf(s)] : false;
                bool wt = (w.inBound(this.tiles) && !this.walled(l, CardinalDir.WEST )) ? marks[this.tiles.indexOf(w)] : false;

                bool nowMarked = wasMarked || nt || et || st || wt;

                updated = updated || (wasMarked != nowMarked);
                marks[this.tiles.indexOf(l)] = nowMarked;

            }
        } while (updated);
    }

    private void randomWalkFrom(TileLocation location) {
        for (int step = 0; step < 10; ++step) {
            TileLocation next;
            CardinalDir dir;
            do {
                dir = (CardinalDir)RNG.random.Next(4);
                next = location[dir];
            } while (!next.inBound(this.tiles));

            switch (dir) {
                case CardinalDir.NORTH:
                    this.walls[this.tiles.indexOf(location)].north = false;
                    break;
                case CardinalDir.EAST:
                    this.walls[this.tiles.indexOf(location)].east = false;
                    break;
                case CardinalDir.SOUTH:
                    this.walls[this.tiles.indexOf(next)].north = false;
                    break;
                default:
                    this.walls[this.tiles.indexOf(next)].east = false;
                    break;
            }

            location = next;
        }
    }
}

