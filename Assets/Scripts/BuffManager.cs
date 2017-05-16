using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class BuffManager : MonoBehaviour {
	public class BuffNumbers {
		public int amount = 0;
		public float multiplier = 0;
	}

	public static BuffNumbers getBuffNumbers(List<Buff> buffs, BuffType type) {
		BuffNumbers bnumbers = new BuffNumbers();

		for (int i = 0; i < buffs.Count; i++) {
			if (buffs[i].kind == type) {
				bnumbers.multiplier += buffs[i].multiplier;
				bnumbers.amount += buffs[i].amount;
			}
		}

		return bnumbers;
	}

	[SerializeField]
	CombatLogManager log;

	//passive skill data
	public IEnumerator ApplyBuff(CharacterObject attacker,
								CharacterObject defender,
								Damage dmg,
								PassiveSkillData skill,
								Effect eff,
								CharacterObject skill_caster) {		

		if (!ViolenceCalculator.pass_test(eff.buff.probability)) {
			Debug.Log("DODGE");
			yield break;
		}

		Debug.Log("Buff '" + eff.buff + "' applied.");
		List<CharacterObject> targets_aux = new List<CharacterObject>();
		
		//show label
		if (eff.alsoShowLabel || eff.kind == EffectKind.LABEL_SHOW) {
			StartCoroutine(skill_caster.label.showLabel(skill.name));
		}

		//build targets
		for (int i = 0; i < eff.targets.Count; i++) {
			var target = eff.targets[i];
			switch (target) {
				case TargetKind.LAST_ATTACKER:
					targets_aux.Add(attacker);
					break;
				case TargetKind.LAST_DEFENDER:
					targets_aux.Add(defender);
					break;
				case TargetKind.ADJACENT_TO_CASTER:
					targets_aux.AddRange(skill_caster.column.get_adjacent_characters(skill_caster));
					break;
				case TargetKind.CASTER:
					targets_aux.Add(skill_caster);
					break;
			}
		}

		//show buff or debuff animation (arrows up and down)
		Coroutine toWait = null;
		if (eff.showBuffDebuffAnimation) {
			for (int i = 0; i < targets_aux.Count; i++) {
				toWait = StartCoroutine(FadeIn_Up_FadeOut(targets_aux[i], eff.buff));
			}

			if (toWait != null) {
				yield return toWait;
			}
		}

		//apply buffs
		for (int i = 0; i < targets_aux.Count; i++) {
			CharacterObject target = targets_aux[i];
			target.take_buffs(new List<Buff> {eff.buff});
		}
	}

	//active skill data
	public IEnumerator ApplyBuff(CharacterObject target,
								ActiveSkillData skill,
								ElementActive elem,
								CharacterObject caster) {		

		Buff buff = elem.buff;

		if (!ViolenceCalculator.pass_test(buff.probability)) {
			yield return target.dodge_motion();
			yield break;
		}

		Debug.Log("Buff '" + elem.buff + "' applied.");
		List<CharacterObject> targets_aux = new List<CharacterObject>();

		if (elem.showBuffLabel) {
			StartCoroutine(caster.label.showLabel(skill.name));
		}

		//show buff or debuff animation (arrows up and down)
		Coroutine toWait = null;
		toWait = StartCoroutine(FadeIn_Up_FadeOut(target, elem.buff));
		yield return toWait;

		//apply buffs
		target.take_buffs(new List<Buff> {elem.buff});
	}

	IEnumerator FadeIn_Up_FadeOut(CharacterObject character, Buff buff) {
		BuffContainer container = character.buffContainer;
		GameObject buffObject;

		if (buff.gender == BuffGender.BUFF) {
			buffObject = container.buff;
		} else /*if (buff.gender == BuffGender.DEBUFF)*/ {
			buffObject = container.debuff;
		}

		buffObject.gameObject.SetActive(true);
		setOpacity(buffObject, 0f);

		Image bg = buffObject.GetComponentInChildren<Image>();
		Vector2 buffOriginalPosition = bg.transform.localPosition;

		Color fullOpacityBG = HushPuppy.getColorWithOpacity(bg.color, 1f);
		Color zeroOpacityBG = HushPuppy.getColorWithOpacity(bg.color, 0f);

		bg.DOColor(fullOpacityBG, 0.5f);
		bg.transform.DOLocalMoveY(bg.transform.localPosition.y + 20f * bg.transform.localScale.y, 0.5f);
		yield return new WaitForSeconds(0.5f);

		bg.DOColor(zeroOpacityBG, 0.5f);
		yield return new WaitForSeconds(0.5f);

		bg.transform.localPosition = buffOriginalPosition;
		buffObject.gameObject.SetActive(false);
	}

	void setOpacity(GameObject buffContainer, float value) {
		Image bg = buffContainer.GetComponentInChildren<Image>();
		bg.color = HushPuppy.getColorWithOpacity(bg.color, value);
	}

	public static float getCriticalMultiplier(Buff b) {
		return getBuffNumbers(new List<Buff> {b}, BuffType.CRIT_MULTIPLIER).multiplier;
	}

	public static float getIgnoreMultiplier(Buff b) {
		return getBuffNumbers(new List<Buff> {b}, BuffType.IGNORE_CHANCE).multiplier;
	}
}
