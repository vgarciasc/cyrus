using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviour {
	public static EncounterManager getEncounterManager() {
		return (EncounterManager) HushPuppy.safeFindComponent("GameController", "EncounterManager");
	}

	public EncounterData currentEncounter = null;
	public List<CharacterDataJSON> allies = new List<CharacterDataJSON>();
	public List<CharacterDataJSON> randomPoolCharacters = new List<CharacterDataJSON>();
	public CharacterDataJSON boss_json = new CharacterDataJSON();
	public static bool boss_encounter = false;

	public EncounterData Generate_Encounter() {
		if (boss_encounter) {
			return Generate_Boss_Encounter();
		}

		EncounterData enc = new EncounterData();

		enc.allies = new List<CharacterDataJSON>();
		enc.allies.AddRange(allies);
		enc.encounterName = "Encounter #" + Random.Range(0, 255);
		enc.enemies = new List<CharacterDataJSON>();

		for (int i = 0; i < 4; i++) {
			enc.enemies.Add(Generate_Character());
		}

		return enc;
	}

	public EncounterData Generate_Boss_Encounter() {
		EncounterData enc = new EncounterData();

		enc.allies = new List<CharacterDataJSON>();
		enc.allies.AddRange(allies);
		enc.encounterName = "Encounter #" + Random.Range(0, 255);
		enc.enemies = new List<CharacterDataJSON>();
		enc.enemies.Add(boss_json);

		return enc;
	}

	CharacterDataJSON Generate_Character() {
		return randomPoolCharacters[Random.Range(0, randomPoolCharacters.Count)];
	}
}
