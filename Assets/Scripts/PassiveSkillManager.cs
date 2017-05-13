using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkillManager : MonoBehaviour {
	[SerializeField]
	BuffManager buffManager;
	[SerializeField]
	ArenaManager arenaManager;

	public IEnumerator on_match_start() {
		List<CharacterObject> possible_casters = new List<CharacterObject>();
		
		possible_casters.AddRange(arenaManager.get_player_column().charObj);
		possible_casters.AddRange(arenaManager.get_enemy_column().charObj);
		
		CharacterObject skill_caster;

		for (int i = 0; i < possible_casters.Count; i++) {
			for (int j = 0; j < possible_casters[i].skillsDeluxe.Count; j++) {
				PassiveSkillData psv = possible_casters[i].skillsDeluxe[j];
				skill_caster = possible_casters[i];

				for (int k = 0; k < psv.triggers.Count; k++) {
					//ON_START_MATCH
					if (psv.triggers[k].kind == TriggerKind.ON_START_MATCH) {
						yield return CastEffects(psv,
												skill_caster);
					}
				}
			}
		}
	}

	public IEnumerator on_turn_start(SlotColumn column) {

		CharacterObject skill_caster;

		for (int i = 0; i < column.charObj.Count; i++) {
			for (int j = 0; j < column.charObj[i].skillsDeluxe.Count; j++) {
				PassiveSkillData psv = column.charObj[i].skillsDeluxe[j];
				skill_caster = column.charObj[i];

				for (int k = 0; k < psv.triggers.Count; k++) {
					//ON_OWN_TURN_START
					if (psv.triggers[k].kind == TriggerKind.ON_OWN_TURN_START) {
						yield return CastEffects(psv,
												skill_caster);
					}
				}
			}
		}
		
		yield break;
	}

	public IEnumerator on_attack(CharacterObject attacker,
								CharacterObject defender,
								Damage dmg) {
		
		CharacterObject skill_caster;

		for (int i = 0; i < attacker.skillsDeluxe.Count; i++) {
			PassiveSkillData psv = attacker.skillsDeluxe[i];
			skill_caster = attacker;

			for (int j = 0; j < psv.triggers.Count; j++) {
				//ON_SUCCESSFUL_ATTACK
				if (psv.triggers[j].kind == TriggerKind.ON_SUCCESSFUL_ATTACK
					&& dmg.amount > 0) {
					yield return CastEffects(attacker,
											defender,
											dmg,
											psv,
											skill_caster);
				}
				//ON_SUCCESSFUL_CRIT
				if (psv.triggers[j].kind == TriggerKind.ON_SUCCESSFUL_CRIT
					&& dmg.crit) {
					yield return CastEffects(attacker,
											defender,
											dmg,
											psv,
											skill_caster);	
				}
			}
		}
	}

	//used majorly on skills activated during battle
	IEnumerator CastEffects(CharacterObject attacker,
							CharacterObject defender,
							Damage dmg,
							PassiveSkillData skill,
							CharacterObject skill_caster) {

		List<Effect> effects = skill.effects;						
		
		for (int i = 0; i < effects.Count; i++) {
			Effect eff = effects[i];
			switch (eff.kind) {
				case EffectKind.BUFF_DEBUFF:
					yield return buffManager.ApplyBuff(attacker,
						defender,
						dmg,
						skill,
						eff,
						skill_caster);
					break;
				case EffectKind.LABEL_SHOW:
					yield return skill_caster.label.showLabel(skill.name);
					break;
			}
		}

		yield break;
	}

	//used majorly on skills activated out of battle
	//ie: on starting turns
	IEnumerator CastEffects(PassiveSkillData skill,
							CharacterObject skill_caster) {

		List<Effect> effects = skill.effects;						
		
		for (int i = 0; i < effects.Count; i++) {
			Effect eff = effects[i];
			switch (eff.kind) {
				case EffectKind.BUFF_DEBUFF:
					yield return buffManager.ApplyBuff(null,
						null,
						null,
						skill,
						eff,
						skill_caster);
					break;
			}
		}

		yield break;
	}
}
