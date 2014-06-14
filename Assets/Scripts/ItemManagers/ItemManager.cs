using UnityEngine;
using System.Collections;

public class Item {
	public GameObject itemPrefab;
}

public abstract class ItemManager : MonoBehaviour {
	public enum ItemType { Lamp, Key };
	public ItemType itemType;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public abstract ItemAction Action ();
}

public class ItemAction {
	void doActivePower () {

	}

	void doPassivePower () {
		
	}

}


