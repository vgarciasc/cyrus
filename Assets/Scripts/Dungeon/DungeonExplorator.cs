using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DungeonExplorator : MonoBehaviour {

	int linhas;
	int colunas;

	InventoryManager inventoryManager;
	
	[SerializeField]
	Text alertText;
	[SerializeField]
	GameObject dungeonExploratorContainer;
	[SerializeField]
	GameObject dungeonExploratorPrefab;
	[SerializeField]
	DungeonGenerator gen;

	public static DungeonExplorator Get_Dungeon_Explorator() {
		return (DungeonExplorator) HushPuppy.safeFindComponent("GameController", "DungeonExplorator");
	}

	#region Initialization
		void Start() {
			inventoryManager = InventoryManager.Get_Inventory_Manager();
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
	#endregion

	#region Explorator Arrows
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
	#endregion

	#region Player Movement
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
			Reset_Explorators();
			// Show_Arrows(new_tile);
			gen.Update_Visible_Connections(new_tile);
			gen.Set_Adjacent_Semi_Explored(new_tile);

			Handle_Tile_Event(new_tile);
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
	#endregion

	#region Tile Events
		void Handle_Tile_Event(DungeonTile tile) {
			switch (tile.type) {
				case DungeonTileType.ACTIVE:

					if (tile.Get_Treasure()) {
						tile.Set_Treasure(false);
						alertText.text = "You got a treasure.";
						return;
					}

					float chance = Random.Range(0.2f, 0.4f);
					float dice = Random.Range(0f, 1f);
					if (dice <= chance) {
						StartCoroutine(Go_Encounter(tile));
					} else {
						StartCoroutine(No_Encounter(tile));
					}
					break;
			}
		}

		IEnumerator Go_Encounter(DungeonTile tile) {
			alertText.text = ".";
			yield return new WaitForSeconds(0.5f);
			alertText.text = "..";
			yield return new WaitForSeconds(0.5f);
			alertText.text = "...";
			yield return new WaitForSeconds(0.5f);
			alertText.text = "!!!!!! BATTLE";
			gen.Save_State();

			yield return new WaitForSeconds(2f);

			SceneManager.LoadScene("MainScene");
		}

		IEnumerator No_Encounter(DungeonTile tile) {
			alertText.text = ".";
			yield return new WaitForSeconds(0.5f);
			alertText.text = "..";
			yield return new WaitForSeconds(0.5f);
			alertText.text = "...";
			yield return new WaitForSeconds(0.5f);
			alertText.text = "Nothing.";
			Show_Arrows(tile);
			gen.Save_State();
		}

	#endregion
}
