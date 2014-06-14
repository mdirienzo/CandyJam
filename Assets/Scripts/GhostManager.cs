using UnityEngine;
using System.Collections;

public class GhostManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float highestThreat = 0;
		GameObject currentTarget = null;
		foreach (GameObject obj in GameManager.instance.playerRefs) {
			float distance = (obj.transform.position - this.gameObject.transform.position).magnitude;
			if (distance < highestThreat){
				highestThreat = distance;
				currentTarget = obj;
			}

		}
		this.transform.position = Vector3.MoveTowards (this.transform.position, currentTarget.transform.position, (highestThreat * .10f) + 10f);
//		Vector3 direction = (currentTarget.transform.position - this.gameObject.transform.position).Normalize + ((distance * .10) + 10);
//		this.gameObject. += direction;
	}
}
