using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileType {
    public GameObject floorPrefab;
    public bool hasNorthWall;
    public bool hasWestWall;
}

public class LevelManager : MonoBehaviour {
    public TileType[] tileTypes;
    public GameObject[] northWallPrefabs;
    public GameObject[] westWallPrefabs;
    public int columns;
    public int rows;


	void Start() {
		Debug.Log("LevelManager Start");

		// prevalidate that the tile types are populate correctly, otherwise we won't be able to make the map
        if (!this.ValidateConfiguration()) {
            Debug.LogError("Level manager not configured correctly and cannot start.");
            return;
        }

        this.BuildMap();
        this.PositionCameraAndLight();
	}

	// Update is called once per frame
	void Update() {

	}

    private void BuildMap() {
        System.Random rnd = new System.Random();

        // create the map now that the configuration is validated
        for (int r = 0; r < rows; ++r) {
            for (int c = 0; c < columns; ++c) {
                Vector3 position = new Vector3(c, r, 0);
                TileType tileType = this.tileTypes[rnd.Next(tileTypes.Length)];
                Instantiate(tileType.floorPrefab, position, Quaternion.identity);

                /*
                if (tileType.hasNorthWall || r == 0) {
                    GameObject prefab =
                }

                if (tileType.hasWestWall || c == 0) {
                }
                */
            }
        }
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

        // validate tile types
        if (tileTypes.Length == 0) {
            Debug.LogError("No tile types set.");
            invalidConfiguration = true;
        }

        for (int i = 0; i < tileTypes.Length; ++i) {
            TileType tileType = tileTypes [i];
            if (tileType.floorPrefab == null) {
                Debug.LogError("Tile type " + i + " is not configured with a floor prefab.");
                invalidConfiguration = true;
            }
        }

        // and the wall prefabs
        if (northWallPrefabs.Length == 0) {
            Debug.LogError("North wall prefabs not set.");
            invalidConfiguration = true;
        }

        for (int i = 0; i < northWallPrefabs.Length; ++i) {
            Debug.LogError("North wall prefab #" + i + " not set.");
            invalidConfiguration = true;
        }

        if (westWallPrefabs.Length == 0) {
            Debug.LogError("West wall prefab not set.");
            invalidConfiguration = true;
        }

        for (int i = 0; i < westWallPrefabs.Length; ++i) {
            Debug.LogError("West wall prefab #" + i + " not set.");
            invalidConfiguration = true;
        }

        // and the row/column count
        if (rows <= 0) {
            Debug.LogError("Invalid row count.");
            invalidConfiguration = true;
        }
        if (columns <= 0) {
            Debug.LogError("Invalid column count.");
            invalidConfiguration = true;
        }

        return invalidConfiguration;
    }
}
