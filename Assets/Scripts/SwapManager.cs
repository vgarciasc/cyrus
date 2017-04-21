﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	void end_swap(CharacterObject swap_target) {
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
}