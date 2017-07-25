using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { WEAPON };

[System.Serializable]
public class ItemData {
	public string nome;
	public string sprite_name;
	// public ItemType type = ItemType.WEAPON;

	// [Header("Weapon Attributes")]
	// public WeaponKind kind = WeaponKind.DEFAULT;
	// public AttackKind att = AttackKind.PHYSICAL;
	// public int damage;
	// public float accuracy;
	// public float criticalBonus;
}

[System.Serializable]
public class InventoryData {
	public List<ItemData> items = new List<ItemData>();
}