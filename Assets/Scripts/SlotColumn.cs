using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

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
		for (int i = 0; i < characters.Count; i++) {
			charObj[i].set_data(i, characters[i]);
			charObj[i].transform.position = slots[i].transform.position;
			
			charObj[i].click_event += register_click;
		}
	}
	
	public void kill_slot(CharacterObject charObj) {
		kill_slot(get_slotID_by_charobj(charObj));
	}

	public void kill_slot(int slotID) {
		get_charobj_by_slotID(slotID).kill();
		
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
					charObj[i].set_new_pos(slots[i].transform.position);
				}
				break;
			case 3:
				get_charobj_by_slotID(0).set_new_pos(slots[0].transform.position);
				get_charobj_by_slotID(1).set_new_pos(slots[1].transform.position +
					(slots[2].transform.position - slots[1].transform.position) / 2);
				get_charobj_by_slotID(3).set_new_pos(slots[3].transform.position);
				break;
			case 2:
				get_charobj_by_slotID(0).set_new_pos(slots[0].transform.position +
					(slots[1].transform.position - slots[0].transform.position) / 2);
				get_charobj_by_slotID(3).set_new_pos(slots[2].transform.position +
					(slots[3].transform.position - slots[2].transform.position) / 2);
				break;
			case 1:
				get_charobj_by_slotID(0).set_new_pos(slots[1].transform.position +
					(slots[2].transform.position - slots[1].transform.position) / 2);
				break;
		}
	}

	void register_click(CharacterObject co) {
		ClickManager.getClickManager().click_character(co);
	}

	int get_slotID_by_charobj(CharacterObject co) {
		return charObj.IndexOf(co);
	}

	public CharacterObject get_charobj_by_slotID(int slotID) {
		int charID = slots[slotID].charID;
		foreach (CharacterObject co in charObj) {
			if (co.charID == charID) {
				return co;
			}
		}

		Debug.Log("Error searching for charObj in slotID " + slotID);
		return null;
	}

	public void toggle_all_lanes(bool value) {
		foreach (CharacterObject co in charObj) {
			co.lane.toggle_lane(value);
			co.lane.toggle_cancel(value);
		}
	}

	public void swap_characters(CharacterObject char1, CharacterObject char2) {
		foreach (Slot s in slots) {
			int c = s.charID;
			if (c == char1.charID) c = char2.charID;
			else if (c == char2.charID) c = char1.charID;

			s.set_charID(c);
		}

		List<CharacterObject> aux = new List<CharacterObject>();
		foreach (CharacterObject co in charObj) {
			var c = co;
			if (co == char1) c = char2;
			if (co == char2) c = char1;

			aux.Add(c);
		}
		charObj = aux;

		adjust_positions();
	}
}
