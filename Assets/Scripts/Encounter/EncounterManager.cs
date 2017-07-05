using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviour {
	public static EncounterManager getEncounterManager() {
		return (EncounterManager) HushPuppy.safeFindComponent("EncounterManager", "EncounterManager");
	}

	public EncounterData currentEncounter;
	
	public void Initiate_Encounter(EncounterData encounter) {
		currentEncounter = encounter;

		//destroys old copies
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("EncounterManager")) {
			if (go != this.gameObject) {
				Destroy(go);
			}
		}

		DontDestroyOnLoad(this.gameObject);
		SceneManager.LoadScene("MainScene");
	}
}
