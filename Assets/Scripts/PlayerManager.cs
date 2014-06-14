using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	private ItemManager currentItem;
	private ItemManager currentPower;

	// Use this for initialization
	void Start () {
		currentItem = null;
		currentPower = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void SetPlayerPower (ItemManager item) {
		this.currentPower = item;
		Debug.Log ("SetPlayerPower called with " + item.itemType);
	}

	public void SetPlayerItem (ItemManager item) {
		this.currentItem = item;
		Debug.Log ("SetPlayerPower called with " + item.itemType);
	}


}
