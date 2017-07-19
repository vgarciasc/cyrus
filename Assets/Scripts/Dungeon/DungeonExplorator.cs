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
}
