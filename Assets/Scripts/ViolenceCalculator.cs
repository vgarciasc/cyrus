using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackModule {NORMAL_ATTACK, COUNTER_ATTACK};
public enum AttackKind {PHYSICAL, MAGICAL};

public class AttackBonus {
	public float totalMultiplier = 1f;
}

public class ViolenceCalculator : MonoBehaviour {
	[SerializeField]
	CombatLogManager log;
	[SerializeField]
	PassiveSkillManager passiveManager;

	string log_info = "";

	public Damage effective_damage(CharacterObject attacker,
								CharacterObject defender,
								AttackModule mod,
								AttackBonus bonus) {

		Damage dmg = new Damage();

		var aux = "";
		int accuracy = accuracy_prob(attacker, defender, mod);
		
		if (!pass_test(accuracy)) {
			if (mod == AttackModule.COUNTER_ATTACK) aux = "<color=red>COUNTER: </color>";
			else if (mod == AttackModule.NORMAL_ATTACK) aux = "<color=green>NORMAL: </color>";
			log.insert(aux + "'<color=gray>" + attacker.data.nome +
						"</color>' has not passed the accuracy test (<color=gray>" + 
						accuracy + "%</color>).");
			dmg.amount = 0;
			return dmg;
		}

		if (pass_test(ignore_prob(attacker, defender, mod))) {
			Debug.Log("Attack ignored.");
			dmg.amount = 0;
			return dmg;
		}

		dmg.attackKind = get_attack_kind(attacker, defender, mod);
		dmg.attackModule = mod;

		int amount = theoretical_damage(attacker, defender, mod);
		int crit = critical_prob(attacker, defender, mod);

		if (pass_test(crit)) {
			if (mod == AttackModule.COUNTER_ATTACK) aux = "<color=red>COUNTER: </color>";
			else if (mod == AttackModule.NORMAL_ATTACK) aux = "<color=green>NORMAL: </color>";
			log.insert(aux + "'<color=gray>" + attacker.data.nome + "</color>' has passed the critical test (<color=gray>"
						+ crit + "%</color>).");
			amount *= 2;
			dmg.crit = true;
		}

		if (mod == AttackModule.COUNTER_ATTACK) aux = "<color=red>COUNTER: </color>";
		else if (mod == AttackModule.NORMAL_ATTACK) aux = "<color=green>NORMAL: </color>";
		log.insert(aux + "'<color=gray>" + attacker.data.nome + "</color>' hits '<color=gray>" + defender.data.nome +
				"</color>' for <color=gray>" + amount + "</color> damage. " + log_info);
		
		dmg.amount = amount;

		if (bonus != null) {
			dmg.amount = (int) (dmg.amount * bonus.totalMultiplier);
		}

		return dmg;
	}

	public int theoretical_damage(CharacterObject attacker,
								CharacterObject defender,
								AttackModule mod) {
		int damage = 0;
		int attacking_attrib;
		int defending_attrib;
		log_info = "";

		switch (get_attack_kind(attacker, defender, mod)) {
			case AttackKind.PHYSICAL: default:
				attacking_attrib = attacker.data.FOR;
				defending_attrib = defender.data.DEF;
				log_info += "(FOR + DMG[arma] - DEF) = ";
				break;
			case AttackKind.MAGICAL:
				attacking_attrib = attacker.data.INT;
				defending_attrib = defender.data.RES;
				log_info += "(INT + DMG[arma] - RES) = ";
				break;
		}

		damage += attacking_attrib;
		damage += attacker.weapon.damage;
		damage -= defending_attrib;
		log_info += "(" + attacking_attrib + " + " +
					attacker.weapon.damage + " - " +
					defending_attrib + ")";

		if (mod == AttackModule.COUNTER_ATTACK) {
			log_info += " / 2";
			damage /= 2;
		}

		damage = Mathf.Clamp(damage, 0, damage);
		return damage;
	}

	public static AttackKind get_attack_kind(CharacterObject attacker,
								CharacterObject defender,
								AttackModule mod) {

		switch (attacker.weapon.att) {
			case AttackKind.PHYSICAL: default:
				return AttackKind.PHYSICAL;
			case AttackKind.MAGICAL:
				return AttackKind.MAGICAL;
		}
	}

	public static bool pass_test(float probability) {
		if (probability <= 1f && probability >= 0f) {
			probability *= 100f;
		}
		
		return (Random.Range(0, 100) <= probability);
	}

	#region accuracy
		public int accuracy_prob(CharacterObject attacker,
								CharacterObject defender,
								AttackModule mod) {
			int onus = 0;
			if (mod == AttackModule.COUNTER_ATTACK) {
				onus = 15;
			}

			int prob = 0;
			prob += (attacker.data.DES - defender.data.AGI) * 3;
			prob += (int) (attacker.weapon.accuracy * 100);
			prob -= onus;

			return prob;
		}
	#endregion

	#region critical
		public int critical_prob(CharacterObject attacker,
								CharacterObject defender,
								AttackModule mod) {
			int prob = 0;
			prob += (attacker.data.DES - defender.data.AGI) * 3;
			prob -= 100;
			prob += (int) (attacker.weapon.accuracy * 100 / 2);
			prob += (int) (attacker.weapon.criticalBonus * 100);

			if (mod == AttackModule.COUNTER_ATTACK) {
				prob /= 2;
			}

			float multiplier = passiveManager.Get_Crit_Probability(attacker, defender, mod);
			if (multiplier != 0) {
				prob = (int) (prob * multiplier);
			}

			prob = Mathf.Clamp(prob, 0, 100);
			return prob;
		}
	#endregion

	#region ignore
		public int ignore_prob(CharacterObject attacker,
								CharacterObject defender,
								AttackModule mod) {
			int prob = 0;

			float aux = passiveManager.Get_Ignore_Probability(attacker, defender, mod);
			prob += (int) (aux * 100);

			return prob;
		}
	#endregion
}
