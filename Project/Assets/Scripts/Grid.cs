using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace PathFinding
{
	public class Grid : MonoBehaviour{
		public static Grid inst;

		//Map settings
		public Vector3 mapPosition = Vector3.zero;
		const int mapWidth = 8;		//TODO: (CHANGE TO NO CONSTANT)
		const int mapHeight = 8;

		//Hex Settings
		public float hexRadius = 1;

		//Hexagon prefabs
		public GameObject defaultHex;
		public GameObject forestHex;
		public GameObject grassHex;
		public GameObject desertHex;
		public GameObject mountainHex;
		public GameObject riverHex;

		//Map control variables
		private bool startNodeSelected = false;
		private bool endNodeSelected = false;
		private Vector2 startNode;
		private Vector2 endNode;
		private Node[,] graph;

		//Dictionaries
		private Dictionary<string, Vector3> grid = new Dictionary<string, Vector3>();	//Tile reference and World position
		private Dictionary<string, int> Costs = new Dictionary<string, int>();	//Tile reference and cost of it hexagon

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

			GenerateGrid();
		}

		//Generate the shape and world position of the tiles
		private void GenerateShape() 
		{
			//Starting Pos (0,0)Tile

			Vector3 pos = mapPosition;
			
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
					pos.z = hexRadius * 3.0f/(2.3f)* x;

					//Save positions

					grid.Add(y.ToString() + x.ToString(), pos);	
				}
			}
		}

		//Paint map with corresponding tile on index
		//TODO: IF INDEX IS LEFT WITHOUT HEX FILL IT WITH DEFAULT HEX
		private void PaintMap()
		{
			//Painting mountains
			List<Vector2> MountainList = new List<Vector2>();

			MountainList.Add(new Vector2(1,0)); 
			MountainList.Add(new Vector2(2,0)); 
			MountainList.Add(new Vector2(1,1)); 
			MountainList.Add(new Vector2(2,2)); 
			MountainList.Add(new Vector2(3,3)); 
			MountainList.Add(new Vector2(3,4)); 
			MountainList.Add(new Vector2(5,6)); 
			MountainList.Add(new Vector2(5,7)); 
			MountainList.Add(new Vector2(6,7)); 
			MountainList.Add(new Vector2(7,7));

			PaintWith(mountainHex, MountainList);

			//Painting grass
			List<Vector2> GrassList = new List<Vector2>();

			GrassList.Add(new Vector2(0,0));
			GrassList.Add(new Vector2(3,0));
			GrassList.Add(new Vector2(5,0));
			GrassList.Add(new Vector2(3,1));
			GrassList.Add(new Vector2(0,2));
			GrassList.Add(new Vector2(3,2));
			GrassList.Add(new Vector2(4,2));
			GrassList.Add(new Vector2(1,4));
			GrassList.Add(new Vector2(1,5));
			GrassList.Add(new Vector2(4,5));
			GrassList.Add(new Vector2(2,6));
			GrassList.Add(new Vector2(7,6));
			GrassList.Add(new Vector2(1,7));
			GrassList.Add(new Vector2(3,7));

			PaintWith(grassHex, GrassList);

			//Painting forest
			List<Vector2> ForestList = new List<Vector2>();

			ForestList.Add(new Vector2(0,1));
			ForestList.Add(new Vector2(2,1));
			ForestList.Add(new Vector2(4,1));
			ForestList.Add(new Vector2(6,2));
			ForestList.Add(new Vector2(2,3));
			ForestList.Add(new Vector2(4,3));
			ForestList.Add(new Vector2(6,3));
			ForestList.Add(new Vector2(2,4));
			ForestList.Add(new Vector2(4,4));
			ForestList.Add(new Vector2(6,5));
			ForestList.Add(new Vector2(7,5));
			ForestList.Add(new Vector2(3,6));
			ForestList.Add(new Vector2(4,6));
			ForestList.Add(new Vector2(6,6));
			ForestList.Add(new Vector2(2,7));
			ForestList.Add(new Vector2(4,7));

			PaintWith(forestHex, ForestList);

			//Painting desert
			List<Vector2> DesertList = new List<Vector2>();

			DesertList.Add(new Vector2(6,0));
			DesertList.Add(new Vector2(7,0));
			DesertList.Add(new Vector2(5,1));
			DesertList.Add(new Vector2(6,1));
			DesertList.Add(new Vector2(7,1));
			DesertList.Add(new Vector2(1,2));
			DesertList.Add(new Vector2(5,2));
			DesertList.Add(new Vector2(7,2));
			DesertList.Add(new Vector2(0,3));
			DesertList.Add(new Vector2(1,3));
			DesertList.Add(new Vector2(7,3));
			DesertList.Add(new Vector2(0,4));
			DesertList.Add(new Vector2(5,5));

			PaintWith(desertHex, DesertList);

			//Painting rivers
			List<Vector2> RiverList = new List<Vector2>();

			RiverList.Add(new Vector2(4,0));
			RiverList.Add(new Vector2(5,3));
			RiverList.Add(new Vector2(5,4));
			RiverList.Add(new Vector2(6,4));
			RiverList.Add(new Vector2(7,4));
			RiverList.Add(new Vector2(0,5));
			RiverList.Add(new Vector2(2,5));
			RiverList.Add(new Vector2(3,5));
			RiverList.Add(new Vector2(0,6));
			RiverList.Add(new Vector2(1,6));
			RiverList.Add(new Vector2(0,7));

			PaintWith(riverHex, RiverList);
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

					graph[x,y].normalCost = Costs[x.ToString() + y.ToString()];
				}
			}

			//Hardcoding diagonals for even and oddrow difrences on its neighbours

			for(int x = 0; x < mapWidth; x++)
			{
				for (int y = 0; y < mapHeight; y++)
				{
					//TODO: CHANGE THIS TO MAKE IT WORK IN AN ODD HEIGHT/WIDTH MAP

					if(x>0)
					{
						graph[x,y].neighbours.Add(graph[x-1,y]); //Right
					}
					if(x < mapWidth - 1)
					{
						graph[x,y].neighbours.Add(graph[x+1,y]); //Left
					}

					//Checking EvenRow diagonals

					if (y%2 == 0)
					{
						if(y > 0)
						{
							if (x < mapWidth-1)
								graph[x,y].neighbours.Add(graph[x+1,y-1]); //Up Left

							graph[x,y].neighbours.Add(graph[x,y-1]); //Up Right

						}
						if(y < mapHeight-1)
						{
							if(x < mapWidth -1)
								graph[x,y].neighbours.Add(graph[x+1,y+1]); //Down Left


							graph[x,y].neighbours.Add(graph[x,y+1]); //Down Right
						}
					}

					//Checking OddRow Digonals

					else
					{
						if(y > 0)
						{
							graph[x,y].neighbours.Add(graph[x,y-1]); //Up Left
							if(x>0)
							{
								graph[x,y].neighbours.Add(graph[x-1,y-1]); //Up Right
							}
						}
						if(y < mapHeight-1)
						{
							graph[x,y].neighbours.Add(graph[x,y+1]); //Down Left

							if(x > 0)
							{
								graph[x,y].neighbours.Add(graph[x-1,y+1]); //Down Right
							}
						}
					}
				}
			}
		}

		//The instantiator fo the tiles
		private void PaintWith(GameObject prefab, List<Vector2> xy)
		{
			for (int i = 0; i < xy.Count(); i++)
			{
				GameObject tile = GameObject.Instantiate(prefab, grid[xy[i].x.ToString() + xy[i].y.ToString()], Quaternion.identity);
				HexTile hexTile = tile.GetComponent<HexTile>();
				hexTile.SetIndex(Mathf.RoundToInt(xy[i].x), Mathf.RoundToInt(xy[i].y), Mathf.RoundToInt(- xy[i].y - xy[i].x));	//Setting the hex index
				hexTile.gridRef = this;	//Making a reference to the grid

				//SettingCost

				Costs.Add(xy[i].x.ToString() + xy[i].y.ToString(), hexTile.cost);

				//TODO: Dictionary with reference to the real tile;
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		//----------------------------------------------MAP CONTROL----------------------------------------------------
		//-------------------------------------------------------------------------------------------------------------

		private void Update() 
		{
		}

		public IList<IAStarNode> currentPath = new List<IAStarNode>();

		public void SetSelectedTile(Vector2 node)
		{
			if (!startNodeSelected)
			{
				startNode = node;
				startNodeSelected = true;
			}
			else
			{
				currentPath = AStar.GetPath(graph[Mathf.RoundToInt(startNode.x), Mathf.RoundToInt(startNode.y)], graph[Mathf.RoundToInt(endNode.x), Mathf.RoundToInt(endNode.y)]);
				
				endNode = node;
				endNodeSelected = true;
			}
		}

		//Tester
		private void OnDrawGizmos() 
		{
			if (endNodeSelected)
			{
				foreach (Node v in currentPath)
				{
					Debug.Log(v + " = " + v.x.ToString() + v.y.ToString());
					
					Vector3 vPos = grid[v.x.ToString() + v.y.ToString()];

					Gizmos.color = Color.red;
					Gizmos.DrawWireSphere(vPos, hexRadius);
					
				};
				
			}
		}

	}
}