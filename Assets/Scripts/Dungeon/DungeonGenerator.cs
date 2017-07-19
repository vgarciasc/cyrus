using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum Direcao { CIMA, BAIXO, DIREITA, ESQUERDA };

public class DungeonGenerator : MonoBehaviour {
	public int linhas = 5;
	public int colunas = 7;
	
	[SerializeField]
	GameObject dungeonRoomContainer;
	[SerializeField]
	GameObject dungeonRoomPrefab;
	[SerializeField]
	GameObject dungeonConnectionsContainer;
	[SerializeField]
	GameObject dungeonConnectionsPrefab;
	[SerializeField]
	DungeonExplorator dungeonExplorator;

	DungeonData currentDungeon;

	void Start() {
		StartCoroutine(Dungeon());
	}

	IEnumerator Dungeon() {

		//if no dungeon, generate dungeon
		if (!Is_There_A_Current_Dungeon()) {
			Populate_Tiles();
			yield return new WaitForSeconds(0.25f);
			Generate_Dungeon();
		}
		//if dungeon, use it
		else {
			currentDungeon = Load_Dungeon_From_Disk();
			this.linhas = currentDungeon.linhas;
			this.colunas = currentDungeon.colunas;
			
			dungeonExplorator.Set_Linhas_Colunas(linhas, colunas);
			dungeonExplorator.Populate_Explorators();

			Populate_Tiles();
			Reset_Connections();

			yield return new WaitForSeconds(0.25f);

			Unserialize_Dungeon(currentDungeon);
		}
	}

	public void Populate_Tiles() {
		HushPuppy.destroyChildren(dungeonRoomContainer);

		for (int i = 0; i < linhas; i++) {
			for (int j = 0; j < colunas; j++) {
				GameObject go = Instantiate(
					dungeonRoomPrefab,
					dungeonRoomContainer.transform,
					false);
				var tile = go.GetComponentInChildren<DungeonTile>();
				tile.Set_Pos(i, j);

				go.name = "Tile #" + (i * colunas + j);
			}
		}

		for (int i = 0; i < linhas; i++) {
			for (int j = 0; j < colunas; j++) {
				GameObject go = Instantiate(
					dungeonConnectionsPrefab,
					dungeonConnectionsContainer.transform,
					false);
				go.name = "Connection #" + (i * colunas + j);
			}
		}
	}

	#region Serialization
		void Unserialize_Dungeon(DungeonData dg) {
			foreach (DungeonTileData dt in dg.dungeonTiles) {
				var obj = dungeonRoomContainer.transform.GetChild(dt.linha * colunas + dt.coluna);
				var tile = obj.GetComponentInChildren<DungeonTile>();

				tile.Reset();

				tile.linha = dt.linha;
				tile.coluna = dt.coluna;
				tile.Set_Explored(dt.explored);

				tile.Set_Player_Tile(dt.currentPlayerTile);
				tile.Set_Type(dt.kind);
			}

			foreach (DungeonTileData dt in dg.dungeonTiles) {
				dungeonExplorator.Show_Arrows(Get_Player_Tile());
				Toggle_Connection_Bottom(dt.linha, dt.coluna, dt.bottomConnection);
				Toggle_Connection_Right(dt.linha, dt.coluna, dt.rightConnection);
			}
		}

		DungeonData Serialize_Dungeon() {
			DungeonData dg = new DungeonData();

			dg.dungeonTiles.Clear();

			dg.linhas = linhas;
			dg.colunas = colunas;

			int mainCorridorSize = 0;

			for (int i = 0; i < linhas; i++) {
				for (int j = 0; j < colunas; j++) {
					var obj = dungeonRoomContainer.transform.GetChild(i * colunas + j);
					var tile = obj.GetComponentInChildren<DungeonTile>();
					dg.dungeonTiles.Add(
						new DungeonTileData(
							tile.type,
							tile.linha, tile.coluna,
							tile.bottomConnection,
							tile.rightConnection,
							tile.Get_Explored(),
							tile.Get_Is_Player_Tile()
						)
					);

					if (tile.type != DungeonTileType.INACTIVE) {
						mainCorridorSize++;
					}
				}
			}

			dg.mainCorridorSize = mainCorridorSize;

			Save_Dungeon_To_Disk(dg);

			// print("Saved!");

			return dg;
		}
	#endregion	

