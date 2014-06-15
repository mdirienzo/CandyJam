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
		Debug.Log ("DEADED");
		isAlive = false;
		Destroy (gameObject);
		// play death animation and scream.
	}

	public void dropItem (){
		// drop item on the ground or destroy it.
		currentItemAction = null;
	}

	public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Key") {
            GameManager.instance.keyPickedUp(collision.gameObject);
        } else if (collision.gameObject.tag == "Door") {
            GameManager.instance.doorTouched(this.gameObject);
        }
	}


	public void SetPlayerItemAction (ItemAction itemAction) {
		this.currentItemAction = itemAction;
		Debug.Log ("SetPlayerPower called with " + itemAction);
	}


}
