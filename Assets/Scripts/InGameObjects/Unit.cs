using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	// Logic variables
	public float movementSpeed;
	private bool isSelected;
	private Team team;
	private Vector3 destination;
	private Vector3[] path;
	private int targetIndex;

	// Model variables
	public bool displayWayPointGizmos;
	private Color startColor = Color.black;
	private Color selectedColor = Color.red;
	private Transform sphere;

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	private void Start() {
		this.isSelected = false;
		GetComponent<Renderer> ().material.color = startColor;
		sphere = transform.FindChild ("Sphere");
		sphere.GetComponent<Renderer> ().material.color = startColor;
	}

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/	

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
	
	public void makeMove(Vector3 target){
		PathRequestManager.RequestPath(transform.position, target, OnPathFound);		
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful){
		if(pathSuccessful){
			path = newPath;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	/*********************************************************************************/
	/*	Private Functions - Order: Alphabetic										 */		
	/*********************************************************************************/
	
	private IEnumerator FollowPath() {
		if(path.GetLength(0) > 0){
			Vector3 currentWaypoint = path[0];
			targetIndex = 0;
			
			while(true){
				if (transform.position.x == currentWaypoint.x && transform.position.y == currentWaypoint.y && transform.position.z == currentWaypoint.z){
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

	/*********************************************************************************/
	/*	DEBUG Function - Order: Alphabetic							 	             */		
	/*********************************************************************************/

	public bool getDisplayWayPointGizmos() {
		return this.displayWayPointGizmos;
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

	public void setDisplayWayPointGizmos(bool willDisplayWayPointGizmos) {
		this.displayWayPointGizmos = willDisplayWayPointGizmos;
	}
}
