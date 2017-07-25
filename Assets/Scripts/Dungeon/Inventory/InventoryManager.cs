using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour {

	[SerializeField]
	GameObject itemPrefab;
	[SerializeField]
	GameObject itemContainer;
	[SerializeField]
	GameObject inventory;
	[SerializeField]
	GameObject bottomButtonsContainer;
	[SerializeField]
	InventoryData inventoryData;

	public static InventoryManager Get_Inventory_Manager() {
		return (InventoryManager) HushPuppy.safeFindComponent("GameController", "InventoryManager");
	}

	void Start() {
		if (inventory != null)
			inventory.SetActive(false);
		if (bottomButtonsContainer != null)
			bottomButtonsContainer.SetActive(true);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.E)) {
			ItemData item = new ItemData();
			item.nome = "Item #" + Time.timeSinceLevelLoad;
			item.sprite_name = "Sword2";
			
			if (inventoryData == null) {
				inventoryData = Load_Inventory_From_Disk();
			}

			item.ID = inventoryData.items.Count;
			Add_Item(item);
		}
	}

	public void Toggle_Inventory() {
		inventory.SetActive(!inventory.activeSelf);
		bottomButtonsContainer.SetActive(!inventory.activeSelf);

		if (inventory.activeSelf) {
			Populate_Inventory();
		}
	}

	#region Serialization
		string Get_Inventory_Path() {
			var path = Application.streamingAssetsPath + "/Inventory.json";

			return path;
		}

		void Save_Inventory_To_Disk(InventoryData ivt) {
			var path = Get_Inventory_Path();
			var json = JsonUtility.ToJson(ivt, true);

			System.IO.StreamWriter sw = System.IO.File.CreateText(path);
			sw.Close();
			System.IO.File.WriteAllText(path, json);

			#if UNITY_EDITOR
				UnityEditor.AssetDatabase.Refresh();
			#endif
		}

		InventoryData Load_Inventory_From_Disk() {
			var path = Get_Inventory_Path();
			var stream = System.IO.File.OpenText(path);
			var file = stream.ReadToEnd();
			stream.Close();

			InventoryData items = new InventoryData();
			JsonUtility.FromJsonOverwrite(file, items);
			return items;
		}
	#endregion

	void Populate_Inventory() {
		HushPuppy.destroyChildren(itemContainer);

		InventoryData ivt = Load_Inventory_From_Disk();
		List<int> equipped = TeamManager.Get_Team_Manager().All_Equipped_Weapon_IDs();

		foreach (ItemData id in ivt.items) {
			GameObject go = Instantiate(
				itemPrefab,
				itemContainer.transform,
				false	
			);

			var iiui = go.GetComponentInChildren<InventoryItemUI>();
			iiui.Initialize(id);

			if (equipped.Contains(id.ID)) {
				iiui.Set_Equipped(true);
			}
		}
	}

	public void Add_Item(ItemData item) {
		InventoryData ivt = Load_Inventory_From_Disk();
		for (int i = 0; i < ivt.items.Count; i++) {
			if (ivt.items[i].ID == item.ID) {
				print("Inventory already has an item of ID #" + item.ID + ". This should not be happening.");
				return;
			}
		}

		ivt.items.Add(item);

		Save_Inventory_To_Disk(ivt);
	}

	public int Get_New_ID() {
		InventoryData ivt = Get_Inventory();
		return ivt.items.Count;
	}

	public ItemData Get_Item_By_ID(int ID) {
		InventoryData ivt = Get_Inventory();

		for (int i = 0; i < ivt.items.Count; i++) {
			if (ivt.items[i].ID == ID) {
				return ivt.items[i];
			}
		}

		print("There is no item with ID #" + ID + " in the inventory.");
		return null;
	}

	InventoryData Get_Inventory() {
		if (inventoryData == null || inventoryData.items.Count == 0) {
			inventoryData = Load_Inventory_From_Disk();
		}

		return inventoryData;
	}
}
