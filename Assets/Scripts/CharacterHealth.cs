using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour {
	[HeaderAttribute("References")]
	[SerializeField]
	Text healthIndicator;

	public int hp = 0,
			max = 10;

	public void init(CharacterData data) {
		this.max = data.maxHealth;
		this.hp = max;
		updateIndicator();
	}

	public void add_health(int amount) {
		hp += amount;
		hp = Mathf.Clamp(hp, 0, max);
		updateIndicator();
	}

	void updateIndicator() {
		var msg = hp + " / " + max;
		healthIndicator.text = msg;
	}
}
