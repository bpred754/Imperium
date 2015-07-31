using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class PersonPlayer : Player
{
	// Constants
	public const string BUILDING = "Building";

	// Prefabs
	private Building buildingPrefab;

	// Logic variables
	private int buildingCount = 0;
	private Dictionary<string, Building> buildings = new Dictionary<string, Building> ();
	private CameraManager camera;
	private String formation = "Shell";
	private GUIManager guiManager;
	private InputManager inputManager;
	private LayerMask layer;
	private List<Unit> selectedUnitsList = new List<Unit>();
	private int unitCount = 0;
	private Dictionary<string, Unit> units = new Dictionary<string, Unit> ();
	
	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	private void Awake() {

		// Load Resources
		this.buildingPrefab = Resources.Load<Building> ("Building");

		GameObject player = GetComponent<Transform> ().gameObject;
		this.camera = player.AddComponent<CameraManager> ();
		this.inputManager = player.AddComponent<InputManager> ();
		this.guiManager = player.AddComponent<GUIManager> ();
	}

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/

	public void createBuilding(Vector3 position) {
		Building buildingObject = (Building)Instantiate(buildingPrefab, position, Quaternion.identity);
		buildingObject.initialize (position, buildingCount);
		string buildingName = BUILDING + buildingCount;
		buildingObject.name = buildingName;
		this.buildings.Add(buildingName, buildingObject);
		this.buildingCount++;
	}

	public void createUnit(String _buildingName) {
		Unit newUnit = buildings [_buildingName].createUnit (this.team);
		if (newUnit != null) {
			this.units.Add (newUnit.name, newUnit);
			this.unitCount++;

			// DEBUG
			newUnit.setDisplayWayPointGizmos(false);
		}
	}

	public bool hasBuilding(String _buildingName) {
		bool hasBuilding = false;
		if (this.buildings.ContainsKey (_buildingName)) {
			hasBuilding = true;
		}
		return hasBuilding;
	}

	public bool hasUnit(String _unitName) {
		bool hasUnit = false;
		if (this.units.ContainsKey (_unitName)) {
			hasUnit = true;
		}
		return hasUnit;
	}

	public void moveSelectedUnitsInFormation(Vector3 mousePosition) {
		createFormation(this.formation, mousePosition);
	}

	// Set units selection attribute when a unit is clicked
	public void setUnitsSelection(String _unitName, bool isGroup) {

		foreach (KeyValuePair<string, Unit> entry in units) {
			Unit unit = entry.Value;
			if (entry.Key == _unitName) {
				unit.setSelected (true);
			} else {
				// Don't unselect units if the control key is pressed
				if(!isGroup) {
					unit.setSelected (false);
				}
			}
		}
	}

	// Set units selection attribute when a unit is in selection rectangle
	public void setUnitsSelection(Rect _selection) {
		foreach (KeyValuePair<string,Unit> entry in units) {
			Unit unit = entry.Value;
			if (unit.isVisible () && Input.GetMouseButton (0)) {
				Vector3 camPos = Camera.main.WorldToScreenPoint (unit.transform.position);
				camPos.y = InputManager.InvertMouseY (camPos.y);
				unit.setSelected (_selection.Contains (camPos));
			}			
		}
	}

	public void unselectUnits() {
		foreach (KeyValuePair<string,Unit> entry in units) {
			Unit unit = entry.Value;
			unit.setSelected (false);
		}
	}

	/*********************************************************************************/
	/*	Private Logic Functions - Order: Alphabetic							 	     */		
	/*********************************************************************************/

	//Creates formation, takes string:formationName
	private void createFormation(string formationName, Vector3 mousePosition){
		if(getNumUnitsSelected() > 1){
			Vector3 movePosition = mousePosition;
			int numberUnits = getNumUnitsSelected ();
			int unitSpace = 1;
			if (formationName == "Square" || formationName == "Squared") {
				float side = Mathf.Sqrt (numberUnits);
				foreach (Unit unit in selectedUnitsList) {
					unit.makeMove (movePosition);
					movePosition.x += unitSpace;
					if (movePosition.x >= mousePosition.x + unitSpace * side) {
						movePosition.x = mousePosition.x;
						movePosition.z -= unitSpace;
					}
				}
			} else if (formationName == "Shell" || formationName == "Shelled") {
				int circumference = unitSpace * numberUnits * 2;
				float radius = circumference / (Mathf.PI * 2);
				float degreeOffset = 360 / numberUnits;
				float radOffset = (degreeOffset * Mathf.PI) / 180;
				float radianOffset = 0;
				
				foreach (Unit unit in selectedUnitsList) {
					movePosition = mousePosition;
					
					movePosition.x += radius * Mathf.Sin (radianOffset);
					movePosition.z += radius * Mathf.Cos (radianOffset);

					unit.makeMove (movePosition);
					
					radianOffset += radOffset;
				}
			}
		}else{ //If only one unit is selected
			foreach (Unit unit in selectedUnitsList) {
				unit.makeMove (mousePosition);
			}
		}
	}

	private int getNumUnitsSelected(){
		
		// Clears the list to make way for new units
		this.selectedUnitsList.Clear ();
		
		foreach (KeyValuePair<string,Unit> entry in units) {
			Unit unit = entry.Value;
			if(unit.getSelected()){
				this.selectedUnitsList.Add(unit);
			}
		}
		return selectedUnitsList.Count;
	}

	/*********************************************************************************/
	/*	DEBUG Functions - Order: Alphabetic							 	             */		
	/*********************************************************************************/

	public void setUnitsWayPointGizmos(bool display) {
		foreach (Unit unit in selectedUnitsList) {
			if (display && !unit.getDisplayWayPointGizmos ()) {
				unit.setDisplayWayPointGizmos (true);
			} else if (!display && unit.getDisplayWayPointGizmos()) {
				unit.setDisplayWayPointGizmos(false);
			}
		}
	}
}
