using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventoryEquipManager : MonoBehaviour {
	[SerializeField]
	GameObject bottomButtons;
	[SerializeField]
	Image weapon_old_sprite;
	[SerializeField]
	Image weapon_new_sprite;
	[SerializeField]
	GameObject equip_screen;
	[SerializeField]
	List<TeamCharacterDataUI> characters_on_display = new List<TeamCharacterDataUI>();

	InventoryManager inventory;
	TeamManager team;
	CharacterDataJSON current_character;
	ItemData current_equipped_weapon;
	ItemData current_equippable_weapon;

	public static InventoryEquipManager Get_Inventory_Equip_Manager() {
		return (InventoryEquipManager) HushPuppy.safeFindComponent("GameController", "InventoryEquipManager");
	}

	void Start() {
		equip_screen.SetActive(false);
	}

	public void Initialize() {
		inventory = InventoryManager.Get_Inventory_Manager();
		team = TeamManager.Get_Team_Manager();

		for (int i = 0; i < characters_on_display.Count; i++) {
			var tcdui = characters_on_display[i];
			tcdui.Initialize(
				i,
				team.Get_Current_Team()[i]
			);
		}
	}

	public void Select_Character_To_Equip(int index) {
		var current_team = team.Get_Current_Team();

		for (int i = 0; i < characters_on_display.Count; i++) {
			var ch = characters_on_display[i];
			ch.Toggle_Character_For_Equip(false);
		}

		characters_on_display[index].Toggle_Character_For_Equip(true);
		current_character = current_team[index];

		Update_Current_Character_Weapon();
	}

	public void Enter_Equip_Mode(ItemData item) {
		Initialize();

		inventory.Toggle_Inventory();
		equip_screen.SetActive(true);
		bottomButtons.SetActive(false);
		Select_Character_To_Equip(0);

		Update_Selected_Weapon(item);
		Update_Current_Character_Weapon();
		Update_Who_Can_Equip();
	}

	public void Exit_Equip_Mode() {
		equip_screen.SetActive(false);
		bottomButtons.SetActive(true);
	}

	void Update_Who_Can_Equip() {
		var current_team = team.Get_Current_Team();
		List<int> equippable = new List<int>();

		for (int i = 0; i < characters_on_display.Count; i++) {
			var ch = characters_on_display[i];

			var aux = CharacterData.Can_Equip(
				current_team[i].classe,
				current_equippable_weapon.kind
			);

			ch.Toggle_Character_Can_Equip(aux);

			if (aux) {
				equippable.Add(i);
			}
		}

		Select_Character_To_Equip(equippable[0]);
	}

	void Update_Current_Character_Weapon() {
		current_equipped_weapon = inventory.Get_Item_By_ID(current_character.weapon_ID);
		weapon_old_sprite.sprite = ResourceLoader.Get_Weapon_Sprite(current_equipped_weapon.sprite_name);
	}

	void Update_Selected_Weapon(ItemData item) {
		current_equippable_weapon = item;
		weapon_new_sprite.sprite = ResourceLoader.Get_Weapon_Sprite(item.sprite_name);
	}

	public void Equip_Weapon() {
		team.Equip_Weapon(current_character, current_equippable_weapon.ID);
		Exit_Equip_Mode();
		inventory.Toggle_Inventory();
	}
}