using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatLogManager : MonoBehaviour {
	[HeaderAttribute("References")]
	[SerializeField]
	Text logText;
	[SerializeField]
	int size = 5;

	List<string> log = new List<string>();

	public void insert(string action) {
		log.Add(action);
		if (log.Count > size) {
			log.RemoveAt(0);
		}
		updateLog();
	}

	void updateLog() {
		string aux = "";
		for (int i = 0; i < log.Count; i++) {
			aux += log[i];
			aux += "\n";
		}

		logText.text = aux;
	}
}
