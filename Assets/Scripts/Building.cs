using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Building : MonoBehaviour
{

	// Constants
	public const string UNIT = "Unit";

	// Variables linked from scene
	public Unit unit; // Prefab (Unit)

	// Logic variables
	private bool isSelected = false;
	private Team team;
	private int unitCount = 0;
	private Vector3 position;
	private KeyValuePair<Vector3,bool>[] availablePositions = new KeyValuePair<Vector3,bool>[12];

	// Model variables
	private Color startColor = new Color(0,1f,0);

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	private void Start() {}
	
	private void OnMouseDown() {
		//Debug.Log("Building Selected");
		//setSelected (!isSelected);
	}

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/	
	
	public Unit createUnit(Team inTeam) {

		Unit unitObject = null;
		for (int i = 0; i < availablePositions.Length; i++) {
			if(availablePositions[i].Value) {
				availablePositions[i] = new KeyValuePair<Vector3, bool>(availablePositions[i].Key,false);
				unitObject = (Unit)Instantiate(unit, availablePositions[i].Key, Quaternion.identity);
				unitObject.name = UNIT + unitCount;
				unitObject.setTeam(inTeam);
				unitCount++;
				break;
			}
		}
		return unitObject;
	}
	
	public void initialize(Vector3 inPosition) {
		this.position = inPosition;
		
		int index = 0;
		for(int z = (int)this.position.z - 3; z <= (int)this.position.z + 3 && index < this.availablePositions.Length; z = z + 2) {
			for(int x = (int)this.position.x-3; x <= (int)this.position.x + 3; x = x + 2) {
				if((Math.Abs(this.position.x - x) != 1 ^ Math.Abs(this.position.z - z) != 1) || ((this.position.x - x) != 1 && Math.Abs(this.position.z - z) != 1)) {
					this.availablePositions[index] = new KeyValuePair<Vector3, bool>(new Vector3(x, .5f, z), true);
					index++;
				}
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


	/*********************************************************************************/
	/*	Getter and Setter Functions - Order: Alphabetic							 	 */		
	/*********************************************************************************/
	
	public bool getSelected() {
		return this.isSelected;
	}
	
	public void setSelected(bool selected) {
		
		if (selected) {
			GetComponent<Renderer> ().material.color = Color.yellow;
		} else {
			GetComponent<Renderer> ().material.color = startColor;
		}
		this.isSelected = selected;
	}

	public void setTeam(Team inTeam) {
		this.team = inTeam;
	}
}
