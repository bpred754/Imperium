using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager {
	
	private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	private PathRequest currentPathRequest;
	private static PathRequestManager instance;
	private PathFinding pathfinding;
	private bool isProcessingPath;

	// Constructor
	public PathRequestManager(Grid _grid) {

		if (instance == null) {
			instance = this;
			pathfinding = new PathFinding(this, _grid);
		}
	}

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/	
	
	public void FinishedProcessingPath(Vector3[] path, bool success) {
		currentPathRequest.executeCallback(path,success);
		isProcessingPath = false;
		TryProcessNext();
	}

	public static void RequestPath(Vector3 pathStart, RaycastHit pathEnd, Action<Vector3[], bool> callback) {
		PathRequest newRequest = new PathRequest(pathStart,pathEnd,callback);
		instance.pathRequestQueue.Enqueue(newRequest);
		instance.TryProcessNext();
	}

	/*********************************************************************************/
	/*	Private Functions - Order: Alphabetic										 */		
	/*********************************************************************************/
	
	private void TryProcessNext() {
		if (!isProcessingPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.getPathStart(), currentPathRequest.getPathEnd());
		}
	}
}
