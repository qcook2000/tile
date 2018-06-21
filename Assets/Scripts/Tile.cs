using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public enum Type { Ladybug, Roach, Mosquito, Ant, RollyPolly, Queen };

	public HexBoard board;
	public HexBoard.HexCord cord;
	public Type type;
//	private Vector3 dragOrigin;
	public bool isSpot;


	// Use this for initialization
	void Start () {
		
	}
	
	void OnMouseDrag() 
	{
		// if (!selected) {
		// 	board.SetTileSelected(this);
		// }
		// if (!dragging) {
		// 	// Start Drag
		// 	x = Input.mousePosition.x;
        // 	y = Input.mousePosition.y;
		// }
		// Debug.Log("DRAGGING HANDLER: " + Input.mousePosition);
		// dragging = true;
		// TODO: Send Drag Event
	}

	void OnMouseUp() 
	{
		// if (dragging) {
		// 	board.TileDragEnded(this);
		// 	dragging = false;
		// } else {
		// 	board.TileTappedOrClicked(this);
		// }
		board.TileOrSpotTouched(this);
	}

	// Update is called once per frame
	void Update () 
	{
		// if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) 
		// {
		// 	dragging = true;
		// 	// TODO: Send Drag Event
		// } 
		// else if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)) 
		// {
		// 	if (dragging) {
		// 		board.TileDragEnded(this);
		// 		dragging = false;
		// 	} else {
		// 		board.TileTappedOrClicked(this);
		// 	}
		// }
	}
}
