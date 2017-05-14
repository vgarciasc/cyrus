using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AllyTarget {
	NONE, //cannot swap with anybody
	ADJACENT, //default
	EVERYONE //character can swap with everyone
};

public class SwapManager : MonoBehaviour {
	[SerializeField]
	ClickManager clickManager;
	[SerializeField]
	ArenaManager arenaManager;

	CharacterObject char_swap1,
					char_swap2;
	
	void Start() {
		init_delegates();
	}

	void init_delegates() {
		clickManager.activate_choosing_swap_target_event += start_swap;
		clickManager.deactivate_choosing_swap_target_event += end_swap;
	}

	void start_swap(CharacterObject swapper) {
		char_swap1 = swapper;
	}

	void cancel_swap() {
		char_swap1 = null;
		char_swap2 = null;
	}

	void end_swap(CharacterObject swap_target) {
		if (swap_target == char_swap1) {
			cancel_swap();
			return;
		}

		char_swap2 = swap_target;
		swap();
	}

	void swap() {
		char_swap1.use_action();
		char_swap1.column.swap_characters(char_swap1, char_swap2);
	}

	public bool is_swap_valid(CharacterObject swap_target) {
		return (arenaManager.get_swap_targets(char_swap1).Contains(swap_target));
	}

	public IEnumerator swap_characters(CharacterObject co1, CharacterObject co2) {
		start_swap(co1);
		end_swap(co2);

		yield return wait_swap_end();
	}

	public IEnumerator random_swap(CharacterObject swapper) {
		start_swap(swapper);
		end_swap(arenaManager.get_swap_targets(swapper)[0]);
		
		Coroutine toWait = null;
		toWait = StartCoroutine(wait_swap_end());

		yield return toWait;
	}

	public IEnumerator wait_swap_end() {
		yield return new WaitForSeconds(CharacterObject.swapWaitTime);
	}
}
