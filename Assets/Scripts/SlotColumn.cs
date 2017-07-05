using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class SlotColumn : MonoBehaviour {
	public bool enemyColumn = false;

	public List<Slot> slots = new List<Slot>();
	public List<SlotForeground> slotsForeground = new List<SlotForeground>();
	public List<SlotBackground> slotsBackground = new List<SlotBackground>();
	public List<CharacterData> characters = new List<CharacterData>();
	public List<CharacterObject> charObj = new List<CharacterObject>();

	int groups = -1;

	void Awake() {
		init_slots();
		init_characters();

		// StartCoroutine(dasd());
	}

	IEnumerator dasd() {
		yield return new WaitForSeconds(2.0f);
		kill_slot(2);
	}

	void init_slots() {
		groups = 4;
		for (int i = 0; i < slots.Count; i++) {
			slots[i].set_IDs(i, i);
			slotsBackground[i].set_ID(i);

			slotsBackground[i].click_event += register_click_slot;
		}
	}

	void init_characters() {
		EncounterManager encounterManager = EncounterManager.getEncounterManager();
		if (encounterManager == null) {
			Debug.Log("Encounter Manager not found.");
			Debug.Break();
		}
		List<CharacterData> encounterCharacters = enemyColumn ?
			encounterManager.currentEncounter.enemies :
			encounterManager.currentEncounter.allies;

		for (int i = 0; i < characters.Count; i++) {
			charObj[i].set_data(i, encounterCharacters[i]);
			charObj[i].transform.position = slots[i].transform.position;
			
			charObj[i].click_event += register_click;
		}
	}
	
	public void kill_slot(CharacterObject co) {
		
		List<SlotBackground> sb = new List<SlotBackground>();
		foreach (SlotBackground s in slotsBackground) {
			if (s.gameObject.activeSelf) {
				sb.Add(s);
			}
		}

		List<CharacterObject> cb = new List<CharacterObject>();
		foreach (CharacterObject c in charObj) {
			if (c.gameObject.activeSelf) {
				cb.Add(c);
			}
		}

		sb[cb.IndexOf(co)].kill();
		co.kill();

		kill_slot(get_slotID_by_charobj(co));
	}

	public void kill_slot(int slotID) {
		// get_slotbg_by_charobj(get_charobj_by_slotID(slotID)).kill();
		// get_charobj_by_slotID(slotID).kill();

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
		
		// for (int i = 0; i < slotsBackground.Count; i++) {
		// 	if (!real_IDs.Contains(i)) {
		// 		slotsBackground[i].gameObject.SetActive(false);
		// 	}
		// }

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

		List<SlotBackground> bgs = new List<SlotBackground>();
		foreach (SlotBackground sb in slotsBackground) {
			if (sb.gameObject.activeSelf) {
				bgs.Add(sb);
			}
		}

		foreach (SlotBackground sb in bgs) {
			sb.set_ID(bgs.IndexOf(sb));
		}	
		
		adjust_positions(true);
	}

	void adjust_positions(bool death) {
		switch (groups) {
			case 4:
				for (int i = 0; i < slots.Count; i++) {
					charObj[i].set_new_pos(slots[i].transform.position);
					slotsBackground[i].set_new_pos(slots[i].transform.position);
				}
				break;
			case 3:
				get_charobj_by_index(0).set_new_pos(slots[0].transform.position);
				get_charobj_by_index(1).set_new_pos(slots[1].transform.position +
					(slots[2].transform.position - slots[1].transform.position) / 2);
				get_charobj_by_index(2).set_new_pos(slots[3].transform.position);

				if (death)  {
					get_slotbg_by_index(1).resize(2);

					get_slotbg_by_index(0).set_new_pos(slots[0].transform.position);
					get_slotbg_by_index(1).set_new_pos(slots[1].transform.position +
						(slots[2].transform.position - slots[1].transform.position) / 2);
					get_slotbg_by_index(2).set_new_pos(slots[3].transform.position);
				}

				break;
			case 2:
				get_charobj_by_index(0).set_new_pos(slots[0].transform.position +
					(slots[1].transform.position - slots[0].transform.position) / 2);
				get_charobj_by_index(1).set_new_pos(slots[2].transform.position +
					(slots[3].transform.position - slots[2].transform.position) / 2);

				if (death)  {
					get_slotbg_by_index(0).resize(2);
					get_slotbg_by_index(1).resize(2);

					get_slotbg_by_index(0).set_new_pos(slots[0].transform.position +
						(slots[1].transform.position - slots[0].transform.position) / 2);
					get_slotbg_by_index(1).set_new_pos(slots[2].transform.position +
						(slots[3].transform.position - slots[2].transform.position) / 2);
				}

				break;
			case 1:
				get_charobj_by_index(0).set_new_pos(slots[1].transform.position +
					(slots[2].transform.position - slots[1].transform.position) / 2);

				if (death)  {
					get_slotbg_by_index(0).resize(4);
					get_slotbg_by_index(0).set_new_pos(slots[1].transform.position +
						(slots[2].transform.position - slots[1].transform.position) / 2);
				}

				break;
		}
	}

	void register_click(CharacterObject co) {
		ClickManager.getClickManager().click_character(co);
	}

	void register_click_slot(int slotID) {
		// Debug.Log("slotID: " + slotID + "\nslotbg: " + get_slotbg_by_slotID(slotID));
		// TargetManager.getTargetManager().click_character(get_slotbg_by_slotID(slotID).target);
	}

	int get_slotID_by_charobj(CharacterObject co) {
		return charObj.IndexOf(co);
	}

	public Slot get_slot_by_charobj(CharacterObject co) {
		return slots[get_slotID_by_charobj(co)];
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

	// public SlotBackground get_slotbg_by_slotID(int slotID) {
	// }

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

		adjust_positions(false);
	}

	public List<CharacterObject> get_adjacent_characters(CharacterObject co) {
		List<CharacterObject> targets = new List<CharacterObject>();

		CharacterObject previous = null;
		bool is_next = false;
			
		for (int i = 0; i < co.column.charObj.Count; i++) {
			if (co.column.charObj[i].charID == co.charID) {
				if (previous != null) {
					targets.Add(previous);
				}

				is_next = true;
				continue;
			}

			if (is_next && 
				co.column.charObj[i].gameObject.activeSelf) {
				targets.Add(co.column.charObj[i]);
				break;
			}

			if (co.column.charObj[i].gameObject.activeSelf) {
				previous = co.column.charObj[i];
			}
		}

		return targets;
	}

	public List<CharacterObject> get_column_characters_except_targetter(CharacterObject co) {
		List<CharacterObject> targets = new List<CharacterObject>();

		for (int i = 0; i < charObj.Count; i++) {
			if (charObj[i].gameObject.activeSelf && charObj[i] != co) {
				targets.Add(charObj[i]);
			}
		}

		return targets;
	}

	public SlotBackground get_slotbg_by_charobj(CharacterObject co) {
		List<SlotBackground> sb = new List<SlotBackground>();
		foreach (SlotBackground s in slotsBackground) {
			if (s.gameObject.activeSelf) {
				sb.Add(s);
			}
		}

		List<CharacterObject> cb = new List<CharacterObject>();
		foreach (CharacterObject c in charObj) {
			if (c.gameObject.activeSelf) {
				cb.Add(c);
			}
		}

		return sb[cb.IndexOf(co)];
	}

	public SlotForeground get_slotfg_by_charobj(CharacterObject co) {
		List<SlotForeground> sb = new List<SlotForeground>();
		foreach (SlotForeground s in slotsForeground) {
			if (s.gameObject.activeSelf) {
				sb.Add(s);
			}
		}

		List<CharacterObject> cb = new List<CharacterObject>();
		foreach (CharacterObject c in charObj) {
			if (c.gameObject.activeSelf) {
				cb.Add(c);
			}
		}

		return sb[cb.IndexOf(co)];
	}

	public CharacterObject get_charobj_by_slotbg(SlotBackground sbg) {
		List<SlotBackground> sb = new List<SlotBackground>();
		foreach (SlotBackground s in slotsBackground) {
			if (s.gameObject.activeSelf) {
				sb.Add(s);
			}
		}

		List<CharacterObject> cb = new List<CharacterObject>();
		foreach (CharacterObject c in charObj) {
			if (c.gameObject.activeSelf) {
				cb.Add(c);
			}
		}

		//TODO: BUGANDO QUANDO FICA SO UM EM TELA
		return cb[sb.IndexOf(sbg)];
	}

	public SlotBackground get_slotbg_by_index(int index) {
		List<SlotBackground> sb = new List<SlotBackground>();
		foreach (SlotBackground s in slotsBackground) {
			if (s.gameObject.activeSelf) {
				sb.Add(s);
			}
		}

		return sb[index];
	}

	public CharacterObject get_charobj_by_index(int index) {
		List<CharacterObject> cb = new List<CharacterObject>();
		foreach (CharacterObject c in charObj) {
			if (!c.is_dead()) {
				cb.Add(c);
			}
		}

		return cb[index];
	}
}
