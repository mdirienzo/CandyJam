using UnityEngine;
using System.Collections;

public class KeyManager : ItemManager {
	override public ItemAction Action() { return new LampAction(); }
}


public class KeyAction : ItemAction {
	void doActivePower () {
		
	}
	
	void doPassivePower () {
		
	}
}

