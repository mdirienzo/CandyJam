using UnityEngine;
using System.Collections;

public class LampManager : ItemManager {
	override public ItemAction Action() { return new LampAction(); }
}

public class LampAction : ItemAction {
	void doActivePower () {
		
	}
	
	void doPassivePower () {
		
	}
}
