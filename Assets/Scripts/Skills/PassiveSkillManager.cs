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
			for (int j = 0; j < possible_casters[i].passiveSkills.Count; j++) {
				PassiveSkillData psv = possible_casters[i].passiveSkills[j];
				skill_caster = possible_casters[i];

				for (int k = 0; k < psv.triggers.Count; k++) {
					TriggerKind trigger = psv.triggers[k].kind;

					//ON_START_MATCH
					if (trigger == TriggerKind.ON_START_MATCH) {
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
			if (column.charObj[i].is_dead()) {
				continue;
			}

			for (int j = 0; j < column.charObj[i].passiveSkills.Count; j++) {
				PassiveSkillData psv = column.charObj[i].passiveSkills[j];
				skill_caster = column.charObj[i];

				for (int k = 0; k < psv.triggers.Count; k++) {
					TriggerKind trigger = psv.triggers[k].kind;
					bool should_cast = false;

					//ON_OWN_TURN_START
					if (trigger == TriggerKind.ON_OWN_TURN_START) {
						should_cast = true;
					}

					if (should_cast) {
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

		for (int i = 0; i < attacker.passiveSkills.Count; i++) {
			PassiveSkillData psv = attacker.passiveSkills[i];
			skill_caster = attacker;
			
			if (should_cast(dmg, psv, true)) {
				yield return CastEffects(attacker,
										defender,
										dmg,
										psv,
										skill_caster);	
			}
		}

		for (int i = 0; i < defender.passiveSkills.Count; i++) {
			PassiveSkillData psv = defender.passiveSkills[i];
			skill_caster = defender;

			if (should_cast(dmg, psv, false)) {
				yield return CastEffects(attacker,
										defender,
										dmg,
										psv,
										skill_caster);	
			}
		}
	}

	public IEnumerator on_counter_attack(CharacterObject attacker,
										CharacterObject defender,
										Damage dmg) {
		
		yield return StartCoroutine(on_attack(attacker, defender, dmg));

		yield break;
	}

	public IEnumerator on_block(CharacterObject blocker,
								CharacterObject attacker,
								CharacterObject defender,
								AttackModule mod) {

		CharacterObject skill_caster;

		for (int j = 0; j < attacker.column.charObj.Count; j++) {
			skill_caster =  attacker.column.charObj[j];
			for (int i = 0; i < skill_caster.passiveSkills.Count; i++) {
				PassiveSkillData psv = skill_caster.passiveSkills[i];
				
				if (should_cast_protect_ally(blocker, attacker, defender, mod, psv, false)) {
					yield return CastEffects(psv,
											skill_caster);	
				}
			}
		}

		for (int j = 0; j < defender.column.charObj.Count; j++) {
			skill_caster =  defender.column.charObj[j];
			for (int i = 0; i < skill_caster.passiveSkills.Count; i++) {
				PassiveSkillData psv = skill_caster.passiveSkills[i];
				bool caster_is_blocker = skill_caster == blocker;

				if (should_cast_protect_ally(blocker, attacker, defender, mod, psv, caster_is_blocker)) {
					yield return CastEffects(psv,
											skill_caster);	
				}
			}
		}

		yield break;
	}	

	#region getters of probability
		public float Get_Crit_Probability(CharacterObject attacker,
									CharacterObject defender,
									AttackModule mod) {
			
			float prob = 0;

			for (int i = 0; i < attacker.passiveSkills.Count; i++) {
				PassiveSkillData psv = attacker.passiveSkills[i];

				if (should_cast(attacker, defender, mod, psv, true)) {
					for (int k = 0; k < psv.effects.Count; k++) {
						if (psv.effects[k].kind == EffectPassive.BUFF_DEBUFF &&
							ViolenceCalculator.pass_test(psv.effects[k].buff.probability)) {
							prob += BuffManager.getCriticalMultiplier(psv.effects[k].buff);
						}
					}
				}
			}

			for (int i = 0; i < defender.passiveSkills.Count; i++) {
				PassiveSkillData psv = defender.passiveSkills[i];

				if (should_cast(attacker, defender, mod, psv, false)) {
					for (int k = 0; k < psv.effects.Count; k++) {
						if (psv.effects[k].kind == EffectPassive.BUFF_DEBUFF &&
							ViolenceCalculator.pass_test(psv.effects[k].buff.probability)) {
							prob += BuffManager.getCriticalMultiplier(psv.effects[k].buff);
						}
					}
				}
			}

			return prob;
		}

		public float Get_Ignore_Probability(CharacterObject attacker,
									CharacterObject defender,
									AttackModule mod) {
			
			float prob = 0;

			for (int i = 0; i < attacker.passiveSkills.Count; i++) {
				PassiveSkillData psv = attacker.passiveSkills[i];

				if (should_cast(attacker, defender, mod, psv, true)) {
					for (int k = 0; k < psv.effects.Count; k++) {
						if (psv.effects[k].kind == EffectPassive.BUFF_DEBUFF &&
							ViolenceCalculator.pass_test(psv.effects[k].buff.probability)) {
							prob += BuffManager.getIgnoreMultiplier(psv.effects[k].buff);
						}
					}
				}
			}

			for (int i = 0; i < defender.passiveSkills.Count; i++) {
				PassiveSkillData psv = defender.passiveSkills[i];

				if (should_cast(attacker, defender, mod, psv, false)) {
					for (int k = 0; k < psv.effects.Count; k++) {
						if (psv.effects[k].kind == EffectPassive.BUFF_DEBUFF &&
							ViolenceCalculator.pass_test(psv.effects[k].buff.probability)) {
							prob += BuffManager.getIgnoreMultiplier(psv.effects[k].buff);
						}
					}
				}
			}

			return prob;
		}
		
		public float Get_Block_Probability(CharacterObject attacker,
										CharacterObject defender,
										CharacterObject potential_blocker,
										AttackModule mod) {
			
			float prob = 0;

			for (int j = 0; j < potential_blocker.passiveSkills.Count; j++) {
				PassiveSkillData psv = potential_blocker.passiveSkills[j];

				if (should_cast_protect_ally(potential_blocker, attacker, defender, mod, psv, true)) {
					for (int k = 0; k < psv.effects.Count; k++) {
						if (psv.effects[k].kind == EffectPassive.PROTECT_ALLY) {
							Debug.Log("Added BLOCK_CHANCE from " + potential_blocker + " in skill " + psv.name + ".");
							prob += psv.effects[k].protectAlly.probability;
						}
					}
				}
			}

			prob = Mathf.Clamp(prob, 0f, 1f);
			return prob;
		}
	#endregion

	//used after attack has been calculated
	bool should_cast(Damage dmg,
					PassiveSkillData psv,
					bool caster_is_attacker) {

		bool should_cast_OR = false;
		bool should_cast_AND = true;

		for (int j = 0; j < psv.triggers.Count; j++) {
			TriggerKind trigger = psv.triggers[j].kind;

			//ON_MAKE_ATTACK
			if (trigger == TriggerKind.ON_MAKE_ATTACK && caster_is_attacker) {
				should_cast_OR = true;
				continue;
			}
			//ON_RECEIVE_ATTACK
			if (trigger == TriggerKind.ON_RECEIVE_ATTACK && !caster_is_attacker) {
				should_cast_OR = true;
				continue;
			}

			//ON_SUCCESSFUL_ATTACK
			else if (trigger == TriggerKind.ON_SUCCESSFUL_ATTACK
				&& dmg.amount > 0) {
				should_cast_OR = true;
				continue;
			}
			//ON_CRIT_ATTACK
			else if (trigger == TriggerKind.ON_CRIT_ATTACK
				&& dmg.crit) {
				should_cast_OR = true;
				continue;
			}
			//ON_PHYSICAL_ATTACK
			else if (trigger == TriggerKind.ON_PHYSICAL_ATTACK
				&& dmg.attackKind == AttackKind.PHYSICAL) {
				should_cast_OR = true;
				continue;
			}
			//ON_MAGICAL_ATTACK
			else if (trigger == TriggerKind.ON_MAGICAL_ATTACK
				&& dmg.attackKind == AttackKind.MAGICAL) {
				should_cast_OR = true;
				continue;
			}
			//ON_COUNTER_ATTACK
			else if (trigger == TriggerKind.ON_SIMPLE_COUNTER
				&& dmg.attackModule == AttackModule.COUNTER_ATTACK) {
				should_cast_OR = true;
				continue;
			}
			//ON_SIMPLE_ATTACK
			else if (trigger == TriggerKind.ON_SIMPLE_ATTACK
				&& dmg.attackModule == AttackModule.NORMAL_ATTACK) {
				should_cast_OR = true;
				continue;
			}
			//ON_MISS_ATTACK
			else if (trigger == TriggerKind.ON_MISS_ATTACK
				&& dmg.amount == 0) {
				should_cast_OR = true;
				continue;
			}
			else {
				should_cast_AND = false;
			}
		}

		should_cast_AND &= should_cast_OR;
		bool should_cast = false;
		if (psv.operation == PassiveSkillData.TriggerOperation.AND && should_cast_AND) {
			should_cast = true;
		}
		else if (psv.operation == PassiveSkillData.TriggerOperation.OR && should_cast_OR) {
			should_cast = true;
		}

		return should_cast;
	}

	//used during attack calculations
	bool should_cast(CharacterObject attacker,
					CharacterObject defender,
					AttackModule mod,
					PassiveSkillData psv,
					bool caster_is_attacker) {
		
		bool should_cast_OR = false;
		bool should_cast_AND = true;
		
		for (int j = 0; j < psv.triggers.Count; j++) {
			TriggerKind trigger = psv.triggers[j].kind;

			//ON_MAKE_ATTACK
			if (trigger == TriggerKind.ON_MAKE_ATTACK && caster_is_attacker) {
				should_cast_OR = true;
				continue;
			}
			//ON_RECEIVE_ATTACK
			else if (trigger == TriggerKind.ON_RECEIVE_ATTACK && !caster_is_attacker) {
				should_cast_OR = true;
				continue;
			}
			//ON_SIMPLE_ATTACK
			else if (trigger == TriggerKind.ON_SIMPLE_ATTACK &&
				mod == AttackModule.NORMAL_ATTACK) {
				should_cast_OR = true;
				continue;
			}
			//ON_COUNTER_ATTACK
			else if (trigger == TriggerKind.ON_SIMPLE_COUNTER &&
				mod == AttackModule.COUNTER_ATTACK) {
				should_cast_OR = true;
				continue;
			}
			//ON_PHYSICAL_ATTACK
			else if (trigger == TriggerKind.ON_PHYSICAL_ATTACK
				&& ViolenceCalculator.get_attack_kind(attacker, defender, mod) == AttackKind.PHYSICAL) {
				should_cast_OR = true;
				continue;
			}
			//ON_MAGICAL_ATTACK
			else if (trigger == TriggerKind.ON_MAGICAL_ATTACK
				&& ViolenceCalculator.get_attack_kind(attacker, defender, mod) == AttackKind.MAGICAL) {
				should_cast_OR = true;
				continue;
			}
			else {
				should_cast_AND = false;
			}
		}

		should_cast_AND &= should_cast_OR;
		bool should_cast = false;
		if (psv.operation == PassiveSkillData.TriggerOperation.AND && should_cast_AND) {
			should_cast = true;
		}
		else if (psv.operation == PassiveSkillData.TriggerOperation.OR && should_cast_OR) {
			should_cast = true;
		}

		return should_cast;
	}

	//used by protect ally
	bool should_cast_protect_ally(CharacterObject blocker,
						CharacterObject attacker,
						CharacterObject defender,
						AttackModule mod,
						PassiveSkillData psv,
						bool caster_is_blocker) {
		
		bool should_cast_OR = false;
		bool should_cast_AND = true;
		
		for (int j = 0; j < psv.triggers.Count; j++) {
			TriggerKind trigger = psv.triggers[j].kind;

			//ON_SIMPLE_ATTACK
			if (trigger == TriggerKind.ON_SIMPLE_ATTACK &&
				mod == AttackModule.NORMAL_ATTACK) {
				should_cast_OR = true;
				continue;
			}
			//ON_COUNTER_ATTACK
			else if (trigger == TriggerKind.ON_SIMPLE_COUNTER &&
				mod == AttackModule.COUNTER_ATTACK) {
				should_cast_OR = true;
				continue;
			}
			//ON_PHYSICAL_ATTACK
			else if (trigger == TriggerKind.ON_PHYSICAL_ATTACK
				&& ViolenceCalculator.get_attack_kind(attacker, defender, mod) == AttackKind.PHYSICAL) {
				should_cast_OR = true;
				continue;
			}
			//ON_MAGICAL_ATTACK
			else if (trigger == TriggerKind.ON_MAGICAL_ATTACK
				&& ViolenceCalculator.get_attack_kind(attacker, defender, mod) == AttackKind.MAGICAL) {
				should_cast_OR = true;
				continue;
			}
			//ON_ADJACENT_ALLY_ATTACKED
			else if (trigger == TriggerKind.ON_ADJACENT_ALLY_ATTACKED
				&& defender.column.get_adjacent_characters(defender).Contains(blocker)) {
				should_cast_OR = true;
				continue;
			}
			//ON_ANY_ALLY_ATTACKED
			else if (trigger == TriggerKind.ON_ANY_ALLY_ATTACKED
				&& blocker != defender) {
				should_cast_OR = true;
				continue;
			}
			//ON_BLOCK
			else if (trigger == TriggerKind.ON_BLOCK
				&& caster_is_blocker) {
				should_cast_OR = true;
				continue;
			}
			//ON_BEING_DEFENDED_WITH_BLOCK
			else if (trigger == TriggerKind.ON_BEING_DEFENDED_WITH_BLOCK
				&& !caster_is_blocker) {
				should_cast_OR = true;
				continue;
			}
			else {
				should_cast_AND = false;
			}
		}

		should_cast_AND &= should_cast_OR;

		bool should_cast = false;
		if (psv.operation == PassiveSkillData.TriggerOperation.AND && should_cast_AND) {
			should_cast = true;
		}
		else if (psv.operation == PassiveSkillData.TriggerOperation.OR && should_cast_OR) {
			should_cast = true;
		}

		return should_cast;
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
				case EffectPassive.BUFF_DEBUFF:
					yield return buffManager.ApplyBuff_ToCharacter(attacker,
						defender,
						dmg,
						skill,
						eff,
						skill_caster);
					break;

				case EffectPassive.LABEL_SHOW:
					yield return skill_caster.label.showLabel(skill.title);
					break;
					
				default:
					Debug.Log("This should not be happening.");
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
				case EffectPassive.BUFF_DEBUFF:
					yield return buffManager.ApplyBuff_ToCharacter(null,
						null,
						null,
						skill,
						eff,
						skill_caster);
					break;
				case EffectPassive.LABEL_SHOW:
					yield return skill_caster.label.showLabel(skill.title);
					break;
				default:
					Debug.Log("This should not be happening. Skill not being cast: " + skill + " , " + eff.kind);
					break;
			}
		}

		yield break;
	}
}