	#region JSON
		string Get_Current_Dungeon_Path() {
			var path = Application.persistentDataPath;
			var aux = "\\Resources";
			path += aux;

			if (!System.IO.Directory.Exists(path)) {
				System.IO.Directory.CreateDirectory(path);
			}

			aux = "\\Dungeons";
			path += aux;

			if (!System.IO.Directory.Exists(path)) {
				System.IO.Directory.CreateDirectory(path);
			}

			aux = "\\Current_Dungeon.json";
			path += aux;

			if (!System.IO.File.Exists(path)) {
				System.IO.File.Create(path);
			}

			return path;
		}

		bool Is_There_A_Current_Dungeon() {
			var path = Get_Current_Dungeon_Path();
			var stream = System.IO.File.OpenText(path);
			var file = stream.ReadToEnd();
			stream.Close();
			
			// print("Stream: " + file.ToString());

			if (file.Length == 0) {
				return false;
			}

			return true;
		}

		void Save_Dungeon_To_Disk(DungeonData dg) {
			var path = Get_Current_Dungeon_Path();
			var json = JsonUtility.ToJson(dg);

			System.IO.StreamWriter sw = System.IO.File.CreateText(path);
			sw.Close();
			System.IO.File.WriteAllText(path, json);

			#if UNITY_EDITOR
				UnityEditor.AssetDatabase.Refresh();
				// UnityEditor.AssetDatabase.SaveAssets();
			#endif
		}

		DungeonData Load_Dungeon_From_Disk() {
			var path = Get_Current_Dungeon_Path();
			var stream = System.IO.File.OpenText(path);
			var file = stream.ReadToEnd();
			stream.Close();

			DungeonData dg = new DungeonData();
			JsonUtility.FromJsonOverwrite(file, dg);
			return dg;
		}
	#endregion

	#region Generate Dungeon
		void Reset_Connections() {
			foreach (Transform t in dungeonConnectionsContainer.transform) {
				t.GetComponentInChildren<ConnectionTile>().Reset();
			}
		}

		void Reset_Tiles() {
			foreach (Transform t in dungeonRoomContainer.transform) {
				t.GetComponentInChildren<DungeonTile>().Reset();
			}
		}

