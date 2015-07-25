using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>{

	private Node parent;
	private Vector3 worldPosition;
	private int gridX;
	private int gridY;
	private int gCost;
	private int hCost;
	private int heapIndex;
	private int floorNum;
	private bool walkable;
	private bool ramp = false;
	private bool ground = false;
	private bool floor = false;
	private bool obstacle = false;

	// Constructor
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

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/

	public int CompareTo(Node nodeToCompare){
		int compare = this.getFCost().CompareTo(nodeToCompare.getFCost());
		if(compare == 0){
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}

	public int getFCost() {
		return gCost + hCost;
	}

	/*********************************************************************************/
	/*	Getter and Setter Functions - Order: Alphabetic							 	 */		
	/*********************************************************************************/

	public int getFloorNum() {
		return this.floorNum;
	}

	public int getGCost() {
		return this.gCost;
	}

	public int getGridX() {
		return this.gridX;
	}

	public int getGridY() {
		return this.gridY;
	}

	public int getHCost() {
		return this.hCost;
	}

	public int getHeapIndex() {
		return this.heapIndex;
	}

	public Node getParent() {
		return this.parent;
	}

	public Vector3 getWorldPosition() {
		return this.worldPosition;
	}

	public bool isFloor() {
		return this.floor;
	}

	public bool isGround() {
		return this.ground;
	}

	public bool isRamp() {
		return this.ramp;
	}

	public bool isWalkable() {
		return this.walkable;
	}

	public void setGCost(int _gCost) {
		this.gCost = _gCost;
	}

	public void setHCost(int _hCost) {
		this.hCost = _hCost;
	}
	
	public void setHeapIndex(int _heapIndex) {
		this.heapIndex = _heapIndex;
	}

	public void setParent(Node _parent) {
		this.parent = _parent;
	}

	public void setWorldPosition(Vector3 _worldPosition) {
		this.worldPosition = _worldPosition; 
	}
}
