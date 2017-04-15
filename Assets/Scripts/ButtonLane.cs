using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLane : MonoBehaviour {

	public bool active = false;
	[HeaderAttribute("References")]
	public CharacterObject character;
	public GameObject buttonContainer;
	public List<GameObject> buttons = new List<GameObject>();

	void Start() {
		ClickManager.getClickManager().show_duel_info_event += deactivate_lane;

		toggle_lane(false);
	}

	public void register_attack() {
		ClickManager.getClickManager().click_attack_button(character);
	}

	void deactivate_lane() {
		toggle_lane(false);
	}

	public void toggle_lane(bool value) {
		active = value;
		buttonContainer.SetActive(value);
	}
}
