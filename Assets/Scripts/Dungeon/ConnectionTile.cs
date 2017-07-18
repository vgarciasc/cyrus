using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ConnectionTile : MonoBehaviour {
	[SerializeField]
	Image bottom;
	[SerializeField]
	Image right;

	void Start() {
		Reset();	
	}

	public void Reset() {
		Toggle_Bottom(false);
		Toggle_Right(false);
	}

	public void Toggle_Bottom(bool value) {
		bottom.enabled = value;
	}

	public void Toggle_Right(bool value) {
		right.enabled = value;
	}
}
