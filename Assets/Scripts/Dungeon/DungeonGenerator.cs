using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
	public int linhas = 5;
	public int colunas = 7;
	
	[SerializeField]
	GameObject tilesContainer;
	[SerializeField]
	GameObject tilePrefab;

	void Start() {
		StartCoroutine(Dungeon());
	}

	IEnumerator Dungeon() {
		Populate_Tiles();
		yield return new WaitForEndOfFrame();
		Generate_Dungeon();
	}

	public void Populate_Tiles() {
		HushPuppy.destroyChildren(tilesContainer);

		for (int i = 0; i < linhas; i++) {
			for (int j = 0; j < colunas; j++) {
				GameObject go = Instantiate(tilePrefab, tilesContainer.transform, false);
				go.GetComponentInChildren<DungeonTile>().Set_Pos(i, j);
				go.name = "Tile #" + (i * colunas + j);
			}
		}
	}

	enum Direcao { CIMA, BAIXO, DIREITA };

	public void Generate_Dungeon() {
		int[,] matrix = new int[linhas, colunas];

		int start_linha, start_coluna, start_pos;
		int boss_linha, boss_coluna, boss_pos;

		do {
			start_linha = Get_Random_Linha();
			start_coluna = Get_Random_Coluna();
			start_pos = matrix[start_linha, start_coluna];
		} while (start_pos != (int) DungeonTileType.INACTIVE);

		matrix[start_linha, start_coluna] = (int) DungeonTileType.START;

		do {
			boss_linha = Get_Random_Linha();
			boss_coluna = Get_Random_Coluna();
			boss_pos = matrix[boss_linha, boss_coluna];
		} while (boss_pos != (int) DungeonTileType.INACTIVE);

		matrix[boss_linha, boss_coluna] = (int) DungeonTileType.BOSS;

		int esquerda_linha, esquerda_coluna,
			direita_linha, direita_coluna;

		if (boss_coluna > start_coluna) {
			direita_linha = boss_linha;
			direita_coluna = boss_coluna;
			esquerda_linha = start_linha;
			esquerda_coluna = start_coluna;
		}
		else {
			direita_linha = start_linha;
			direita_coluna = start_coluna;
			esquerda_linha = boss_linha;
			esquerda_coluna = boss_coluna;
		}

		int diferenca_linha, diferenca_coluna;
		diferenca_linha = direita_linha - esquerda_linha;
		diferenca_coluna = direita_coluna - esquerda_coluna;

		List<Direcao> trajeto = new List<Direcao>();
		for (int i = 0; i < diferenca_coluna; i++) {
			trajeto.Add(Direcao.DIREITA);
		}

		for (int i = 0; i < Mathf.Abs(diferenca_linha); i++) {
			if (Mathf.Sign(diferenca_linha) == 1) {
				trajeto.Add(Direcao.BAIXO);
			}
			else if (Mathf.Sign(diferenca_linha) == -1) {
				trajeto.Add(Direcao.CIMA);
			}
		}

		int current_linha, current_coluna;
		current_linha = esquerda_linha;
		current_coluna = esquerda_coluna;
		foreach (Direcao dir in Shuffle_Trajeto(trajeto)) {
			switch (dir) {
				case Direcao.BAIXO:
					current_linha++;
					break;
				case Direcao.CIMA:
					current_linha--;
					break;
				case Direcao.DIREITA:
					current_coluna++;
					break;
			}

			if (matrix[current_linha, current_coluna] == (int) DungeonTileType.INACTIVE) {
				matrix[current_linha, current_coluna] = (int) DungeonTileType.ACTIVE;
			}
		}

		for (int i = 0; i < (linhas * colunas) / 2; i++) {
			int aux_linha = -1, aux_coluna = -1;
			int k = 0;

			do {
				k++;
				if (k > 50) break;
				aux_linha = Get_Random_Linha();
				aux_coluna = Get_Random_Coluna();
			} while (!Adjacent_To_Active(matrix, aux_linha, aux_coluna) ||
				matrix[aux_linha, aux_coluna] != (int) DungeonTileType.INACTIVE);

			if (matrix[aux_linha, aux_coluna] == (int) DungeonTileType.INACTIVE) {
				matrix[aux_linha, aux_coluna] = (int) DungeonTileType.ACTIVE;
			}
		}

		for (int i = 0; i < linhas; i++) {
			for (int j = 0; j < colunas; j++) {
				var obj = tilesContainer.transform.GetChild(i * colunas + j);
				DungeonTile tile = obj.GetComponentInChildren<DungeonTile>();
				tile.Set_Type((DungeonTileType) matrix[i, j]);
			}
		}
	}

	bool Adjacent_To_Active(int[,] matrix, int linha, int coluna) {
		bool adjacent = false;

		if (coluna > 0 && matrix[linha, coluna - 1] == (int) DungeonTileType.ACTIVE) {
			adjacent = true;
		}
		if (linha > 0 && matrix[linha - 1, coluna] == (int) DungeonTileType.ACTIVE) {
			adjacent = true;
		}
		if (coluna < colunas - 1 && matrix[linha, coluna + 1] == (int) DungeonTileType.ACTIVE) {
			adjacent = true;
		}
		if (linha < linhas - 1 && matrix[linha + 1, coluna] == (int) DungeonTileType.ACTIVE) {
			adjacent = true;
		}

		return adjacent;
	}

	int Get_Random_Linha() {
		return Random.Range(0, linhas);
	}

	int Get_Random_Coluna() {
		return Random.Range(0, colunas);
	}

	DungeonTile Get_Random_Tile() {
		return Get_Tile(
			Get_Random_Linha(),
			Get_Random_Coluna()
		);
	}

	DungeonTile Get_Inactive_Random_Tile() {
		DungeonTile aux = Get_Random_Tile();

		while (aux.type != DungeonTileType.INACTIVE) {
			aux = Get_Random_Tile();
		}

		return aux;
	}

	List<Direcao> Shuffle_Trajeto(List<Direcao> trajeto) {
		for (int i = 0; i < trajeto.Count; i++) {
			Direcao temp = trajeto[i];
			int randomIndex = Random.Range(i, trajeto.Count);
			trajeto[i] = trajeto[randomIndex];
			trajeto[randomIndex] = temp;
		}

		return trajeto;
	}

	DungeonTile Get_Tile(int linha, int coluna) {
		return tilesContainer.transform.GetChild(
			linha * colunas +
			coluna
		).GetComponentInChildren<DungeonTile>();
	}

	void Print_Matrix(int[,] matrix) {
		string aux = "[";

		for (int i = 0; i < matrix.GetLength(0); i++) {
			for (int j = 0; j < matrix.GetLength(1); j++) {
				aux += matrix[i, j];
				if (j != matrix.GetLength(1) - 1) {
					aux += ", ";
				}
				else {
					if (i == matrix.GetLength(0) - 1) {
						aux += "]";
						print(aux);
						return;
					}
					else {
						aux += "\n";
					}
				}
			}
		}
	}
}
