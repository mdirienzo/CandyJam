using UnityEngine;
using System.Collections;


public class LevelManager : MonoBehaviour {
    public static LevelManager instance = null;

    public enum TileSide {
        NORTH, EAST, SOUTH, WEST
    };

    public GameObject borderPrefab;
    public GameObject[] floorPrefabs;
    public GameObject[] xWallPrefabs;
    public GameObject[] yWallPrefabs;
    public int columns;
    public int rows;

    private GameObject xClipWallPrefab = null;
    private GameObject yClipWallPrefab = null;

    void Awake() {
        LevelManager.instance = this;
        this.xClipWallPrefab = (GameObject)Resources.Load("x-clip-wall", typeof(GameObject));
        this.yClipWallPrefab = (GameObject)Resources.Load("y-clip-wall", typeof(GameObject));
    }

	void Start() {
		Debug.Log("LevelManager Start");

		// prevalidate that the tile types are populate correctly, otherwise we won't be able to make the map
        if (!this.ValidateConfiguration()) {
            Debug.LogError("Level manager not configured correctly and cannot start.");
            return;
        }

        this.BuildMap();
        this.PositionBorder();
        this.PositionCameraAndLight();
	}

	void Update() { }

    public Vector3 CenterOfMap() {
        return this.CenterOfTile(this.rows / 2, this.columns / 2);
    }

    public Vector3 CenterOfTile(int r, int c) {
        return new Vector3(c, r, 0);
    }

    public Vector3 CenterOfWall(int r, int c, TileSide side) {
        switch (side) {
            case TileSide.NORTH:
                return this.CenterOfTile(r, c) + new Vector3( 0.0f,  0.5f, 0.0f);
            case TileSide.EAST:
                return this.CenterOfTile(r, c) + new Vector3( 0.5f,  0.0f, 0.0f);
            case TileSide.SOUTH:
                return this.CenterOfTile(r, c) + new Vector3( 0.0f, -0.5f, 0.0f);
            case TileSide.WEST:
                return this.CenterOfTile(r, c) + new Vector3(-0.5f,  0.0f, 0.0f);
            default:
                return Vector3.zero;
        }
    }

    public Vector3 ExtentsOfMap() {
        return this.CenterOfTile(this.rows-1, this.columns-1) + new Vector3(0.5f, 0.5f, 0.0f);
    }

    private void BuildMap() {
        System.Random rnd = new System.Random();

        // create the map now that the configuration is validated
        for (int r = 0; r < this.rows; ++r) {
            for (int c = 0; c < this.columns; ++c) {
                GameObject floorPrefab = this.floorPrefabs[rnd.Next(floorPrefabs.Length)];
                Instantiate(floorPrefab, this.CenterOfTile(r, c), Quaternion.identity);
            }
        }

        /*
            GameObject wallPrefab = this.yWallPrefabs[rnd.Next(yWallPrefabs.Length)];
            Vector3 wallPosition = floorPosition + new Vector3(-0.5f, 0.0f, 0.0f);
            Instantiate(wallPrefab, wallPosition, Quaternion.identity);

            GameObject wallPrefab = this.xWallPrefabs[rnd.Next(xWallPrefabs.Length)];
            Vector3 wallPosition = floorPosition + new Vector3(0.0f, -0.5f, 0.0f);
            Instantiate(wallPrefab, wallPosition, Quaternion.identity);
        */

        for (int r = 0; r < rows; ++r) {
            Vector3 southWallPosition = this.CenterOfWall(r, 0, TileSide.SOUTH);
            Instantiate(this.xClipWallPrefab, southWallPosition, Quaternion.identity);
            Vector3 northWallPosition = this.CenterOfWall(r, this.rows, TileSide.NORTH);
            Instantiate(this.xClipWallPrefab, northWallPosition, Quaternion.identity);
        }

        for (int c = 0; c < columns; ++c) {
            Vector3 westWallPosition = this.CenterOfWall(0, c, TileSide.WEST);
            Instantiate(this.yClipWallPrefab, westWallPosition, Quaternion.identity);
            Vector3 eastWallPosition = this.CenterOfWall(this.rows, c, TileSide.EAST);
            Instantiate(this.yClipWallPrefab, eastWallPosition, Quaternion.identity);
        }
    }

    private void PositionBorder() {
        Vector3 position = this.ExtentsOfMap() / 2.0f;
        position.z = 5.0f;
        GameObject borderObj = (GameObject)Instantiate(this.borderPrefab, Vector3.zero, Quaternion.identity);
        borderObj.transform.localScale = this.ExtentsOfMap() + new Vector3(1.1f, 1.1f, 0.0f);
        borderObj.transform.position = position;
    }

    private void PositionCameraAndLight() {
        // Compute the center of the board, subtracting 0.5 since each tile is 1.0x1.0 with the origin at the center
        // The camera and light will be -5 Z above just to give plenty of room for walls and things
        Vector3 center = new Vector3(columns / 2.0f - 0.5f, rows / 2.0f - 0.5f, -5.0f);

        // Reposition the camera at the location we want
        GameObject mainCameraObj = GameObject.FindWithTag("MainCamera");
        mainCameraObj.transform.position = center;

        // Set the ortho size (which is half the vertical) to the total vertical height plus a fudge
        // hope that nobody runs with a portrait orientation, because this won't deal with it
        Camera mainCamera = mainCameraObj.GetComponent<Camera>();
        mainCamera.orthographicSize = rows / 2.0f + 0.25f /* nudge to give a little border at the top and bottom */;

        GameObject sunObj = GameObject.FindWithTag("Sun");
        sunObj.transform.position = center;
    }

    private bool ValidateConfiguration() {
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

        // and the row/column count
        if (this.rows <= 0) {
            Debug.LogError("Invalid row count.");
            invalidConfiguration = true;
        }
        if (this.columns <= 0) {
            Debug.LogError("Invalid column count.");
            invalidConfiguration = true;
        }

        return !invalidConfiguration;
    }
}
