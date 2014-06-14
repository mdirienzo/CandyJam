using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	
	private ItemManager currentItem;
	private float lightRadius;
	public bool isAlive;

	// Use this for initialization
	void Start () {
		currentItem = new ItemManager();
		lightRadius = 50;
	}
	
	// Update is called once per frame
	void Update () {
		if (currentItem is LampManager) {
			lightRadius = 150;
		}
	}

	public void killPlayer () {
		isAlive = false;
		// play death animation and scream.
	}

	public void dropItem (){
		// drop item on the ground or destroy it.
		currentItem = null;
	}

	public void onCollisionEnter(Collision collision) {
		Debug.Log ("COLLISION");
		ItemManager[] items = collision.gameObject.GetComponents <ItemManager> ();
		Destroy (collision.gameObject);
	}


	public void SetPlayerItem (ItemManager item) {
		this.currentItem = item;
		Debug.Log ("SetPlayerPower called with " + item);
	}


}
