using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : System.Object {
	public int amount = 0;
	public bool crit = false;
	public List<Buff> buffs = new List<Buff>();

	public Damage(int amount, List<Buff> buffs) {
		this.amount = amount;
		this.buffs = buffs;
	}

	public Damage(Damage dmg) {
		this.amount = dmg.amount;
		this.buffs = dmg.buffs;
	}

	public Damage() {}
}
