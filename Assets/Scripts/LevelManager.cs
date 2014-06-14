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
    public GameObject northWallPrefab;
    public GameObject westWallPrefab;
    public int columns;
    public int rows;


	void Start () {
		Debug.Log("LevelManager Start");
        System.Random rnd = new System.Random();

		// prevalidate that the tile types are populate correctly, otherwise we won't be able to make the map
        bool invalidConfiguration = false;
        for (int i = 0; i < tileTypes.Length; ++i) {
            TileType tileType = tileTypes [i];
            if (tileType.floorPrefab == null) {
                Debug.LogError("Tile type " + i + " is not configured with a floor prefab.");
                invalidConfiguration = true;
            }
        }

        // and the wall prefabs
        if (northWallPrefab == null) {
            Debug.LogError("North wall prefab not set.");
            invalidConfiguration = true;
        }

        if (westWallPrefab == null) {
            Debug.LogError("West wall prefab not set.");
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

        if (invalidConfiguration) {
            Debug.LogError("Level manager not configured correctly and cannot start.");
            return;
        }

        // create the map now that the configuration is validated

        for (int r = 0; r < rows; ++r) {
            for (int c = 0; c < columns; ++c) {
                Vector3 position = new Vector3(c, r, 0);
                TileType tileType = tileTypes[rnd.Next(tileTypes.Length)];
                Instantiate(tileType.floorPrefab, position, Quaternion.identity);
            }
        }


	}

	// Update is called once per frame
	void Update () {

	}
}
