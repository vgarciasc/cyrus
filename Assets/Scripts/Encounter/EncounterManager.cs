using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviour {
	public static EncounterManager getEncounterManager() {
		return (EncounterManager) HushPuppy.safeFindComponent("GameController", "EncounterManager");
	}

	public EncounterData currentEncounter = null;
	public List<CharacterData> allies = new List<CharacterData>();
	public List<CharacterData> randomPoolCharacters = new List<CharacterData>();

	public EncounterData Generate_Encounter() {
		EncounterData enc = new EncounterData();

		enc.allies = new List<CharacterData>();
		enc.allies.AddRange(allies);
		enc.encounterName = "Encounter #" + Random.Range(0, 255);
		enc.enemies = new List<CharacterData>();

		for (int i = 0; i < 4; i++) {
			enc.enemies.Add(Generate_Character());
		}

		return enc;
	}

	CharacterData Generate_Character() {
		return randomPoolCharacters[Random.Range(0, randomPoolCharacters.Count)];
	}
}
