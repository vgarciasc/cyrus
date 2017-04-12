using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotColumn : MonoBehaviour {
	public List<Slot> slots = new List<Slot>();

	void Start() {
		init_slots();
	}

	void init_slots() {
		for (int i = 0; i < slots.Count; i++) {
			slots[i].set_IDs(i, i);
		}
	}

	public void kill_slot(int slotID) {
		//in the case of a slot being killed, put it in the set of the
		//previous slot, except if it is the 1st (in this case get the 2nd)
		int new_setID;
		if (slotID == 0) {
			new_setID = slots [1].setID;
		} else {
			new_setID = slots [slotID - 1].setID;
		}

		slots [slotID].set_setID (new_setID);
	}
}
