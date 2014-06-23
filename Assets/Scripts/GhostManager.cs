using UnityEngine;
using System.Collections;

public class GhostManager : MonoBehaviour {

    private float lastAppearance;
    private float lifetime;

	// Use this for initialization
	void Start () {
        this.apparate();
	}

    void apparate() {
        this.lastAppearance = Time.time;
        this.lifetime = 10.0f + 5.0f * (float)RNG.random.NextDouble();
        LevelManager level = LevelManager.instance;
        Vector3 pos;
        float nearestDistance;
        do {
            nearestDistance = 0.0f;
            pos = level.centerOfTile(level.tiles.random());
            GameObject nearestPlayer = null;
            foreach (GameObject player in GameManager.instance.playerRefs) {
                float distance = Vector3.Distance(player.transform.position, pos);
                if (nearestPlayer == null || distance < nearestDistance) {
                    nearestPlayer = player;
                    nearestDistance = distance;
                }
            }
        } while (nearestDistance <= 1.0f);

        this.gameObject.active = true;
        this.gameObject.transform.position = pos;
    }


	// Update is called once per frame
	void Update () {
        if (Time.time > this.lastAppearance + this.lifetime) {
            this.gameObject.active = false;
            this.Invoke("apparate", 2.0f);
        }

        if (!this.gameObject.active) return;

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
    		this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, currentTarget.transform.position, Time.deltaTime * ((highestThreat * 0.25f) + 0.15f));
        }
	}
}
