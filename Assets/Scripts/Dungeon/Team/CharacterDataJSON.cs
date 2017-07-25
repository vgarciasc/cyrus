using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterDataJSON {
	public string nome;
	public CharacterClass classe;
    public CharacterRace raca;
	public List<string> skillsPassive = new List<string>();
	public List<string> skillsActive = new List<string>();

	public int weapon_ID;

	[Header("Atributos")]
	public int VIT = 5;
	public int FOR = 5;
	public int DEF = 5; 
	public int INT = 5;
	public int RES = 5;
	public int AGI = 5;
	public int DES = 5;
}

[System.Serializable]
public class CharacterDatabaseJSON {
	public List<CharacterDataJSON> database = new List<CharacterDataJSON>();
	public List<CharacterDataJSON> current_team = new List<CharacterDataJSON>();
}