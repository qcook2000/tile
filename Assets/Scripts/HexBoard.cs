using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class HexBoard : MonoBehaviour {

	// Game setup constants
	static Dictionary<Tile.Type, int> TileCounts = new Dictionary<Tile.Type, int> {
		{Tile.Type.Ladybug, 2}, 
		{Tile.Type.Mosquito, 3}, 
		{Tile.Type.GrassHopper, 4}, 
		{Tile.Type.Ant, 2}, 
		{Tile.Type.RollyPolly, 3}, 
		{Tile.Type.Queen, 1}
	};
	static float basePlaneSize = 10.0f;

	// Hex Structs
	public enum GameArea {Board, WhiteBase, BlackBase}; 
	
	// Game objects
	public GameObject tilePrefab;
	public GameObject highlightSpotPrefab;
	public GameObject whiteBase;
	public GameObject blackBase;

	// Internal
	private Dictionary<Hex, Tile> boardPieces = new Dictionary<Hex, Tile>();
	private Dictionary<Hex, Tile> whiteBasePieces = new Dictionary<Hex, Tile>();
	private Dictionary<Hex, Tile> blackBasePieces = new Dictionary<Hex, Tile>();
	private List<Tile> highlightedSpots = new List<Tile>();
	private Tile selectedTile;
	private bool firstTurn;

	#region Setup

    private void Start()
	{
		GenerateBoard();
		firstTurn = true;
		// EnableFakeBoard("Origin");

	}

	static Dictionary<String,List<Hex>> testBoards = new Dictionary<String,List<Hex>> {
		{"Origin", 
		new List<Hex>() { new Hex(0,0) , new Hex(0,-1), new Hex(2,-2), new Hex(-1,-1)}},
		{"Complex", 
		new List<Hex>() { new Hex(0,-1), new Hex(-1,0), new Hex(-1,1), new Hex(0,1), 
			new Hex(1,0), new Hex(1,-1), new Hex(2,-2), new Hex(2,0), new Hex(3,-1) }},
		{"Simple", 
		new List<Hex>() { new Hex(0,-1), new Hex(-1,0), new Hex(-1,1), new Hex(0,1), 
			new Hex(1,0), new Hex(1,-1) }}
	};

	private void EnableFakeBoard(String board)
	{
		List<Hex> fakeBoard = testBoards[board];
		List<Tile> usableTiles = new List<Tile>(blackBasePieces.Values);
		for (int i = 0; i < fakeBoard.Count; i++) {
			MoveTileToHex(usableTiles[i], fakeBoard[i]);
		}
		firstTurn = false;
	}

	private void GenerateBoard()
	{
		AdjustBase();
		List<Tile.Type> types = new List<Tile.Type>(TileCounts.Keys);
		foreach (Tile.Type type in types)
		{	
			int count = TileCounts[type];
			for (int y = 0; y < count; y++)
			{
				GenerateTile(type, Tile.Color.White);
				GenerateTile(type, Tile.Color.Black);
			}
		}
	}

	private void AdjustBase()
	{
		int rMax = 0, rMin = 0;
		List<Tile.Type> types = new List<Tile.Type>(TileCounts.Keys);
		Vector3 oldW = whiteBase.gameObject.transform.position;
		Vector3 oldB = blackBase.gameObject.transform.position;
		float x = oldW.x;
		float hHeight = (float)Hex.HexShortWidthForSize(Tile.hexSize);

		// Nudge Even Bases
		if (types.Count % 2 == 0) {
			x = (float)(Hex.HexShortWidthForSize(Tile.hexSize) * .5);
		}

		foreach (Hex hex in boardPieces.Keys) {
			rMax = rMax < hex.r ? hex.r : rMax;
			rMin = rMin > hex.r ? hex.r : rMin;
		}

		// Calc new z positions
		float padding = 5;
		float zB = (float)(rMax * Tile.hexSize * 1.5 + padding) * -1;
		float zW = (float)(rMin * Tile.hexSize * 1.5 - padding) * -1;

		whiteBase.gameObject.transform.position = new Vector3(x, oldW.y, zW);
		blackBase.gameObject.transform.position = new Vector3(x, oldB.y, zB);
	}

	private void GenerateTile(Tile.Type type, Tile.Color color)
	{
		// Create and add tile
		GameObject tileGameObject = Instantiate(tilePrefab) as GameObject;
		GameObject baseObject = color == Tile.Color.White ? whiteBase : blackBase;
		Dictionary<Hex, Tile> baseTiles = color == Tile.Color.White ? whiteBasePieces : blackBasePieces;
		tileGameObject.transform.SetParent(baseObject.transform);
		Tile t = tileGameObject.GetComponent<Tile>();
		t.Setup(this, type, color);
		
		// Place the tile.
		// Bases are not on the normal map. They float. As such they only have q and y cords in their hex. 
		List<Tile.Type> types = new List<Tile.Type>(TileCounts.Keys);
		int q = types.Count/2 - types.IndexOf(t.type);
		int i = 0;
		Hex hex;
		do
		{
			hex = new Hex(-q, 0, q, i);
			if (baseTiles.ContainsKey(hex)) 
			{
				i++;
			} else 
			{
				baseTiles.Add(hex, t);
				t.Hex = hex;
				Debug.Log("H - " + hex);
				return;
			}
		} while (true);
	}

	#endregion

	#region Turn Logic

	private void MoveTileToHex(Tile t, Hex h) 
	{
		// Debug.Log("Moving to " + h);
		if (t.location != Tile.Location.Board) {
			whiteBasePieces.Remove(t.Hex);
			blackBasePieces.Remove(t.Hex);
			t.location = Tile.Location.Board;
			t.gameObject.transform.SetParent(transform.parent, true);
		}
	
		// Remove from dictionary if needed.
		boardPieces.Remove(t.Hex);
		// Add tile to dict with new cord and set new cord
		boardPieces.Add(h, t);
		// This moves the piece
		t.Hex = h;
		AdjustBase();
	}

	private void HighlightSpotsForSelected()
	{
		if (firstTurn)
		{
			ShowSpot(new Hex(0,0));
			return;
		}
		if (selectedTile.location != Tile.Location.Board) {
			// TODO: Change to real base placing rule
			ShowExteriorSpots();
			return;
		}
		switch (selectedTile.type)
		{
			case Tile.Type.Ant:
			{
				// Show all neighbor spots
				ShowExteriorSpots();
				break;
			}
			case Tile.Type.GrassHopper:
			default:
			{
				// Show all neighbor spots
				ShowJumpSpots();
				break;
			}
		}
	}

	private void ShowSpot(Hex hc) 
	{
		// Spot Rules #1: Cannot be where you started. 
		if (selectedTile.location == Tile.Location.Board && hc.Equals(selectedTile.Hex)) return;
		Tile t = (Instantiate(highlightSpotPrefab) as GameObject).GetComponent<Tile>();
		t.SetupSpot(this);
		highlightedSpots.Add(t);
		t.Hex = hc;
	}

	public void TileOrSpotTouched(Tile t)
	{
		UnhighlightAllSpots();
		Hex hex = t.Hex;
		if (t.type == Tile.Type.Spot) 
		{
			// Select a spot
			MoveTileToHex(selectedTile, hex);
			selectedTile = null;
			firstTurn = false;
			return;
		} 

		if (selectedTile && selectedTile.location == Tile.Location.Board) {
			// Add selected tile back to dict
			boardPieces.Add(selectedTile.Hex, selectedTile);
		}

		if (t == selectedTile || !CanMoveTile(t)) 
		{
			// Unselect the already selected tile
			selectedTile = null;
		} 
		else
		{ 
			// select a brand new tile
			selectedTile = t;
			if (t.location == Tile.Location.Board) {
				// Remove from dict to show
				boardPieces.Remove(t.Hex);
			} 
			HighlightSpotsForSelected();
		}
	}

	private bool CanMoveTile(Tile tile)
	{
		if (tile.location != Tile.Location.Board) return true;
		// TODO: check if tile is top tile of >2 stack
		// TODO: check if tile is !top tile of >2 stack
		
		// Main rule: cannot create islands
		List<Hex> boardHexes = new List<Hex>(boardPieces.Keys);
		boardHexes.Remove(tile.Hex);
		HashSet<Hex> continuousHexes = BreadthFirstSearch(boardHexes);
		if (continuousHexes.Count != boardHexes.Count) return false;
		return true;
	}

	private HashSet<Hex> BreadthFirstSearch(List<Hex> validHexes) {
		var visited = new HashSet<Hex>();
			
		var queue = new Queue<Hex>();
		queue.Enqueue(validHexes.First());

		while (queue.Count > 0) {
			var vertex = queue.Dequeue();

			if (visited.Contains(vertex)) {
				continue;
			}

			visited.Add(vertex);

			foreach(var neighbor in vertex.Neighbors()) {
				if (!validHexes.Contains(neighbor)) {
					continue;
				}
				if (!visited.Contains(neighbor)) {
					queue.Enqueue(neighbor);
				}
			}
		}

		return visited;
	}

	private void UnhighlightAllSpots() 
	{
		while (highlightedSpots.Count > 0) 
		{
			Tile t = highlightedSpots[0];
			highlightedSpots.RemoveAt(0);
			GameObject go = t.gameObject;
			DestroyImmediate(go);
		}
	}

	public void TileDragged(Tile t) 
	{
		Debug.Log("TileDragged");
	}

	public void TileDragEnded(Tile t)
	{
		Debug.Log("TileDragEnded");
	}

	#endregion

	#region Spot Finders

	// private List<bool> WithHexes(Hex hex)
	// {
	// 	List<Hex> validDirections = new List<Hex>();
	// 	foreach (Hex neighbor in neighbors) {
	// 		if (boardPieces.ContainsKey(neighbor)) {
	// 			Hex direction = neighbor.Subtract(selectedTile.Hex);
	// 			validDirections.Add(direction);
	// 		}
	// 	}
	// }

	private void ShowJumpSpots()
	{
		List<Hex> neighbors = selectedTile.Hex.Neighbors();
		List<Hex> validDirections = new List<Hex>();
		foreach (Hex neighbor in neighbors) {
			if (boardPieces.ContainsKey(neighbor)) {
				Hex direction = neighbor.Subtract(selectedTile.Hex);
				validDirections.Add(direction);
			}
		}
		foreach (Hex validDirection in validDirections)
		{
			Hex checker = selectedTile.Hex.Add(validDirection);
			while (boardPieces.ContainsKey(checker)) {
				checker = checker.Add(validDirection);
			}
			ShowSpot(checker);
		}
	}
	
	private void ShowExteriorSpots() 
	{
		// Get the first on the outside path.
		Hex mostRemoteHex = Hex.LongestHex(new List<Hex>(boardPieces.Keys));
		List<Hex> outerRing = Ring(mostRemoteHex.Length()+1);
		List<Hex> neighbors = mostRemoteHex.Neighbors();
		
		Hex currentSpot = outerRing.Intersect(neighbors).First();
		Hex firstSpot = currentSpot;
		Hex currentBoard = mostRemoteHex;

		// Going clockwise around the circle
		do {
			Debug.Log(currentSpot);
			ShowSpot(currentSpot);
			neighbors = currentSpot.Neighbors();
			// First, push forward the current board piece. 
			bool adjustBoardTile = true;
			while (adjustBoardTile) {
				Hex nplus1 = neighbors[(neighbors.IndexOf(currentBoard) + 1) % 6];
				Hex nplus2 = neighbors[(neighbors.IndexOf(currentBoard) + 2) % 6];
				if (!boardPieces.ContainsKey(nplus1) && !boardPieces.ContainsKey(nplus2)) 
				{
					// Path is clear - dont adjust the board tile
					adjustBoardTile = false;
					currentSpot = nplus1;
				} else 
				{
					// This is too tight an area! 
					currentBoard = boardPieces.ContainsKey(nplus2) ? nplus2 : nplus1;
				}
			}
		} while (!currentSpot.Equals(firstSpot));
	}

	private List<Hex> Ring(int radius) 
	{
		List<Hex> ring = new List<Hex>();
		Hex ringMember = Hex.Direction(4).Scale(radius);
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < radius; j++)
			{
				ring.Add(ringMember);
				ringMember = ringMember.Neighbor(i);
			}
		}
		return ring;
	}

	#endregion
}
