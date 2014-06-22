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
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update() {
		anim.SetBool("IsMoving", rigidbody.velocity.magnitude != 0);
		anim.SetBool("HorizontalPressed", false);

		anim.SetFloat("VerticalSpeed", Input.GetAxis(axisName + "Vertical"));
		anim.SetFloat("HorizontalSpeed", Mathf.Abs(Input.GetAxis(axisName + "Horizontal")));

		if (isMovable) {
			if (Input.GetAxis(axisName + "Horizontal") > 0) {
				rigidbody.AddForce(Vector3.right * movementSpeed * Input.GetAxis(axisName + "Horizontal"),ForceMode.Force);
				transform.localScale = new Vector3(1,1,1);
				anim.SetBool("HorizontalPressed", true);
			}

			if (Input.GetAxis(axisName + "Horizontal") < 0) {
				rigidbody.AddForce(Vector3.left * movementSpeed * -Input.GetAxis(axisName + "Horizontal"),ForceMode.Force);
				transform.localScale = new Vector3(-1,1,1);
				anim.SetBool("HorizontalPressed", true);
			}

			if (Input.GetAxis(axisName + "Vertical") > 0) {
				rigidbody.AddForce(Vector3.up * movementSpeed * Input.GetAxis(axisName + "Vertical"),ForceMode.Force);
			}

			if (Input.GetAxis(axisName + "Vertical") < 0) {
				rigidbody.AddForce(Vector3.down * movementSpeed * -Input.GetAxis(axisName + "Vertical"),ForceMode.Force);
			}

			if (rigidbody.velocity.magnitude > maxVelocity) {
				rigidbody.velocity = rigidbody.velocity.normalized * maxVelocity;
			}

			if (rigidbody.velocity.magnitude == 0) {
				anim.StopPlayback();
			}
		}
	}
}
