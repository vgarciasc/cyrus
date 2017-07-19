using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour {
	//data variables
	public int positionID = -1;
	public int charID = -1;

	//references
	[HeaderAttribute("References")]
	[SerializeField]
	SlotColumn column;

	public void set_IDs(int positionID, int charID) {
		this.positionID = positionID;
		set_charID (charID);
	}

	public void set_charID(int new_set) {
		this.charID = new_set;
	}

	public SlotColumn get_column() {
		return column;
	}

	//35 IS MAGIC NUMBER
	public Vector3 get_character_position(int multiplier) {
		return this.transform.position + new Vector3(0f, 35f / multiplier);
	}
}
