using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class TeamCharacterDataUI : MonoBehaviour {
	[SerializeField]
	Image characterSprite;
	[SerializeField]
	Text characterName;
	[SerializeField]
	Image weaponSprite;
	[SerializeField]
	Image overlay;
	[SerializeField]
	Image selectedBackground;
	[SerializeField]
	Image unselectedBackground;

	[HideInInspector]
	public bool swap_selected = false;

	CharacterDataJSON json;
	public int position = -1;

	public void Initialize(int position, CharacterDataJSON json) {
		this.position = position;
		this.json = json;

		characterName.text = json.nome;
		characterSprite.sprite = Resources.Load<Sprite>(
			"Sprites\\" + json.raca.ToString() + "\\" + 
			json.raca.ToString().ToUpper() + "_headLINE"
		);
		
		if (weaponSprite != null) {
			weaponSprite.sprite = Resources.Load<Sprite>(
				"Sprites\\Weapon\\" + 
				InventoryManager.Get_Inventory_Manager().Get_Item_By_ID(json.weapon_ID).sprite_name
			);
		}
	}

	public void Toggle_Character_For_Swap() {
		swap_selected = !swap_selected;
		Vector3 offset = new Vector3(50, 0);
		if (swap_selected) {
			this.transform.position += offset;
		}
		else {
			this.transform.position -= offset;
		}

		TeamManager.Get_Team_Manager().Select_Position_For_Swap(position);
	}

	public void Deselect_Character_For_Swap() {
		Vector3 offset = new Vector3(50, 0);
		if (swap_selected) {
			this.transform.position -= offset;
		}
		swap_selected = false;
	}

	public void Swap_To(Vector3 position) {
		Deselect_Character_For_Swap();
		float time = 0.5f;
		this.transform.DOMoveY(position.y, time);
	}

	public void Enter_Details() {
		var dscm = DungeonCharacterStatsManager.Get_DCSM();
		TeamManager.Get_Team_Manager().Enter_Details();
		dscm.Initialize(json);
	}

	public void Select_Equip() {
		InventoryEquipManager.Get_Inventory_Equip_Manager().Select_Character_To_Equip(position);
	}

	public void Toggle_Character_For_Equip(bool value) {
		unselectedBackground.enabled = !value;
		selectedBackground.enabled = value;
	}

	public void Toggle_Character_Can_Equip(bool value) {
		overlay.enabled = !value;
	}
}
