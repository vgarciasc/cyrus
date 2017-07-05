using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponKind {DEFAULT, MANOPLA, ADAGA, ESPADA,
						MACHADO, CAJADO, TOMO, INSTRUMENTO};

[CreateAssetMenu(menuName = "Custom/WeaponData")]
public class WeaponData : ScriptableObject {
	public string nome = "default";
	public Sprite sprite;
	public WeaponKind kind = WeaponKind.DEFAULT;
	public AttackKind att = AttackKind.PHYSICAL;

	[RangeAttribute(0, 30)]
	public int damage = 1;
	[RangeAttribute(0, 1)]
	public float accuracy = 0.5f;
	[RangeAttribute(0, 1)]
	public float criticalBonus = 0.5f;
}