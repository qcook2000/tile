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
			case Tile.Type.Ladybug: return 1;
			case Tile.Type.Mosquito: return 1;
			case Tile.Type.Roach: return 1;
		}
		return -1;
	}
	static List<Tile.Type> typeOrder = new List<Tile.Type> {Tile.Type.Ladybug, Tile.Type.Ladybug, Tile.Type.Roach};

	// Hex Structs
	public struct HexCord
	{
		public int x;
		public int y;
	}

	// Game areas
	public Dictionary<HexCord, Stack<Tile>> tileBoard = new Dictionary<HexCord, Stack<Tile>>();
	public Dictionary<Tile.Type, Stack<Tile>> whitesTiles = new Dictionary<Tile.Type, Stack<Tile>>();
	public Dictionary<Tile.Type, Stack<Tile>> blacksTiles = new Dictionary<Tile.Type, Stack<Tile>>();
	
	// Game objects
	public GameObject tilePrefab;

	private void Start()
	{
		GenerateBoard();
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
		Dictionary<Tile.Type, Stack<Tile>> homeBase = isWhite ? whitesTiles : blacksTiles;
		if (!homeBase.ContainsKey(type)) { 
			homeBase[type] = new Stack<Tile>();
		}
		homeBase[type].Push(t);
		HexCord cord = GetBaseHexCordForTypeAndColor(type, isWhite);
		MoveTileToHexCord(t, cord);
	}

	private HexCord GetBaseHexCordForTypeAndColor(Tile.Type type, bool isWhite)
	{
		int y = (int)Math.Floor(GetBoardYDim()/2.0) + 2;
		if (isWhite) y *= -1;
		int x = (int)Math.Floor(typeOrder.Count/2.0) + typeOrder.IndexOf(type);
		return new HexCord() {x = x, y = y};
	}

	private int GetBoardYDim() 
	{
		return 1;
	}

	private void MoveTileToHexCord(Tile t, HexCord hc) 
	{
		// TODO
	}
}
