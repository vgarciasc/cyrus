using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour {
	[HeaderAttribute("References")]
	[SerializeField]
	Text healthIndicator;
	[SerializeField]
	CharacterObject character;

	public int hp = 0,
			max = 10;

	public void init(CharacterData data) {
		this.max = data.VIT;
		this.hp = max;
		updateIndicator();
	}

	public void add_health(int amount) {
		//if take damage and barrier is up
		if (amount < 0 && character.barrier.points > 0) {
			amount = character.barrier.add_health(amount);
		}

		hp += amount;
		hp = Mathf.Clamp(hp, 0, max);
		updateIndicator();
	}

	void updateIndicator() {
		var msg = hp + " / " + max;
		healthIndicator.text = msg;
	}
}
