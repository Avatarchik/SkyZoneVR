using UnityEngine;
using System.Collections;

public class SpawnFloor : MonoBehaviour {

	public GameObject trampolines;
//	public float width = 3f, height = 3f;

	public int rows = 6, columns = 6;

	//[System.NonSerialized]
	public GameObject[,] tiles;
	public bool[,] tilesFilled;

	void Awake () {
		tiles = new GameObject[columns, rows];
		for(int i = 0; i < columns; i++) {
			for(int j = 0; j < rows; j++) {
				tiles[i,j] = trampolines.transform.FindChild(i + " " + j).gameObject;
			}
		}
		tilesFilled = new bool[columns, rows];
		for(int i = 0; i < columns; i++) {
			for(int j = 0; j < rows; j++) {
				tilesFilled[i,j] = false;
			}
		}
	}

	public void ResetTilesFilled()
	{
		for(int i = 0; i < columns; i++) 
		{
			for(int j = 0; j < rows; j++) 
			{
				tilesFilled[i,j] = false;
			}
		}
	}
}