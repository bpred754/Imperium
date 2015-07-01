using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersonPlayer : Player
{
	// Constants
	public const string BUILDING = "Building";

	// Variables linked from scene
	public Texture2D selectionHighlight;
	public Building building; // Prefab (Building)

	// Logic variables
	private Vector3 startClick = -Vector3.one; 
	private Rect selection = new Rect (0,0,0,0);
	private Dictionary<string, Unit> units = new Dictionary<string, Unit> ();
	private Dictionary<string, Building> buildings = new Dictionary<string, Building> ();
	private int unitCount = 0;
	private int selectedUnitCount = 0;
	private int buildingCount = 0;
	private List<Unit> selectedUnitsList = new List<Unit>();

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	// Called when PersonPlayer is instantiated
	private void Start() {}

	// Called every frame
	private void Update() {
		checkPlayerClick();
		checkPlayerSelection();
		moveCamera();
	}

	// Called several times per frame when needed
	private void OnGUI()
	{
		if (startClick != -Vector3.one) {
			GUI.color = new Color(1,1,1,0.5f);
			GUI.DrawTexture(selection,selectionHighlight);
		}
	}

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/

	public void createBuilding(Vector3 position) {
		Building buildingObject = (Building)Instantiate(building, position, Quaternion.identity);
		buildingObject.initialize (position);
		string buildingName = BUILDING + buildingCount;
		buildingObject.name = buildingName;
		buildings.Add(buildingName, buildingObject);
		buildingCount++;
	}

	/*********************************************************************************/
	/*	Private Functions - Order: Alphabetic							 			 */		
	/*********************************************************************************/

	// Executes logic when user clicks
	private void checkPlayerClick() {

		bool isControlPressed = false;
		if (Input.GetKey (KeyCode.LeftControl)) {
			isControlPressed = true;
		}
		
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				/*Debug.Log ("Name = " + hit.collider.name);
				Debug.Log ("Tag = " + hit.collider.tag);
				Debug.Log ("Hit Point = " + hit.point);
				Debug.Log ("Object position = " + hit.collider.gameObject.transform.position);
				Debug.Log ("--------------");*/

				// If building is clicked create new unit
				if (buildings.ContainsKey (hit.collider.name)) {
					Unit newUnit = buildings [hit.collider.name].createUnit (this.team);
					if (newUnit != null) {
						units.Add (newUnit.name, newUnit);
						unitCount++;
					}
				}
				
				// If a unit is clicked unselect other units
				if (units.ContainsKey (hit.collider.name)) {
					foreach (KeyValuePair<string, Unit> entry in units) {
						Unit unit = entry.Value;
						if (entry.Key == hit.collider.name) {
							unit.setSelected (true);
						} else {
							// Don't unselect units if the control key is pressed
							if(!isControlPressed) {
								unit.setSelected (false);
							}
						}
					}
				}
				
				// If the ground is clicked unselect units
				if (hit.collider.name == "Ground") {
					if(!isControlPressed){
						foreach (KeyValuePair<string,Unit> entry in units) {
							Unit unit = entry.Value;
							unit.setSelected (false);
						}
					}
				}
			}
		} //If right click is pressed down, i.e. move order is made
		else if (Input.GetMouseButtonDown (1)) {
			//Creates move order based on mouse screen coordinates
			Vector3 moveOrder = Input.mousePosition;
			//Creates formation with units selected
			//My dad thinks there should be a new "formation" class that gets made right now
			// on the right mouse click. The formation code probably shouldn't be in person player.
			// I just don't where else to put it.
			//createFormation ("Square");
			createFormation("Shell");
			//createFormation ("Clustered");
			//createFormation ("V");
		}
	}

	// Executes logic when user clicks and drags (creates selection box)
	private void checkPlayerSelection() {

		// Detect mouse click
		if(Input.GetMouseButtonDown(0)) {
			startClick = Input.mousePosition;
		} else if (Input.GetMouseButtonUp(0)) {
			startClick = -Vector3.one;
		}
		
		// Determine selection box area
		if (Input.GetMouseButton (0)) {
			selection = new Rect (startClick.x, InvertMouseY (startClick.y), Input.mousePosition.x - startClick.x, InvertMouseY (Input.mousePosition.y) - InvertMouseY (startClick.y));
			if (selection.width < 0) {
				selection.x += selection.width;
				selection.width = -selection.width;
			}
			if (selection.height < 0) {
				selection.y += selection.height;
				selection.height = -selection.height;
			}

			// Only execute logic when the user creates a selection box, not when they click
			if (selection.height > 1) {
				foreach (KeyValuePair<string,Unit> entry in units) {
					Unit unit = entry.Value;
					if (unit.isVisible () && Input.GetMouseButton (0)) {
						Vector3 camPos = Camera.main.WorldToScreenPoint (unit.transform.position);
						camPos.y = InvertMouseY (camPos.y);
						unit.setSelected (selection.Contains (camPos));
					}			
				}
			}
		}

		// Loop through buildings and select the buildings that are in selection area
		/*foreach(KeyValuePair<string,Building> entry in buildings) {
			Building building = entry.Value;
			if(building.isVisible() && Input.GetMouseButton(0)) {
				Vector3 camPos = Camera.main.WorldToScreenPoint(building.transform.position);
				camPos.y = InvertMouseY(camPos.y);
				building.setSelected(selection.Contains(camPos));
			}			
		}*/
	}
	//Takes a unit and a move order and moves unit to that location
	private void giveMoveOrder(Vector3 moveOrder, Unit unit){
		//Sets z to current camera height, this needs to be updated for varying heights of camera
		moveOrder.z = 28;
		//Debug.Log ("moveOrder1: " + moveOrder.ToString());
		//Converts screen coordinates of move order to world coordinates
		moveOrder = Camera.main.ScreenToWorldPoint(moveOrder);
		moveOrder.y = unit.transform.position.y;
		//Y gets set to zero because they aren't moving in that direction at all
		//Mouse height somtimes changes depending on where you click so this is necessary
		//moveOrder.y = 0;
		//Debug.Log ("moveOrder2: " + moveOrder.ToString());
		//Commits move order in unit class.

		unit.makeMove(moveOrder);
	}
	
	// Inverts the Y component of the mouse vector
	private static float InvertMouseY(float y)
	{
		return Screen.height - y;
	}
	
	// Executes logic when the user moves the camera
	private void moveCamera() {
		int screenScrollLimit = 100;
		float scrollRate = 0.2f;
		Vector3 movementVectorX;
		Vector3 movementVectorZ;
	
		// Scrolling with the mouse as it enters edges of screen
		if (Input.mousePosition.y < screenScrollLimit && Input.mousePosition.y >= 0) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position -= movementVectorZ;
		} else if (Input.mousePosition.y > Screen.height - screenScrollLimit && Input.mousePosition.y <= Screen.height) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position += movementVectorZ;
		}
		if (Input.mousePosition.x < screenScrollLimit && Input.mousePosition.x >= 0) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position -= movementVectorX;
		} else if (Input.mousePosition.x > Screen.width - screenScrollLimit && Input.mousePosition.x <= Screen.width) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position += movementVectorX;
		}

		//Scrolling with the arrow keys 
		if (Input.GetKey ("up") && !Input.GetKey ("down")) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position += movementVectorZ;
		} else if (Input.GetKey ("down") && !Input.GetKey ("up")) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position -= movementVectorZ;
		}
		if (Input.GetKey ("left") && !Input.GetKey ("right")) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position -= movementVectorX;
		} else if (Input.GetKey ("right") && !Input.GetKey ("left")) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position += movementVectorX;
		}
	}

	//Creates formation, takes string:formationName
	private void createFormation(string formationName){
		Vector3 mousePosition = Input.mousePosition;
		Vector3 movePosition = mousePosition;
		int numberUnits = getNumUnitsSelected ();
		int unitSpace = 25;
		if (formationName == "Square" || formationName == "Squared") {
			float side = Mathf.Sqrt (numberUnits);
			float middleSide = (side/2)*unitSpace;
			foreach (Unit unit in selectedUnitsList) {
				giveMoveOrder (movePosition, unit);
				movePosition.x += unitSpace;
				if (movePosition.x >= mousePosition.x + unitSpace * side) {
					movePosition.x = mousePosition.x;
					movePosition.y -= unitSpace;
				}
				//loops through rings of hexagons
				//first ring 1 unit, second ring 6 units, third ring 12 units, fourth ring 18 units
			}
		} else if (formationName == "Shell" || formationName == "Shelled") {
			int circumference = unitSpace * numberUnits * 2;
			float radius = circumference / (Mathf.PI * 2);
			float degreeOffset = 360 / numberUnits;
			float radOffset = (degreeOffset * Mathf.PI) / 180;
			float radianOffset = 0;

			if (numberUnits == 1) {
				foreach (Unit unit in selectedUnitsList) {
					giveMoveOrder (movePosition, unit);
				}
			} else {
				foreach (Unit unit in selectedUnitsList) {
					float x;
					float y;
					movePosition = mousePosition;

					movePosition.x += radius * Mathf.Sin (radianOffset);
					movePosition.y += radius * Mathf.Cos (radianOffset);
	
					giveMoveOrder (movePosition, unit);

					radianOffset += radOffset;
				}
			}
		} else if (formationName == "V") {
			int count = 0;
			foreach (Unit unit in selectedUnitsList) {
				count += 1;
				if(count == 1){
					giveMoveOrder (movePosition, unit);
				} else if ((count % 2) == 1){//if odd
					
				} else {

				}
			}
		}
	}

	private int getNumUnitsSelected(){
		//unitCount
		//"Cluster" will be default. Units attempts to be as close to center as possible without overlapping eachother.

		//clears the list to make way for new units
		selectedUnitsList.Clear ();
		//selectedUnitsList = new List<Unit>();

		foreach (KeyValuePair<string,Unit> entry in units) {
			Unit unit = entry.Value;
			if(unit.getSelected()){
				selectedUnitsList.Add(unit);
			}
		}
		//sets class variable to be used when necessary without high overhead. hopefully.
		selectedUnitCount = selectedUnitsList.Count;
		return selectedUnitsList.Count;
	}
}

