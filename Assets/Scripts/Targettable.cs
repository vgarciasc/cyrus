using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targettable : MonoBehaviour {
	CharacterObject charObj;
	public TargetContainer container;
	public Buffable buffable;

	public void toggle_targetlow(bool value) {
		container.low.SetActive(value);
	}

	public void toggle_targethi(bool value) {
		container.high.SetActive(value);
	}

	public void toggle_targets(bool value) {
		toggle_targetlow(value);
		toggle_targethi(value);
	}

	public CharacterObject GetCharacter() {
		var aux = this.GetComponentInChildren<CharacterObject>();
		if (aux != null) {
			//this is not a slot
			return aux;
		}

		//this is a slot
		var slot = this.GetComponentInChildren<SlotBackground>();
		return null;
	}
}
