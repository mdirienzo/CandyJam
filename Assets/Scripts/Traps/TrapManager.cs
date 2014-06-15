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


    void OnTriggerEnter(Collider obj) {
        if (obj.gameObject.tag == "Player") {
			GameManager.instance.RemovePlayer(obj.gameObject, true);
		}
	}
}
