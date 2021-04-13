using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace PathFinding
{
	public class Grid : MonoBehaviour{
		public static Grid inst;
		//Map settings
		[Header("Map settings")]
		[Space]
		public int mapWidth = 8;
		public int mapHeight = 8;
		public float hexRadius = 0.5785f;
		public float yRowSeparation = 2.33f;

		[Header("Hex types settings")]
		[Space]
		//Hexagon prefabs and their lists
		public GameObject defaultHex;
		public List<Vector2> ForestList = new List<Vector2>();
		public GameObject forestHex;
		public List<Vector2> GrassList = new List<Vector2>();
		public GameObject grassHex;
		public List<Vector2> DesertList = new List<Vector2>();
		public GameObject desertHex;
		public List<Vector2> MountainList = new List<Vector2>();
		public GameObject mountainHex;
		public List<Vector2> WaterList = new List<Vector2>();
		public GameObject waterHex;

		//Map control variables
		private bool startNodeSelected = false;
		private bool endNodeSelected = false;
		private Vector2 startNode;
		private Vector2 endNode;
		private Node[,] graph;

		//Enumerables
		private List<Vector2> tilesOcuppied = new List<Vector2>();	//List of occupied tiles
		private List<Vector2> tilesCreated = new List<Vector2>();	//List of created tiles
		private Dictionary<string, Vector3> grid = new Dictionary<string, Vector3>();	//Tile reference and World position
		private Dictionary<string, int> costs = new Dictionary<string, int>();	//Tile reference and cost of it hexagon
		private Dictionary<string, bool> walkable = new Dictionary<string, bool>();	//Tile reference and ifMovable reference 
		private Dictionary<string, GameObject> actualTiles = new Dictionary<string, GameObject>();	//Tile reference and actual game object reference
		public IList<IAStarNode> currentPath = new List<IAStarNode>();	//Reference to the current path

		//-------------------------------------------------------------------------------------------------------------
		//----------------------------------------------MAP GENERATION-------------------------------------------------
		//-------------------------------------------------------------------------------------------------------------

		public void GenerateGrid() 
		{
			GenerateShape();	//Generate the shape and world position of the tiles
			PaintMap();		//Instantiate the tiles
			GenerateGraph();	//Generate the node graph
		}

		private void Awake() {
			if(!inst)
				inst = this;

			GenerateGrid();	//Creating it on awake
		}

		//Generate the shape and world position of the tiles
		private void GenerateShape() 
		{
			Vector3 pos = Vector3.zero;
			
			for(int x = 0; x < mapHeight; x++)
			{
				//OffRow var pusher for the hexagon tile shape
				int rOff = x/2;

				for (int y = 0; y < mapWidth; y++)
				{
					int q = -rOff;
					pos.x = hexRadius * Mathf.Sqrt(3.0f) * (y+q + x/2.0f);

					//Pushing the even row to the left
					if (x%2 == 0)
					{
						pos.x +=  hexRadius * Mathf.Sqrt(3.0f);
					}

					//TODO FUNCTION TO CALCULATE THE MODEL SIZE AND USE IT FOR THE GRID CREATION
					//Hardcode without actual idea of the model real size
					pos.z = hexRadius * 3.0f/(yRowSeparation)* x;

					//Save positions

					grid.Add(y.ToString() + x.ToString(), pos);	
					tilesCreated.Add(new Vector2(y,x));
				}
			}
		}

		//Paint map with corresponding tile on index
		private void PaintMap()
		{
			//Painting forest

			PaintWith(forestHex, ForestList);

			//Painting grass

			PaintWith(grassHex, GrassList);

			//Painting mountains

			PaintWith(mountainHex, MountainList);

			//Painting desert

			PaintWith(desertHex, DesertList);

			//Painting rivers

			PaintWith(waterHex, WaterList);
			
			//Filling not painted hexagons to default
			//Default hexagons are not walkable

			List<Vector2> ToFill = new List<Vector2>();

			for (int c = 0; c < tilesCreated.Count; c++)
			{
				bool free = true;

				for(int o = 0; o < tilesOcuppied.Count; o++)
				{
					if (tilesCreated[c] == tilesOcuppied[o])
					{
						free = false;
					}
				}

				if (free)
				{
					ToFill.Add(tilesCreated[c]);
					Debug.Log("Debug: " + tilesCreated[c].ToString() + " hexagon not painted");
				}
			}

			PaintWith(defaultHex, ToFill);
		}

		//Generating the node graph and the neighbours
		public void GenerateGraph()
		{
			//Instantiating the 2D array and it nodes

			graph = new Node[mapWidth, mapHeight];

			for(int x=0; x < mapWidth; x++) {
				for(int y=0; y < mapHeight; y++) {					
					graph[x,y] = new Node();
					graph[x,y].x = x;
					graph[x,y].y = y;

					//Setting each node its cost

					graph[x,y].normalCost = costs[x.ToString() + y.ToString()];
				}
			}

			//Hardcoding diagonals for even and oddrow difrences on its neighbours

			for(int x = 0; x < mapWidth; x++)
			{
				for (int y = 0; y < mapHeight; y++)
				{
					if(x>0)
					{
						if (walkable[(x-1).ToString() + y.ToString()])
							graph[x,y].neighbours.Add(graph[x-1,y]); //Right
					}
					if(x < mapWidth - 1)
					{
						if (walkable[(x+1).ToString() + y.ToString()])
							graph[x,y].neighbours.Add(graph[x+1,y]); //Left
					}

					//Checking EvenRow diagonals

					if (y%2 == 0)
					{
						if(y > 0)
						{
							if (x < mapWidth-1)
							{
								if (walkable[(x+1).ToString() + (y-1).ToString()])
									graph[x,y].neighbours.Add(graph[x+1,y-1]); //Up Left
							}
							if (walkable[x.ToString() + (y-1).ToString()])
								graph[x,y].neighbours.Add(graph[x,y-1]); //Up Right
						}
						if(y < mapHeight-1)
						{
							if(x < mapWidth -1)
							{	
								if (walkable[(x+1).ToString() + (y+1).ToString()])
									graph[x,y].neighbours.Add(graph[x+1,y+1]); //Down Left
							}

							if (walkable[x.ToString() + (y+1).ToString()])
								graph[x,y].neighbours.Add(graph[x,y+1]); //Down Right
						}
					}

					//Checking OddRow Digonals

					else
					{
						if(y > 0)
						{
							if (walkable[x.ToString() + (y-1).ToString()])
								graph[x,y].neighbours.Add(graph[x,y-1]); //Up Left
							if(x>0)
							{
								if (walkable[(x-1).ToString() + (y-1).ToString()])
									graph[x,y].neighbours.Add(graph[x-1,y-1]); //Up Right
							}
						}
						if(y < mapHeight-1)
						{
							if (walkable[x.ToString() + (y+1).ToString()])
								graph[x,y].neighbours.Add(graph[x,y+1]); //Down Left

							if(x > 0)
							{
								if (walkable[(x-1).ToString() + (y+1).ToString()])
									graph[x,y].neighbours.Add(graph[x-1,y+1]); //Down Right
							}
						}
					}
				}
			}
		}

		//The tile instantiator
		private void PaintWith(GameObject prefab, List<Vector2> xy)
		{
			for (int i = 0; i < xy.Count(); i++)
			{
				if (xy[i].x < mapWidth && xy[i].y < mapHeight)	//Checking if painted more tiles than the map allowed
				{
					GameObject tile = GameObject.Instantiate(prefab, grid[xy[i].x.ToString() + xy[i].y.ToString()], Quaternion.identity);
					HexTile hexTile = tile.GetComponent<HexTile>();
					hexTile.SetIndex(Mathf.RoundToInt(xy[i].x), Mathf.RoundToInt(xy[i].y));	//Setting the hex index
					hexTile.gridRef = this;	//Making a reference to the grid

					//Saving cost

					costs.Add(xy[i].x.ToString() + xy[i].y.ToString(), hexTile.cost);

					//Saving if can be walked on

					walkable.Add(xy[i].x.ToString() + xy[i].y.ToString(), hexTile.walkable);

					//Tile reference

					actualTiles.Add(xy[i].x.ToString() + xy[i].y.ToString(), tile);

					//Occupied tile

					tilesOcuppied.Add(xy[i]);
				}
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		//----------------------------------------------MAP UPDATER----------------------------------------------------
		//-------------------------------------------------------------------------------------------------------------

		public void TileClicked(Vector2 node)
		{
			//Third click reset the path

			if (node == startNode)
				return;

			if(startNodeSelected && endNodeSelected)
			{
				startNodeSelected = false;
				endNodeSelected = false;

				foreach(Node n in currentPath)
				{
					actualTiles[n.x.ToString() + n.y.ToString()].GetComponent<HexTile>().ResetColor();	//Reseting tiles color
				}

				currentPath.Clear();
			}

			//First click select the start node

			else if (!startNodeSelected)
			{
				startNode = node;
				startNodeSelected = true;

				actualTiles[startNode.x.ToString() + startNode.y.ToString()].GetComponent<HexTile>().Selected(true);	//Selected tile color
			}

			//Second click select the end node

			else if (startNodeSelected && !endNodeSelected)
			{			
				endNode = node;
				endNodeSelected = true;

				actualTiles[endNode.x.ToString() + endNode.y.ToString()].GetComponent<HexTile>().Selected(true);	//Selected tile color
				currentPath = AStar.GetPath(graph[Mathf.RoundToInt(startNode.x), Mathf.RoundToInt(startNode.y)], graph[Mathf.RoundToInt(endNode.x), Mathf.RoundToInt(endNode.y)]);	//Getting Path

				foreach(Node n in currentPath)
				{
					//Setting pathColor

					if(new Vector2(n.x, n.y) != startNode && new Vector2(n.x, n.y) != endNode)
						actualTiles[n.x.ToString() + n.y.ToString()].GetComponent<HexTile>().Selected();	
				}
			}
		}
	}
}