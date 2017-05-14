using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : System.Object {
	public int amount = 0;
	public bool crit = false;
	public AttackKind attackKind = AttackKind.PHYSICAL;
	public AttackModule attackModule = AttackModule.NORMAL_ATTACK;
	public List<Buff> buffs = new List<Buff>();

	public Damage(int amount, AttackKind attackKind, List<Buff> buffs) {
		this.amount = amount;
		this.buffs = buffs;
		this.attackKind = attackKind;
	}

	public Damage(Damage dmg) {
		this.amount = dmg.amount;
		this.buffs = dmg.buffs;
		this.attackKind = dmg.attackKind;
	}

	public Damage() {}
}
