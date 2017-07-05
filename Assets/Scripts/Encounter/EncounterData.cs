using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Encounter Data")]
public class EncounterData : ScriptableObject {
	public string encounterName = "DEFAULT ENCOUNTER";
	
	[Header("Enemies")]
	public List<CharacterData> enemies = new List<CharacterData>();

	[Header("Allies")]
	public List<CharacterData> allies = new List<CharacterData>();
}
