using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	public float movementSpeed;
	public string axisName;
	private bool isMovable = true;
	private float maxVelocity = 2;

	private Animator anim;
	private bool isMoving = false;

	// Use this for initialization
	void Start () {
		axisName = "Player1_";
		anim = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () {

		if (rigidbody.velocity.magnitude == 0) {
				isMoving = false;
		} else {
				isMoving = true;
		}
		anim.SetBool ("IsMoving", isMoving);
		anim.SetBool ("HorizontalPressed", false);

		//anim.SetFloat ("VerticalSpeed", Mathf.Abs (Input.GetAxis ("Vertical")));

		anim.SetFloat ("VerticalSpeed", Input.GetAxis (axisName + "Vertical"));


			//anim.SetFloat ("VerticalSpeed", Input.GetAxis ("Vertical"));
			//anim.SetFloat ("HorizontalSpeed", Mathf.Abs(Input.GetAxis ("Horizontal")));
		//if(Mathf.Abs(Input.GetAxis ("Vertical")) < (Mathf.Abs(Input.GetAxis ("Horizontal")))){
		anim.SetFloat ("HorizontalSpeed", Mathf.Abs(Input.GetAxis (axisName + "Horizontal")));
			//anim.SetFloat ("VerticalSpeed", Input.GetAxis ("Vertical"));
		//}

		Debug.Log (Input.GetAxis(axisName + "Horizontal"));
		if (isMovable) {
			if(Input.GetAxis(axisName + "Horizontal") > 0){
				//transform.Translate (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"));
				rigidbody.AddForce (Vector3.right * movementSpeed * Input.GetAxis(axisName + "Horizontal"),ForceMode.Force);
				transform.localScale = new Vector3(1,1,1);
				anim.SetBool ("HorizontalPressed", true);

			}

			if(Input.GetAxis(axisName + "Horizontal") < 0){
				//transform.Translate (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"));
				rigidbody.AddForce (Vector3.left * movementSpeed * -Input.GetAxis(axisName + "Horizontal"),ForceMode.Force);
				transform.localScale = new Vector3(-1,1,1);
				anim.SetBool ("HorizontalPressed", true);
			}


			if(Input.GetAxis(axisName + "Vertical") > 0){
				//transform.Translate (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"));
				rigidbody.AddForce (Vector3.up * movementSpeed * Input.GetAxis(axisName + "Vertical"),ForceMode.Force);
			}
			if(Input.GetAxis(axisName + "Vertical") < 0){
				//transform.Translate (Vector3.right * movementSpeed * Input.GetAxis("Horizontal"));
				rigidbody.AddForce (Vector3.down * movementSpeed * -Input.GetAxis(axisName + "Vertical"),ForceMode.Force);
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

			if(rigidbody.velocity.magnitude == 0){
				anim.StopPlayback();
			}

		}
	}


	void OnTriggerEnter(Collider collider){

		if (collider.gameObject.tag == "Wall") {
			Debug.Log ("Collided with " + collider.gameObject.tag);
		}
	}
}
