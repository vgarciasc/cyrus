using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectKind {
	BUFF_DEBUFF, //effect is a buff or a debuff
	LABEL_SHOW, //effect is showing label on appropriate character
	PROTECT_ALLY //effect is protecting ally from incoming shot
}

[System.Serializable]
public class Effect : System.Object {
	public EffectKind kind = EffectKind.BUFF_DEBUFF;
	public Buff buff;
	public ProtectAlly protectAlly;
	public List<TargetKind> targets = new List<TargetKind>();
	
	[TooltipAttribute("Mark as true to show label during effect. Mark as false if you want to show the label another time (or never).")]
	public bool alsoShowLabel = true;
	[TooltipAttribute("Mark as true to emit buff or debuff animation when passive is activated.")]
	public bool showBuffDebuffAnimation = true;
}