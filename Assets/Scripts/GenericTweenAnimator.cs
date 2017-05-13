using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GenericTweenAnimator : MonoBehaviour {

	public IEnumerator FadeIn_FadeOut(GameObject container) {
		Image bg = container.GetComponentInChildren<Image>();
		Text text = container.GetComponentInChildren<Text>();

		container.SetActive(true);
		setOpacity(container, 0f);

		Color fullOpacityBG = HushPuppy.getColorWithOpacity(bg.color, 1f);
		Color zeroOpacityBG = HushPuppy.getColorWithOpacity(bg.color, 0f);
		Color fullOpacityTXT = HushPuppy.getColorWithOpacity(text.color, 1f);
		Color zeroOpacityTXT = HushPuppy.getColorWithOpacity(text.color, 0f);

		bg.DOColor(fullOpacityBG, 0.5f);
		text.DOColor(fullOpacityTXT, 0.5f);
		yield return new WaitForSeconds(0.5f);

		bg.DOColor(zeroOpacityBG, 0.5f);
		text.DOColor(zeroOpacityTXT, 0.5f);
	}

	public IEnumerator FadeIn_Up_FadeOut(GameObject container) {
		Image bg = container.GetComponentInChildren<Image>();
		Text text = container.GetComponentInChildren<Text>();

		container.SetActive(true);
		setOpacity(container, 0f);

		Color fullOpacityBG = HushPuppy.getColorWithOpacity(bg.color, 1f);
		Color zeroOpacityBG = HushPuppy.getColorWithOpacity(bg.color, 0f);
		Color fullOpacityTXT = HushPuppy.getColorWithOpacity(text.color, 1f);
		Color zeroOpacityTXT = HushPuppy.getColorWithOpacity(text.color, 0f);

		bg.DOColor(fullOpacityBG, 0.5f);
		text.DOColor(fullOpacityTXT, 0.5f);
		yield return new WaitForSeconds(0.5f);

		yield return new WaitForSeconds(0.5f);

		bg.DOColor(zeroOpacityBG, 0.5f);
		text.DOColor(zeroOpacityTXT, 0.5f);
	}

	void setOpacity(GameObject container, float value) {
		Image bg = container.GetComponentInChildren<Image>();
		Text text = container.GetComponentInChildren<Text>();

		bg.color = HushPuppy.getColorWithOpacity(bg.color, value);
		text.color = HushPuppy.getColorWithOpacity(text.color, value);
	}
}
