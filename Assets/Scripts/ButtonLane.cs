using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLane : MonoBehaviour {

	[HeaderAttribute("References")]
	public GameObject buttonContainer;
	public List<GameObject> buttons = new List<GameObject>();

	public bool active = false;

	void Start() {
		toggle_lane(false);
	}

	public void toggle_lane(bool value) {
		active = value;
		buttonContainer.SetActive(value);
	}
}
