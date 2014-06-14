using UnityEngine;
using System.Collections;

public class TrapManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void OnCollisionEnter(Collision collision) {
		PlayerManager[] playersCollided = collision.gameObject.GetComponents <PlayerManager> ();

		if (playersCollided.Length > 0) {
			playersCollided [0].killPlayer ();
			Destroy (collision.gameObject);
		}

	}
}
