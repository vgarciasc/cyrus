using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour {
	[SerializeField]
	GameObject teamOverview;
	[SerializeField]
	Transform teamMembersContainer;
	[SerializeField]
	GameObject bottomButtons;
	[SerializeField]
	DungeonCharacterStatsManager dcsm;
	
	public CharacterData characterData;

	CharacterDatabaseJSON database;

	public static TeamManager Get_Team_Manager() {
		return (TeamManager) HushPuppy.safeFindComponent("GameController", "TeamManager");
	}

	void Start() {
		if (teamOverview != null)
			teamOverview.SetActive(false);
		if (bottomButtons != null)
			bottomButtons.SetActive(true);
		if (teamMembersContainer != null)
			Populate_Roster();

		if (dcsm != null) {
			dcsm.Switch_Details(false);
		}
	}

	public void Toggle_Team_Overview() {
		teamOverview.SetActive(!teamOverview.activeSelf);
		bottomButtons.SetActive(!teamOverview.activeSelf);

		if (teamOverview.activeSelf) {
			Populate_Roster();
		}
	}

	public void Enter_Details() {
		teamOverview.SetActive(!teamOverview.activeSelf);
	}

	#region Serialization
		string Get_Character_Database_Path() {
			var path = Application.streamingAssetsPath + "/Current_Team.json";

			return path;
		}

		void Save_Character_Database_To_Disk(CharacterDatabaseJSON cd) {
			var path = Get_Character_Database_Path();
			var json = JsonUtility.ToJson(cd, true);

			System.IO.StreamWriter sw = System.IO.File.CreateText(path);
			sw.Close();
			System.IO.File.WriteAllText(path, json);

			#if UNITY_EDITOR
				UnityEditor.AssetDatabase.Refresh();
			#endif
		}

		CharacterDatabaseJSON Load_Characters_From_Disk() {
			var path = Get_Character_Database_Path();
			var stream = System.IO.File.OpenText(path);
			var file = stream.ReadToEnd();
			stream.Close();

			CharacterDatabaseJSON items = new CharacterDatabaseJSON();
			JsonUtility.FromJsonOverwrite(file, items);
			database = items;

			return items;
		}

		CharacterDataJSON Serialize_Character_Data(CharacterData data) {
			CharacterDataJSON json = new CharacterDataJSON();
			
			json.nome = data.nome;
			json.raca = data.raca;
			json.classe = data.classe;

			json.AGI = data.AGI;
			json.DEF = data.DEF;
			json.DES = data.DES;
			json.FOR = data.FOR;
			json.INT = data.INT;
			json.RES = data.RES;
			json.VIT = data.VIT;

			json.skillsPassive = new List<string>();
			foreach (PassiveSkillData psv in data.skillsPassive) {
				json.skillsPassive.Add(psv.name);
			}

			json.skillsActive = new List<string>();
			foreach (ActiveSkillData psv in data.skillsActive) {
				json.skillsActive.Add(psv.name);
			}

			return json;
		}
	#endregion

	#region Overview Canvas
		void Populate_Roster() {
			CharacterDatabaseJSON db = Load_Characters_From_Disk();
			
			for (int i = 0; i < 4; i++) {
				CharacterDataJSON json = db.current_team[i];
				teamMembersContainer.GetChild(i).GetComponentInChildren<TeamCharacterDataUI>().Initialize(
					i,
					json
				);
			}
		}
	#endregion

	CharacterDatabaseJSON Get_Current_Database() {
		if (database == null || database.current_team.Count == 0) {
			database = Load_Characters_From_Disk();
		}

		return database;
	}

	public List<CharacterDataJSON> Get_Current_Team() {
		CharacterDatabaseJSON json = Get_Current_Database();
		return json.current_team;
	}

	public void Add_Character(CharacterData data) {
		CharacterDatabaseJSON cd = Load_Characters_From_Disk();
		cd.current_team.Add(Serialize_Character_Data(data));
		Save_Character_Database_To_Disk(cd);
	}

	public void Select_Position_For_Swap(int position) {
		TeamCharacterDataUI selected_1 = null;
		TeamCharacterDataUI selected_2 = null;

		for (int i = 0; i < teamMembersContainer.childCount; i++) {
			Transform t = teamMembersContainer.GetChild(i);
			var aux = t.GetComponentInChildren<TeamCharacterDataUI>();
		
			if (aux.swap_selected) {
				if (selected_1 == null)
					selected_1 = aux;
				else
					selected_2 = aux;
			}
		}

		if (selected_1 != null && selected_2 != null) { //swap
			selected_1.Swap_To(selected_2.transform.position);
			selected_2.Swap_To(selected_1.transform.position);
			
			
			int pos_aux = selected_2.position;
			selected_2.position = selected_1.position;
			selected_1.position = pos_aux;

			CharacterDatabaseJSON db = Get_Current_Database();
			var aux = db.current_team[selected_1.position];
			db.current_team[selected_1.position] = db.current_team[selected_2.position];
			db.current_team[selected_2.position] = aux;

			selected_1.transform.SetSiblingIndex(selected_2.position);
		} else {
			for (int i = 0; i < teamMembersContainer.childCount; i++) {
				Transform t = teamMembersContainer.GetChild(i);
				var x = t.GetComponentInChildren<TeamCharacterDataUI>();
				if (x.position == position) continue;

				x.Deselect_Character_For_Swap();
			}
		}
	}

	public List<int> All_Equipped_Weapon_IDs() {
		List<CharacterDataJSON> cd = Get_Current_Team();
		List<int> output = new List<int>();

		for (int i = 0; i < cd.Count; i++) {
			var id = cd[i].weapon_ID;
			if (!output.Contains(id))
				output.Add(id);
		}

		return output;
	}

	public void Equip_Weapon(CharacterDataJSON character, int weapon_id) {
		print(
			"Character #" + database.current_team.IndexOf(character) + " had weapon #" +
			database.current_team[database.current_team.IndexOf(character)].weapon_ID +
			", now has weapon #" + weapon_id
		);
		database.current_team[database.current_team.IndexOf(character)].weapon_ID = weapon_id;
		Save_Unsaved_Changes();
	}

	public void Save_Unsaved_Changes() {
		Save_Character_Database_To_Disk(database);
	}
}