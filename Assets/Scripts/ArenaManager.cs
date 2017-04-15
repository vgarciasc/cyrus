using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour {

	[SerializeField]
	ClickManager clickManager;
	[SerializeField]
	AttackManager attackManager;

	[Header("Slot Columns")]
	[SerializeField]
	SlotColumn left = new SlotColumn();
	[SerializeField]
	SlotColumn right = new SlotColumn();

	public static ArenaManager getArenaManager() {
		return (ArenaManager) HushPuppy.safeFindComponent("GameController", "ArenaManager");
	}

	void Start() {
		// StartCoroutine (test_killing_slots());
		init_delegates();
	}

	IEnumerator test_killing_slots() {

		right.kill_slot (0);

		yield return new WaitForSeconds (1f);
		// yield return new WaitForSeconds (1f);

		// right.kill_slot (2);

		// yield return new WaitForSeconds (1f);

		// right.kill_slot (0);
	}

	void init_delegates() {
		clickManager.activate_choosing_attack_target_event += show_attack_targets;
		clickManager.deactivate_choosing_attack_target_event += unshow_attack_targets;

		clickManager.activate_choosing_swap_target_event += show_swap_targets;
		clickManager.deactivate_choosing_swap_target_event += unshow_swap_targets;

		clickManager.conclude_attack_event += unshow_swap_targets;

		attackManager.set_new_target_event += set_specific_attack_targets;
	}

	#region attack targets
		void show_attack_targets(CharacterObject targeter) {
			foreach (CharacterObject co in get_attack_targets(targeter)) {
				if (co == get_default_attack_target(targeter)) {
					co.toggle_targethi(true);
				}
				else {
					co.toggle_targetlow(true);
				}
			}
		}

		void set_specific_attack_targets(CharacterObject targeter, CharacterObject target) {
			unshow_attack_targets(targeter);
			foreach (CharacterObject co in get_attack_targets(targeter)) {
				if (co == target) {
					co.toggle_targethi(true);
				}
				else {
					co.toggle_targetlow(true);
				}
			}
		}
		
		void unshow_attack_targets(CharacterObject targeter) {
			foreach (CharacterObject co in left.charObj) {
				co.toggle_targets(false);
			}

			foreach (CharacterObject co in right.charObj) {
				co.toggle_targets(false);
			}
		}
	#endregion

	#region swap targets
		void show_swap_targets(CharacterObject targeter) {	
			foreach (CharacterObject co in get_swap_targets(targeter)) {
				co.toggle_targetlow(true);
			}
		}
		
		void unshow_swap_targets(CharacterObject targeter) {
			unshow_swap_targets();
		}

		void unshow_swap_targets() {
			foreach (CharacterObject co in left.charObj) {
				co.toggle_targets(false);
			}

			foreach (CharacterObject co in right.charObj) {
				co.toggle_targets(false);
			}
		}
	#endregion

	#region getters
		public SlotColumn get_other_column(SlotColumn current) {
			return current == left? right : left;
		}

		public List<CharacterObject> get_attack_targets(CharacterObject targeter) {
			List<CharacterObject> targets = new List<CharacterObject>();
			List<int> slotsOccupied = new List<int>();

			for (int i = 0; i < targeter.column.slots.Count; i++) {
				if (targeter.column.slots[i].charID == targeter.charID) {
					slotsOccupied.Add(i);
				}
			}

			SlotColumn opposite_column = get_other_column(targeter.column);

			foreach (int pos in slotsOccupied) {
				targets.Add(opposite_column.get_charobj_by_slotID(pos));
			}

			return targets;
		}

		public CharacterObject get_default_attack_target(CharacterObject targeter) {
			return get_attack_targets(targeter)[0];
		}

		public List<CharacterObject> get_swap_targets(CharacterObject targetter) {
			List<CharacterObject> targets = new List<CharacterObject>();

			int index = targetter.column.charObj.IndexOf(targetter);
			if (index > 0) {
				targets.Add(targetter.column.charObj[index - 1]);
			}
			if (index < targetter.column.charObj.Count - 1) {
				targets.Add(targetter.column.charObj[index + 1]);
			}

			return targets;
		}
	#endregion
}
