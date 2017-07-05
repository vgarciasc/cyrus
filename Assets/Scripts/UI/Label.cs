using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Label : MonoBehaviour {
	[HeaderAttribute("References")]
	[SerializeField]
	GameObject container;
	[SerializeField]
	Text text;
	[SerializeField]
	Image bg;

	string labelName = "default";

	void Start() {
		container.SetActive(false);	
	}

	public void setLabelName(string name) {
		labelName = name;
		text.text = labelName;
	}

	public IEnumerator showLabel(string name) {
		setLabelName(name);
		yield return StartCoroutine(show());
	}

	IEnumerator show() {
		container.SetActive(true);
		setOpacity(0f);

		Color fullOpacityBG = HushPuppy.getColorWithOpacity(bg.color, 1f);
		Color zeroOpacityBG = HushPuppy.getColorWithOpacity(bg.color, 0f);
		Color fullOpacityTXT = HushPuppy.getColorWithOpacity(text.color, 1f);
		Color zeroOpacityTXT = HushPuppy.getColorWithOpacity(text.color, 0f);

		bg.DOColor(fullOpacityBG, 0.5f);
		text.DOColor(fullOpacityTXT, 0.5f);
		yield return new WaitForSeconds(0.5f);

		bg.DOColor(zeroOpacityBG, 0.5f);
		text.DOColor(zeroOpacityTXT, 0.5f);
		yield return new WaitForSeconds(0.5f);

		container.SetActive(false);
	}

	void setOpacity(float value) {
		Image bg = container.GetComponentInChildren<Image>();
		Text text = container.GetComponentInChildren<Text>();

		bg.color = HushPuppy.getColorWithOpacity(bg.color, value);
		text.color = HushPuppy.getColorWithOpacity(text.color, value);
	}
}
