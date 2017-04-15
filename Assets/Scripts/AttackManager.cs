using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttackManager : MonoBehaviour {
	[HeaderAttribute("References")]
	public GameObject attackScreen;
	public TextMeshProUGUI textLeft, textRight;

	[SerializeField]
	ClickManager clickManager;

	[HideInInspector]
	public CharacterObject char_attacking,
							char_attacked;

	void Start() {
		clickManager.start_attack_event += start_attack;
		clickManager.cancel_attack_event += cancel_attack;
		clickManager.set_attack_target_event += set_target;
		clickManager.conclude_attack_event += conclude_attack;

		toggle_screen(false);
	}

	//an attacker has been selected
	void start_attack(CharacterObject charObj) {
		char_attacking = charObj;
		// toggle_screen(true);
	}

	//a target has been selected, now we can show info
	void set_target(CharacterObject charObj) {
		//can't attack allies
		if (charObj.column == char_attacking.column) {
			return;
		}

		char_attacked = charObj;
		textRight.text = get_char_info(char_attacking);
		textLeft.text = get_char_info(char_attacked);
		toggle_screen(true);
	}

	void cancel_attack(CharacterObject charObj) {
		reset_attack();
	}

	//attack has not been cancelled, player has pressed OK
	void conclude_attack() {
		Debug.Log("Attack is occurring.");
		attack();
	}

	void attack() {
		char_attacked.column.kill_slot(char_attacked);
		reset_attack();
	}

	void reset_attack() {
		char_attacked = null;
		char_attacking = null;
		toggle_screen(false);
	}

	string get_char_info(CharacterObject charObj) {
		string aux = "";
		aux += "FOR: " + charObj.data.FOR;
		aux += "\nAGI: " + charObj.data.AGI;
		return aux;
	}

	void toggle_screen(bool value) {
		attackScreen.SetActive(value);
	}
}
