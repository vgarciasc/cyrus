﻿using System.Collections;
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
	public IEnumerator ApplyBuff_ToCharacter(CharacterObject attacker,
								CharacterObject defender,
								Damage dmg,
								PassiveSkillData skill,
								Effect eff,
								CharacterObject skill_caster) {		

		Buff buff = new Buff(eff.buff);

		if (!ViolenceCalculator.pass_test(eff.buff.probability)) {
			Debug.Log("DODGE");
			yield break;
		}

		List<CharacterObject> targets_aux = new List<CharacterObject>();
		
		//show label
		if (eff.alsoShowLabel || eff.kind == EffectPassive.LABEL_SHOW) {
			StartCoroutine(skill_caster.label.showLabel(skill.title));
		}

		//build targets
		for (int i = 0; i < eff.targets.Count; i++) {
			var target = eff.targets[i];
			switch (target) {
				case TargetPassive.LAST_ATTACKER:
					targets_aux.Add(attacker);
					break;
				case TargetPassive.LAST_DEFENDER:
					targets_aux.Add(defender);
					break;
				case TargetPassive.ADJACENT_TO_CASTER:
					targets_aux.AddRange(skill_caster.column.get_adjacent_characters(skill_caster));
					break;
				case TargetPassive.CASTER:
					targets_aux.Add(skill_caster);
					break;
			}
		}

		//show buff or debuff animation (arrows up and down)
		Coroutine toWait = null;
		if (eff.showBuffDebuffAnimation) {
			for (int i = 0; i < targets_aux.Count; i++) {
				toWait = StartCoroutine(FadeIn_Up_FadeOut(targets_aux[i].status.container, buff));
			}

			if (toWait != null) {
				yield return toWait;
			}
		}

		buff.skillName = skill.title;

		//apply buffs
		for (int i = 0; i < targets_aux.Count; i++) {
			CharacterObject target = targets_aux[i];
			target.take_buffs(new List<Buff> {buff});
		}
	}

	//active skill data
	public IEnumerator ApplyBuff(Targettable target,
								ActiveSkillData skill,
								ElementActive elem,
								CharacterObject caster) {		

		Buff buff = new Buff(elem.buff);

		if (!ViolenceCalculator.pass_test(buff.probability)) {
			Debug.Log("Buff " + buff + " did not pass the " + buff.probability * 100 + "% chance.");
			yield break;
		}

		Debug.Log("Buff '" + elem.buff + "' applied.");

		if (elem.showBuffLabel) {
			StartCoroutine(caster.label.showLabel(skill.title));
		}

		//show buff or debuff animation (arrows up and down)
		Coroutine toWait = null;
		toWait = StartCoroutine(FadeIn_Up_FadeOut(target.buffable.container, elem.buff));
		yield return toWait;

		//if delayed attack, store damage
		if (elem.buff.kind == BuffType.DELAYED_ATTACK) {
			buff.amount = ViolenceCalculator.theoretical_damage(
				caster,
				target.GetCharacter(),
				AttackModule.NORMAL_ATTACK
			);
		}

		buff.skillName = skill.title;

		//apply buffs
		target.buffable.insert(new List<Buff> {buff});
	}

	IEnumerator FadeIn_Up_FadeOut(BuffContainer container, Buff buff) {
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
