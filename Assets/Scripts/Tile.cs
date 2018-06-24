using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public enum Type { Ladybug, GrassHopper, Mosquito, Ant, RollyPolly, Queen, Spot };
	public enum Location { WhiteBase, BlackBase, Board }
	public enum Color { White, Black };

	public HexBoard board;
    private Hex hex;
    public Type type;
	public Location location;
	public Color color;
	public string hexStr; 

	public static double hexSize = 1.0;
	public static double hexHeight = 0.3;
	
	private Vector3 VectorFromHex(Hex h)
	{
		Point p = h.ToPixel(hexSize);
    	return new Vector3((float)p.x, (float)(h.y * hexHeight + 0.8 * hexHeight), (float)-p.y);
	}

    public Hex Hex
    {
        get
        {
            return hex;
        }

        set
        {
            hex = value;
			hexStr = hex.ToString();
			Vector3 newPos = VectorFromHex(hex);
			gameObject.transform.localPosition = newPos;
        }
    }


    // Use this for initialization
    void Start () {
		
	}

	public void Setup(HexBoard board, Type type, Color color) 
	{
		this.board = board;
		this.type = type;
		this.color = color;
		this.location = color == Color.White ? Location.WhiteBase : Location.BlackBase;
	}

	public void SetupSpot(HexBoard board) 
	{
		this.board = board;
		this.type = Type.Spot;
		this.location = Location.Board;
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
