using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour {
	[SerializeField]
	GameObject statsScreen;
	[SerializeField]
	Text statsText;
	[SerializeField]
	Image statsPortrait;
	[SerializeField]
	ClickManager clickManager;

	CharacterObject focusedCharacter;

	void Start() {
		toggle_screen(false);
		init_delegates();
	}

	#region delegate receiving
		void init_delegates() {
			clickManager.activate_choosing_attrib_target_event += show_attrib_screen;
			clickManager.deactivate_choosing_attrib_target_event += unshow_attrib_screen;

			clickManager.set_attrib_target_event += set_attrib_target;	
		}

		void set_attrib_target(CharacterObject charObj) {
			focusedCharacter = charObj;
			update_screen();
		}

		void show_attrib_screen(CharacterObject charObj) {
			focusedCharacter = charObj;
			toggle_screen(true);
			update_screen();
		}

		void unshow_attrib_screen(CharacterObject charObj) {
			toggle_screen(false);
		}
	#endregion

	#region screen methods
		void toggle_screen(bool value) {
			statsScreen.SetActive(value);
		}

		void update_screen() {
			statsText.text = focusedCharacter.data.nome;
			statsPortrait.sprite = focusedCharacter.data.sprite;
		}
	#endregion
}
