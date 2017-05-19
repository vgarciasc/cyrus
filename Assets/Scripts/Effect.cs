using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectPassive {
	BUFF_DEBUFF, //effect is a buff or a debuff
	LABEL_SHOW, //effect is showing label on appropriate character
	PROTECT_ALLY //effect is protecting ally from incoming shot
}

public enum EffectActive {
	SWAP, //swaps TARGET 1 with TARGET 2
	COMPLETE_ATTACK, //makes TARGET 1 complete-attack TARGET 2
	SIMPLE_ATTACK, //makes TARGET 1 simple-attack TARGET 2
	SIMPLE_COUNTER, //makes TARGET 1 simple-counter TARGET 2
	SHOW_LABEL, //shows label on TARGET 1
	BUFF_DEBUFF, //casts buff/debuff on TARGET 1
	HEAL //heals TARGET 1
};

[System.Serializable]
public class Effect : System.Object {
	public EffectPassive kind = EffectPassive.BUFF_DEBUFF;
	public Buff buff;
	public ProtectAlly protectAlly;
	public List<TargetPassive> targets = new List<TargetPassive>();
	
	[TooltipAttribute("Mark as true to show label during effect. Mark as false if you want to show the label another time (or never).")]
	public bool alsoShowLabel = true;
	[TooltipAttribute("Mark as true to emit buff or debuff animation when passive is activated.")]
	public bool showBuffDebuffAnimation = true;
}