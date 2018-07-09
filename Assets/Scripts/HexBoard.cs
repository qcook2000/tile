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
		{Tile.Type.Beetle, 3}, 
		{Tile.Type.Spider, 3}, 
		// {Tile.Type.Pillbug, 3}, 
		{Tile.Type.Queen, 1}
	};

	// Hex Structs
	public enum MoveType {TouchSpot, TouchCanMove, TouchCantMove, TouchSelected}; 
	public struct HexWalker {
		public Hex currentSpot;  
    	public Hex currentBoard;  
		public bool clockwise;
		public HexWalker(Hex s, Hex b, bool c) {
			currentSpot = s;
			currentBoard = b;
			clockwise = c;
		}
	}
	
	// Game objects
	public GameObject tilePrefab;
	public GameObject highlightSpotPrefab;
	public GameObject whiteBase;
	public GameObject blackBase;

	// Internal
	private Dictionary<Hex, Tile> pieces = new Dictionary<Hex, Tile>();
	private List<Hex> BoardHexes() {
		return new List<Hex>(pieces.Keys.Where(h => (h.area == GameArea.Board)));
	} 
	private Dictionary<Hex, Tile> highlightedSpots = new Dictionary<Hex, Tile>();
	private int turn;

	#region Setup

    private void Start()
	{
		GenerateBoard();
		turn = -1;
		// EnableFakeBoard("Origin");
		StartNewTurn();

	}

	static int Mod(int k, int n) {  return ((k %= n) < 0) ? k+n : k;  }

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
		List<Tile> usableTiles = new List<Tile>(pieces.Values);
		for (int i = 0; i < fakeBoard.Count; i++) {
			MoveTileToHex(usableTiles[i], fakeBoard[i]);
		}
		turn = fakeBoard.Count;
	}

	private void PrintTiles(Dictionary<Hex, Tile> dict)
	{
		string logString = "Printind Dict: Count " + dict.Count;
		foreach (KeyValuePair<Hex, Tile> kv in dict) {
			logString += "\n" + kv.Key + " " + kv.Value.team + " " + kv.Value.type;
		}
		Debug.Log(logString);
	}

	private void PrintHexes(IEnumerable<Hex> list)
	{
		string logString = "Printind Lisst: Count " + list.Count();
		foreach (Hex h in list) {
			logString += "\n" + h;
		}
		Debug.Log(logString);
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
				GenerateTile(type, Tile.Team.White);
				GenerateTile(type, Tile.Team.Black);
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

		// Nudge Even Bases
		if (types.Count % 2 == 0) {
			x = (float)(Hex.HexShortWidthForSize(Tile.hexSize) * .5);
		}


		foreach (Hex hex in pieces.Keys.Where(hex => (hex.area == GameArea.Board)))
		{
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

	private void GenerateTile(Tile.Type type, Tile.Team color)
	{
		// Create and add tile
		GameObject tileGameObject = Instantiate(tilePrefab) as GameObject;
		GameObject baseObject = color == Tile.Team.White ? whiteBase : blackBase;
		tileGameObject.transform.SetParent(baseObject.transform);
		Tile t = tileGameObject.GetComponent<Tile>();
		t.Setup(this, type, color);
		
		// Place the tile.
		// Bases are not on the normal map. They float. As such they only have q and y cords in their hex. 
		List<Tile.Type> types = new List<Tile.Type>(TileCounts.Keys);
		int q = types.Count/2 - types.IndexOf(t.type);
		int i = 0;
		Hex hex;
		GameArea area = color == Tile.Team.Black ? GameArea.BlackBase : GameArea.WhiteBase;
		do
		{
			hex = new Hex(-q, 0, q, i, area);
			if (pieces.ContainsKey(hex)) 
			{
				i++;
			} else 
			{
				pieces.Add(hex, t);
				t.Hex = hex;
				//Debug.Log("H - " + hex);
				return;
			}
		} while (true);
	}

	#endregion

	#region Turn Logic

	private void MoveTileToHex(Tile t, Hex h) 
	{
		// Debug.Log("Moving to " + h);

		// Remove from base
		if (t.Hex.area != GameArea.Board) {
			t.gameObject.transform.SetParent(transform.parent, true);
		}
		// Remove from dictionary if needed.
		pieces.Remove(t.Hex);
		// Add tile to dict with new cord and set new cord
		pieces.Add(h, t);
		// This moves the piece
		t.Hex = h;
		AdjustBase();
	}

	private void ShowSpotsForTileAndType(Tile tile, Tile.Type type) {
		switch (type) {
			case Tile.Type.Ladybug:     ShowLadybugSpots(tile); break;
			case Tile.Type.GrassHopper: ShowJumpSpots(tile); break;
			case Tile.Type.Mosquito:    ShowMosquitoSpots(tile); break;
			case Tile.Type.Ant:         ShowExteriorSpots(tile); break;
			case Tile.Type.Beetle:      ShowBeetleSpots(tile); break;
			case Tile.Type.Spider:      ShowStepSpots(3, tile); break;
			case Tile.Type.Pillbug:     ShowExteriorSpots(tile); break;
			case Tile.Type.Queen:       ShowStepSpots(1, tile); break;
		}
	}

	private void ShowSpot(Hex hc) 
	{
		// Cannot have spot where there is a tile
		
		if (pieces.ContainsKey(hc)) throw new ArgumentException("Cannot show spot where tile is");
		// Dont double highlight.
		if (highlightedSpots.ContainsKey(hc)) return;
		Tile t = (Instantiate(highlightSpotPrefab) as GameObject).GetComponent<Tile>();
		t.SetupSpot(this);
		highlightedSpots.Add(hc, t);
		t.Hex = hc;
	}

	private Tile SelectedTile()
	{
		List<Tile> selected = new List<Tile>();
		foreach (Hex h in pieces.Keys) {
			if (pieces[h].State == Tile.TileState.Selected) selected.Add(pieces[h]);
		}
		if (selected.Count > 1) throw new ArgumentException("Too many items selected");
		if (selected.Count == 0) return null;
		return selected[0];
	}

	public void ClearBoardState()
	{
		foreach (Hex h in highlightedSpots.Keys) {
			DestroyImmediate(highlightedSpots[h].gameObject);
		}
		highlightedSpots.Clear();
		foreach (Hex h in pieces.Keys) {
			pieces[h].State = Tile.TileState.Normal;
		}
	}

	public void SpotTouched(Tile t)
	{
		Tile tileToMove = SelectedTile();
		Hex location = t.Hex;
		ClearBoardState();
		// Select a spot
		MoveTileToHex(tileToMove, location);
		StartNewTurn();
		return;
	}

	public void SelectTile(Tile t)
	{
		Tile.TileState incomingState = t.State;
		ClearBoardState();
		Hex hex = t.Hex;
		if (incomingState == Tile.TileState.Normal || incomingState == Tile.TileState.Selected)
		{
			ResetTurn();
			return;
		}
		else
		{
			// Select a brand new tile
			t.State = Tile.TileState.Selected;
			if (t.Hex.area != GameArea.Board) {
				ShowBasePlacingSpots(t);
			} else {
				ShowSpotsForTileAndType(t, t.type);
			}
		}
	}

	private void StartNewTurn()
	{
		turn += 1;
		ResetTurn();
	}

	private Hex FindQueen(Tile.Team team)
	{
		return pieces.Where(kv => (kv.Value.type == Tile.Type.Queen && kv.Value.team == team)).First().Key;
	}

	private void ResetTurn()
	{
		// Show which tiles can be moved
		Tile.Team team = turn % 2 == 0 ? Tile.Team.Black : Tile.Team.White;
		GameArea homebase = team == Tile.Team.Black ? GameArea.BlackBase : GameArea.WhiteBase;
		

		// First determine if the queen has been placed
		bool queenMustHaveBeenPlaced = (turn/2.0 >= 2.0);

		Hex queenLocation = FindQueen(team);
		if (queenMustHaveBeenPlaced && queenLocation.area != GameArea.Board) {
			pieces[queenLocation].State = Tile.TileState.CanBeSelected;
			// Only valid move is the queen
			return;
		}

		// First highlight the pieces in the base
		foreach (Hex h in pieces.Keys.Where(h => (h.area == homebase))) {
			Hex higherHex = h.Up();
			if (!pieces.ContainsKey(higherHex)) {
				pieces[h].State = Tile.TileState.CanBeSelected;
			}
		}
		if (queenLocation.area == GameArea.Board) {
			// Highlight valid moves on the board
			foreach (KeyValuePair<Hex, Tile> kv in pieces.Where(kv => (kv.Value.team == team))) {
				// Cannot be under another tile
				Hex higherHex = kv.Key.Up();
				if (pieces.ContainsKey(higherHex)) continue;

				// Cannot create islands
				List<Hex> boardHexes = BoardHexes();
				boardHexes.Remove(kv.Key);
				HashSet<Hex> continuousHexes = BreadthFirstSearch(boardHexes);
				if (continuousHexes.Count != boardHexes.Count) continue;
				
				// Great, it can be selected
				pieces[kv.Key].State = Tile.TileState.CanBeSelected;
			}
		}
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

	private void ShowBasePlacingSpots(Tile t)
	{
		if (turn == 0) {
			ShowSpot(new Hex(0,0));
			return;
		} 
		if (turn == 1) {
			List<Hex> neighbors = (new Hex(0,0)).Neighbors();
			foreach (Hex hex in neighbors) ShowSpot(hex);
			return;
		}
		// Get top board pieces
		IEnumerable<Hex> possibleMoves = new HashSet<Hex>();
		IEnumerable<Hex> impossibleMoves = new HashSet<Hex>();
		foreach(KeyValuePair<Hex, Tile> kv in pieces.Where(kv => (kv.Key.area == GameArea.Board))) {
			// If this piece if not the top piece, ignore
			if (pieces.ContainsKey(kv.Key.Up())) continue;
			IEnumerable<Hex> openSpots = kv.Key.Neighbors().Where(n => (!pieces.ContainsKey(n)));
			if (kv.Value.team == t.team) possibleMoves = possibleMoves.Union(openSpots);
			else impossibleMoves = impossibleMoves.Union(openSpots);
		}
		PrintHexes(possibleMoves);
		PrintHexes(impossibleMoves);
		foreach (Hex hex in possibleMoves) {
			if (impossibleMoves.Contains(hex)) continue;
			ShowSpot(hex);
		}		
	}

	private void ShowLadybugSpots(Tile selectedTile)
	{
		HashSet<Hex> visited = new HashSet<Hex>() {selectedTile.Hex};
		List<List<Hex>> fringes = new List<List<Hex>>(); // array of arrays of cubes
		fringes.Add(new List<Hex>(){selectedTile.Hex});
		
    	for (int k = 0; k < 2; k++) {
			fringes.Add(new List<Hex>());
			foreach (Hex hex in fringes[k])
			{
				foreach (Hex neighbor in hex.Neighbors()) 
				{
					if (!visited.Contains(neighbor) && pieces.ContainsKey(neighbor)) {
						visited.Add(neighbor);
						fringes[k+1].Add(neighbor);
						if (k == 1) {
							// These are the possible board pieces the ladybug can jump down from
							foreach (Hex n in neighbor.Neighbors()) {
								if (!pieces.ContainsKey(n)) {
									ShowSpot(n);
								}
							}
						}
					}
				}
			}
		}
	}

	private void ShowMosquitoSpots(Tile selectedTile) 
	{
		foreach(Hex h in selectedTile.Hex.Neighbors()) 
		{
			if (pieces.ContainsKey(h)) {
				// Get the type of this neighbor and show the spots for that type
				if (pieces[h].type != Tile.Type.Mosquito){
					ShowSpotsForTileAndType(selectedTile, pieces[h].type);
				}
			}
		}
	}

	private bool HasNeighborOnBoard(Hex h)
	{
		foreach(Hex n in h.Neighbors()) 
		{
			if (pieces.ContainsKey(n)) {
				return true;
			}
		}
		return false;
	}

	private void ShowBeetleSpots(Tile selectedTile)
	{
		foreach(Hex h in selectedTile.Hex.Neighbors()) 
		{
			int index = 0;
			Hex topSpot;
			do {
				topSpot = new Hex(h.q, h.r, h.s, index);
				index++;
			} while (pieces.ContainsKey(topSpot));
			ShowSpot(topSpot);
		}
	}

	private void ShowStepSpots(int steps, Tile selectedTile)
	{
		Hex currentSpot = selectedTile.Hex;
		Hex firstSpot = currentSpot;

		List<Hex> allNeighborSpots = currentSpot.Neighbors();
		bool[] boardNeighbors = {false, false, false, false, false, false};
		

		for (int i = 0; i < 6; i++) {
			boardNeighbors[i] = pieces.ContainsKey(allNeighborSpots[i]);
			if (boardNeighbors[i] && !boardNeighbors[Mod((i-1),6)]) {
				if (i == 5 && boardNeighbors[0]) return; // Then we have a loop
				// Found a spot to start highlighting from
				Debug.Log("i:" + i + " o:" + firstSpot + " b:" + allNeighborSpots[i]);
				HexWalker rwalker = new HexWalker(firstSpot, allNeighborSpots[i], true);
				HexWalker lwalker = new HexWalker(firstSpot, allNeighborSpots[i], false);
				for (int j = 0; j < steps; j++) {
					// Debug.Log(currentSpot);
					rwalker = GetNextExteriorSpot(rwalker);
					lwalker = GetNextExteriorSpot(lwalker);
				}
				ShowSpot(rwalker.currentSpot);
				ShowSpot(lwalker.currentSpot);
			}
		}
	}

	private void ShowJumpSpots(Tile selectedTile)
	{
		List<Hex> neighbors = selectedTile.Hex.Neighbors();
		List<Hex> validDirections = new List<Hex>();
		foreach (Hex neighbor in neighbors) {
			if (pieces.ContainsKey(neighbor)) {
				Hex direction = neighbor.Subtract(selectedTile.Hex);
				validDirections.Add(direction);
			}
		}
		foreach (Hex validDirection in validDirections)
		{
			Hex checker = selectedTile.Hex.Add(validDirection);
			while (pieces.ContainsKey(checker)) {
				checker = checker.Add(validDirection);
			}
			ShowSpot(checker);
		}
	}
	
	private void ShowExteriorSpots(Tile selectedTile) 
	{
		// Get the first on the outside path.
		Hex mostRemoteHex = Hex.LongestHex(BoardHexes());
		List<Hex> outerRing = Ring(mostRemoteHex.Length()+1);
		List<Hex> neighbors = mostRemoteHex.Neighbors();
		
		Hex firstSpot = outerRing.Intersect(neighbors).First();
		HexWalker walker = new HexWalker(firstSpot, mostRemoteHex, true);

		do {
			// Debug.Log(currentSpot);
			ShowSpot(walker.currentSpot);
			walker = GetNextExteriorSpot(walker);
		} while (!walker.currentSpot.Equals(firstSpot));
	}

	private HexWalker GetNextExteriorSpot(HexWalker h)
	{
		Hex currentSpot = h.currentSpot;
		Hex currentBoard = h.currentBoard;
		List<Hex> neighbors = currentSpot.Neighbors();
		// First, push forward the current board piece. 
		bool adjustBoardTile = true;
		while (adjustBoardTile) {
			int direction = h.clockwise ? 1 : -1;
			Hex nplus1 = neighbors[Mod((neighbors.IndexOf(currentBoard) + (1*direction)), 6)];
			Hex nplus2 = neighbors[Mod((neighbors.IndexOf(currentBoard) + (2*direction)), 6)];
			if (!pieces.ContainsKey(nplus1) && !pieces.ContainsKey(nplus2)) 
			{
				// Path is clear - dont adjust the board tile
				adjustBoardTile = false;
				currentSpot = nplus1;
			} else 
			{
				// This is too tight an area! 
				currentBoard = pieces.ContainsKey(nplus2) ? nplus2 : nplus1;
			}
		}
		return new HexWalker(currentSpot, currentBoard, h.clockwise);
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
