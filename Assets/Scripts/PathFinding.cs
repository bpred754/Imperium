using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathFinding : MonoBehaviour {

	PathRequestManager requestManager;

	Vector3 targetPosition;
	bool newTarget = false;
	int buildingHeight = 1;

	Grid grid;

	void Awake(){
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid>();
	}

	public void StartFindPath(Vector3 startPos, Vector3 targetPos){
		StartCoroutine(FindPath (startPos,targetPos));
	}

	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos){

		Vector3[] wayPoints = new Vector3[0];
		bool pathSuccess = false;
		targetPosition = targetPos;

		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		newTarget = false;

		//This one doesn't let units move if they are currently on an unwalkable square
		//We'll have to make sure units never get created on unwalkable squares or get placed there
		//in some other fashion. But I felt it was best to let them move out of an unwalkable
		//location if it accidently happens
		//if(startNode.walkable && targetNode.walkable){
		

		if(!targetNode.walkable){
			newTarget = true;
			//Debug.Log ("Target is unwalkable...");
			//Debug.Log ("Original Target: " + targetNode.gridX + ", " + targetNode.gridY);

			targetNode  = FindNearestNeighbor(targetNode);

			//Debug.Log ("New Target: " + targetNode.gridX + ", " + targetNode.gridY);
		}


		if (targetNode.walkable) {
			Heap<Node> openSet = new Heap<Node> (grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node> ();
			openSet.Add (startNode);

			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst ();
				closedSet.Add (currentNode);

				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}

				foreach (Node neighbor in grid.GetNeighbors(currentNode)) {
					if (!neighbor.walkable || closedSet.Contains (neighbor)) {
						continue;
					}
					if(currentNode.ramp && neighbor.ramp || //ramp to ramp
					   currentNode.ramp && neighbor.floor || //ramp to floor
					   currentNode.floor && neighbor.floor || 
					   currentNode.floor && neighbor.ramp ||
					   currentNode.ground && neighbor.ground ||
					   currentNode.ground && neighbor.ramp ||
					   currentNode.ramp && neighbor.ground){

						int newMovementCostToNeighbor = currentNode.gCost + GetDistance (currentNode, neighbor);
						if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains (neighbor)) {
							neighbor.gCost = newMovementCostToNeighbor;
							neighbor.hCost = GetDistance (neighbor, targetNode);
							neighbor.parent = currentNode;
							
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
		yield return null;
		if (pathSuccess) {
			//Debug.Log ("Path found...");
			wayPoints = RetracePath (startNode, targetNode);
			requestManager.FinishedProcessingPath (wayPoints, pathSuccess);
		} else {
			//Debug.Log ("No path found...");
			requestManager.FinishedProcessingPath (wayPoints, pathSuccess);
		}
	}

	Vector3[] RetracePath(Node startNode, Node endNode){
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		Vector3[] targetArray = new Vector3[1];
		if(currentNode != startNode){
			while(currentNode!= startNode){
				path.Add (currentNode);
				currentNode = currentNode.parent;
			}
			Vector3[] waypoints = SimplifyPath(path);
			Array.Reverse (waypoints);
			//Debug.Log ("WayPoints Length: " + waypoints.Length);
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

	Vector3[] SimplifyPath(List<Node> path){
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;

		for(int i = 0; i < path.Count; i ++){
			if(path[i].floorNum == 1) //ground
				path[i].worldPosition.y = 0.5f;
			else if(path[i].floorNum == 2) //floor
				path[i].worldPosition.y = 2.5f * buildingHeight;
			else if(path[i].floorNum == 3) //ramp
				path[i].worldPosition.y = 1.5f * buildingHeight;
		}
		waypoints.Add (path[0].worldPosition);
		for(int i = 1; i < path.Count; i ++){
			Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
			if(i != path.Count-1){
				if(directionNew != directionOld ||
				   path[i].floorNum != path[i-1].floorNum ||
				   path[i+1].floorNum != path[i].floorNum){
					waypoints.Add (path[i].worldPosition);
					//Debug.Log ("Pathfinding " + path[i].worldPosition.y);
				}
			}//else
				//waypoints.Add (path[i].worldPosition);
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}

	int GetDistance(Node nodeA, Node nodeB){
		int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if(distX > distY)
			return 14*distY + 10*(distX-distY);
		else
			return 14*distX + 10*(distY-distX);
	}

	Node FindNearestNeighbor(Node targetNode){
		//Debug.Log ("Acquiring new target...");
		if(!targetNode.walkable){
			List<Node> toCheck = new List<Node>();
			List<Node> newNeighbors = new List<Node>();
			newNeighbors.Add(targetNode);
			List<Node> Checked = new List<Node>();
			//int testCount = 0;

			while(true){
				//Debug.Log ("Searching...");
				//Debug.Log ("newNeighbors: " + newNeighbors.Count);

				toCheck.Clear();
				foreach(Node node in newNeighbors){
					toCheck.Add(node);
				}
				newNeighbors.Clear();
				//testCount++;

				//Debug.Log ("toCheck: " + toCheck.Count);
				//Debug.Log ("Checked: " + Checked.Count);

				//Test Break
				//if(testCount >= 20){
				//	Debug.Log ("Acquisition failed 1...");
				//	return(targetNode);
				//}
				//if(toCheck.Count == 0){
				//	Debug.Log ("Acquisition failed 2...");
				//	return(targetNode);
				//}
				//check current list of Nodes
				foreach(Node node in toCheck){
					if(node.walkable){
						//Debug.Log ("Target Acquired...");
						return node;
					}else{
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
		}else{

			return(targetNode);
		}
	}

	List<Node> GetNeighbors(Node targetNode){
		List<Node> tempList = new List<Node>();
		tempList.Add (grid.NodeFromGridPoint(targetNode.gridX + 1, targetNode.gridY + 1));
		tempList.Add (grid.NodeFromGridPoint(targetNode.gridX + 1, targetNode.gridY - 1));
		tempList.Add (grid.NodeFromGridPoint(targetNode.gridX - 1, targetNode.gridY + 1));
		tempList.Add (grid.NodeFromGridPoint(targetNode.gridX - 1, targetNode.gridY - 1));
		return(tempList);

	}
}
