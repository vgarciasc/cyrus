using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkillManager : MonoBehaviour {

	[SerializeField]
	TargetManager targetManager;
	[SerializeField]
	ClickManager clickManager;
	[SerializeField]
	SwapManager swapManager;
	[SerializeField]
	AttackManager attackManager;
	[SerializeField]
	BuffManager buffManager;

	public IEnumerator CastSkill(ActiveSkillData act,
								CharacterObject caster,
								List<CharacterObject> previous_targets,
								int previous_element_index) {
		
		List<CharacterObject> current_targets = new List<CharacterObject>();
		if (previous_targets != null) {
			current_targets.AddRange(previous_targets);
		}

		for (int i = previous_element_index; i < act.elements.Count; i++) {
			switch (act.elements[i].kind) {

				case ElementActiveKind.EFFECT:
					clickManager.end_targetting(caster, true);
					yield return CastEffect(act,
								act.elements[i],
								caster,
								current_targets);
					break;

				case ElementActiveKind.TARGET:
					targetManager.enter_targetting(caster, act.elements[i], current_targets);

					yield return new WaitUntil(() => 
						targetManager.current_state == TargetManager.States.INITIAL_STATE);
					
					CharacterObject tgt = targetManager.GetTarget();
					if (tgt != null) {
						current_targets.Add(tgt);
					}
					else {
						//error occurred, e.g. player may have canceled
						//return to previous targetting, if available
						//if not, you MUST make this targetting

						//previous target is available
						if (i > 0 && act.elements[i - 1].kind == ElementActiveKind.TARGET) {
							current_targets.RemoveAt(current_targets.Count - 1);
							StartCoroutine(CastSkill(act, caster, current_targets, i - 1));
							yield break;
						}
						//previous target is not available
						else {
							bool has_applied_an_effect = false;

							for (int j = 0; j < i - 1; j++) {
								if (act.elements[j].kind == ElementActiveKind.EFFECT) {
									has_applied_an_effect = true;
								}
							}

							if (has_applied_an_effect) {
								//you cannot escape this target
								StartCoroutine(CastSkill(act, caster, current_targets, i));
								yield break;
							}
							else {
								//can safely return to screen
								clickManager.end_targetting(caster, false);
								yield break;
							}
						}
					}

					break;
			}
		}

		clickManager.end_targetting(caster, true);
		yield break;
	}

	public IEnumerator CastEffect(ActiveSkillData act,
								ElementActive elem,
								CharacterObject caster,
								List<CharacterObject> targets) {
		
		int tgt_index_1, tgt_index_2, tgt_index_3;
		CharacterObject tgt_1 = null, tgt_2 = null, tgt_3 = null;

		if (elem.targetIndex.Count > 0) {
			tgt_index_1 = elem.targetIndex[0];
			tgt_1 = tgt_index_1 == -1 ? caster : targets[tgt_index_1];
		}
		if (elem.targetIndex.Count > 1) {
			tgt_index_2 = elem.targetIndex[1];
			tgt_2 = tgt_index_2 == -1 ? caster : targets[tgt_index_2];
		}
		if (elem.targetIndex.Count > 2) {
			tgt_index_3 = elem.targetIndex[2];
			tgt_3 = tgt_index_3 == -1 ? caster : targets[tgt_index_3];
		}

		AttackBonus bonus = new AttackBonus();
		bonus.totalMultiplier = elem.totalAttackMultiplier;

		switch (elem.effect) {
			case EffectActive.SWAP:
				yield return swapManager.swap_characters(tgt_1, tgt_2);
				break;

			case EffectActive.COMPLETE_ATTACK:
				yield return attackManager.complete_attack(tgt_1, tgt_2, bonus);
				break;

			case EffectActive.SIMPLE_ATTACK:
				yield return attackManager.simple_attack(tgt_1, tgt_2, bonus);
				break;

			case EffectActive.SIMPLE_COUNTER:
				yield return attackManager.simple_counter(tgt_1, tgt_2, bonus);
				break;

			case EffectActive.SHOW_LABEL:
				yield return caster.label.showLabel(act.name);
				break;

			case EffectActive.BUFF_DEBUFF:
				if (ViolenceCalculator.pass_test(elem.buff.probability)) {
					yield return buffManager.ApplyBuff(tgt_1, act, elem, caster);
				}
				else {
					yield return tgt_1.dodge_motion();
				}
				break;

			default:
				Debug.Log("This should not be printed.");
				break;
		}
		
		yield break;
	}
}
