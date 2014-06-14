using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileType {
    public GameObject floor;
    public boolean hasNorthWall;
    public boolean hasWestWall;
}

public class LevelManager : MonoBehaviour {
    public TileType[] tileTypes;
    public GameObject northWallPrefab;
    public GameObject westWallPrefab;
    public int columns;
    public int rows;

	// Use this for initialization
	void Start () {
	    int[,] map = new int[columns,row];
        // fill out map
        // go through map and clone
	}

	// Update is called once per frame
	void Update () {

	}
}
