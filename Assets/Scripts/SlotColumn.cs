using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SlotColumn : MonoBehaviour {
	public List<Slot> slots = new List<Slot>();
	public List<CharacterData> characters = new List<CharacterData>();
	public List<CharacterObject> charObj = new List<CharacterObject>();

	int groups = -1;

	void Awake() {
		init_slots();
		init_characters();
	}

	void init_slots() {
		groups = 4;
		for (int i = 0; i < slots.Count; i++) {
			slots[i].set_IDs(i, i);
		}
	}

	void init_characters() {
		adjust_positions();
		for (int i = 0; i < characters.Count; i++) {
			charObj[i].set_data(characters[i]);
		}
	}

	// CharacterObject get_charobj_by_pos(int posID) {
	// 	for (int i = 0; i < slots.Count; i++) {
	// 		if (slots[i].charID == )
	// 	}
	// }

	public void kill_slot(int slotID) {
		charObj[slots[slotID].charID].kill();
		
		List<int> real_IDs = new List<int>();		
		foreach (Slot s in slots) {
			if (s.charID != slots[slotID].charID && 
				!real_IDs.Contains(s.charID)) {
				real_IDs.Add(s.charID);
			}
		}

		//real_IDs is a list with the set IDs that still exist
		//ex: {1, 2, 3, 4} => kill 3 => {1, 2, 4}
		//ex: {1, 2, 2, 4} => kill 2 => {1, 4}
		
		//everyone was killed
		if (groups-- == 0) {
			Debug.Log("everyone is dead");
			return;
		}

		switch (groups) {
			//ex: {1, 2, 4} => {1, 2, 2, 4}
			case 3:
				real_IDs.Insert(1, real_IDs[1]);
				break;
			//ex: {1, 2} => {1, 1, 2, 2}
			case 2:
				real_IDs.Insert(1, real_IDs[1]);
				real_IDs.Insert(0, real_IDs[0]);
				break;
			//ex: {1} => {1, 1, 1, 1}
			case 1:
				for (int i = 0; i < 3; i++) {
					real_IDs.Add(real_IDs[0]);
				}
				break;
		}

		//if a slot's set should be changed, change it.
		for (int i = 0; i < real_IDs.Count; i++) {
			if (slots[i].charID != real_IDs[i]) {
				slots[i].set_charID(real_IDs[i]);
			}
		}
		
		adjust_positions();
	}

	void adjust_positions() {
		switch (groups) {
			case 4:
				for (int i = 0; i < slots.Count; i++) {
					charObj[i].transform.position = slots[i].transform.position;
				}
				break;
			case 3:
				charObj[slots[0].charID].set_new_pos(slots[0].transform.position);
				charObj[slots[1].charID].set_new_pos(slots[1].transform.position +
					(slots[2].transform.position - slots[1].transform.position) / 2);
				charObj[slots[3].charID].set_new_pos(slots[3].transform.position);
				break;
			case 2:
				charObj[slots[0].charID].set_new_pos(slots[0].transform.position +
					(slots[1].transform.position - slots[0].transform.position) / 2);
				charObj[slots[3].charID].set_new_pos(slots[2].transform.position +
					(slots[3].transform.position - slots[2].transform.position) / 2);
				break;
			case 1:
				charObj[slots[0].charID].set_new_pos(slots[1].transform.position +
					(slots[2].transform.position - slots[1].transform.position) / 2);
				break;
		}
	}
}
