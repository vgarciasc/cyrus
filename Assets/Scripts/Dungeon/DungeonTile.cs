using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum DungeonTileType {
	INACTIVE,
	ACTIVE,
	START,
	BOSS
};

[ExecuteInEditMode]
public class DungeonTile : MonoBehaviour {
	[SerializeField]
	Image tileImage;
	[SerializeField]
	Image playerIcon;

	public DungeonTileType type;
	public int linha;
	public int coluna;
	
	[SerializeField]
	bool currentPlayerTile = false;
	[SerializeField]
	bool explored = false;

	public bool bottomConnection = false;
	public bool rightConnection = false;

	void Start() {
		Reset();
	}

	public void Reset() {
		Set_Player_Tile(false);
		Set_Explored(false);

		bottomConnection = false;
		rightConnection = false;
	}

	public void Set_Pos(int linha, int coluna) {
		this.linha = linha;
		this.coluna = coluna;
	}

	public void Set_Type(DungeonTileType type) {
		this.type = type;
		Update_Appearance();

		if (type == DungeonTileType.INACTIVE) {
			tileImage.enabled = false;
		}
		else {
			tileImage.enabled = true;
		}
	}

	void Update_Appearance() {
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
				if (!explored) {
					// color /= 2f;
					// color += new Color(0f, 0f, 0f, 1f);
					color = Color.clear;
				}
				break;
		}

		tileImage.color = color;
	}

	public void Set_Player_Tile(bool value) {
		currentPlayerTile = value;
		playerIcon.enabled = currentPlayerTile;
		
		if (value) {
			Set_Explored(true);
		}
	}

	public bool Get_Is_Player_Tile() {
		return currentPlayerTile;
	}

	public void Set_Explored(bool value) {
		explored = value;
		Update_Appearance();
	}

	public bool Get_Explored() {
		return explored;
	}
}
