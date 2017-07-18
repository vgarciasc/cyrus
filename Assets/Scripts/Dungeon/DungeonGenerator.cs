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

		for (int i = 0; i < linhas * colunas; i++) {
			GameObject go = Instantiate(tilePrefab, tilesContainer.transform, false);
			go.name = "Tile #" + i;
		}
	}

	public void Generate_Dungeon() {
		int[,] matrix = new int[linhas, colunas];

		matrix[Random.Range(0, linhas), Random.Range(0, colunas)] = 1;
		matrix[Random.Range(0, linhas), Random.Range(0, colunas)] = 2;

		// Print_Matrix(matrix);

		for (int i = 0; i < linhas; i++) {
			for (int j = 0; j < colunas; j++) {
				var obj = tilesContainer.transform.GetChild(i * colunas + j);
				DungeonTile tile = obj.GetComponentInChildren<DungeonTile>();
				tile.Set_Type((DungeonTileType) matrix[i, j]);
			}
		}
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
