﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class ButtonLane : MonoBehaviour {

	public bool laneActive = false,
				cancelActive = false;
	[HeaderAttribute("References")]
	public CharacterObject character;
	public GameObject buttonContainer;
	public GameObject skillButtonPrefab;

	[HeaderAttribute("Lane Button References")]
	public GameObject attackButton;
	public GameObject viewInfoButton;
	public GameObject swapButton;
	public GameObject cancelButton;

	List<SkillLaneButton> skillButtons = new List<SkillLaneButton>();

	void Start() {
		init_delegates();
		init_skills();
		toggle(false);
	}

	void init_delegates() {
		ClickManager cm = ClickManager.getClickManager();

		cm.deactivate_all_lanes += deactivate_lane;
		cm.deactivate_all_cancels += deactivate_cancel;
		cm.deactivate_all_lanes_cancels += deactivate_everything;

		cm.conclude_attack_event += deactivate_cancel;
		cm.conclude_attack_event += deactivate_lane;

		cm.conclude_swap_event += deactivate_cancel;
		cm.conclude_swap_event += deactivate_lane;
	}

	void init_skills() {
		if (character.column.enemyColumn) {
			return;
		}

		for (int i = 0; i < character.activeSkills.Count; i++) {
			GameObject aux = Instantiate(skillButtonPrefab,
				buttonContainer.transform,
				false);
			SkillLaneButton slb = aux.GetComponent<SkillLaneButton>();
			skillButtons.Add(slb);
				
			slb.init(character, character.activeSkills[i]);
		}
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
			update_action_buttons();
		}
	#endregion

	void update_action_buttons() {
		if (character.has_general_actions()) {
			toggle_active(attackButton, true);
		}
		else {
			toggle_active(attackButton, false);
		}
		if (character.has_swap_actions()) {
			toggle_active(swapButton, true);
		}
		else {
			toggle_active(swapButton, false);
		}
		
		ActiveSkillManager asm = ActiveSkillManager.getActiveSkillManager();
		foreach (SkillLaneButton slb in skillButtons) {
			if (asm.can_cast(character, slb.data)) {
				toggle_active(slb.gameObject, true);
			}
			else {
				toggle_active(slb.gameObject, false);
			}
		}
	}

	void toggle_active(GameObject button, bool value) {
		if (value) {
			var aux = button.GetComponentInChildren<Image>().color;
			button.GetComponentInChildren<Image>().color = HushPuppy.getColorWithOpacity(aux, 1f);
		}
		else {
			var aux = button.GetComponentInChildren<Image>().color;
			button.GetComponentInChildren<Image>().color = HushPuppy.getColorWithOpacity(aux, 0.2f);
		}
	}

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

			// cancelButton.SetActive(false);
		}
	#endregion

	void deactivate_everything() {
		toggle_cancel(false);
		toggle_lane(false);
	}
}
