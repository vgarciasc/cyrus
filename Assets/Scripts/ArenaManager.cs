using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour {

	[Header("Slot Columns")]
	[SerializeField]
	SlotColumn left = new SlotColumn();
	[SerializeField]
	SlotColumn right = new SlotColumn();

	void Start() {
		StartCoroutine (test_killing_slots());
	}

	IEnumerator test_killing_slots() {
		yield return new WaitForSeconds (1f);

		left.kill_slot (0);

		yield return new WaitForSeconds (1f);

		left.kill_slot (2);

		yield return new WaitForSeconds (1f);

		left.kill_slot (0);
	}

	SlotColumn get_other_column(SlotColumn current) {
		return current == left? right : left;
	}

	List<Slot> get_targets(SlotColumn column, int slotID) {
		List<int> positions_occupied_by_slot = new List<int> ();
		for (int i = 0; i < column.slots.Count; i++) {
			if (column.slots [i].charID == column.slots [slotID].charID) {
				positions_occupied_by_slot.Add (i);
			}
		}

		SlotColumn other = get_other_column(column);
		List<Slot> targets = new List<Slot>();

		for (int i = 0; i < positions_occupied_by_slot.Count; i++) {
			targets.Add (other.slots [positions_occupied_by_slot[i]]);
		}

		return targets;
	}
}
