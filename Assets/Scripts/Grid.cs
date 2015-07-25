using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public bool displayGridGizmos;
	public LayerMask unwalkableMask;
	public LayerMask Floor;
	public LayerMask Ground;
	public LayerMask Ramp;
	public LayerMask Unit;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Awake(){
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
		CreateGrid ();
	}

	void Update(){
		CreateGrid ();
	}

	public int MaxSize{
		get{
			return gridSizeX * gridSizeY;
		}
	}

	public void CreateGrid(){
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
		int floorNum = -1;

		for (int x = 0; x < gridSizeX; x++) {
			for (int y = 0; y < gridSizeY; y++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

				bool testGround = (Physics.CheckSphere(worldPoint,nodeRadius,Ground));
				bool testFloor = (Physics.CheckSphere(worldPoint,nodeRadius,Floor));
				bool testRamp = (Physics.CheckSphere(worldPoint,nodeRadius,Ramp));
				bool testObstacle = (Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));

				if(testObstacle){ // obstacle
					floorNum = 0;
				}else if(testRamp){ //ramp
					floorNum = 3;
				}else if(testFloor){ //second floor
					floorNum = 2;
				}else if(testGround){ //ground floor
					floorNum = 1;
				}else{
					floorNum = -1;
				}
				//print(floorNum);

				grid[x,y] = new Node(floorNum,worldPoint,x,y);
			}
		}
	}

	public List<Node> GetNeighbors(Node node){
		List<Node> neighbors = new List<Node>();

		for(int x = -1; x <= 1; x++){
			for(int y = -1; y <= 1; y++){
				if(x == 0 && y == 0)
					continue;
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY){
					neighbors.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbors;
	}

	public Node NodeFromWorldPoint(Vector4 worldPosition){
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

		return(grid [x, y]);
	}

	public Node NodeFromGridPoint(int x, int y){
		return grid[x,y];
	}

	/*********************************************************************************/
	/*	DEBUG Function - Order: Alphabetic							 	             */		
	/*********************************************************************************/
	
	public bool getDisplayGridGizmos() {
		return this.displayGridGizmos;
	}

	void OnDrawGizmos(){
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));
		Vector3 newVector = Vector3.one;
		newVector = newVector * (nodeDiameter - 0.1f);
		newVector.y = .01f;

		if (grid != null && displayGridGizmos) {
			foreach (Node node in grid){
				if(node.floorNum == 3){
					Gizmos.color = Color.green;
				}else if(node.floorNum == 2){
					Gizmos.color = Color.blue;
				}else if(node.floorNum == 1){
					Gizmos.color = Color.white;
				}else if(node.floorNum == 0){
					Gizmos.color = Color.red;
				}else{
					Gizmos.color = Color.clear;
				}
					Gizmos.DrawCube(node.worldPosition, newVector);
			}
		}
	}

	public void setDisplayGridGizmos(bool willDisplayGridGizmos) {
		this.displayGridGizmos = willDisplayGridGizmos;
	}
}