		public void Generate_Dungeon() {
			int[,] matrix = new int[linhas, colunas];
			Reset_Connections();
			Reset_Tiles();

			//position start room
			int start_linha, start_coluna, start_pos;
			do {
				start_linha = Get_Random_Linha();
				start_coluna = Get_Random_Coluna();
				start_pos = matrix[start_linha, start_coluna];
			} while (start_pos != (int) DungeonTileType.INACTIVE);

			matrix[start_linha, start_coluna] = (int) DungeonTileType.START;

			//position boss room
			int boss_linha, boss_coluna, boss_pos;
			do {
				boss_linha = Get_Random_Linha();
				boss_coluna = Get_Random_Coluna();
				boss_pos = matrix[boss_linha, boss_coluna];
			} while (boss_pos != (int) DungeonTileType.INACTIVE);

			matrix[boss_linha, boss_coluna] = (int) DungeonTileType.BOSS;

			//position rooms between boss room and start room
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
			trajeto = Shuffle_Trajeto(trajeto);
			foreach (Direcao dir in trajeto) {
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

			//populate the rest of the dungeon randomly
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

			Apply_Matrix(matrix);
			Get_Start_Tile().Set_Player_Tile(true);
			dungeonExplorator.Show_Arrows(Get_Start_Tile());

			current_linha = esquerda_linha;
			current_coluna = esquerda_coluna;
			foreach (Direcao dir in trajeto) {
				switch (dir) {
					case Direcao.BAIXO:
						Toggle_Connection_Bottom(current_linha, current_coluna, true);
						current_linha++;
						break;
					case Direcao.CIMA:
						Toggle_Connection_Bottom(current_linha - 1, current_coluna, true);
						current_linha--;
						break;
					case Direcao.DIREITA:
						Toggle_Connection_Right(current_linha, current_coluna, true);
						current_coluna++;
						break;
				}
			}

			//apply random connections
			for (int i = 0; i < linhas; i++) {
				for (int j = 0; j < colunas; j++) {
					if (matrix[i, j] == (int) DungeonTileType.ACTIVE) {
						int dice = Random.Range(0, 3);
						switch (dice) {
							case 0:
								Toggle_Connection_Bottom(i, j, true);
								break;
							case 1:
								Toggle_Connection_Right(i, j, true);
								break;
							case 2:
								Toggle_Connection_Bottom(i, j, true);
								Toggle_Connection_Right(i, j, true);
							break;						
						}
					}
				}
			}

			Polish_Connections();
		}

		void Apply_Matrix(int[,] matrix) {
			for (int i = 0; i < linhas; i++) {
				for (int j = 0; j < colunas; j++) {
					var obj = dungeonRoomContainer.transform.GetChild(i * colunas + j);
					DungeonTile tile = obj.GetComponentInChildren<DungeonTile>();
					tile.Set_Type((DungeonTileType) matrix[i, j]);
				}
			}
		}

		void Polish_Connections() {
			List<DungeonTile> main_rooms = new List<DungeonTile>();
			var start = Get_Start_Tile();
			if (start == null) {
				print("No start room!!");
				Debug.Break();
			}

			main_rooms = Get_Componente_Conexa_Rooms(start);
			foreach (Transform t in dungeonRoomContainer.transform) {
				var tile = t.GetComponentInChildren<DungeonTile>();
				if (!main_rooms.Contains(tile)) {
					Deactivate_Tile(tile);
				}
			}

			if (main_rooms.Count < 10) {
				Generate_Dungeon();
			}
			
			dungeonExplorator.Show_Arrows(Get_Player_Tile());
			Serialize_Dungeon();
		}
	#endregion

	#region Getters
		List<DungeonTile> Get_Componente_Conexa_Rooms(DungeonTile tile) {
			List<DungeonTile> componente = new List<DungeonTile>() { tile };

			DungeonTile next_tile;
			int k = 0;

			while (true) {
				next_tile = componente[k];

				foreach (DungeonTile dt in Get_Connected_To_Tile(next_tile)) {
					if (!componente.Contains(dt)) {
						componente.Add(dt);
					}
				}

				k++;
				if (k >= componente.Count) {
					break;
				}
			}

			return componente;
		}

		List<DungeonTile> Get_Connected_To_Tile(DungeonTile tile) {
			List<DungeonTile> output = new List<DungeonTile>();

			int linha = tile.linha;
			int coluna = tile.coluna;

			if (tile.bottomConnection) {
				output.Add(dungeonRoomContainer.transform.GetChild((linha + 1) * colunas + coluna).GetComponentInChildren<DungeonTile>());
			}
			if (tile.rightConnection) {
				output.Add(dungeonRoomContainer.transform.GetChild((linha) * colunas + (coluna + 1)).GetComponentInChildren<DungeonTile>());
			}
			if (linha > 0) {
				var above = dungeonRoomContainer.transform.GetChild((linha - 1) * colunas + coluna).GetComponent<DungeonTile>();
				if (above.bottomConnection) {
					output.Add(above);
				}
			}
			if (coluna > 0) {
				var left = dungeonRoomContainer.transform.GetChild((linha) * colunas + (coluna - 1)).GetComponent<DungeonTile>();
				if (left.rightConnection) {
					output.Add(left);
				}
			}

			return output;
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

		void Deactivate_Tile(DungeonTile tile) {
			tile.Set_Type(DungeonTileType.INACTIVE);
			Toggle_Connection_Bottom(tile.linha, tile.coluna, false);
			Toggle_Connection_Right(tile.linha, tile.coluna, false);

			if (tile.linha > 0) {
				var above = dungeonRoomContainer.transform.GetChild((tile.linha - 1) * colunas + tile.coluna).GetComponent<DungeonTile>();
				Toggle_Connection_Bottom(above.linha, above.coluna, false);
			}
			if (tile.coluna > 0) {
				var left = dungeonRoomContainer.transform.GetChild((tile.linha) * colunas + (tile.coluna - 1)).GetComponent<DungeonTile>();
				Toggle_Connection_Right(left.linha, left.coluna, false);
			}
		}

		DungeonTile Get_Start_Tile() {
			for (int i = 0; i < dungeonRoomContainer.transform.childCount; i++) {
				var aux = dungeonRoomContainer.transform.GetChild(i).GetComponent<DungeonTile>();
				if (aux.type == DungeonTileType.START) {
					return aux;
				}
			}

			return null;
		}

		public DungeonTile Get_Player_Tile() {
			for (int i = 0; i < dungeonRoomContainer.transform.childCount; i++) {
				var aux = dungeonRoomContainer.transform.GetChild(i).GetComponent<DungeonTile>();
				if (aux.Get_Is_Player_Tile()) {
					return aux;
				}
			}

			Debug.Log("No player found!");
			Debug.Break();
			return null;
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

		public DungeonTile Get_Tile(int linha, int coluna) {
			return dungeonRoomContainer.transform.GetChild(
				linha * colunas +
				coluna
			).GetComponentInChildren<DungeonTile>();
		}

		ConnectionTile Get_Connection(int linha, int coluna) {
			return dungeonConnectionsContainer.transform.GetChild(
				linha * colunas +
				coluna
			).GetComponentInChildren<ConnectionTile>();
		}
	#endregion

	#region Toggle Connections
		void Toggle_Connection_Bottom(int linha, int coluna, bool value) {
			if (linha == linhas - 1) {
				//theres no room below
				return;
			}

			if (Get_Tile(linha + 1, coluna).type == (int) DungeonTileType.INACTIVE) {
				//not a valid tile below
				return;
			}

			if (!Get_Tile(linha, coluna).Get_Explored() &&
				!Get_Tile(linha + 1, coluna).Get_Explored()) {
				//both are not explored. connection is there, but not visible
				Get_Tile(linha, coluna).bottomConnection = value;
				return;
			}
			
			Get_Connection(linha, coluna).Toggle_Bottom(value);
			Get_Tile(linha, coluna).bottomConnection = value;
		}

		void Toggle_Connection_Right(int linha, int coluna, bool value) {
			if (coluna == colunas - 1) {
				//theres no room to the right
				return;
			}

			if (Get_Tile(linha, coluna + 1).type == (int) DungeonTileType.INACTIVE) {
				//not a valid tile to the right
				return;
			}

			if (!Get_Tile(linha, coluna).Get_Explored() &&
				!Get_Tile(linha, coluna + 1).Get_Explored()) {
				//both are not explored. connection is there, but not visible
				Get_Tile(linha, coluna).rightConnection = value;
				return;
			}

			Get_Connection(linha, coluna).Toggle_Right(value);
			Get_Tile(linha, coluna).rightConnection = value;
		}

		public void Update_Visible_Connections(DungeonTile tile) {
			Toggle_Connection_Bottom(tile.linha, tile.coluna, tile.bottomConnection);
			Toggle_Connection_Right(tile.linha, tile.coluna, tile.rightConnection);

			if (tile.linha > 0) {
				var above = Get_Tile(tile.linha - 1, tile.coluna);
				Toggle_Connection_Bottom(tile.linha - 1, tile.coluna, above.bottomConnection);
			}
			if (tile.coluna > 0) {
				var left = Get_Tile(tile.linha, tile.coluna - 1);
				Toggle_Connection_Right(tile.linha, tile.coluna - 1, left.rightConnection);
			}
		}
	#endregion

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
