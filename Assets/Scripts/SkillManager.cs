using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour {
	[SerializeField]
	CombatLogManager log;
	
	public void cast(CharacterObject user, SkillData skill) {
		user.use_action();

		log.insert("<color=magenta>SKILL</color>: '<color=gray>" + user.data.nome +
			"</color>' used <color=gray>" + skill.title.ToUpper() + "</color>.");
	}

	#region on_attack
		public List<Buff> on_attack_buffs(CharacterObject attacker,
							CharacterObject defender,
							AttackModule module) {
			List<Buff> buffs = new List<Buff>();

			for (int i = 0; i < attacker.skills.Count; i++) {
				if (on_attack_should_activate(attacker, defender, module, attacker.skills[i])) {
					buffs.AddRange(attacker.skills[i].buffs);

					log.insert("<color=magenta>SKILL</color>: '<color=gray>" + attacker.data.nome +
					"</color>' used <color=gray>" + attacker.skills[i].title.ToUpper() + "</color> on '<color=gray>" +
					defender.data.nome + "'</color>.");
				}
			}

			return buffs;
		}

		public bool on_attack_should_activate(CharacterObject attacker,
							CharacterObject defender,
							AttackModule module,
							SkillData skill) {
			
			if (skill.targetingStyle != SkillTargeting.ON_ATTACK) {
				return false;
			}

			if (module == AttackModule.COUNTER_ATTACK &&
				!skill.activateEvenWhenCounterAttack) {
				return false;
			}

			return true;
		}
	#endregion
}
