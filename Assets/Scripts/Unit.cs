﻿using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	// Logic variables
	private bool isSelected;
	private Team team;
	private float movementSpeed = 0.9f;
	private Vector3 destination;
	private bool isMoving = false;

	// Model variables
	private Color startColor = Color.gray;
	private Color selectedColor = Color.red;


	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	private void Start() {
		this.isSelected = false;
		GetComponent<Renderer> ().material.color = startColor;
	}
	//While isMoving = true, units move towards their targets
	void Update(){
		if (isMoving) {
			float step = movementSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, destination, step);
			//Unfortunately this never occurs, I am trying to figure it out.
			//Movement still works though, they just keep adjusting their positions.
			if(transform.position == destination){
				isMoving = false;
				Debug.Log ("Unit has stopped moving");
			}
		}
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
	//Sets the destination to the target and isMoving to true.
	//Movement gets done in Update method.
	public void makeMove(Vector3 destination){ //Maybe should return bool, if unit cannot get to location return false?? otherwise true?
		//For now void, We can think about that later, when we are doing pathfinding etc.
		this.destination = destination;
		//Debug.Log ("Destination: " + destination.ToString ());
		isMoving = true;		
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
		} else {
			GetComponent<Renderer> ().material.color = startColor;
		}
		this.isSelected = selected;
	}
	
	public void setTeam(Team inTeam) {
		this.team = inTeam;
	}
}
