using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>{

	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public Node parent;
	int heapIndex;

	public int floorNum;
	public bool ramp = false;
	public bool ground = false;
	public bool floor = false;
	public bool obstacle = false;

	public Node(int floorNum, Vector3 worldPosition, int gridX, int gridY){
		this.floorNum = floorNum;
		if(floorNum == 0){
			this.walkable = false;
		}else{
			this.walkable = true;
		}
		if(floorNum == 3)
			ramp = true;
		else if(floorNum == 2)
			floor = true;
		else if(floorNum == 1)
			ground = true;
		else if(floorNum == 0)
			obstacle = true;

		this.worldPosition = worldPosition;
		this.gridX = gridX;
		this.gridY = gridY;
	}

	public int fCost{
		get{
			return gCost + hCost;
		}
	}

	public int HeapIndex{
		get{
			return heapIndex;
		}
		set{
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare){
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if(compare == 0){
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
