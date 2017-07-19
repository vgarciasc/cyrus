using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonExplorator : MonoBehaviour {

	int linhas;
	int colunas;

	[SerializeField]
	GameObject dungeonExploratorContainer;
	[SerializeField]
	GameObject dungeonExploratorPrefab;

	[SerializeField]
	DungeonGenerator gen;

	public static DungeonExplorator Get_Dungeon_Explorator() {
		return (DungeonExplorator) HushPuppy.safeFindComponent("GameController", "DungeonExplorator");
	}

	public void Set_Linhas_Colunas(int linhas, int colunas) {
		this.linhas = linhas;
		this.colunas = colunas;
	}

	public void Populate_Explorators() {
		for (int i = 0; i < linhas; i++) {
			for (int j = 0; j < colunas; j++) {
				GameObject obj = Instantiate(
					dungeonExploratorPrefab,
					dungeonExploratorContainer.transform,
					false
				);
				var tile = obj.GetComponentInChildren<DungeonExploratorTile>();
				tile.Reset();
			}
		}
	}

	public void Show_Arrows(DungeonTile player) {
		Reset_Explorators();

		if (player.bottomConnection) {
			Get_Explorator(player.linha, player.coluna).Set_Bottom(true);
		}
		
		if (player.rightConnection) {
			Get_Explorator(player.linha, player.coluna).Set_Right(true);
		}

		if (player.linha > 0 && gen.Get_Tile(player.linha - 1, player.coluna).bottomConnection) {
			Get_Explorator(player.linha, player.coluna).Set_Up(true);
		}

		if (player.coluna > 0 && gen.Get_Tile(player.linha, player.coluna - 1).rightConnection) {
			Get_Explorator(player.linha, player.coluna).Set_Left(true);
		}
	}

	void Reset_Explorators() {
		foreach (Transform t in dungeonExploratorContainer.transform) {
			t.GetComponentInChildren<DungeonExploratorTile>().Reset();
		}
	}

	DungeonExploratorTile Get_Explorator(int linha, int coluna) {
		return dungeonExploratorContainer.transform.GetChild(
			linha * colunas +
			coluna
		).GetComponentInChildren<DungeonExploratorTile>();
	}

	public void Move_Player(Direcao dir) {
		var player = gen.Get_Player_Tile();
		player.Set_Player_Tile(false);

		DungeonTile new_tile = null;

		switch (dir) {
			case Direcao.BAIXO:
				new_tile = gen.Get_Tile(player.linha + 1, player.coluna);
				break;
			case Direcao.CIMA:
				new_tile = gen.Get_Tile(player.linha - 1, player.coluna);
				break;
			case Direcao.ESQUERDA:
				new_tile = gen.Get_Tile(player.linha, player.coluna - 1);
				break;
			case Direcao.DIREITA:
				new_tile = gen.Get_Tile(player.linha, player.coluna + 1);
				break;
		}

		new_tile.Set_Player_Tile(true);
		Show_Arrows(new_tile);
		gen.Update_Visible_Connections(new_tile);
		gen.Set_Adjacent_Semi_Explored(new_tile);
	}

	public void Set_Player_Tile(DungeonTile tile) {
		if (tile == null) {
			Debug.Log("This should not be happening.");
			Debug.Break();
		}

		tile.Set_Player_Tile(true);
		Show_Arrows(tile);
		gen.Update_Visible_Connections(tile);
		gen.Set_Adjacent_Semi_Explored(tile);
	}
}
