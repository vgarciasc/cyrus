using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventoryItemUI : MonoBehaviour {
	[SerializeField]
	Text itemName;
	[SerializeField]
	Text action;
	[SerializeField]
	Image sprite;

	ItemData item;

	public void Initialize(ItemData item) {
		this.item = item;

		action.text = "Equipar";
		itemName.text = item.nome;
		// sprite.sprite = item.sprite;
	}
}
