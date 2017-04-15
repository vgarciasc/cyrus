using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLane : MonoBehaviour {

	public bool laneActive = false,
				cancelActive = false;
	[HeaderAttribute("References")]
	public CharacterObject character;
	public GameObject buttonContainer;
	public GameObject cancelButton;
	public List<GameObject> buttons = new List<GameObject>();

	void Start() {
		init_delegates();
		toggle(false);
	}

	void init_delegates() {
		ClickManager cm = ClickManager.getClickManager();

		cm.deactivate_all_lanes += deactivate_lane;

		cm.conclude_attack_event += deactivate_cancel;
		cm.conclude_attack_event += deactivate_lane;

		cm.conclude_swap_event += deactivate_cancel;
		cm.conclude_swap_event += deactivate_lane;
	}

	#region register clicks
	public void register_viewinfo() {
		ClickManager.getClickManager().click_attrib_button(character);
	}

	public void register_swap() {
		ClickManager.getClickManager().click_swap_button(character);
	}

	public void register_attack() {
		ClickManager.getClickManager().click_attack_button(character);
	}

	public void register_cancel() {
		ClickManager.getClickManager().click_cancel_button(character);
	}
	#endregion

	public void toggle(bool value) {
		toggle_lane(value);
		toggle_cancel(value);
	}

	//lane: contains the action buttons
	#region setting lane
	void activate_lane() {
		toggle_lane(true);
	}

	void deactivate_lane() {
		toggle_lane(false);
	}

	public void toggle_lane(bool value) {
		laneActive = value;
		buttonContainer.SetActive(value);
	}
	#endregion

	//cancel: can the player cancel the current action?
	#region setting cancel
	void activate_cancel() {
		toggle_cancel(true);
	}

	void deactivate_cancel() {
		toggle_cancel(false);
	}

	public void toggle_cancel(bool value) {
		cancelActive = value;
		cancelButton.SetActive(value);
	}
	#endregion
}
