using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public static InventoryManager Get_Inventory_Manager() {
		return (InventoryManager) HushPuppy.safeFindComponent("GameController", "InventoryManager");
	}

	void Start() {
		inventory.SetActive(false);
		bottomButtonsContainer.SetActive(true);
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

		foreach (ItemData id in ivt.items) {
			GameObject go = Instantiate(
				itemPrefab,
				itemContainer.transform,
				false	
			);

			go.GetComponentInChildren<InventoryItemUI>().Initialize(id);
		}
	}

	public void Add_Item(ItemData item) {
		InventoryData ivt = Load_Inventory_From_Disk();
		ivt.items.Add(item);

		Save_Inventory_To_Disk(ivt);
	}
}
