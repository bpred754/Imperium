using UnityEngine;
using System.Collections;
using System;

public class PathRequest {
	private Vector3 pathStart;
	private Vector3 pathEnd;
	private Action<Vector3[], bool> callback;

	// Constructor
	public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback) {
		this.pathStart = _start;
		this.pathEnd = _end;
		this.callback = _callback;
	}

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/	

	public void executeCallback(Vector3[] _vectorArray, bool _bool) {
		this.callback (_vectorArray, _bool);
	}

	/*********************************************************************************/
	/*	Getter and Setter Functions - Order: Alphabetic							 	 */		
	/*********************************************************************************/

	public Vector3 getPathEnd() {
		return this.pathEnd;
	}

	public Vector3 getPathStart() {
		return this.pathStart;
	}
}

