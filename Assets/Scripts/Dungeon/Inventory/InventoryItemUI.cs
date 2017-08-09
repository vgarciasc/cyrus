using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventoryItemUI : MonoBehaviour {
	[SerializeField]
	Text itemName;
	[SerializeField]
	Text actionText;
	[SerializeField]
	Image actionImage;
	[SerializeField]
	Image sprite;

	ItemData item;
	bool equippable = true;

	public void Initialize(ItemData item) {
		this.item = item;

		actionText.text = "Equipar";
		actionImage.color = HushPuppy.getColorWithOpacity(actionImage.color, 1f);
		itemName.text = item.nome;
		// sprite.sprite = item.sprite;
	}

	public void Set_Equipped(bool value) {
		equippable = !value;

		if (!equippable) {
			itemName.color = Color.gray;
			actionImage.color = HushPuppy.getColorWithOpacity(actionImage.color, 0.5f);
		}
	}

	public void Enter_Equip_Mode() {
		if (equippable) {
			InventoryEquipManager.Get_Inventory_Equip_Manager().Enter_Equip_Mode(item);
		}
	}
}
