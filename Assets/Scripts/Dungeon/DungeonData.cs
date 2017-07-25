using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Dungeon Data")]
public class DungeonData : ScriptableObject {
	public int linhas;
	public int colunas;

	public int mainCorridorSize;

	public List<DungeonTileData> dungeonTiles = new List<DungeonTileData>();
}

[System.Serializable]
public class DungeonTileData {
	public DungeonTileType kind;
	
	public bool treasure;
	public bool explored;
	public bool semiExplored;
	public int linha;
	public int coluna;

	public bool bottomConnection;
	public bool rightConnection;
	public bool currentPlayerTile;

	public DungeonTileData(
		DungeonTileType kind,
		int linha, int coluna,
		bool bottom, bool right,
		bool treasure,
		bool explored,
		bool semiExplored,
		bool currentPlayerTile) {
			
		this.kind = kind;
		this.linha = linha;
		this.coluna = coluna;
		this.bottomConnection = bottom;
		this.rightConnection = right;
		this.treasure = treasure;
		this.explored = explored;
		this.semiExplored = semiExplored;
		this.currentPlayerTile = currentPlayerTile;
	}
}