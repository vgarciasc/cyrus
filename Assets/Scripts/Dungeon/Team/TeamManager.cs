using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour {
	[SerializeField]
	GameObject teamOverview;
	[SerializeField]
	GameObject bottomButtons;

	void Start() {
		teamOverview.SetActive(false);
		bottomButtons.SetActive(true);
	}

	public void Toggle_Team_Overview() {
		teamOverview.SetActive(!teamOverview.activeSelf);
		bottomButtons.SetActive(!teamOverview.activeSelf);
	}
}