using UnityEngine;
using System.Collections;

public class GhostManager : MonoBehaviour {

    private float _apparation;
    private float _lifetime;

	// Use this for initialization
	void Start () {
        this._apparation = Time.time;
        this._lifetime = 10.0f + 5.0f * (float)RNG.random.NextDouble();
	}

    void OnDestroy() {
        this.CancelInvoke();
    }

    public float apparation {
        get { return _apparation; }
    }

    public float lifetime {
        get { return _lifetime; }
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
    		this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, currentTarget.transform.position, Time.deltaTime * ((highestThreat * 0.25f) + 0.20f));
        }
	}
}
