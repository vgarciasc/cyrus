using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EncounterData {
	public string encounterName = "DEFAULT ENCOUNTER";
	
	[Header("Enemies")]
	public List<CharacterDataJSON> enemies = new List<CharacterDataJSON>();

	[Header("Allies")]
	public List<CharacterDataJSON> allies = new List<CharacterDataJSON>();
}
