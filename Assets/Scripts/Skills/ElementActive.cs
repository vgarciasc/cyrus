using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementActiveKind {TARGET, EFFECT};

[System.Serializable]
public class ElementActive : System.Object {
	//MAIN
	public ElementActiveKind kind = ElementActiveKind.TARGET;
	public List<int> targetIndex = new List<int>(); //-1 for SELF

	//TARGET
	public TargetActive target;

	//EFFECT
	public EffectActive effect;
	public bool useAction;

	//MAIN
	[RangeAttribute(0f, 5f)]
	public float totalAttackMultiplier = 1f;
	public Buff buff;
	public bool showBuffLabel = true;
}
