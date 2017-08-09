using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DungeonCharacterStatsManager : MonoBehaviour {

	CharacterDataJSON current_char;
	ItemData current_weapon;

	TeamManager teamManager;
	List<CharacterDataJSON> team;

	enum Screen { LEFT, RIGHT };
	Screen current_screen = Screen.LEFT;

	[Header("Both Screens")]
	[SerializeField]
	GameObject leftScreen;
	[SerializeField]
	GameObject rightScreen;
	[SerializeField]
	GameObject navigation_arrow_right;
	[SerializeField]
	GameObject navigation_arrow_left;

	[Header("Left Screen")]
	[SerializeField]
	GameObject details_container;
	[SerializeField]
	Image portrait;
	[SerializeField]
	Text weapon_name;
	[SerializeField]
	GameObject navigation_arrow_top;
	[SerializeField]
	GameObject navigation_arrow_bottom;

	[Header("Right Screen")]
	[SerializeField]
	GameObject skill_container;
	[SerializeField]
	GameObject skill_prefab;
	[SerializeField]
	GameObject skill_alert;
	[SerializeField]
	Text skill_alert_title;
	[SerializeField]
	Text skill_alert_description;
	
	public static DungeonCharacterStatsManager Get_DCSM() {
		return (DungeonCharacterStatsManager) HushPuppy.safeFindComponent("GameController", "DungeonCharacterStatsManager");
	}

	void Start() {
		teamManager = TeamManager.Get_Team_Manager();
		team = teamManager.Get_Current_Team();		
	}

	public void Initialize(CharacterDataJSON json) {
		current_char = json;
		current_weapon = InventoryManager.Get_Inventory_Manager().Get_Item_By_ID(json.weapon_ID);

		Switch_Details(true);
		Update_Details();
	}

	public void Switch_Details(bool value) {
		details_container.SetActive(value);
	}

	void Update_Details() {
		Update_Team_Navigation_Arrows();

		//left screen
		portrait.sprite = Resources.Load<Sprite>(
			"Sprites\\" + current_char.raca.ToString() + "\\" + 
			current_char.raca.ToString().ToUpper() + "_headLINE"
		);
		weapon_name.text = current_weapon.nome;

		//right screen
		int k = 0;
		HushPuppy.destroyChildren(skill_container);
		for (int i = 0; i < current_char.skillsPassive.Count; i++) {
			k++;

			GameObject aux = Instantiate(
				skill_prefab,
				skill_container.transform,
				false
			);

			aux.GetComponentInChildren<TeamCharacterSkillDetails>().Initialize(
				k,
				current_char.skillsPassive[i]
			);
		}
	}

	void Update_Team_Navigation_Arrows() {
		int index = team.IndexOf(current_char);
		navigation_arrow_top.SetActive(index != 0);
		navigation_arrow_bottom.SetActive(index != team.Count - 1);
		Toggle_Which_Screen();
		Toggle_Which_Screen();
	}

	public void Exit_Details() {
		Switch_Details(false);
		teamManager.Toggle_Team_Overview();
	}

	public void Navigate_Through_Characters(bool cima) {
		if (cima) {
			current_char = team[team.IndexOf(current_char) - 1];
		} else /*baixo*/ {
			current_char = team[team.IndexOf(current_char) + 1];
		}

		Update_Details();
	}

	public void Toggle_Which_Screen() {
		if (current_screen == Screen.RIGHT) {
			current_screen = Screen.LEFT;

			navigation_arrow_left.SetActive(false);
			navigation_arrow_right.SetActive(true);
			leftScreen.SetActive(false);
			rightScreen.SetActive(true);
		}
		else if (current_screen == Screen.LEFT) {
			current_screen = Screen.RIGHT;

			navigation_arrow_left.SetActive(true);
			navigation_arrow_right.SetActive(false);
			rightScreen.SetActive(false);
			leftScreen.SetActive(true);
		}
	}

	public void Show_Details_Skill(string name) {
		skill_alert.SetActive(true);

		var passive = Resources.Load<PassiveSkillData>("Skills\\Passive\\" + name);
		var active = Resources.Load<ActiveSkillData>("Skills\\Active\\" + name);
		SkillData skill = null;
		if (passive != null) {
			skill = passive;
		}
		else if (active != null) {
			skill = active;
		}
		else {
			print("This should not be happening.");
			print("Skill name: '" + name + "'.");
			Debug.Break();
		}

		skill_alert_title.GetComponentInChildren<Text>().text = skill.title;
		skill_alert_description.GetComponentInChildren<Text>().text = skill.description;
	}

	public void Exit_Details_Skill() {
		skill_alert.SetActive(false);
	}
}