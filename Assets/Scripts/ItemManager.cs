using UnityEngine;
using System.Collections;

public class Item {
	public GameObject itemPrefab;

}



public class ItemManager : MonoBehaviour {

	public enum ItemType { POWER_TYPE, ITEM_TYPE };
	public ItemType itemType;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
