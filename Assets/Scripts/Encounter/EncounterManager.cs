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

	public EncounterData Generate_Encounter() {
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

	CharacterDataJSON Generate_Character() {
		return randomPoolCharacters[Random.Range(0, randomPoolCharacters.Count)];
	}
}
