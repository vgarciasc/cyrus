using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Barrier : MonoBehaviour {
	[HeaderAttribute("Components")]
	[SerializeField]
	Image barrier;

	public int points = 0;

	void Start() {
		toggle(false);
	}

	public void Insert() {
		toggle(true);
	}

	public void Remove() {
		toggle(false);
	}

	public int add_health(int amount) {
		//if barrier was non-existant, now it exists
		if (points == 0 && amount > 0) {
			points += amount;
			Insert();
			return 0;
		}

		points += amount;
		
		//if barrier is negative, now it doesnt exist
		if (points < 0) {
			points = 0;
			Remove();
			return points;
		}

		return 0;
	}

	void toggle(bool value) {
		barrier.enabled = value;
	}
}
