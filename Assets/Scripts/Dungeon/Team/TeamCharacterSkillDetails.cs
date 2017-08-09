using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TeamCharacterSkillDetails : MonoBehaviour {

	string name_id;

	[SerializeField]
	Text skill_label;
	[SerializeField]
	Text skill_title_button;

	public void Initialize(int id, string nome) {
		name_id = nome;

		skill_label.text = "Skill #" + id;
		skill_title_button.text = nome.ToUpper();
	}

	public void Toggle_Skill_Details() {
		var dcsm = DungeonCharacterStatsManager.Get_DCSM();
		dcsm.Show_Details_Skill(name_id);
	}
}
