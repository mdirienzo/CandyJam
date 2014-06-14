using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public LevelManager levelManager;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}




	// User picks up an item
	void PlayerPicksUpItemHandler(PlayerManager player, ItemManager item) {
		switch (item.itemType) {
			case ItemManager.ItemType.POWER_TYPE:
				player.SetPlayerPower (item);
				break;
			case ItemManager.ItemType.ITEM_TYPE:
				player.SetPlayerItem (item);
				break;
		}
	}
}
