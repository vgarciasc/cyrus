using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class CharacterObject : MonoBehaviour {
	[HeaderAttribute("References")]
	public CharacterData data;

	Image img;

	public void set_data(CharacterData data) {
		this.data = data;
		init();
	}

	public void set_new_pos(Vector3 position) {
		this.transform.DOMove(position, 0.6f, false);
	}

	public void kill() {
		this.gameObject.SetActive(false);
	}

	void init() {
		img = GetComponentInChildren<Image>();
		img.sprite = data.sprite;
	}
}
