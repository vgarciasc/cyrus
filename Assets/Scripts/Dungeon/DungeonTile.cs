using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum DungeonTileType {
	NORMAL,
	START,
	BOSS,
	INACTIVE
};

[ExecuteInEditMode]
public class DungeonTile : MonoBehaviour {
	Image img;
	DungeonTileType type;

	void Start() {
		Init();
	}

	void Init() {
		img	= this.GetComponentInChildren<Image>();
	}

	public void Set_Type(DungeonTileType type) {
		Init();

		this.type = type;
		Update_Color();

		if (type == DungeonTileType.INACTIVE) {
			img.enabled = false;
		}
		else {
			img.enabled = true;
		}
	}

	void Update_Color() {
		Color color = Color.white;

		switch (type) {
			case DungeonTileType.START:
				color = Color.cyan;
				break;
			case DungeonTileType.BOSS:
				color = Color.red;
				break;
			case DungeonTileType.NORMAL:
				color = new Color(0.32f, 0.32f, 0.32f, 1f);
				break;
		}

		img.color = color;
	}
}
