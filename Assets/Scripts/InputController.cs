using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	public float movementSpeed;
	public string axisName;
	private bool isMovable = true;
	private float maxVelocity = 2;

	// Use this for initialization
	void Start () {
		axisName = "";
	}
	
	// Update is called once per frame
	void Update () {
	
		if (isMovable) {
			if(Input.GetAxis(axisName + "Horizontal") > 0){
				//transform.Translate (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"));
				rigidbody.AddForce (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"),ForceMode.Force);
			}

			if(Input.GetAxis(axisName + "Horizontal") < 0){
				//transform.Translate (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"));
				rigidbody.AddForce (Vector3.left * movementSpeed * -Input.GetAxis("Horizontal"),ForceMode.Force);
			}


			if(Input.GetAxis(axisName + "Vertical") > 0){
				//transform.Translate (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"));
				rigidbody.AddForce (Vector3.up * movementSpeed * Input.GetAxis("Vertical"),ForceMode.Force);
			}
			
			if(Input.GetAxis(axisName + "Vertical") < 0){
				//transform.Translate (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"));
				rigidbody.AddForce (Vector3.down * movementSpeed * -Input.GetAxis("Vertical"),ForceMode.Force);
			}

			if(Input.GetAxis("Horizontal") < 0){
				//transform.Translate (Vector3.left * movementSpeed * -Input.GetAxis("Horizontal"));
			}
			if(Input.GetAxis("Vertical") > 0){
				//transform.Translate (Vector3.up * movementSpeed * Input.GetAxis("Vertical"));
			}
			if(Input.GetAxis("Vertical") < 0){
				//transform.Translate (Vector3.down * movementSpeed * -Input.GetAxis("Vertical"));
			}

			if(rigidbody.velocity.magnitude > maxVelocity){
				rigidbody.velocity = rigidbody.velocity.normalized * maxVelocity;
			}
			                                 
		}
	}


	void OnTriggerEnter(Collider collider){

		if (collider.gameObject.tag == "Wall") {
			Debug.Log ("Collided with " + collider.gameObject.tag);
		}
	}
}
