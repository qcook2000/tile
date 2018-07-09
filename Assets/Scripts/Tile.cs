using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalagaan.POFX;


public class Tile : MonoBehaviour {

	public enum Type { Ladybug, Pillbug, Mosquito, GrassHopper, Ant, Beetle, Queen, Spider, Spot };
	public enum Team { White, Black };
	public enum TileState { Normal, CanBeSelected, Selected }


	public HexBoard board;
    private Hex hex;
    public Type type;
	public Team team;
	public string hexStr;
    private TileState state;

    public GameObject bugPrefab;
	public GameObject bugBlender;

	private POFX_Rim rimLayer;
    private POFX_Outline outlineLayer;

	public Material blackMaterial;
	public Material whiteMaterial;

	public static double hexSize = 1.0;
	public static double hexHeight = 0.3;

	public static float canBeSelectedOutlineIntesity = 0.25f;
	public static float canBeSelectedRimIntesity = 0.1f;
	public static float selectedOutlineIntesity = 0.66f;
	public static float selectedRimIntesity = 0.33f;
	
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

    public TileState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
			switch (value) {
				case TileState.CanBeSelected:
					rimLayer.m_cParams.intensity = canBeSelectedRimIntesity;
					outlineLayer.m_cParams.intensity = canBeSelectedOutlineIntesity;
					break;
				case TileState.Selected:
					rimLayer.m_cParams.intensity = selectedRimIntesity;
					outlineLayer.m_cParams.intensity = selectedOutlineIntesity;
					break;
				case TileState.Normal:
				default: 
					rimLayer.m_cParams.intensity = 0;
					outlineLayer.m_cParams.intensity = 0;
					break;
			}
        }
    }


    // Use this for initialization
    void Start () {
		
	}

	public string StringForType(Type type)
	{
		// Debug.Log(type);
		switch (type) {
			case Type.Ladybug: return "Ladybug";
			case Type.GrassHopper: return "Grasshopper";
			case Type.Mosquito: return "Mosquito";
			case Type.Ant: return "Ant";
			case Type.Beetle: return "TankBeetle";
			case Type.Spider: return "Spider";
			case Type.Pillbug: return "Pincher";
			case Type.Queen: return "Queen";
		}
		return null;
	}

	public void Setup(HexBoard board, Type type, Team team) 
	{
		this.board = board;
		this.type = type;
		this.team = team;
		
		// Set color material
        GetComponent<Renderer>().material = team == Team.Black ? blackMaterial : whiteMaterial;

		// Add bug object
		GameObject bugGameObject = Instantiate(bugPrefab) as GameObject;
		bugGameObject.transform.SetParent(this.transform);

		// Set correct mesh for bug type
		string typeString = StringForType(type);
		GameObject newBugBlend = bugBlender.transform.Find(typeString).gameObject;
		bugGameObject.GetComponent<MeshFilter>().mesh = newBugBlend.GetComponent<MeshFilter>().sharedMesh;

		// Scale bug to fit
		float tilewidth = Mathf.Sqrt(3) * (float)hexSize;
		Vector3 bugScale = bugGameObject.GetComponent<Renderer>().bounds.size;
		float padding = 0.5f;
		float xFraction = (tilewidth - padding) / bugScale.x;
		float zFraction = (tilewidth - padding) / bugScale.z;
		float fraction = Mathf.Min(xFraction, zFraction);
		bugGameObject.transform.localScale *= fraction;

		// Position bug correctly
		bugGameObject.transform.localPosition = new Vector3(0, 0, (float)hexHeight/2 + 0.01f);
		
		// Set the state to normal
		outlineLayer = (POFX_Outline)GetComponent<POFX>().GetLayer(0);
		rimLayer = (POFX_Rim)GetComponent<POFX>().GetLayer(1);
		this.State = TileState.Normal;
	}

	public void SetupSpot(HexBoard board) 
	{
		this.board = board;
		this.type = Type.Spot;
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
		if (type == Type.Spot) board.SpotTouched(this);
		else board.SelectTile(this);
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
