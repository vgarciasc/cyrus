using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillLaneButton : MonoBehaviour {
	[HeaderAttribute("References")]
	[SerializeField]
	Text buttonText;

	SkillData data;
	CharacterObject character;	

	public void init(CharacterObject charac, SkillData skill) {
		this.character = charac;
		this.data = skill;

		buttonText.text = skill.title.ToUpper();
	}

	public void click_skill() {
		ClickManager.getClickManager().click_skill_button(character, data);
	}
}
