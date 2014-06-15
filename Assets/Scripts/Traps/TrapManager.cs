using UnityEngine;
using System.Collections;

public class TrapManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*
	public void OnCollisionEnter(Collision collision) {
		PlayerManager playerCollided = collision.gameObject.GetComponent <PlayerManager> ();
		Debug.Log ("TRAPS");
		Debug.Log ("Num players: " + playersCollided.Length);
		//if (playersCollided.Length > 0) {
			playersCollided [0].killPlayer ();
			Destroy (collision.gameObject);
		}

	}*/

	public void OnTriggerEnter(Collider otherObject){
		if (otherObject.tag == "Player") {

			//otherObject.GetComponent<PlayerManager>().killPlayer();
			GameManager.instance.KillPlayer(otherObject.gameObject);
			//Debug.Log ("other object tag is: " + otherObject.tag);
		}

	}
}
