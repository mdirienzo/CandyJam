using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	
	private ItemAction currentItemAction = null;
	private float lightRadius;
	public bool isAlive;

	// Use this for initialization
	void Start () {
		lightRadius = 50;
	}
	
	// Update is called once per frame
	void Update () {
		if (currentItemAction is LampAction) {
			lightRadius = 150;
		}
	}

	public void killPlayer () {
		isAlive = false;
		// play death animation and scream.
	}

	public void dropItem (){
		// drop item on the ground or destroy it.
		currentItemAction = null;
	}

	public void onCollisionEnter(Collision collision) {
		Debug.Log ("COLLISION");
		ItemManager[] items = collision.gameObject.GetComponents <ItemManager> ();
		this.currentItemAction = items [0].Action ();
		Destroy (collision.gameObject);
	}


	public void SetPlayerItemAction (ItemAction itemAction) {
		this.currentItemAction = itemAction;
		Debug.Log ("SetPlayerPower called with " + itemAction);
	}


}
