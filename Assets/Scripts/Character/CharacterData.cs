using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass {NULL, LUTADOR, LADINO, GENERAL, BARDO,
							EVOCADOR, BRUXO, EXORCISTA, CURANDEIRO};

[CreateAssetMenu(menuName = "Custom/CharacterData")]
public class CharacterData : ScriptableObject {
	public string nome;
	public CharacterClass classe;
    public CharacterRace raca;
	public Sprite sprite;
	public List<PassiveSkillData> skillsPassive = new List<PassiveSkillData>();
	public List<ActiveSkillData> skillsActive = new List<ActiveSkillData>();

	[Header("Atributos")]
	public int VIT = 5;
	public int FOR = 5;
	public int DEF = 5; 
	public int INT = 5;
	public int RES = 5;
	public int AGI = 5;
	public int DES = 5;

	public static bool Can_Equip(CharacterClass classe, WeaponKind type) {
		if (classe == CharacterClass.BRUXO &&
			type == WeaponKind.CAJADO) {
			return true;
		}
		else if (classe == CharacterClass.LUTADOR &&
			type == WeaponKind.ESPADA) {
			return true;
		}
		else if (classe == CharacterClass.GENERAL &&
			type == WeaponKind.MACHADO) {
			return true;
		}
		else if (classe == CharacterClass.LADINO &&
			type == WeaponKind.ADAGA) {
			return true;
		}
		else {
			return false;
		}
	}
}