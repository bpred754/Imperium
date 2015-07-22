using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	// Logic variables
	private bool isSelected;
	private Team team;
	public float movementSpeed;
	private Vector3 destination;
	//private Vector3 testPosition;
	public bool displayWayPointGizmos;

	// Model variables
	private Color startColor = Color.black;
	private Color selectedColor = Color.red;
	Transform sphere;

	Vector3[] path;
	int targetIndex;
	
	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	private void Start() {
		this.isSelected = false;
		GetComponent<Renderer> ().material.color = startColor;
		//testPosition = transform.position;
		sphere = transform.FindChild ("Sphere");
		sphere.GetComponent<Renderer> ().material.color = startColor;

	}

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/	

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful){
		if(pathSuccessful){
			path = newPath;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}
	
	IEnumerator FollowPath() {
		if(path.GetLength(0) > 0){
			Vector3 currentWaypoint = path[0];
			targetIndex = 0;
			
			while(true){
				if (transform.position.x == currentWaypoint.x && transform.position.y == currentWaypoint.y && transform.position.z == currentWaypoint.z){
					//Debug.Log(currentWaypoint.y);
					targetIndex++;
					if(targetIndex >= path.Length){
						transform.rotation = Quaternion.Euler(0,transform.eulerAngles.y,transform.eulerAngles.z);
						yield break;
					}
					currentWaypoint = path[targetIndex];
				}

				transform.LookAt(currentWaypoint);
				transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, movementSpeed * Time.deltaTime);
				yield return null;
			}
		}
	}

	public bool isTeam(Team inTeam) {
		bool isTeam = false;
		if(this.team == inTeam) {
			isTeam = true;
		}
		return isTeam;
	}
	
	public bool isVisible() {
		return GetComponent<Renderer> ().isVisible;
	}

	public void makeMove(RaycastHit target){
		PathRequestManager.RequestPath(transform.position, target, OnPathFound);		
	}

	/*********************************************************************************/
	/*	Getter and Setter Functions - Order: Alphabetic							 	 */		
	/*********************************************************************************/
	
	public bool getSelected() {
		return this.isSelected;
	}
	
	public void setSelected(bool selected) {

		if (selected) {
			GetComponent<Renderer> ().material.color = selectedColor;
			sphere.GetComponent<Renderer> ().material.color = selectedColor;
		} else {
			GetComponent<Renderer> ().material.color = startColor;
			sphere.GetComponent<Renderer> ().material.color = startColor;
		}
		this.isSelected = selected;
	}
	
	public void setTeam(Team inTeam) {
		this.team = inTeam;
	}

	public void OnDrawGizmos() {
		if (displayWayPointGizmos) {
			if (path != null) {
				for (int i = targetIndex; i < path.Length; i ++) {
					Gizmos.color = Color.cyan;
					Gizmos.DrawCube (path [i], Vector3.one);
					
					if (i == targetIndex) {
						Gizmos.DrawLine (transform.position, path [i]);
					} else {
						Gizmos.DrawLine (path [i - 1], path [i]);
					}
				}
			}
		}
	}
}
