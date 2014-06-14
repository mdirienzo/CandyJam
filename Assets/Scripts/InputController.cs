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
				//transform.Translate (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"));
				rigidbody.AddForce (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"),ForceMode.Force);
			}

			if(Input.GetAxis("Horizontal") < 0){
				//transform.Translate (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"));
				rigidbody.AddForce (Vector3.left * movementSpeed * -Input.GetAxis("Horizontal"),ForceMode.Force);
			}


			if(Input.GetAxis("Vertical") > 0){
				//transform.Translate (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"));
				rigidbody.AddForce (Vector3.up * movementSpeed * Input.GetAxis("Vertical"),ForceMode.Force);
			}
			
			if(Input.GetAxis("Vertical") < 0){
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
			                                 
		}
	}


	void OnCollisionStay(Collision collisionInfo){

	/*	foreach (GameObject gameobject in collisionInfo) {
			Debug.Log ("Collided with " + gameobject);
		}*/
			if (collisionInfo.gameObject.tag == "Wall") {



						/*igidbody.angularVelocity = new Vector3(0,0,0);
			rigidbody.velocity = new Vector3(0,0,0);
		}*/

						Debug.Log ("Collided with " + collisionInfo);
				}
	}
}
