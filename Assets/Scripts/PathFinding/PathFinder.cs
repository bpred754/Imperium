using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathFinder {


	private Vector3 targetPosition;
	private bool newTarget = false;
	private int buildingHeight = 1;
	private Grid grid;

	// Constructor
	public PathFinder(Grid _grid) {
		this.grid = _grid;
	}

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/	

	public Vector3[] StartFindPath(Vector3 startPos, RaycastHit targetPos){
		return(FindPath (startPos, targetPos));
	}

	/*********************************************************************************/
	/*	Private Functions - Order: Alphabetic										 */		
	/*********************************************************************************/

	private Node FindNearestNeighbor(Node targetNode){
		if(!targetNode.isWalkable()){
			List<Node> toCheck = new List<Node>();
			List<Node> newNeighbors = new List<Node>();
			newNeighbors.Add(targetNode);
			List<Node> Checked = new List<Node>();
			
			while(true){
				toCheck.Clear();
				foreach(Node node in newNeighbors){
					toCheck.Add(node);
				}
				newNeighbors.Clear();
				foreach(Node node in toCheck){
					if(node.isWalkable()){
						return node;
					} else {
						if(!Checked.Contains (node)){
							Checked.Add (node);
						}
						List<Node> temps = GetNeighbors (node);
						foreach( Node temp in temps){
							if(!Checked.Contains(temp) && !newNeighbors.Contains(temp)){
								newNeighbors.Add (temp);
							}
						}
					}
				}
			}
		} else{
			return(targetNode);
		}
	}

	private Vector3[] FindPath(Vector3 startPos, RaycastHit targetPos){

		Vector3[] wayPoints = new Vector3[0];
		bool pathSuccess = false;
		targetPosition = targetPos.point;

		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPosition);
		newTarget = false;

		if(!targetNode.isWalkable()){
			newTarget = true;
			targetNode  = FindNearestNeighbor(targetNode);
		}

		if (targetNode.isWalkable()) {
			Heap<Node> openSet = new Heap<Node> (grid.getMaxSize());
			HashSet<Node> closedSet = new HashSet<Node> ();
			openSet.Add (startNode);

			while (openSet.getCurrentItemCount() > 0) {
				Node currentNode = openSet.RemoveFirst ();
				closedSet.Add (currentNode);

				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}

				foreach (Node neighbor in grid.GetNeighbors(currentNode)) {
					if (!neighbor.isWalkable() || closedSet.Contains (neighbor)) {
						continue;
					}
					if(IsLegalMove(currentNode,neighbor)){

						int newMovementCostToNeighbor = currentNode.getGCost() + GetDistance (currentNode, neighbor);
						if (newMovementCostToNeighbor < neighbor.getGCost() || !openSet.Contains (neighbor)) {
							neighbor.setGCost(newMovementCostToNeighbor);
							neighbor.setHCost(GetDistance (neighbor, targetNode));
							neighbor.setParent(currentNode);
							
							if (!openSet.Contains (neighbor)) {
								openSet.Add (neighbor);
							}else{
								openSet.UpdateItem(neighbor);
							}
						}
					}
				}
			}
		}
		if (pathSuccess) {
			wayPoints = RetracePath (startNode, targetNode);
			return wayPoints;
		} else {
			return wayPoints;
		}
	}

	private int GetDistance(Node nodeA, Node nodeB) {
		int distX = Mathf.Abs(nodeA.getGridX() - nodeB.getGridX());
		int distY = Mathf.Abs(nodeA.getGridY() - nodeB.getGridY());
		
		if(distX > distY)
			return 14*distY + 10*(distX-distY);
		else
			return 14*distX + 10*(distY-distX);
	}

	private bool IsLegalMove(Node current, Node neighbor){
		if (current.isRamp() && neighbor.isRamp() || //ramp to ramp
		    current.isRamp() && neighbor.isFloor() || //ramp to floor
		    current.isFloor() && neighbor.isFloor() || 
		    current.isFloor() && neighbor.isRamp() ||
		    current.isGround() && neighbor.isGround() ||
		    current.isGround() && neighbor.isRamp() ||
		    current.isRamp() && neighbor.isGround()) {
			return true;
		} else {
			return false;
		}
	}
	
	private Vector3[] RetracePath(Node startNode, Node endNode){
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		Vector3[] targetArray = new Vector3[1];
		if(currentNode != startNode){
			while(currentNode!= startNode){
				path.Add (currentNode);
				currentNode = currentNode.getParent();
			}
			Vector3[] waypoints = SimplifyPath(path);
			Array.Reverse (waypoints);
			if(waypoints.Length > 0 && !newTarget){
				targetPosition.y = waypoints[waypoints.Length-1].y;
				waypoints[waypoints.Length-1] = targetPosition;
				return waypoints;
			}else{
				return waypoints;
			}
		}
		targetArray[0] = targetPosition;
		return targetArray;
	}

	private Vector3[] SimplifyPath(List<Node> path){
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;

		for(int i = 0; i < path.Count; i ++){
			Vector3 worldPosition = path [i].getWorldPosition ();
			if(path[i].getFloorNum() == 1) //ground
				path[i].setWorldPosition(new Vector3(worldPosition.x, 0.5f, worldPosition.z));
			else if(path[i].getFloorNum() == 2) //floor
				path[i].setWorldPosition(new Vector3(worldPosition.x, 2.5f * buildingHeight, worldPosition.z));
			else if(path[i].getFloorNum() == 3) //ramp
				path[i].setWorldPosition(new Vector3(worldPosition.x, 1.5f * buildingHeight, worldPosition.z));
		}

		waypoints.Add (path[0].getWorldPosition());
		for(int i = 1; i < path.Count; i ++){
			Vector2 directionNew = new Vector2(path[i-1].getGridX() - path[i].getGridX(), path[i-1].getGridY() - path[i].getGridY());
			if(i != path.Count-1){
				if(directionNew != directionOld ||
				   path[i].getFloorNum() != path[i-1].getFloorNum() ||
				   path[i+1].getFloorNum() != path[i].getFloorNum()){
					waypoints.Add (path[i].getWorldPosition());
				}
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}

	private List<Node> GetNeighbors(Node targetNode){
		List<Node> tempList = new List<Node>();
		tempList.Add (grid.NodeFromGridPoint(targetNode.getGridX() + 1, targetNode.getGridY() + 1));
		tempList.Add (grid.NodeFromGridPoint(targetNode.getGridX() + 1, targetNode.getGridY() - 1));
		tempList.Add (grid.NodeFromGridPoint(targetNode.getGridX() - 1, targetNode.getGridY() + 1));
		tempList.Add (grid.NodeFromGridPoint(targetNode.getGridX() - 1, targetNode.getGridY() - 1));
		return(tempList);
	}
}
