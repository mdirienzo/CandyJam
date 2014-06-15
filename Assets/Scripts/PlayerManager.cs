using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	public bool hasLantern;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

	public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Key") {
            GameManager.instance.keyPickedUp(this.gameObject, collision.gameObject);
        } else if (collision.gameObject.tag == "Door") {
            GameManager.instance.doorTouched(this.gameObject);
        } else if (collision.gameObject.tag == "Torch") {
            GameManager.instance.torchPickedUp(this.gameObject, collision.gameObject);
        }
	}
}
