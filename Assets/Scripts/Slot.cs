using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour {
	//data variables
	public int positionID = -1;
	public int setID = -1;

	//references
	[SerializeField]
	Image background;

	//temp
	List<Color> colors = new List<Color>() {
		Color.red,
		Color.blue,
		Color.green,
		Color.magenta
	};

	public void set_IDs(int positionID, int setID) {
		this.positionID = positionID;
		set_setID (setID);
	}

	public void set_setID(int new_set) {
		this.setID = new_set;
		updateBackground ();
	}

	void updateBackground() {
		if (setID != -1) {
			this.background.color = colors [setID];
		}
	}
}
