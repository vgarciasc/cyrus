using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType {ATT_FOR, ATT_DEF, ATT_INT, ATT_RES,
					ATT_AGI, ATT_DES, ATT_VIT};

[System.Serializable]
public class Buff : System.Object {
	public BuffType kind;
	public int amount = 0;
	public int turnsDuration = 0; //-1 for infinite
	[HideInInspector]
	public int turnsLeft = 0;
	public bool stackable = true;

	public override string ToString() {
		return kind.ToString() + ": " + amount;
	}

	public Buff (Buff b) {
		this.kind = b.kind;
		this.amount = b.amount;
		this.turnsLeft = b.turnsDuration; //starts with maxturns
		this.turnsDuration = b.turnsDuration;
		this.stackable = b.stackable;
	}

	public bool Equals(Buff b) {
		return (amount == b.amount &&
			turnsDuration == b.turnsDuration &&
			stackable == b.stackable &&
			kind == b.kind);
	}
}
