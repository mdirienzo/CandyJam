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
            float distance = Vector3.Distance(obj.transform.position, this.gameObject.transform.position);
			if (currentTarget == null || distance < highestThreat){
				highestThreat = distance;
				currentTarget = obj;
			}
		}

        if (currentTarget != null) {
            Debug.Log("currentTarget = " + currentTarget + ", position = " + this.gameObject.transform.position + ", highestThreat = " + highestThreat);
    		this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, currentTarget.transform.position, (highestThreat * 0.01f) + 0.001f);
    //		Vector3 direction = (currentTarget.transform.position - this.gameObject.transform.position).Normalize + ((distance * .10) + 10);
    //		this.gameObject. += direction;
        }
	}
}
