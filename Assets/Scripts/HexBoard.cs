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
			case Tile.Type.Roach: return 1;
		}
		return -1;
	}
	static List<Tile.Type> typeOrder = new List<Tile.Type> {Tile.Type.Ladybug, Tile.Type.Mosquito, Tile.Type.Roach};
	static float hexSize = 1f;
	static float hexHeight = 0.3f;

	// Hex Structs
	public struct HexCord
	{
		public int x; // Left right
		public int y; // Up down
		public int z; // Front back
	}
	public String HCStr(HexCord hc) { return "x: " + hc.x + ", y: " + hc.y + ", z: " + hc.z;}

	// Game areas
	public Dictionary<HexCord, Stack<Tile>> tileBoard = new Dictionary<HexCord, Stack<Tile>>();
	public Dictionary<Tile.Type, Stack<Tile>> whitesTiles = new Dictionary<Tile.Type, Stack<Tile>>();
	public Dictionary<Tile.Type, Stack<Tile>> blacksTiles = new Dictionary<Tile.Type, Stack<Tile>>();
	
	// Game objects
	public GameObject tilePrefab;
	public GameObject highlightSpotPrefab;
    private Tile selectedTile;
	private List<Tile> highlightedSpots = new List<Tile>();

	// Game Logic
	private bool firstTurn;

    private void Start()
	{
		GenerateBoard();
		firstTurn = true;
	}

	private void GenerateBoard()
	{
		foreach (Tile.Type type in typeOrder)
		{
			int count = CountOfType(type);
			for (int y = 0; y < count; y++)
			{
				GenerateTile(type, true);
				GenerateTile(type, false);
			}
		}
	}

	private void GenerateTile(Tile.Type type, bool isWhite)
	{
		GameObject tileGameObject = Instantiate(tilePrefab) as GameObject;
		tileGameObject.transform.SetParent(transform);
		Tile t = tileGameObject.GetComponent<Tile>();
		t.board = this;
		Dictionary<Tile.Type, Stack<Tile>> homeBase = isWhite ? whitesTiles : blacksTiles;
		if (!homeBase.ContainsKey(type)) { 
			homeBase[type] = new Stack<Tile>();
		}
		homeBase[type].Push(t);
		HexCord cord = GetBaseHexCordForTypeColorAndDepth(type, isWhite, homeBase[type].Count - 1);
		MoveTileToHexCord(t, cord);


		// tileGameObject.
	}

	private HexCord GetBaseHexCordForTypeColorAndDepth(Tile.Type type, bool isWhite, int depth)
	{
		int z = (int)Math.Floor(GetBoardYDim()/2.0) + 2;
		if (isWhite) z *= -1;
		int x = (int)Math.Floor(typeOrder.Count/2.0) + typeOrder.IndexOf(type) - 1;
		if (!isWhite) x -=(int)Math.Floor(GetBoardYDim()/2.0) + 2;
		// Debug.Log("Type: " + type + ", Index:" + typeOrder.IndexOf(type));
		return new HexCord() {x = x, z = z, y = depth};
	}

	private int GetBoardYDim() 
	{
		return 1;
	}

	private void MoveTileToHexCord(Tile t, HexCord hc) 
	{
		// Get transform from hex cord
		// Debug.Log(HCStr(hc));
    	float x = hexSize * ((float)Math.Sqrt(3) * hc.x  + (float) Math.Sqrt(3)/2 * hc.z);
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
            if (selectedTile) {
				// Todo: Clear old selected
				UnhighlightAllSpots();
			}
			selectedTile = value;
			// Highlight possible positions
			if (firstTurn) {
				HighlightHexCord(new HexCord(){x=0,y=0,z=0});
			} else {
				// Highlight Spots For Selected
				
			}
        }
    }

	private void HighlightHexCord(HexCord hc) 
	{
		// Highlight
		GameObject highlightSpotGameObject = Instantiate(highlightSpotPrefab) as GameObject;
		highlightSpotGameObject.transform.SetParent(transform);
		Tile t = highlightSpotGameObject.GetComponent<Tile>();
		t.board = this;
		highlightedSpots.Add(t);
		MoveTileToHexCord(t, hc);
	}

	private void UnhighlightAllSpots() 
	{
		foreach (Tile t in highlightedSpots)
		{
			Destroy(t.gameObject);
		}
		highlightedSpots = new List<Tile>();
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
