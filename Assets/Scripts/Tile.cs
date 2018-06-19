using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public enum Type { Ladybug, Roach, Mosquito };
	
	public static List<KeyValuePair<Type, int>> TypeCounts = new List<KeyValuePair<Type, int>>
    {
        new KeyValuePair<Type, int>(Type.Ladybug, 2),
        new KeyValuePair<Type, int>(Type.Roach, 1),
        new KeyValuePair<Type, int>(Type.Mosquito, 3)
    };

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
