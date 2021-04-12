using UnityEngine;
using System.Collections.Generic;

namespace PathFinding
{
	public class Node: IAStarNode 
	{
		public List<Node> neighbours;
		public int x;
		public int y;
		public int normalCost = 1;	//Cost per tile type
		
		public Node()
		{
			neighbours = new List<Node>();
		}		

		//Interface
		public IEnumerable<IAStarNode> Neighbours
		{
			get
			{
				return neighbours;
			}
		}

		public float EstimatedCostTo(IAStarNode target)
		{
			return normalCost + 2;
		}

		public float CostTo(IAStarNode neighbour)
		{
			return normalCost;
		}
	}
}