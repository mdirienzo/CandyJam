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
    		this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, currentTarget.transform.position, (highestThreat * 0.005f) + 0.002f);
        }
	}

    void OnTriggerEnter(Collider obj) {
        if (obj.gameObject.tag == "Player") {
            this.eatPlayer(obj.gameObject);
        }
    }

    void eatPlayer(GameObject player) {
        GameManager.instance.RemovePlayer(player, true);
    }
}
