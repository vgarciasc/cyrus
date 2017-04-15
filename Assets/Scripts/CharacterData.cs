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
	
	[Header("Atributos")]
	public int VIT;
	public int FOR;
	public int DEF; 
	public int INT;
	public int RES;
	public int AGI;
	public int DES;
}