using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void SetPlayerPower (ItemManager item) {
		Debug.Log ("SetPlayerPower called with " + item.itemType);
	}

	public void SetPlayerItem (ItemManager item) {
		Debug.Log ("SetPlayerPower called with " + item.itemType);
	}
}
