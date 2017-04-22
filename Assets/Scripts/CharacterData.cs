using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass {NULL, LUTADOR, LADINO, GENERAL, BARDO,
							EVOCADOR, BRUXO, EXORCISTA, CURANDEIRO};

[CreateAssetMenu(menuName = "Custom/CharacterData")]
public class CharacterData : ScriptableObject {
	public string nome;
	public CharacterClass classe;
	public Sprite sprite;

	//TEMPORARIO
	public WeaponData weapon;

	[Header("Atributos")]
	public int VIT = 5;
	public int FOR = 5;
	public int DEF = 5; 
	public int INT = 5;
	public int RES = 5;
	public int AGI = 5;
	public int DES = 5;
}