using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DelayedAttack {
	[RangeAttribute(0f, 5f)]
	public float attackMultiplier = 1f;
	public int afterTurns = 0;

	public static IEnumerator conclude(CharacterObject co, Buff buff) {
		if (co.is_dead()) {
			yield break;
		}

		yield return co.label.showLabel(buff.skillName);
		co.take_hit((int) (buff.amount * buff.multiplier));

		yield return new WaitForSeconds(0.1f);
	}
}