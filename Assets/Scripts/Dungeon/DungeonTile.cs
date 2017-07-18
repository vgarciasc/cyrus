using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum DungeonTileType {
	INACTIVE,
	ACTIVE,
	START,
	BOSS,
};

[ExecuteInEditMode]
public class DungeonTile : MonoBehaviour {
	Image img;
	public DungeonTileType type;
	public int linha;
	public int coluna;

	public bool bottomConnection;
	public bool rightConnection;

	void Start() {
		Init();
	}

	void Init() {
		img	= this.GetComponentInChildren<Image>();
	}

	public void Set_Pos(int linha, int coluna) {
		this.linha = linha;
		this.coluna = coluna;
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
			case DungeonTileType.ACTIVE:
				color = new Color(0.32f, 0.32f, 0.32f, 1f);
				break;
		}

		img.color = color;
	}
}
