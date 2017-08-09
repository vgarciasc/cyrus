using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour {

	public static Sprite Get_Weapon_Sprite(string weapon_name) {
		var aux = Resources.Load<Sprite>(
			"Sprites\\Weapon\\" + 
			weapon_name
		);
		
		if (aux == null) {
			Debug.Log("Sprite with name '" + weapon_name + "' was not found.");
		}

		return aux;
	}
}
