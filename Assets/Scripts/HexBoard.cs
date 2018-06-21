using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexBoard : MonoBehaviour {

	// Game setup constants
	static int CountOfType(Tile.Type type)
	{
		switch (type) 
		{
			case Tile.Type.Ladybug: return 2;
			case Tile.Type.Mosquito: return 3;
			case Tile.Type.Roach: return 4;
			case Tile.Type.Ant: return 2;
			case Tile.Type.RollyPolly: return 3;
			case Tile.Type.Queen: return 1;
		}
		return 0;
	}
	
	static float hexSize = 1f;
	static float hexHeight = 0.3f;

	// Hex Structs
	public enum GameArea {Board, WhiteBase, BlackBase}; 

	// Hex Cord and Conveniance Methods
	public struct HexCord
	{
		public int x; // Left right
		public int y; // Up down
		public int z; // Front back
		public GameArea a; // Area 
	}
	private String HCStr(HexCord hc) { return "x" + hc.x + ", y" + hc.y + ", z" + hc.z + ", a" + hc.a;}
	private HexCord HC(int x, int z) { return new HexCord() {x = x, y = 0, z = z, a = GameArea.Board}; }
	private HexCord HC(int x, int y, int z) { return new HexCord() {x = x, y = y, z = z, a = GameArea.Board}; }
	private HexCord HC(int x, int y, int z, GameArea a) { return new HexCord() {x = x, y = y, z = z, a = a}; }

	
	// Game objects
	public GameObject tilePrefab;
	public GameObject highlightSpotPrefab;
	private Dictionary<HexCord, Tile> tiles = new Dictionary<HexCord, Tile>();
	private List<Tile> highlightedSpots = new List<Tile>();
	private Tile selectedTile;
	private bool firstTurn;



    private void Start()
	{
		GenerateBoard();
		firstTurn = true;

		// Create a fake board layout to test move highlighter
		HexCord[] fakeBoard = {HC(0,-1), HC(-1,0), HC(-1,1), HC(0,1), HC(1,0), HC(1,-1), HC(2,-2), HC(2,0), HC(3,-1)}; 

		// for (int i = 0; i < fakeBoard.Length; i++) {

		// }
	}

	private void GenerateBoard()
	{
		foreach (Tile.Type type in Enum.GetValues(typeof(Tile.Type)))
		{
			int count = CountOfType(type);
			for (int y = 0; y < count; y++)
			{
				GenerateTile(type, true, y);
				GenerateTile(type, false, y);
			}
		}
	}

	private void GenerateTile(Tile.Type type, bool isWhite, int depth)
	{
		GameObject tileGameObject = Instantiate(tilePrefab) as GameObject;
		tileGameObject.transform.SetParent(transform);
		Tile t = tileGameObject.GetComponent<Tile>();
		t.board = this;
		HexCord cord = GetBaseHexCordForTypeColorAndDepth(type, isWhite, depth);
		MoveTileToHexCord(t, cord);
		// tileGameObject.
	}

	private HexCord GetBaseHexCordForTypeColorAndDepth(Tile.Type type, bool isWhite, int depth)
	{
		Array types = Enum.GetValues(typeof(Tile.Type));
		int padding = 6;
		int z = (int)Math.Floor(GetBoardYDim()/2.0) + padding;
		if (types.Length % 2 == 0) z -= 1;
		if (isWhite) z *= -1;
		int x = z/-2 - types.Length/2 + Array.IndexOf(types, type);
		//if (!isWhite) x -=(int)Math.Floor(GetBoardYDim()/2.0) + padding;
		// Debug.Log("Type: " + type + ", Index:" + typeOrder.IndexOf(type));
		return HC(x, depth, z, isWhite ? GameArea.WhiteBase : GameArea.BlackBase);
	}

	private int GetBoardYDim() 
	{
		return 1;
	}

	private void MoveTileToHexCord(Tile t, HexCord hc) 
	{
		Debug.Log("Moving to " + HCStr(hc));
		// Remove from dictionary if needed.
		tiles.Remove(t.cord);
		// Add tile to dict with new cord and set new cord
		tiles.Add(hc, t);
		t.cord = hc;
		// Get transform from hex cord
		
		
		// NOTE note sure why its off by 3.4... but it is. 
    	float x = (hexSize * ((float)Math.Sqrt(3) * hc.x  + (float) Math.Sqrt(3)/2 * hc.z));
    	float z = hexSize * (3.0f/2 * hc.z);
    	Vector3 newPos = new Vector3(x, (hc.y * hexHeight) + (0.5f * hexHeight), z);
		t.gameObject.transform.position = newPos;
	}

	public Tile SelectedTile
    {
        get
        {
            return selectedTile;
        }

        set
        {
            if (selectedTile || !value) 
			{
				// Todo: Clear old selected
				UnhighlightAllSpots();
			}
			selectedTile = value;
			if (selectedTile) 
			{
				// Highlight possible positions
				if (firstTurn)
				{
					HighlightHexCord(new HexCord(){x=0,y=0,z=0});
				} 
				else 
				{
					// Highlight Spots For Selected
				}
			}
        }
    }

	private void HighlightSpotsForSelected()
	{
		// First do ants
		// Ants can go around the outside of the hive
	}

	private void HighlightHexCord(HexCord hc) 
	{
		// Highlight
		GameObject highlightSpotGameObject = Instantiate(highlightSpotPrefab) as GameObject;
		highlightSpotGameObject.transform.SetParent(transform);
		Tile t = highlightSpotGameObject.GetComponent<Tile>();
		t.isSpot = true;
		t.board = this;
		highlightedSpots.Add(t);
		MoveTileToHexCord(t, hc);
	}

	public void TileOrSpotTouched(Tile t)
	{
		if (t.isSpot) 
		{
			// select a spot
			MoveTileToHexCord(selectedTile, t.cord);
			this.SelectedTile = null;
			firstTurn = false;
		} 
		if (t == selectedTile) 
		{
			// unselect the already selected tile
			this.SelectedTile = null;
		} 
		else 
		{ 
			// select a brand new tile
			this.SelectedTile = t;
		}
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
}
