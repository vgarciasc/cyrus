using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Barrier : MonoBehaviour {
	[HeaderAttribute("Components")]
	[SerializeField]
	Image barrier;
	[SerializeField]
	Text health;

	public int points = 0;

	void Start() {
		toggle(false);
	}

	public void Insert() {
		health.text = points.ToString();
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
			int dano_transbordando = points;
			points = 0;
			Remove();
			return dano_transbordando;
		}

		health.text = points.ToString();
		return 0;
	}

	public void toggle(bool value) {
		health.gameObject.SetActive(value);
		barrier.gameObject.SetActive(value);
	}

	public void reset() {
		add_health(0);
	}
}
