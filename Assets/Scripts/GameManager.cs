using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public int numPlayers;
	public GameObject playerPrefab;

	public static GameManager instance;

	private GameObject[] playerRefs;

	void Awake(){

		if (instance == null) {
				instance = this;
		} else {
				Debug.Log ("Only one copy of gamemanager allowed!");
		}
	}


	// Use this for initialization
	void Start () {


		if (playerPrefab == null) 
		{
			Debug.LogError ("Player prefab not set!");
		} else {

			Debug.Log ("Spawning " + numPlayers + " players!");
			//spawn players based on size of map

			playerRefs = new GameObject[numPlayers];

			for(int i = 0; i < numPlayers; i++){
				playerRefs[i] = Instantiate(playerPrefab) as GameObject;
				//playerRefs[i].transform.position = new Vector3(levelMan
			}

		}



	}

	// Update is called once per frame
	void Update () {

	}




	// User picks up an item
	void PlayerPicksUpItemHandler(PlayerManager player, ItemManager item) {
//		switch (item.itemType) {
//			case ItemManager.ItemType.POWER_TYPE:
//				player.SetPlayerPower (item);
//				break;
//			case ItemManager.ItemType.ITEM_TYPE:
//				player.SetPlayerItem (item);
//				break;
//		}
	}
}
