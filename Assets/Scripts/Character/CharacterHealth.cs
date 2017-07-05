using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterHealth : MonoBehaviour {
	[HeaderAttribute("References")]
	[SerializeField]
	Text healthIndicator;
	[SerializeField]
	CharacterObject character;
	[SerializeField]
	Transform heal_image;

	public int hp = 0,
			max = 10;

	void Update() {
		update_barrier();
	}

	public void update_barrier() {
		// SlotBackground slotbg = character.column.get_slotbg_by_charobj(character);
		// if (slotbg.slotBuff.barrier.points > 0 && character.status.barrier.points > 0) {
		// 	character.status.barrier.displayed_points = slotbg.slotBuff.barrier.points + 
		// 		character.status.barrier.points;
		// 	slotbg.slotBuff.barrier.toggle(false);
		// }		
		// else {
		// 	character.status.barrier.reset();
		// 	slotbg.slotBuff.barrier.reset();
		// }
	}

	public void init(CharacterData data) {
		this.max = data.VIT;
		this.hp = max;
		updateIndicator();
	}

	public void add_health(int amount) {
		//if take damage and barrier is up
		if (amount < 0 && character.status.barrier.points > 0) {
			amount = character.status.barrier.add_health(amount);
		}

		//THIS COULD BE BUGGY		
		SlotBackground slotbg = character.column.get_slotbg_by_charobj(character);
		if (amount < 0 && slotbg.slotBuff.barrier.points > 0) {
			
			amount = slotbg.slotBuff.barrier.add_health(amount);
		}

		hp += amount;
		hp = Mathf.Clamp(hp, 0, max);
		updateIndicator();
	}

	void updateIndicator() {
		var msg = hp + " / " + max;
		healthIndicator.text = msg;
	}

	public IEnumerator heal_anim() {
		heal_image.gameObject.SetActive(true);
		setOpacity(heal_image, 0f);

		Image bg = heal_image.GetComponentInChildren<Image>();
		Vector2 buffOriginalPosition = bg.transform.localPosition;

		Color fullOpacityBG = HushPuppy.getColorWithOpacity(bg.color, 1f);
		Color zeroOpacityBG = HushPuppy.getColorWithOpacity(bg.color, 0f);

		bg.DOColor(fullOpacityBG, 0.5f);
		bg.transform.DOLocalMoveY(bg.transform.localPosition.y + 20f * bg.transform.localScale.y, 0.5f);
		yield return new WaitForSeconds(0.5f);

		bg.DOColor(zeroOpacityBG, 0.5f);
		yield return new WaitForSeconds(0.5f);

		bg.transform.localPosition = buffOriginalPosition;
		heal_image.gameObject.SetActive(false);
	}

	void setOpacity(Transform image, float value) {
		Image bg = image.GetComponentInChildren<Image>();
		bg.color = HushPuppy.getColorWithOpacity(bg.color, value);
	}
}
