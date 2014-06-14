using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public numPlayers;
	public GameObject playerPrefab;


    public LevelManager levelManager;

	// Use this for initialization
	void Start () {


		if (playerPrefab == null) 
		{
			Debug.LogError ("Player prefab not set!");
		} else {

			Debug.Log ("Spawning " + numPlayers + " players!");

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
