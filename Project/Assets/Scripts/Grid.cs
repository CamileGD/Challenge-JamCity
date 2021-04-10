using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Grid : MonoBehaviour {
	public static Grid inst;

	//Map settings
	public int mapWidth;
	public int mapHeight;

	//Hex Settings
	public float hexRadius = 1;

	//Hexagon prefabs
	public GameObject normalHex;
	public GameObject forestHex;
	public GameObject grassHex;
	public GameObject desertHex;
	public GameObject mountainHex;
	public GameObject riverHex;

	//Internal variables
	private Dictionary<string, Vector3> grid = new Dictionary<string, Vector3>();
	private Mesh hexMesh = null;
	private CubeIndex[] directions = 
		new CubeIndex[] {
			new CubeIndex(1, -1, 0), 
			new CubeIndex(1, 0, -1), 
			new CubeIndex(0, 1, -1), 
			new CubeIndex(-1, 1, 0), 
			new CubeIndex(-1, 0, 1), 
			new CubeIndex(0, -1, 1)
		}; 

	public Dictionary<string, Vector3> Tiles {
		get {return grid;}
	}
	public void GenerateGrid() {
		//Generating a new grid, clear any remants and initialise values
		GenerateShape();

		PaintMap();
	}

	public int Distance(CubeIndex a, CubeIndex b){
		return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
	}

	public int Distance(Tile a, Tile b){
		return Distance(a.index, b.index);
	}

	private void Awake() {
		if(!inst)
			inst = this;

		GenerateGrid();
	}

	private void GenerateShape() 
	{

		//Grid inicial position

		Vector3 pos = Vector3.zero;
		for(int r = 0; r < mapHeight; r++){
			int rOff = r/2;
			for (int q = -rOff; q < mapWidth - rOff; q++)
			{
				pos.x = hexRadius * Mathf.Sqrt(3.0f) * (q + r/2.0f);

				//Changing to the actual layout

				if (r%2 == 0)
				{
					pos.x += hexRadius * Mathf.Sqrt(3.0f);
				}

				//pos.z = hexRadius * 3.0f/2.0f * r;

				//TODO FUNCTION TO CALCULATE THE MODEL SIZE AND USE IT FOR THEGRID CREATION
			
				//Info used from the grid model

				pos.z = hexRadius * 3.0f/(2.3f)* r;

				//GameObject hexTile = GameObject.Instantiate(normalHex, pos, Quaternion.identity);
				//hexTile.GetComponent<HexTile>().SetIndex(Mathf.RoundToInt(q), Mathf.RoundToInt(r));	

				//Save positions

				//GameObject hexTile = GameObject.Instantiate(normalHex, pos, Quaternion.identity);
				//tile = CreateHexGO(pos,("Hex[" + q + "," + r + "," + (-q-r).ToString() + "]"));
				//tile.index = new CubeIndex(q,r,-q-r);
				grid.Add(q.ToString() + r.ToString(), pos);
			}
		}
	}

	private void PaintMap()
	{
		//Painting mountains
		List<Vector2> MountainList = new List<Vector2>();

		MountainList.Add(new Vector2(1,0)); 
		MountainList.Add(new Vector2(2,0)); 
		MountainList.Add(new Vector2(1,1)); 
		MountainList.Add(new Vector2(1,2)); 
		MountainList.Add(new Vector2(2,3)); 
		MountainList.Add(new Vector2(1,4)); 
		MountainList.Add(new Vector2(1,4)); 
		MountainList.Add(new Vector2(2,6)); 
		MountainList.Add(new Vector2(2,7)); 
		MountainList.Add(new Vector2(3,7)); 
		MountainList.Add(new Vector2(4,7));

		PaintWith(mountainHex, MountainList);

		//Painting grass

		List<Vector2> GrassList = new List<Vector2>();

		GrassList.Add(new Vector2(0,0));
		GrassList.Add(new Vector2(3,0));
		GrassList.Add(new Vector2(5,0));
		GrassList.Add(new Vector2(3,1));
		GrassList.Add(new Vector2(-1,2));
		GrassList.Add(new Vector2(2,2));
		GrassList.Add(new Vector2(3,2));
		GrassList.Add(new Vector2(-1,4));
		GrassList.Add(new Vector2(-1,5));
		GrassList.Add(new Vector2(2,5));
		GrassList.Add(new Vector2(-1,6));
		GrassList.Add(new Vector2(4,6));
		GrassList.Add(new Vector2(-2,7));
		GrassList.Add(new Vector2(0,7));

		PaintWith(grassHex, GrassList);

		//Painting forest

		List<Vector2> ForestList = new List<Vector2>();

		ForestList.Add(new Vector2(0,1));
		ForestList.Add(new Vector2(2,1));
		ForestList.Add(new Vector2(4,1));
		ForestList.Add(new Vector2(5,2));
		ForestList.Add(new Vector2(1,3));
		ForestList.Add(new Vector2(3,3));
		ForestList.Add(new Vector2(5,3));
		ForestList.Add(new Vector2(0,4));
		ForestList.Add(new Vector2(2,4));
		ForestList.Add(new Vector2(4,5));
		ForestList.Add(new Vector2(5,5));
		ForestList.Add(new Vector2(0,6));
		ForestList.Add(new Vector2(1,6));
		ForestList.Add(new Vector2(3,6));
		ForestList.Add(new Vector2(-1,7));
		ForestList.Add(new Vector2(1,7));

		PaintWith(forestHex, ForestList);

		//Painting desert

		List<Vector2> DesertList = new List<Vector2>();

		DesertList.Add(new Vector2(6,0));
		DesertList.Add(new Vector2(7,0));
		DesertList.Add(new Vector2(5,1));
		DesertList.Add(new Vector2(6,1));
		DesertList.Add(new Vector2(7,1));
		DesertList.Add(new Vector2(0,2));
		DesertList.Add(new Vector2(4,2));
		DesertList.Add(new Vector2(6,2));
		DesertList.Add(new Vector2(-1,3));
		DesertList.Add(new Vector2(0,3));
		DesertList.Add(new Vector2(6,3));
		DesertList.Add(new Vector2(-2,4));
		DesertList.Add(new Vector2(3,5));

		PaintWith(desertHex, DesertList);

		//Painting rivers

		List<Vector2> RiverList = new List<Vector2>();

		RiverList.Add(new Vector2(4,0));
		RiverList.Add(new Vector2(4,3));
		RiverList.Add(new Vector2(3,4));
		RiverList.Add(new Vector2(4,4));
		RiverList.Add(new Vector2(5,4));
		RiverList.Add(new Vector2(-2,5));
		RiverList.Add(new Vector2(0,5));
		RiverList.Add(new Vector2(1,5));
		RiverList.Add(new Vector2(-3,6));
		RiverList.Add(new Vector2(-2,6));
		RiverList.Add(new Vector2(-3,7));

		PaintWith(riverHex, RiverList);
	}

	private void PaintWith(GameObject prefab, List<Vector2> qr)
	{

		for (int i = 0; i < qr.Count(); i++)
		{
			GameObject hexTile = GameObject.Instantiate(prefab, grid[qr[i].x.ToString() + qr[i].y.ToString()], Quaternion.identity);
			hexTile.GetComponent<HexTile>().SetIndex(Mathf.RoundToInt(qr[i].x), Mathf.RoundToInt(qr[i].y));	
		}
	}

}