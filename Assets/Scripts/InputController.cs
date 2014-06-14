using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	public float movementSpeed;
	private bool isMovable = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (isMovable) {

			if(Input.GetAxis("Horizontal") > 0){
				transform.Translate (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"));
			}
			if(Input.GetAxis("Horizontal") < 0){
				transform.Translate (Vector3.left * movementSpeed * -Input.GetAxis("Horizontal"));
			}
			if(Input.GetAxis("Vertical") > 0){
				transform.Translate (Vector3.up * movementSpeed * Input.GetAxis("Vertical"));
			}
			if(Input.GetAxis("Vertical") < 0){
				transform.Translate (Vector3.down * movementSpeed * -Input.GetAxis("Vertical"));
			}
			                                 
		}
	}
}
